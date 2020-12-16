using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickupController : MonoBehaviour
{
    private Transform mPlayerTransform;
    private List<GameObject> mPickupGameObjects;

    [Tooltip("Player controller script.")]
    [SerializeField] PlayerController pControllerScript = null;

    [Tooltip("Pickup collection list, add a pickup prefab here.")]
    [SerializeField] GameObject[] mPickupObjectCollection = { };

    [Tooltip("Pickup range which determines how near a player should be near pickups before they collect.")]
    [SerializeField] private float mPickupRange = 1;

    [Tooltip("How close to the player does the pickup need to be before being collected")]
    [SerializeField] private float mCollectPickupRange = 1.0f;

    [Tooltip("Pickup collection speed which determines how fast a pickup moves towards the player.")]
    [SerializeField] private float mPickupCollectSpeed = 0.25f;

    [Tooltip("Determines how much of a force should be applied when the pickup spawns from a crate.")]
    [SerializeField] private float mPickupSpawnForce = 4;

    private GameObject mCollectableUIManagerObject;
    private CollectableUIManager mCollectableUIManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<SphereCollider>().radius = mPickupRange;

        mPlayerTransform = this.gameObject.GetComponent<Transform>();
        mPickupGameObjects = new List<GameObject>();
        mCollectableUIManagerObject = GameObject.Find("CollectableUICanvas").transform.Find("CollectableGUI").transform.Find("CollectableUIManager").gameObject;
        mCollectableUIManagerScript = mCollectableUIManagerObject.GetComponent<CollectableUIManager>();
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.gameObject.layer == 8)
        {
            PickupScript pickupScript = otherCollider.gameObject.GetComponent<PickupScript>();
            ParticleSystem pickupParticleSystem = otherCollider.gameObject.GetComponent<ParticleSystem>();

            if (!pickupScript.hasBeenCollected)
            {
                pickupScript.hasBeenCollected = true;
                mPickupGameObjects.Add(otherCollider.gameObject);
                pickupParticleSystem.Play();
            }
        }

        if (otherCollider.gameObject.layer == 12)
        {
            Transform llamaCageTransform = otherCollider.gameObject.transform;
            LlamaCageScript llamaCageScript = llamaCageTransform.gameObject.GetComponent<LlamaCageScript>();

            if (!llamaCageScript.hasBeenDestroyed && pControllerScript.GetIsDashing())
            {
                llamaCageScript.hasBeenDestroyed = true;
                llamaCageTransform.gameObject.layer = LayerMask.NameToLayer("BrokenPickupCrate");

                if (null == llamaCageTransform)
                    return;

                llamaCageScript.BreakLlamaCage(transform.forward);

                CollectableUIClass collectable = mCollectableUIManagerScript.GetCollectableByName(llamaCageScript.pickupName);

                float rotIncrementPerLlama = 360 / llamaCageScript.llamaAmount;
                float rotIncrement = 0;

                for (int i = 0; i < llamaCageScript.llamaAmount; i++)
                {
                    GameObject llamaObj = PoolSystem.Instance.GetObjectFromPool(llamaCageScript.llamaObject, argShouldExpandPool: true, argActivateObject: true, argShouldCreateNonExistingPool: true);
                    llamaObj.transform.position = llamaCageTransform.position;

                    if (null == llamaObj)
                        continue;

                    MiniLlamaScript llamaScript = llamaObj.GetComponent<MiniLlamaScript>();
                    llamaObj.transform.rotation = Quaternion.Euler(0, rotIncrement, 0);

                    Rigidbody llamaRb = llamaObj.GetComponent<Rigidbody>();
                    llamaRb.isKinematic = false;

                    llamaRb.AddForce((llamaObj.transform.forward + llamaObj.transform.up) * 3, UnityEngine.ForceMode.Impulse);
                    llamaScript.hasSpawned = true;

                    collectable.IncreaseCollectableQuantity();

                    rotIncrement += rotIncrementPerLlama;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision otherCollision)
    {
        if (otherCollision.gameObject.layer == 9)
        {
            Transform pickupCrateTransform = otherCollision.gameObject.GetComponent<Transform>();
            PickupCrateScript pickupCrateScript = otherCollision.gameObject.GetComponent<PickupCrateScript>();

            if (!pickupCrateScript.hasBeenDestroyed && pControllerScript.GetIsDashing())
            {
                pickupCrateScript.hasBeenDestroyed = true;

                float rotationIncrementPerPickup = 360 / pickupCrateScript.pickupAmount;
                float tempRotationIncrement = 0;

                // Get a set amount of pickup objects from the object pool here and store them in a temporary array.
                for (int i = 0; i < pickupCrateScript.pickupAmount; i++)
                {
                    // Get a specific pickup from the object pool here.
                    GameObject pickupObject = null;
                    PickupScript pickupObjectScript = null;

                    foreach (GameObject tempPickupObj in mPickupObjectCollection)
                    {
                        if (pickupCrateScript.pickupTypes[Random.Range(0, pickupCrateScript.pickupTypes.Length - 1)] == tempPickupObj.name)
                        {
                            pickupObject = PoolSystem.Instance.GetObjectFromPool(tempPickupObj, argShouldExpandPool: true, argActivateObject: true, argShouldCreateNonExistingPool: true);

                            if (null == pickupObject)
                                continue;

                            pickupObjectScript = pickupObject.GetComponent<PickupScript>();

                            if (null == pickupObjectScript)
                                continue;
                            break;
                        }
                        else
                            continue;
                    }

                    Vector3 pickupForceDirection = Quaternion.Euler(0, tempRotationIncrement, 0) * Vector3.forward;

                    // Apply a force towards a specific direction and rotation here.
                    pickupObjectScript.ApplyForce(pickupCrateTransform.position + new Vector3(0, pickupCrateTransform.localScale.y / 2, 0) + pickupCrateTransform.up, pickupForceDirection.normalized, mPickupSpawnForce);

                    tempRotationIncrement += rotationIncrementPerPickup;
                }

                AudioSystem.Instance.PlaySound("BreakCrate", 1.0f);
                // Break the crate model here (spawn broken crate mesh).
                pickupCrateScript.SpawnBrokenCrate(transform.forward);
            }
        }
    }


    private void FixedUpdate()
    {
        for (int i = 0; i < mPickupGameObjects.Count; i++)
        {
            Transform gameObjectTransform = mPickupGameObjects[i].GetComponent<Transform>();
            PickupScript gameObjectPickupScript = mPickupGameObjects[i].GetComponent<PickupScript>();
            Renderer gameObjectRenderer = mPickupGameObjects[i].GetComponent<Renderer>();
            ParticleSystem gameObjectParticleSystem = mPickupGameObjects[i].GetComponent<ParticleSystem>();

            float playerToPickupMagnitude = (gameObjectTransform.position - mPlayerTransform.position).magnitude;

            if (gameObjectPickupScript.hasLanded)
            {
                gameObjectTransform.position = Vector3.MoveTowards(gameObjectTransform.position, mPlayerTransform.position, mPickupCollectSpeed);
                gameObjectTransform.localScale = new Vector3(Mathf.Clamp(playerToPickupMagnitude, 0, gameObjectPickupScript.originalScale.x), Mathf.Clamp(playerToPickupMagnitude, 0, gameObjectPickupScript.originalScale.y), Mathf.Clamp(playerToPickupMagnitude, 0, gameObjectPickupScript.originalScale.z));
                gameObjectRenderer.material.color = new Color(gameObjectRenderer.material.color.r, gameObjectRenderer.material.color.g, gameObjectRenderer.material.color.b, Mathf.Clamp(playerToPickupMagnitude, 0.0f, 1.0f));
            }

            if (mCollectPickupRange > playerToPickupMagnitude)
            {
                // Add the pickup to the collection total.
                CollectableUIClass collectable = mCollectableUIManagerScript.GetCollectableByName(gameObjectPickupScript.pickupName);
                collectable.IncreaseCollectableQuantity();

                // Disable the pickup in the object pool, which then removes it from the game scene here.
                gameObjectTransform.gameObject.SetActive(false);

                gameObjectTransform.localScale = gameObjectPickupScript.originalScale;
                gameObjectRenderer.material.color = gameObjectPickupScript.originalColour;
                gameObjectPickupScript.hasBeenCollected = false;

                gameObjectParticleSystem.Stop();

                mPickupGameObjects.RemoveAt(i);

                AudioSystem.Instance.PlaySound("Pickup", 0.2f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
            return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
                continue;

            SetLayerRecursively(child.gameObject, newLayer);
        }
    }*/
}
