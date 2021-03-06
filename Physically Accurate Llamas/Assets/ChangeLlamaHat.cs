﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLlamaHat : MonoBehaviour
{

    [SerializeField] private GameObject mHatDisplayManager;

    private HatDisplayUIManager mHatDisplayUIManagerScript;

    [SerializeField] private GameObject mGliderHat;
    [SerializeField] private PlayerController mPlayerController;

    private void Start()
    {
        mHatDisplayUIManagerScript = mHatDisplayManager.GetComponent<HatDisplayUIManager>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            mHatDisplayUIManagerScript.ActivateSelectedHatUI();

            if (mGliderHat)
            {
                mGliderHat.SetActive(true);
                mPlayerController.mHasGliderHat = true;
                Destroy(gameObject.transform.parent.gameObject);
            }
            else
                print("null");

        }
        else
        {
            print("other");
        }
    }

}