using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class HatSelectionManager : MonoBehaviour
{

    private int mNumberOfCircles = 8;
    private List<GameObject> mCircleList;
    private List<RectTransform> mCircleListTransforms;
    private List<Vector3> mCircleOriginalPositions;
    private List<RectTransform> mCircleTimerTextObjects;

    private List<Vector3> mCurrentPositons = new List<Vector3>();
    private Vector2 mCurrentSize = new Vector2(0,0);
    private Vector3 mCurrentFontSize = new Vector3(0,0,0);

    private Vector2 mOriginalSize = new Vector2(140,140);

    [SerializeField] private bool mHatSelectionActive = false;
    [SerializeField] private bool mMenuToggleActivated = false;
    
    private float mHatSelectionMenuOpeningTimer = 0.0f;

    private bool mSetupComplete = false;

    [SerializeField] private bool mTestLockMouse = true;

    private void Start()
    {

        mCircleList = new List<GameObject>();
        mCircleListTransforms = new List<RectTransform>();
        mCircleOriginalPositions = new List<Vector3>();
        mCircleTimerTextObjects = new List<RectTransform>();

        for (int i = 0; i < mNumberOfCircles; i++)
        {
            string objectName = (i + 1).ToString();
            GameObject objectToAdd = gameObject.transform.parent.Find(objectName).gameObject;

            //Debug.Log("Name: " + objectName + "     Object: " + objectToAdd);

            mCircleList.Add(objectToAdd);
            mCircleListTransforms.Add(mCircleList[i].GetComponent<RectTransform>());
            mCircleOriginalPositions.Add(mCircleListTransforms[i].anchoredPosition3D);
            mCircleTimerTextObjects.Add(mCircleList[i].transform.Find("Timer").gameObject.GetComponent<RectTransform>());

            mCurrentPositons.Add(new Vector3(0,0,0));
        }

        for(int i = 0; i < mCircleListTransforms.Count; i++)
        {
            mCircleListTransforms[i].sizeDelta = new Vector2(0,0);
            mCircleListTransforms[i].anchoredPosition3D = new Vector3(0,0,0);
            mCircleTimerTextObjects[i].localScale = new Vector3(0,0,0);
        }

        mSetupComplete = true;

    }


    private void Update()
    {
        if(mTestLockMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if(mSetupComplete)
        {
            if(mMenuToggleActivated)
            {
                if (mHatSelectionActive)
                {
                    mHatSelectionMenuOpeningTimer += Time.deltaTime * 3;
                    mHatSelectionMenuOpeningTimer = Mathf.Clamp(mHatSelectionMenuOpeningTimer, 0, 1);
                    for (int i = 0; i < mCircleList.Count; i++)
                    {
                        mCircleListTransforms[i].anchoredPosition3D = Vector3.Lerp(mCurrentPositons[i], mCircleOriginalPositions[i], mHatSelectionMenuOpeningTimer);
                        mCircleListTransforms[i].sizeDelta = Vector2.Lerp(mCurrentSize, mOriginalSize, mHatSelectionMenuOpeningTimer);
                        mCircleTimerTextObjects[i].localScale = Vector3.Lerp(mCurrentFontSize, new Vector3(1,1,1), mHatSelectionMenuOpeningTimer);
                    }

                    if (mHatSelectionMenuOpeningTimer >= 1)
                    {
                        mMenuToggleActivated = false;
                    }
                }
                else
                {
                    mHatSelectionMenuOpeningTimer += Time.deltaTime * 3;
                    mHatSelectionMenuOpeningTimer = Mathf.Clamp(mHatSelectionMenuOpeningTimer, 0, 1);
                    for (int i = 0; i < mCircleList.Count; i++)
                    {
                        mCircleListTransforms[i].anchoredPosition3D = Vector3.Lerp(mCurrentPositons[i], new Vector3(0, 0, 0), mHatSelectionMenuOpeningTimer);
                        mCircleListTransforms[i].sizeDelta = Vector2.Lerp(mCurrentSize, new Vector2(0, 0), mHatSelectionMenuOpeningTimer);
                        mCircleTimerTextObjects[i].localScale = Vector3.Lerp(mCurrentFontSize, new Vector3(0,0,0), mHatSelectionMenuOpeningTimer);
                    }

                    if (mHatSelectionMenuOpeningTimer >= 1)
                    {
                        mMenuToggleActivated = false;
                    }
                }
            }
        }

    }


    public void ToggleHatSelectionMenu()
    {

        for (int i = 0; i < mCircleList.Count; i++)
        {
            mCurrentPositons[i] = mCircleListTransforms[i].anchoredPosition3D;
            mCurrentSize = mCircleListTransforms[i].sizeDelta;
            mCurrentFontSize = mCircleTimerTextObjects[i].localScale;
        }

        mHatSelectionActive = !mHatSelectionActive;
        mHatSelectionMenuOpeningTimer = 0.0f;
        mMenuToggleActivated = true;

    }


}