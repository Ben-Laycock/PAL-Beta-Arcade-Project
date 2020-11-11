using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLlamaBounce : MonoBehaviour
{

    [SerializeField] private int mNumberOfJumpPoints = 0;
    [SerializeField] private List<GameObject> mJumpPointObjects = new List<GameObject>();

    private int mCurrentJumpPoint = 0;
    private Rigidbody mLlamaRigidbody;
    private void Start()
    {
        
        for(int i = 0 ; i <= mNumberOfJumpPoints; i++)
        {
            GameObject mJumpPoint = GameObject.Find("Point " + i.ToString());
            if(mJumpPoint != null)
            {
                mJumpPointObjects.Add(mJumpPoint);
            }
        }

        mLlamaRigidbody = gameObject.GetComponent<Rigidbody>();

    }


    private void Update()
    {
        
        if(CheckLlamaIsCloseEnough(mJumpPointObjects[mCurrentJumpPoint]))
        {
            if(mCurrentJumpPoint < mNumberOfJumpPoints - 1)
            {
                mCurrentJumpPoint++;
            }
            else
            {
                mCurrentJumpPoint = 0;
            }
            mLlamaRigidbody.velocity = new Vector3(0,0,0);
            mLlamaRigidbody.AddForce(((gameObject.transform.forward / 2) + (gameObject.transform.up * 2)) * 150);
            Debug.Log("Force Added");
        }

        if(gameObject.transform.position.y < -15)
        {
            gameObject.transform.position = gameObject.transform.parent.position;
            mLlamaRigidbody.velocity = new Vector3(0,0,0);
        }

    }


    private bool CheckLlamaIsCloseEnough(GameObject pointToCheck)
    {
        if((gameObject.transform.position - pointToCheck.transform.position).magnitude <= 0.6)
        {
            return true;
        }

        return false;
    }


}