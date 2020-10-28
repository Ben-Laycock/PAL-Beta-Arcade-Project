using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour 
{

    [SerializeField] private bool mLoadToTestingLevel = false;

    [SerializeField] private float mMenuSlideSpeedModifier = 1.0f;

    [SerializeField] private GameObject mMainMenuObject = null;
    [SerializeField] private GameObject mOptionsMenuObject = null;

    private float mMenuMoveTimer = 0.0f;
    private bool mSwitchToOptionsMenu = false;

    private Vector3 mMainMenuOriginalPosition = Vector3.zero;
    private Vector3 mOptionsMenuOriginalPosition = Vector3.zero;

    private RectTransform mMainMenuObjectRectTransform = null;
    private RectTransform mOptionsMenuObjectRectTransform = null;


    private void Start()
    {

        mMainMenuObjectRectTransform = mMainMenuObject.GetComponent<RectTransform>();
        mOptionsMenuObjectRectTransform = mOptionsMenuObject.GetComponent<RectTransform>();

        mMainMenuOriginalPosition = mMainMenuObjectRectTransform.position;
        mOptionsMenuOriginalPosition = mOptionsMenuObjectRectTransform.position;

    }


    public void OnPlayButtonPressed()
    {

        if(mLoadToTestingLevel)
        {
            SceneManager.LoadSceneAsync("MTL", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadSceneAsync("Level1", LoadSceneMode.Single);
        }

    }


    public void OnOptionsButtonPressed()
    {

        if(!mSwitchToOptionsMenu)
        {
            mMenuMoveTimer = 0.0f;
            mSwitchToOptionsMenu = true;
        }

    }


    public void OnExitButtonPressed()
    {

        Application.Quit();

    }


    private void Update()
    {
        
        if(mSwitchToOptionsMenu)
        {

            mMenuMoveTimer += Time.deltaTime * mMenuSlideSpeedModifier;

            mMainMenuObjectRectTransform.position = Vector3.Lerp(mMainMenuOriginalPosition, mMainMenuOriginalPosition + new Vector3(-Screen.width, 0, 0), Mathf.Clamp(mMenuMoveTimer, 0.0f, 1.0f));
            mOptionsMenuObjectRectTransform.position = Vector3.Lerp(mOptionsMenuOriginalPosition, mOptionsMenuOriginalPosition + new Vector3(-Screen.width, 0, 0), Mathf.Clamp(mMenuMoveTimer, 0.0f, 1.0f));

            if((mMainMenuObjectRectTransform.position - (mMainMenuOriginalPosition + new Vector3(-Screen.width, 0, 0))).magnitude <= 0.05)
            {
                mSwitchToOptionsMenu = false;
            }

        }

    }

}