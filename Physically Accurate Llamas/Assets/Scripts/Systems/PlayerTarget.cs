using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTarget: MonoBehaviour
{

    #region Singleton

    public static PlayerTarget instance;


    private void Awake()
    {
        instance = this;
    }

    #endregion

    public GameObject mPlayer;

}
