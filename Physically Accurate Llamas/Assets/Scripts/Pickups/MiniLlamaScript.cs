using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniLlamaScript : MonoBehaviour
{
    private bool mHasSpawned = false;
    public bool hasSpawned
    {
        get { return mHasSpawned; }
        set { mHasSpawned = value; }
    }

    private Rigidbody mRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        mRigidbody = this.gameObject.GetComponent<Rigidbody>();

        if (null == mRigidbody)
        {
            mRigidbody = this.gameObject.AddComponent<Rigidbody>();
            mRigidbody.isKinematic = true;
        }
    }

    void FixedUpdate()
    {
        if (mHasSpawned)
        {
            foreach (Transform childT in this.gameObject.transform)
            {
                Renderer childRenderer = childT.gameObject.GetComponent<Renderer>();
                childRenderer.material.color = new Color(childRenderer.material.color.r, childRenderer.material.color.g, childRenderer.material.color.b, childRenderer.material.color.a - 0.025f);

                if (childRenderer.material.color.a <= 0)
                {
                    this.gameObject.SetActive(false);
                    mHasSpawned = false;
                    mRigidbody.isKinematic = true;

                    childRenderer.material.color = new Color(childRenderer.material.color.r, childRenderer.material.color.g, childRenderer.material.color.b, 1);
                }
            }
        }
    }
}
