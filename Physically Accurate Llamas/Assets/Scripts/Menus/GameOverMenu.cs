using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    
    [SerializeField] private GameObject mGameOverScreen;

    public void ToggleGameOverOn()
    {
        mGameOverScreen.SetActive(true);
    }

    public void OnPlayAgainButtonPressed()
    {
        SceneManager.LoadScene("DanBlockout", LoadSceneMode.Single);
    }

    public void OnReturnToMainMenuButtonPressed()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

}