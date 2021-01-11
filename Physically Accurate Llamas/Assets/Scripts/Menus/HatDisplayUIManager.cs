using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatDisplayUIManager : MonoBehaviour
{

    [SerializeField] private GameObject mHatDisplayGUIObject;

    public void ActivateSelectedHatUI()
    {
        mHatDisplayGUIObject.SetActive(true);
    }

}