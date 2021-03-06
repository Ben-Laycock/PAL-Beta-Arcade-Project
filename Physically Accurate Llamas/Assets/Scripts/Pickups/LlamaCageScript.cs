﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LlamaCageScript : MonoBehaviour
{
    [Tooltip("Broken llama cage prefab, shouldn't need to be changed.")]
    [SerializeField] private GameObject mBrokenLlamaCage = null;

    [Tooltip("Mini llama game object goes here, shouldn't need to be changed.")]
    [SerializeField] private GameObject mLlamaObject = null;
    public GameObject llamaObject
    {
        get { return mLlamaObject; }
    }

    [Tooltip("How many llamas come from the cage? This also determines how many points it awards for the llama cage collectable.")]
    [SerializeField] private int mLlamaAmount = 1;
    public int llamaAmount
    {
        get { return mLlamaAmount; }
    }

    [Tooltip("The pickup name for the llama cage, shouldn't need to be changed.")]
    [SerializeField] private string mPickupName = "CagedLlama";
    public string pickupName
    {
        get { return mPickupName; }
    }

    private bool mHasBeenDestroyed = false;
    public bool hasBeenDestroyed
    {
        set { mHasBeenDestroyed = value; }
        get { return mHasBeenDestroyed; }
    }

    private float destroyedFadeTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (mHasBeenDestroyed)
        {
            print(destroyedFadeTimer);
            destroyedFadeTimer += Time.fixedDeltaTime;
        }
    }

    public void BreakLlamaCage(Vector3 argPlayerMovementDirection)
    {
        if (argPlayerMovementDirection == null)
            argPlayerMovementDirection = Vector3.forward;

        GameObject brokenLlamaCage = PoolSystem.Instance.GetObjectFromPool(mBrokenLlamaCage, argActivateObject: true, argShouldCreateNonExistingPool: true, argShouldExpandPool: true);
        Transform tempBrokenLlamaCageTransform = brokenLlamaCage.GetComponent<Transform>();

        if (null == tempBrokenLlamaCageTransform)
            return;

        Vector3 cagePosition = this.gameObject.transform.position;

        Destroy(this.gameObject);
        tempBrokenLlamaCageTransform.position = cagePosition;

        foreach (Transform childT in brokenLlamaCage.transform)
        {
            Rigidbody rBody = childT.gameObject.GetComponent<Rigidbody>();
            MeshCollider mCollider = childT.gameObject.GetComponent<MeshCollider>();

            mCollider.isTrigger = false;
            rBody.isKinematic = false;

            rBody.AddForce(argPlayerMovementDirection.normalized * 10.0f, ForceMode.Impulse);
        }
    }
}
