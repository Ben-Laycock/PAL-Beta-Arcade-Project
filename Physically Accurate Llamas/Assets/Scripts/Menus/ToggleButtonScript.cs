using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonScript : MonoBehaviour
{

    //Toggle Button Colors
    [SerializeField] private Color mEnabledColor = Color.white;
    [SerializeField] private Color mDisabledColor = Color.white;

    //Canvas Object
    private GameObject mCanvasObject;

    //Button Objects and Scripts
    private GameObject mToggleButtonHandleObject = null;
    private GameObject mToggleButtonHandleTextObject = null;
    private Image mToggleButtonHandleImage = null;
    private Text mToggleButtonHandleText = null;

    //Button Toggle Values
    private bool mButtonActive = true;
    private bool mStateChanged = false;

    private float mOnPosition;
    private float mOffPosition;

    private float mLerpTimer = 0.0f;


    private void Awake()
    {

        mCanvasObject = GameObject.Find("Canvas");

        //Button Toggle Values
        mToggleButtonHandleObject = transform.GetChild(0).gameObject;
        mToggleButtonHandleTextObject = mToggleButtonHandleObject.transform.GetChild(0).gameObject;
        mToggleButtonHandleImage = mToggleButtonHandleObject.GetComponent<Image>();
        mToggleButtonHandleText = mToggleButtonHandleTextObject.GetComponent<Text>();

        //Setting Position and Handle Values
        Vector2 mHandleSize = mToggleButtonHandleObject.GetComponent<RectTransform>().sizeDelta;

        Vector2 toggleSize = gameObject.GetComponent<RectTransform>().sizeDelta;

        mOnPosition = 0 - (mHandleSize.x / 2); //+ ((toggleSize.y - mHandleSize.y) / 2)
        mOffPosition = mOnPosition * -1;

    }


    private void Update()
    {
        
        if(mStateChanged)
        {

            mLerpTimer += Time.deltaTime * 6;

            if (mButtonActive == true)
            {
                mToggleButtonHandleImage.color = SmoothColor(mDisabledColor, mEnabledColor, mLerpTimer);
                mToggleButtonHandleObject.transform.localPosition = SmoothMove(mOffPosition, mOnPosition, mLerpTimer);
                if (mLerpTimer > 0.5f)
                {
                    mToggleButtonHandleText.text = "On";
                }
            }
            else
            {
                mToggleButtonHandleImage.color = SmoothColor(mEnabledColor, mDisabledColor, mLerpTimer);
                mToggleButtonHandleObject.transform.localPosition = SmoothMove(mOnPosition, mOffPosition, mLerpTimer);
                if (mLerpTimer > 0.5f)
                {
                    mToggleButtonHandleText.text = "Off";
                }
            }

            if(mLerpTimer > 1.0f)
            {
                mLerpTimer = 0.0f;
                mStateChanged = false;
            }

        }

    }


    public void ToggleState()
    {

        mLerpTimer = 0.0f;
        mButtonActive = !mButtonActive;
        mStateChanged = true;

    }


    public void SetToggleState(int value)
    {

        if(value == 0)
        {
            mButtonActive = true;
            mToggleButtonHandleImage.color = mEnabledColor;
            mToggleButtonHandleObject.transform.localPosition = new Vector3(mOnPosition, 0f, 0f);
            mToggleButtonHandleText.text = "On";
        } 
        else if (value == 1)
        {
            mButtonActive = false;
            mToggleButtonHandleImage.color = mDisabledColor;
            mToggleButtonHandleObject.transform.localPosition = new Vector3(mOffPosition, 0f, 0f);
            mToggleButtonHandleText.text = "Off";
        }

    }


    private Vector3 SmoothMove(float startPosX, float endPosX, float timeStep)
    {

        Vector3 position = new Vector3(Mathf.Lerp(startPosX, endPosX, timeStep), 0f, 0f);
        return position;

    }


    private Color SmoothColor(Color startCol, Color endCol, float timeStep)
    {

        Color resultCol;
        resultCol = Color.Lerp(startCol, endCol, timeStep);
        return resultCol;

    }

    public bool GetButtonState()
    {

        return mButtonActive;

    }


}