using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatBobScript : MonoBehaviour
{
    private Vector3 mOriginalPosition;
    private Vector3 mTargetPositon;

    void Start()
    {
        mOriginalPosition = gameObject.transform.position;
    }

    private float mMoveTimer = 0;

    void Update()
    {
        
        if((gameObject.transform.position - mOriginalPosition).magnitude <= 0.1f)
        {
            mTargetPositon = mOriginalPosition + new Vector3(0, 0.5f, 0);
            mMoveTimer = 0;
        }
        else if((gameObject.transform.position - (mOriginalPosition + new Vector3(0, 0.5f, 0))).magnitude <= 0.1f)
        {
            mTargetPositon = mOriginalPosition;
            mMoveTimer = 0;
        }

        gameObject.transform.position = new Vector3(gameObject.transform.position.x, SmoothMove(gameObject.transform.position.y, mTargetPositon.y, mMoveTimer), gameObject.transform.position.z);
        mMoveTimer += Time.deltaTime;

    }

    private float SmoothMove(float startPosX, float endPosX, float timeStep)
    {

        float position = Mathf.Lerp(startPosX, endPosX, timeStep);
        return position;

    }

}