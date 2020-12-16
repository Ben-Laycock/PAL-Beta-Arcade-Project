using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour 
{

    [Header("Controller Navigation Selections")]
    [SerializeField] private GameObject mMainMenuFirstButton, mOptionsFirstButton, mOptionsClosedButton;
    [SerializeField] private GameObject mButtonHintKey;

    [Header("MainMenuValues")]
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

    public GameObject GetOptionsClosedButton()
    {
        return mOptionsClosedButton;
    }

    private void Start()
    {

        mMainMenuObjectRectTransform = mMainMenuObject.GetComponent<RectTransform>();
        mOptionsMenuObjectRectTransform = mOptionsMenuObject.GetComponent<RectTransform>();

        mMainMenuOriginalPosition = mMainMenuObjectRectTransform.position;
        mOptionsMenuOriginalPosition = mOptionsMenuObjectRectTransform.position;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mMainMenuFirstButton);

    }


    public void OnPlayButtonPressed()
    {

        if(mLoadToTestingLevel)
        {
            SceneManager.LoadSceneAsync("MTL", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadSceneAsync("DanBlockout", LoadSceneMode.Single);
        }

    }


    public void OnOptionsButtonPressed()
    {

        if(!mSwitchToOptionsMenu)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(mOptionsFirstButton);

            mMenuMoveTimer = 0.0f;
            mSwitchToOptionsMenu = true;
        }

    }


    public void OnExitButtonPressed()
    {

        Application.Quit();

    }

    private GameObject prevCurrent;

    private void Update()
    {

        if (prevCurrent != EventSystem.current.currentSelectedGameObject)
        {
            prevCurrent = EventSystem.current.currentSelectedGameObject;
            //Debug.Log(EventSystem.current.currentSelectedGameObject);
            //Debug.Log(EventSystem.current.GetComponent<RectTransform>().anchoredPosition3D);

            if (EventSystem.current.currentSelectedGameObject)
            {
                RectTransform EventSystemObject = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();
                RectTransform ButtonHitTransform = mButtonHintKey.GetComponent<RectTransform>();
                ButtonHitTransform.sizeDelta = new Vector2(EventSystemObject.sizeDelta.y, EventSystemObject.sizeDelta.y);
                ButtonHitTransform.anchoredPosition3D = EventSystemObject.anchoredPosition3D + new Vector3((EventSystemObject.sizeDelta.x / 2) + (ButtonHitTransform.sizeDelta.x / 2) + 5, 0, 0);
                
                if (EventSystem.current.currentSelectedGameObject.name == "VolumeSlider" || EventSystem.current.currentSelectedGameObject.name.Contains("Item"))
                {
                    mButtonHintKey.SetActive(false);

                    CheckScrollBarPosition(EventSystem.current.currentSelectedGameObject);
                }
                else
                {
                    mButtonHintKey.SetActive(true);
                }
            }
        }

        if (mSwitchToOptionsMenu)
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

    private void CheckScrollBarPosition(GameObject objectToCheck)
    {
        if (objectToCheck.name.Contains("Item"))
        {
            GameObject dropdown = objectToCheck.transform.parent.parent.parent.parent.gameObject;

            if (dropdown.name == "ResolutionDropdown")
            {
                char one = objectToCheck.name[5];
                char two = objectToCheck.name[6];

                string numberToParse = "";

                int valueToUse = 0;

                if (two == ':')
                {
                    numberToParse = numberToParse + one;
                    valueToUse = int.Parse(numberToParse);
                }
                else
                {
                    numberToParse = numberToParse + one;
                    numberToParse = numberToParse + two;
                    valueToUse = int.Parse(numberToParse);
                }

                float newValue = 1 - ((valueToUse + 1) / 18.0f);

                if (valueToUse == 0)
                {
                    newValue = 1;
                }

                //print("In Resolution Dropdown " + newValue.ToString() + "    " + valueToUse);
                objectToCheck.transform.parent.parent.parent.Find("Scrollbar").gameObject.GetComponent<Scrollbar>().value = newValue;
            }
        }
    }

}