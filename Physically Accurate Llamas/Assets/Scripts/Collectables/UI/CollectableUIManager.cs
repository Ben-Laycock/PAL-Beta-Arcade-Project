using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CollectableUIClass
{

    [SerializeField] private string mCollectableName = "DefaultStringGameObjectPair";
    [SerializeField] private GameObject mCollectableObject = null;
    [SerializeField] private int mCurrentQuantity = 0;
    [Tooltip("If Set to 0 - There is no max quantity!")]
    [SerializeField] private int mMaxQuantity = 10;

    private Image mImage = null;
    private GameObject mTextObject = null;
    private Text mQuantity = null;
    private RectTransform mCollectbaleTransform;

    private bool mAllItemsCollected = false;

    public CollectableUIClass(string objectName, GameObject objectObject)
    {
        mCollectableName = objectName;
        mCollectableObject = objectObject;
    }


    public void InitialiseCollectableUIClass()
    {
        mImage = mCollectableObject.GetComponent<Image>();
        mTextObject = mCollectableObject.transform.Find("Quantity").gameObject;
        mQuantity = mTextObject.GetComponent<Text>();
        mCollectbaleTransform = mCollectableObject.GetComponent<RectTransform>();

        if (mMaxQuantity == 0)
        {
            mQuantity.text = mCurrentQuantity.ToString();
        }
        else
        {
            mQuantity.text = mCurrentQuantity + "/" + mMaxQuantity;
        }

    }


    public void UpdateCollectableQuantity()
    {

        if (mMaxQuantity == 0)
        {
            mQuantity.text = mCurrentQuantity.ToString();
        }
        else
        {
            mQuantity.text = mCurrentQuantity + "/" + mMaxQuantity;
        }

        UpdateCollectableVisuals();

    }

    
    public void UpdateCollectableVisuals()
    {

    }


    //----------------------------------------------------------------------------------//
    //--------------------------- Set Information To Class -----------------------------//
    //----------------------------------------------------------------------------------//
    public void IncreaseCollectableQuantity(int amount = 1)
    {
        mCurrentQuantity += amount;
        mAllItemsCollected = CheckAllItemsCollected();

        UpdateCollectableQuantity();
    }

    public void SetCollectableQuantity(int amount = 0)
    {
        mCurrentQuantity = amount;
        mAllItemsCollected = CheckAllItemsCollected();

        UpdateCollectableQuantity();
    }

    public void SetMaxQuantity(int amount = 10)
    {
        mMaxQuantity = amount;
    }

    public bool CheckAllItemsCollected()
    {
        if(mMaxQuantity == 0) { return false; }
        
        if(mCurrentQuantity == mMaxQuantity)
        {
            return true;
        }

        return false;
    }

    //----------------------------------------------------------------------------------//
    //------------------------- Get Information From Class -----------------------------//
    //----------------------------------------------------------------------------------//
    public string GetCollectableName()
    {
        return mCollectableName;
    }

    public GameObject GetCollectableObject()
    {
        return mCollectableObject;
    }

    public Image GetCollectableImage()
    {
        return mImage;
    }

    public int GetCurrentQuantity()
    {
        return mCurrentQuantity;
    }

    public int GetMaxQuantity()
    {
        return mMaxQuantity;
    }

    public bool GetAllItemsCollected()
    {
        return mAllItemsCollected;
    }

    public RectTransform GetRectTransform()
    {
        return mCollectbaleTransform;
    }

}


public class CollectableUIManager : MonoBehaviour
{

    [Header("Collectable Values")]
    [SerializeField] private List<CollectableUIClass> mCollectableDictionary = null;
    [SerializeField] private GameObject mErrorObject = null;

    [Header("Player Object")]
    [SerializeField] private GameObject mPlayerObject = null;
    private Rigidbody mPlayerRigidBody;

    private void Start()
    {
        
        mPlayerObject = GameObject.Find("Player");
        mPlayerRigidBody = mPlayerObject.GetComponent<Rigidbody>();

        foreach(CollectableUIClass c in mCollectableDictionary)
        {
            c.InitialiseCollectableUIClass();
        }

    }

    private bool mPositionModified = false;
    private bool mShowUI = false;
    private bool mCurrentShowUI = true;
    private float mPreviousVelocity = 0;
    private float mLerpTimer = 0;

    private void Update()
    {
        
        if(Mathf.Abs(mPlayerRigidBody.velocity.magnitude - mPreviousVelocity) > 0.1)
        {
            mPreviousVelocity = mPlayerRigidBody.velocity.magnitude;
            mPositionModified = false;
            if(mPlayerRigidBody.velocity.magnitude <= 0.1)
            {
                mShowUI = true;
            }
            else
            {
                mShowUI = false;
            }
            if(mCurrentShowUI != mShowUI)
            {
                mCurrentShowUI = mShowUI;
                mLerpTimer = 0f;
            }
        }

        if(mPlayerRigidBody.velocity.magnitude <= 0.1 && !mPositionModified)
        {
            mLerpTimer += Time.deltaTime;
            if (mLerpTimer >= 1)
            {
                mLerpTimer = 1;
            }
            foreach (CollectableUIClass c in mCollectableDictionary)
            {
                c.GetRectTransform().anchoredPosition3D = new Vector3(c.GetRectTransform().anchoredPosition3D.x, SmoothMove(c.GetRectTransform().anchoredPosition3D.y, -(c.GetRectTransform().sizeDelta.y / 2 + 10), mLerpTimer), c.GetRectTransform().anchoredPosition3D.z);
            }
            if(mLerpTimer >= 1)
            {
                mPositionModified = true;
            }
        }
        else if(!mPositionModified)
        {
            mLerpTimer += Time.deltaTime;
            if (mLerpTimer >= 1)
            {
                mLerpTimer = 1;
            }
            foreach (CollectableUIClass c in mCollectableDictionary)
            {
                c.GetRectTransform().anchoredPosition3D = new Vector3(c.GetRectTransform().anchoredPosition3D.x, SmoothMove(c.GetRectTransform().anchoredPosition3D.y, (c.GetRectTransform().sizeDelta.y / 2 + 10), mLerpTimer), c.GetRectTransform().anchoredPosition3D.z);
            }
            mPositionModified = true;
            if (mLerpTimer >= 1)
            {
                mPositionModified = true;
            }
        }

    }

    public CollectableUIClass GetCollectableByName(string collectableName)
    {

        foreach(CollectableUIClass c in mCollectableDictionary)
        {
            if(collectableName == c.GetCollectableName())
            {
                return c;
            }
        }

        CollectableUIClass errorCollectable = new CollectableUIClass("Error", mErrorObject);
        Debug.Log("GetCollectableByName Error Occured - Invalid Name Provided!");

        return errorCollectable;

    }


    //Use this with care - Make sure you know which index the collectable is in before modifing any of its values
    public CollectableUIClass GetCollectableByIndex(int index)
    {

        if(index >= mCollectableDictionary.Count)
        {
            CollectableUIClass errorCollectable = new CollectableUIClass("Error", mErrorObject);
            Debug.Log("GetCollectableByName Error Occured - Invalid Name Provided!");

            return errorCollectable;
        }

        return mCollectableDictionary[index];

    }

    private float SmoothMove(float startPosX, float endPosX, float timeStep)
    {

        float position = Mathf.Lerp(startPosX, endPosX, timeStep);
        return position;

    }

}