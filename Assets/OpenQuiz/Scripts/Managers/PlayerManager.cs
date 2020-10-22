using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    private PlayerData playerData;
    public Text playerNameText;
    public Text playerScoreText;
    public Text playerMoneyText;

    public InputField input;
    public GameObject loginMenu;
    public GameObject loginWarning;

    public Text selectedCategory;

    private void Awake()
    {
        instance = this;
        
    }

    void Start()
    {
        playerData = Utils.playerData;
        if (PlayerPrefs.HasKey("Player"))
        {
            UpdateUI();
            loginMenu.SetActive(false);
        }
    }

    /// <summary>
    /// Calls from QuizConfing Manager. Inits player category inventory.
    /// </summary>
    public void FirstInitPlayer()
    {
        playerData.playerMoney = playerData.playerStartMoney; 

        if (playerData.currentCategory.categoryId.Equals(0)) 
        {
            playerData.currentCategory = playerData.playerCategoryInventories[0];
        }

        selectedCategory.text = playerData.currentCategory.categoryName;
        UpdateUI();
    }

    public void UpdateUI()
    {
        playerNameText.text = playerData.playerName;
        playerScoreText.text = playerData.playerScore.ToString();
        playerMoneyText.text = playerData.playerMoney.ToString();
        selectedCategory.text = playerData.currentCategory.categoryName;
    }

    // UI Method. Hooks up from editor
    public void UIMOnUserNameCreated()
    {
        //checks if there is a string entered
        if (string.IsNullOrEmpty(input.text))
        {
            AudioManager.instance.UIMErrorSound();
            loginWarning.SetActive(true);
            return;
        }

        playerData.playerName = input.text;

        //write to prefs the player has created
        if (!PlayerPrefs.HasKey("Player"))
        {
            PlayerPrefs.SetString("Player", playerData.playerName);
        }
        
        UpdateUI();
        loginMenu.SetActive(false);
    }

}
