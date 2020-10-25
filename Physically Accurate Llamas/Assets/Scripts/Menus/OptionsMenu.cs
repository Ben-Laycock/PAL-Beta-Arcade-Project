using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    [SerializeField] private float mMenuSlideSpeedModifier = 1.0f;

    [SerializeField] private GameObject mMainMenuObject;
    [SerializeField] private GameObject mOptionsMenuObject;

    private float mMenuMoveTimer = 0.0f;
    private bool mSwitchToMainMenu = false;

    private Vector3 mMainMenuOriginalPosition;
    private Vector3 mOptionsMenuOriginalPosition;

    private RectTransform mMainMenuObjectRectTransform;
    private RectTransform mOptionsMenuObjectRectTransform;


    private void Start()
    {

        mMainMenuObjectRectTransform = mMainMenuObject.GetComponent<RectTransform>();
        mOptionsMenuObjectRectTransform = mOptionsMenuObject.GetComponent<RectTransform>();

        mMainMenuOriginalPosition = mMainMenuObjectRectTransform.position;
        mOptionsMenuOriginalPosition = mOptionsMenuObjectRectTransform.position;

    }


    public void OnBackButtonPressed()
    {

        if (!mSwitchToMainMenu)
        {
            mMenuMoveTimer = 0.0f;
            mSwitchToMainMenu = true;
        }

    }


    private void Update()
    {

        if (mSwitchToMainMenu)
        {

            mMenuMoveTimer += Time.deltaTime * mMenuSlideSpeedModifier;

            mMainMenuObjectRectTransform.position = Vector3.Lerp(mMainMenuOriginalPosition + new Vector3(-Screen.width, 0, 0), mMainMenuOriginalPosition, Mathf.Clamp(mMenuMoveTimer, 0.0f, 1.0f));
            mOptionsMenuObjectRectTransform.position = Vector3.Lerp(mOptionsMenuOriginalPosition+ new Vector3(-Screen.width, 0, 0), mOptionsMenuOriginalPosition, Mathf.Clamp(mMenuMoveTimer, 0.0f, 1.0f));

            if ((mMainMenuObjectRectTransform.position - mMainMenuOriginalPosition).magnitude <= 0.05)
            {
                mSwitchToMainMenu = false;
            }

        }

    }

}