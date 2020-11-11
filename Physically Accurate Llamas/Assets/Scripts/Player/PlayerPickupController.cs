﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickupController : MonoBehaviour
{
    private Transform mPlayerTransform;
    private List<GameObject> mPickupGameObjects;

    [Tooltip("Pickup collection list, add a pickup prefab here.")]
    [SerializeField] GameObject[] mPickupObjectCollection = { };
    
    private GameObject mCollectableUIManagerObject;
    private CollectableUIManager mCollectableUIManagerScript;

    // Start is called before the first frame update
    void Start()
    {
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
    }

    private void OnCollisionEnter(Collision otherCollision)
    {
        if (otherCollision.gameObject.layer == 9)
        {
            Transform pickupCrateTransform = otherCollision.gameObject.GetComponent<Transform>();
            PickupCrateScript pickupCrateScript = otherCollision.gameObject.GetComponent<PickupCrateScript>();

            if (!pickupCrateScript.hasBeenDestroyed)
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
                        if (pickupCrateScript.pickupTypes[Random.Range(0, pickupCrateScript.pickupTypes.Length - 1)] == tempPickupObj.name) {
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
                    pickupObjectScript.ApplyForce(pickupCrateTransform.position + new Vector3(0, pickupCrateTransform.localScale.y / 2, 0) + pickupCrateTransform.up, pickupForceDirection.normalized, 4);

                    tempRotationIncrement += rotationIncrementPerPickup;
                }

                // Break the crate model here (spawn broken crate mesh).
                pickupCrateScript.SpawnBrokenCrate();
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
                gameObjectTransform.position = Vector3.MoveTowards(gameObjectTransform.position, mPlayerTransform.position, 0.25f);
                gameObjectTransform.localScale = new Vector3(Mathf.Clamp(playerToPickupMagnitude, 0, gameObjectPickupScript.originalScale.x), Mathf.Clamp(playerToPickupMagnitude, 0, gameObjectPickupScript.originalScale.y), Mathf.Clamp(playerToPickupMagnitude, 0, gameObjectPickupScript.originalScale.z));
                gameObjectRenderer.material.color = new Color(gameObjectRenderer.material.color.r, gameObjectRenderer.material.color.g, gameObjectRenderer.material.color.b, Mathf.Clamp(playerToPickupMagnitude, 0.0f, 1.0f));
            }

            if (0.2f > playerToPickupMagnitude)
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
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
