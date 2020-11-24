using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    
    [SerializeField] private GameObject mPauseFirstButton, mOptionsFirstButton, mOptionsClosedButton;
    [SerializeField] private GameObject mButtonHintKey;
    
    [Header("Is Menu Paused")]
    [SerializeField] private bool mGamePaused = false;
    private bool mButtonDebounce = false;
    [SerializeField] private GameObject mMenuBackground;

    [Header("Menu Objects")]
    [SerializeField] private float mMenuSlideSpeedModifier = 1.0f;

    [SerializeField] private GameObject mMenuHandler = null;
    private OptionsMenu mOptionMenuScript = null;
    [SerializeField] private GameObject mPauseMenuObject = null;
    [SerializeField] private GameObject mOptionsMenuObject = null;

    private float mMenuMoveTimer = 0.0f;
    private float mMenuState = 0;
    private bool mSwitchToOptionsMenu = false;

    private Vector3 mPauseMenuOriginalPosition = Vector3.zero;
    private Vector3 mOptionsMenuOriginalPosition = Vector3.zero;

    private RectTransform mPauseMenuObjectRectTransform = null;
    private RectTransform mOptionsMenuObjectRectTransform = null;

    private void Start()
    {

        mPauseMenuObjectRectTransform = mPauseMenuObject.GetComponent<RectTransform>();
        mOptionsMenuObjectRectTransform = mOptionsMenuObject.GetComponent<RectTransform>();

        mPauseMenuOriginalPosition = mPauseMenuObjectRectTransform.position;
        mOptionsMenuOriginalPosition = mOptionsMenuObjectRectTransform.position;

        mOptionMenuScript = mMenuHandler.GetComponent<OptionsMenu>();

        mPauseMenuObject.SetActive(false);
        mOptionsMenuObject.SetActive(false);
        mButtonHintKey.SetActive(false);

        mMenuBackground.SetActive(false);

    }

    private GameObject prevCurrent;
    private float mCloseMenuButtonDebounceTimer = 1.0f;

    private void Update()
    {

        GameConstants.Instance.GamePaused = mGamePaused;

        if(prevCurrent != EventSystem.current.currentSelectedGameObject)
        {
            if(mGamePaused)
            {
                prevCurrent = EventSystem.current.currentSelectedGameObject;
                //Debug.Log(EventSystem.current.currentSelectedGameObject);
                //Debug.Log(EventSystem.current.GetComponent<RectTransform>().anchoredPosition3D);
                if(EventSystem.current.currentSelectedGameObject)
                {
                    RectTransform EventSystemObject = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();
                    RectTransform ButtonHitTransform = mButtonHintKey.GetComponent<RectTransform>();
                    ButtonHitTransform.sizeDelta = new Vector2(EventSystemObject.sizeDelta.y, EventSystemObject.sizeDelta.y);
                    ButtonHitTransform.anchoredPosition3D = EventSystemObject.localPosition + new Vector3((EventSystemObject.sizeDelta.x / 2) + (ButtonHitTransform.sizeDelta.x / 2) + 5, 0, 0);
                
                    if (mGamePaused)
                    {
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
                    else
                    {
                        mButtonHintKey.SetActive(false);
                    }
                }
            } 
        }

        if(Input.GetAxisRaw("Pause") > 0 && !mButtonDebounce)
        {
            mButtonDebounce = true;
            mCloseMenuButtonDebounceTimer = 0.0f;

            if(mMenuState == 0)
            {
                mGamePaused = !mGamePaused;
                CorrectMenu(mGamePaused);

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(mPauseFirstButton);

                if(!mGamePaused)
                {
                    mButtonHintKey.SetActive(false);
                    mMenuBackground.SetActive(false);
                }
                else
                {
                    mMenuBackground.SetActive(true);
                }

            }
            else //Menu is in Options Menu
            {
                mOptionMenuScript.OnBackButtonPressed();
                SetMenuState(0);
            }

        } 

        if(mButtonDebounce)
        {
            mCloseMenuButtonDebounceTimer += Time.deltaTime;
            if(mCloseMenuButtonDebounceTimer >= 0.1f)
            {
                mButtonDebounce = false;
            }
        }

        if (mSwitchToOptionsMenu)
        {

            mMenuMoveTimer += Time.deltaTime * mMenuSlideSpeedModifier;

            mPauseMenuObjectRectTransform.position = Vector3.Lerp(mPauseMenuOriginalPosition, mPauseMenuOriginalPosition + new Vector3(-Screen.width, 0, 0), Mathf.Clamp(mMenuMoveTimer, 0.0f, 1.0f));
            mOptionsMenuObjectRectTransform.position = Vector3.Lerp(mOptionsMenuOriginalPosition, mOptionsMenuOriginalPosition + new Vector3(-Screen.width, 0, 0), Mathf.Clamp(mMenuMoveTimer, 0.0f, 1.0f));

            if ((mPauseMenuObjectRectTransform.position - (mPauseMenuOriginalPosition + new Vector3(-Screen.width, 0, 0))).magnitude <= 0.05)
            {
                mSwitchToOptionsMenu = false;
            }

        }


    }

    private void CheckScrollBarPosition(GameObject objectToCheck)
    {
        if(objectToCheck.name.Contains("Item"))
        {
            GameObject dropdown = objectToCheck.transform.parent.parent.parent.parent.gameObject;

            if (dropdown.name == "ResolutionDropdown")
            {
                char one = objectToCheck.name[5];
                char two = objectToCheck.name[6];

                string numberToParse = "";

                int valueToUse = 0;

                if(two == ':')
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

                if(valueToUse == 0)
                {
                    newValue = 1;
                }

                //print("In Resolution Dropdown " + newValue.ToString() + "    " + valueToUse);
                objectToCheck.transform.parent.parent.parent.Find("Scrollbar").gameObject.GetComponent<Scrollbar>().value = newValue;
            }
        }
    }

    public void OnOptionsButtonPressed()
    {

        if (!mSwitchToOptionsMenu)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(mOptionsFirstButton);

            mMenuState = 1;
            mMenuMoveTimer = 0.0f;
            mSwitchToOptionsMenu = true;
        }

    }

    public void OnResumeButtonPressed()
    {
        if (mMenuState == 0)
        {
            mGamePaused = !mGamePaused;

            CorrectMenu(mGamePaused);
            mMenuBackground.SetActive(false);
        }
    }

    public void OnReturnToMainMenuButtonPressed()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void SetMenuState(int value)
    {
        mMenuState = value;
        if(value == 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(mOptionsClosedButton);
        }
    }

    private void CorrectMenu(bool value)
    {
        if(value)
        {
            mPauseMenuObject.SetActive(true);
            mOptionsMenuObject.SetActive(true);
            mButtonHintKey.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            mPauseMenuObject.SetActive(false);
            mOptionsMenuObject.SetActive(false);
            mButtonHintKey.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

}