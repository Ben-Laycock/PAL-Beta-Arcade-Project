using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float mTimeUntillDestruction = 1.0f;

    private float mTimer = 0.0f;

    private void Start()
    {
        mTimer = 0.0f;
    }

    private void Update()
    {
        mTimer += Time.deltaTime;
        if(mTimer >= mTimeUntillDestruction)
        {
            Destroy(gameObject);
        }
    }
}