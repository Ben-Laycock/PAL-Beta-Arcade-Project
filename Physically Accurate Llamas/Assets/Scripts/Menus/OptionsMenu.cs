using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    [Header("Menu Manager Values and Objects")]
    [SerializeField] private float mMenuSlideSpeedModifier = 1.0f;

    [SerializeField] private GameObject mMainMenuObject = null;
    [SerializeField] private GameObject mOptionsMenuObject = null;

    private float mMenuMoveTimer = 0.0f;
    private bool mSwitchToMainMenu = false;

    private Vector3 mMainMenuOriginalPosition = Vector3.zero;
    private Vector3 mOptionsMenuOriginalPosition = Vector3.zero;

    private RectTransform mMainMenuObjectRectTransform = null;
    private RectTransform mOptionsMenuObjectRectTransform = null;

    //Options Menu Drop Down Objects
    [Header("Drop Down Objects and Values")]
    [SerializeField] private GameObject mWindowStyleDropDownObject = null;
    [SerializeField] private GameObject mResolutionDropDownObject = null;

    private Dropdown mWindowStyleDropDown = null;
    private Dropdown mResolutionDropDown = null;

    [SerializeField] private List<Dropdown.OptionData> mWindowOptions = null;
    [SerializeField] private List<Dropdown.OptionData> mResolutionOptions = null;

    //Vsync bool
    private int mVSync;

    //Audio Bool
    private int mAudioEnabled;

    [Header("Toggle Button Values")]
    [SerializeField] private GameObject mAudioToggleButtonFrameObject = null;
    [SerializeField] private GameObject mVSyncToggleButtonFrameObject = null;
    private ToggleButtonScript mAudioToggleButtonScript = null;
    private ToggleButtonScript mVSyncToggleButtonScript = null;

    [Header("Audio Objects")]
    [SerializeField] private GameObject mAudioSliderObject = null;
    private Slider mAudioSliderScript = null;

    [SerializeField] private GameObject mAudioPercentageTextObject = null;
    private Text mAudioPercentageTextScript = null;

    [Header("Revert Button")]
    [SerializeField] private GameObject mRevertButtonObject = null;

    private void Start()
    {

        mMainMenuObjectRectTransform = mMainMenuObject.GetComponent<RectTransform>();
        mOptionsMenuObjectRectTransform = mOptionsMenuObject.GetComponent<RectTransform>();

        mMainMenuOriginalPosition = mMainMenuObjectRectTransform.position;
        mOptionsMenuOriginalPosition = mOptionsMenuObjectRectTransform.position;

        //Getting the Dropdown Components and Setting their initial values
        mWindowStyleDropDown = mWindowStyleDropDownObject.GetComponent<Dropdown>();
        mResolutionDropDown = mResolutionDropDownObject.GetComponent<Dropdown>();

        mWindowStyleDropDown.ClearOptions();
        mWindowStyleDropDown.AddOptions(mWindowOptions);
        //mWindowStyleDropDown.options.Add(new Dropdown.OptionData("Fullscreen"));
        //mWindowStyleDropDown.options.Add(new Dropdown.OptionData("Windowed"));
        //mWindowStyleDropDown.options.Add(new Dropdown.OptionData("Borderless"));
        mWindowStyleDropDown.value = PlayerPrefs.GetInt("SelectedWindowValue", 0);

        mResolutionDropDown.ClearOptions();
        mResolutionDropDown.AddOptions(mResolutionOptions);
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("1920x1080 16:9"));
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("1768x992"));
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("1680x1050 8:5"));
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("1600x1024 25:16"));
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("1600x900 16:9"));
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("1366x768"));
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("1360x768 85:48"));
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("1280x1024 4:5"));
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("1280x960 4:3"));
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("1280x800 8:5"));
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("1280x720 16:9"));
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("1176x664"));
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("1152x864 4:3"));
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("1024x768 4:3"));
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("800x600 4:3"));
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("720x576 5:4"));
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("720x480 3:2"));
        //mResolutionDropDown.options.Add(new Dropdown.OptionData("640x480 4:3"));
        mResolutionDropDown.value = PlayerPrefs.GetInt("SelectedResolutionValue", 0);

        //Getting Settings
        mVSync = PlayerPrefs.GetInt("VSyncEnabled", 1);
        mAudioEnabled = PlayerPrefs.GetInt("AudioEnabled", 0);

        mVSyncToggleButtonScript = mVSyncToggleButtonFrameObject.GetComponent<ToggleButtonScript>();
        mAudioToggleButtonScript = mAudioToggleButtonFrameObject.GetComponent<ToggleButtonScript>();

        mAudioToggleButtonScript.SetToggleState(mAudioEnabled);
        mVSyncToggleButtonScript.SetToggleState(mVSync);

        Debug.Log(mAudioEnabled + "   " + mVSync);

        //Getting the Slider Script Component from the Slider Object
        mAudioSliderScript = mAudioSliderObject.GetComponent<Slider>();

        int sliderValueFromPlayerPrefs = PlayerPrefs.GetInt("AudioVolumeSliderValue", 100);

        mAudioSliderScript.SetValueWithoutNotify((float)(sliderValueFromPlayerPrefs / 100.0f));

        mAudioSliderScript.onValueChanged.AddListener(delegate
        {

            SliderValueChanged();

        });

        mAudioPercentageTextScript = mAudioPercentageTextObject.GetComponent<Text>();
        mAudioPercentageTextScript.text = sliderValueFromPlayerPrefs.ToString() + "%";

    }


    private void SliderValueChanged()
    {

        PlayerPrefs.SetInt("AudioVolumeSliderValue", (int)(mAudioSliderScript.value * 100));
        mAudioPercentageTextScript.text = ((int)(mAudioSliderScript.value * 100)).ToString() + "%";

    }


    public void OnBackButtonPressed()
    {

        //mWindowStyleDropDown.value = PlayerPrefs.GetInt("SelectedWindowValue", 0);
        //mResolutionDropDown.value = PlayerPrefs.GetInt("SelectedResolutionValue", 0);

        RevertSettings();

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

    public void OnApplyButtonPressed()
    {

        int vSyncBoolFromButton = mVSyncToggleButtonScript.GetButtonState() ? 0 : 1;

        //Checks if anything has been changed
        if (mWindowStyleDropDown.value == PlayerPrefs.GetInt("SelectedWindowValue", -1) &&
            mResolutionDropDown.value == PlayerPrefs.GetInt("SelectedResolutionValue", -1) &&
            vSyncBoolFromButton == PlayerPrefs.GetInt("VSyncEnabled", -1))
        {
            return;
        }

        PlayerPrefs.SetInt("OldSelectedWindowValue", PlayerPrefs.GetInt("SelectedWindowValue", 0));
        PlayerPrefs.SetInt("SelectedWindowValue", mWindowStyleDropDown.value);

        PlayerPrefs.SetInt("OldSelectedResolutionValue", PlayerPrefs.GetInt("SelectedResolutionValue", 0));
        PlayerPrefs.SetInt("SelectedResolutionValue", mResolutionDropDown.value);

        PlayerPrefs.SetInt("OldVSyncEnabled", PlayerPrefs.GetInt("VSyncEnabled"));
        PlayerPrefs.SetInt("VSyncEnabled", vSyncBoolFromButton);

        FullScreenMode mode = FullScreenMode.ExclusiveFullScreen;

        switch (mWindowStyleDropDown.options[mWindowStyleDropDown.value].text)
        { 
            case "Fullscreen":
                mode = FullScreenMode.FullScreenWindow;
                break;

            case "Windowed":
                mode = FullScreenMode.Windowed;
                break;

            case "Borderless":
                mode = FullScreenMode.ExclusiveFullScreen;
                break;

            default:
                mode = FullScreenMode.Windowed;
                break;
        }

        string[] resolutionArrayInfo = mResolutionDropDown.options[mResolutionDropDown.value].text.Split(' ');
        string[] xyRedArray = resolutionArrayInfo[0].Split('x');

        int refreshRate = 0;

        if(PlayerPrefs.GetInt("VSyncEnabled", 1) == 0)
        {
            refreshRate = 60;
        }

        Screen.SetResolution(int.Parse(xyRedArray[0]), int.Parse(xyRedArray[1]), mode, refreshRate);

        mRevertButtonObject.SetActive(true);

    }


    private void RevertSettings()
    {

        mWindowStyleDropDown.SetValueWithoutNotify(PlayerPrefs.GetInt("SelectedWindowValue", 0));
        mResolutionDropDown.SetValueWithoutNotify(PlayerPrefs.GetInt("SelectedResolutionValue", 0));

        int vSyncValue = PlayerPrefs.GetInt("VSyncEnabled", 1);
        int vSyncBoolFromButton = mVSyncToggleButtonScript.GetButtonState() ? 0 : 1;

        if (vSyncValue != vSyncBoolFromButton)
        {

            mVSyncToggleButtonScript.ToggleState();

        }

    }


    public void OnRevertButtonPressed()
    {

        PlayerPrefs.SetInt("SelectedWindowValue", PlayerPrefs.GetInt("OldSelectedWindowValue", 0));
        PlayerPrefs.SetInt("SelectedResolutionValue", PlayerPrefs.GetInt("OldSelectedResolutionValue", 0));

        PlayerPrefs.SetInt("VSyncEnabled", PlayerPrefs.GetInt("OldVSyncEnabled"));

        RevertSettings();

        FullScreenMode mode = FullScreenMode.ExclusiveFullScreen;

        switch (mWindowStyleDropDown.options[mWindowStyleDropDown.value].text)
        {
            case "Fullscreen":
                mode = FullScreenMode.FullScreenWindow;
                break;

            case "Windowed":
                mode = FullScreenMode.Windowed;
                break;

            case "Borderless":
                mode = FullScreenMode.ExclusiveFullScreen;
                break;

            default:
                mode = FullScreenMode.Windowed;
                break;
        }

        string[] resolutionArrayInfo = mResolutionDropDown.options[mResolutionDropDown.value].text.Split(' ');
        string[] xyRedArray = resolutionArrayInfo[0].Split('x');

        int refreshRate = 0;

        if (PlayerPrefs.GetInt("VSyncEnabled", 1) == 0)
        {
            refreshRate = 60;
        }

        Screen.SetResolution(int.Parse(xyRedArray[0]), int.Parse(xyRedArray[1]), mode, refreshRate);


        mRevertButtonObject.SetActive(false);

    }


    public void OnAudioTogglePressed()
    {
        int valueToSet = 0;

        if(PlayerPrefs.GetInt("AudioEnabled", 0) == 0)
        {
            valueToSet = 1;
        }

        PlayerPrefs.SetInt("AudioEnabled", valueToSet);

        mAudioToggleButtonScript.ToggleState();

    }

    public void OnVsyncTogglePressed()
    {
        //int valueToSet = 0;

        //if (PlayerPrefs.GetInt("VSyncEnabled", 0) == 0)
        //{
        //    valueToSet = 1;
        //}

        mVSyncToggleButtonScript.ToggleState();

    }

}