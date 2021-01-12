using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    
    [SerializeField] private GameObject mGameOverScreen;
    [SerializeField] private GameObject mCollectableUIManager;

    [SerializeField] private GameObject mCoinCollectableQuantity;
    [SerializeField] private GameObject mLlamaCageCollectableQuantity;

    [SerializeField] private GameObject mPercentageText;

    private Text mCoinCollectableQuantityText;
    private Text mLlamaCageCollectableQuantityText;

    private Text mPercentageTextText;

    private CollectableUIManager mCollectableUIManagerScript;

    private void Start()
    {
        mCollectableUIManager = GameObject.Find("CollectableUICanvas").transform.Find("CollectableGUI").Find("CollectableUIManager").gameObject;

        mCollectableUIManagerScript = mCollectableUIManager.GetComponent<CollectableUIManager>();

        mCoinCollectableQuantityText = mCoinCollectableQuantity.GetComponent<Text>();
        mLlamaCageCollectableQuantityText = mLlamaCageCollectableQuantity.GetComponent<Text>();

        mPercentageTextText = mPercentageText.GetComponent<Text>();
    }

    public void ToggleGameOverOn()
    {
        SetValues();
        mGameOverScreen.SetActive(true);
    }

    public void SetValues()
    {
        mCoinCollectableQuantityText.text = mCollectableUIManagerScript.GetCollectableByName("Coin").GetCurrentQuantity().ToString();
        mLlamaCageCollectableQuantityText.text = mCollectableUIManagerScript.GetCollectableByName("CagedLlama").GetCurrentQuantity().ToString() + "/" + mCollectableUIManagerScript.GetCollectableByName("CagedLlama").GetMaxQuantity().ToString();


        int Percentage = (mCollectableUIManagerScript.GetCollectableByName("CagedLlama").GetCurrentQuantity() / mCollectableUIManagerScript.GetCollectableByName("CagedLlama").GetMaxQuantity()) * 100;

        mPercentageTextText.text = "LEVEL " + Percentage.ToString() + "% COMPLETE!";
    }

    public void OnPlayAgainButtonPressed()
    {
        AudioSystem.Instance.PlaySound("SliderClick", 1f);
        SceneManager.LoadScene("DanBlockout", LoadSceneMode.Single);
    }

    public void OnReturnToMainMenuButtonPressed()
    {
        AudioSystem.Instance.PlaySound("SliderClick", 1f);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

}