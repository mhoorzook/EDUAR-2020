using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerData", menuName = "Quiz/Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Player")]
    public string playerName;
    public int playerMoney;
    public int playerScore;
    
    [Header("Player Score Options")]
    public int playerStartMoney;
    public int trueFalseBaseScore;
    public int multipleBaseScore;
    public int purcheseCategoryPrice;
    
    [Header("Returns true after first init")]
    public bool isCategoriesCreated;

    [Header("Token Options")]
    public string token;
    public bool useToken;
    public DateTime tokenRequestTime;

    [Header("Default or Selected Category")]
    public PlayerCategoryInventory currentCategory = null;

    [Header("Created Purchesable Categories")]
    public List<PlayerCategoryInventory> playerCategoryInventories = new List<PlayerCategoryInventory>();

    /// <summary>
    /// Transaction money to player
    /// </summary>
    /// <param name="amount"></param>
    public void SpendMoney(int amount)
    {
        if (amount <= playerMoney)
        {
            playerMoney -= amount;
        }
    }
}
