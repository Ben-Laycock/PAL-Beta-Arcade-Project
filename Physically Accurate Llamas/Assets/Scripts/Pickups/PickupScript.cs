using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupScript : MonoBehaviour
{
    private Rigidbody mRigidbody;
    private Transform mTransform;
    private Renderer mRenderer;

    private ParticleSystem mParticleSystem;
    private ParticleSystem.MainModule mParticleSystemModule;

    [Tooltip("The name of the pickup.")]
    [SerializeField] private string mPickupName;

    public string pickupName
    {
        get { return mPickupName; }
    }

    [Tooltip("The layer mask which is ignored.")]
    [SerializeField] private LayerMask mLayerMaskIgnore = 8;

    [Tooltip("The distance of the down raycast. This determines how much it is floating.")]
    [SerializeField] private float mRayDistance = 3f;

    [Tooltip("Rotation speed/increment.")]
    [SerializeField] private float mRotationIncrement = 1;

    private bool mHasLanded = false;
    public bool hasLanded
    {
        get { return mHasLanded; }
    }

    private bool mHasBeenCollected = false;
    public bool hasBeenCollected
    {
        get { return mHasBeenCollected; }
        set { mHasBeenCollected = value; }
    }

    private Vector3 mOriginalScale;
    public Vector3 originalScale
    {
        get { return mOriginalScale; }
    }

    private Color mOriginalColour;
    public Color originalColour
    {
        get { return mOriginalColour; }
    }

    // Start is called before the first frame update
    void Start()
    {
        mRigidbody = this.gameObject.GetComponent<Rigidbody>();

        if (null == mRigidbody) 
            mRigidbody = this.gameObject.AddComponent<Rigidbody>();

        mTransform = this.gameObject.GetComponent<Transform>();






        mOriginalScale = mTransform.localScale;

        if ("" == mPickupName)
            mPickupName = "Default";

        mRenderer = this.gameObject.GetComponent<Renderer>();

        if (null == mRenderer)
            return;

        mParticleSystem = this.gameObject.GetComponent<ParticleSystem>();

        if (null == mParticleSystem)
            return;

        mParticleSystemModule = mParticleSystem.main;
        //mParticleSystemModule.startColor = mRenderer.material.color;

        mOriginalColour = mRenderer.material.color;
        mHasLanded = false;
    }
    
    void FixedUpdate() 
    {
        mTransform.Rotate(new Vector3(0, mRotationIncrement, 0));

        RaycastHit hit;
        //+ (-mTransform.up * (mTransform.localScale.y / 2))
        if (Physics.Raycast(mTransform.position, -mTransform.up, out hit,mRayDistance, ~mLayerMaskIgnore) && false == mHasLanded) 
        {
            mTransform.position = hit.point + transform.up*mRayDistance;
            mHasLanded = true;
            mRigidbody.isKinematic = true;
        }
    }

    public void ApplyForce(Vector3 argInitialPosition, Vector3 argDirection, float argForceStrength) 
    {
        mRigidbody = this.gameObject.GetComponent<Rigidbody>();

        mHasLanded = false;
        this.gameObject.transform.position = argInitialPosition;
        mRigidbody.isKinematic = false;
        mRigidbody.AddForce(argDirection * argForceStrength, UnityEngine.ForceMode.Impulse);
    }
}