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

        if(mMaxQuantity == 0)
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

    }


    //----------------------------------------------------------------------------------//
    //--------------------------- Set Information To Class -----------------------------//
    //----------------------------------------------------------------------------------//
    public void IncreaseCollectableQuantity(int amount = 1)
    {
        mCurrentQuantity += amount;
        UpdateCollectableQuantity();
    }

    public void SetCollectableQuantity(int amount = 0)
    {
        mCurrentQuantity = amount;
        UpdateCollectableQuantity();
    }

    public void SetMaxQuantity(int amount = 10)
    {
        mMaxQuantity = amount;
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


}


public class CollectableUIManager : MonoBehaviour
{

    [SerializeField] private List<CollectableUIClass> mCollectableDictionary = null;
    [SerializeField] private GameObject mErrorObject = null;


    private void Start()
    {
        
        foreach(CollectableUIClass c in mCollectableDictionary)
        {
            c.InitialiseCollectableUIClass();
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


}