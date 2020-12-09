using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouchGameOver : MonoBehaviour
{

    [SerializeField] private GameObject mPauseMenuObject;
    private PauseMenu mPauseMenu;

    private void Start()
    {
        mPauseMenu = mPauseMenuObject.GetComponent<PauseMenu>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "GameOverToggleObject")
        {
            mPauseMenu.SwitchToGameOverScreen();
        }
    }

}