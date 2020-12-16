using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupCrateScript : MonoBehaviour
{
    [Tooltip("Select the pickup that spawns from the crate.")]
    [SerializeField] private string[] mPickupTypes = null;
    public string[] pickupTypes
    {
        get { return mPickupTypes; }
    }

    [Tooltip("Assign how many of a pickup the crate should spawn.")]
    [SerializeField] private int mPickupAmount = 4;
    public int pickupAmount
    {
        get { return mPickupAmount; }
    }

    [Tooltip("Assign the broken crate prefab here.")]
    [SerializeField] private GameObject mBrokenCrate = null;

    Transform mCrateTransform;

    private bool mHasBeenDestroyed = false;
    public bool hasBeenDestroyed
    {
        get { return mHasBeenDestroyed; }
        set { mHasBeenDestroyed = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        mCrateTransform = this.gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnBrokenCrate(Vector3 argPlayerMovementDirection)
    {
        GameObject tempBrokenCrate = PoolSystem.Instance.GetObjectFromPool(mBrokenCrate, argActivateObject: true, argShouldCreateNonExistingPool: true, argShouldExpandPool: true);
        Transform tempBrokenCrateTransform = tempBrokenCrate.GetComponent<Transform>();

        if (null == tempBrokenCrateTransform)
            return;

        Vector3 cratePosition = new Vector3(mCrateTransform.position.x,
            mCrateTransform.position.y + 0.5f,
            mCrateTransform.position.z);

        Destroy(this.gameObject);
        tempBrokenCrateTransform.position = cratePosition;

        foreach (Transform childT in tempBrokenCrate.transform)
        {
            Rigidbody rBody = childT.gameObject.GetComponent<Rigidbody>();

            rBody.isKinematic = false;

            rBody.AddForce(argPlayerMovementDirection * 10.0f, UnityEngine.ForceMode.Impulse);
        }
    }
}
