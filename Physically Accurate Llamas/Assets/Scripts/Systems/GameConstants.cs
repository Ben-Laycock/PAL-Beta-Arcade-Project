using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConstants : MonoBehaviour
{

    private static GameConstants sInstance = null;

    // Get Singleton Instance
    public static GameConstants Instance
    {

        get
        {
            return sInstance;
        }

    }


    private void Awake()
    {

        if (null != sInstance && this != sInstance)
            Destroy(this.gameObject);

        sInstance = this;
        DontDestroyOnLoad(this.gameObject);

    }


    [SerializeField] private string mHorizontalInput = "Horizontal";
    public string HorizontalInput
    {
        get { return mHorizontalInput; }
        set { mHorizontalInput = value; }
    }


    [SerializeField] private string mVerticalInput = "Vertical";
    public string VerticalInput
    {
        get { return mVerticalInput; }
        set { mVerticalInput = value; }
    }


    [SerializeField] private string mHorizontalLookInput = "HorizontalLook";
    public string HorizontalLookInput
    {
        get { return mHorizontalLookInput; }
        set { mHorizontalLookInput = value; }
    }


    [SerializeField] private string mVerticalLookInput = "VerticalLook";
    public string VerticalLookInput
    {
        get { return mVerticalLookInput; }
        set { mVerticalLookInput = value; }
    }


    [SerializeField] private Vector3 mGravityDirection = Vector3.down;
    public Vector3 GravityDirection
    {
        get { return mGravityDirection; }
        set { mGravityDirection = value; }
    }

}
