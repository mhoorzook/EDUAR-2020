using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

public static class Utils
{
    public static QuizData quizData;
    public static PlayerData playerData;
    public static CategoryData categoryData;

    private const string nameKey = "name";
    private const string moneyKey = "money";
    private const string scoreKey = "score";
    private const string categoryKey = "category";
    
    public static string DecodeHtmlWithHttpUtils(this string source)
    {
        return HttpUtility.HtmlDecode(source);
    }

    private static CategoryData GetCategoryDataConfig()
    {
        try
        {
            return Resources.Load<CategoryData>("CategoryData");
        }
        catch (Exception e)
        {
            Debug.LogWarning("CategoryData file in Resources folder deleted or misplaced. Right click and create from Quiz menu.");
            throw;
        }
        
    }
    private static PlayerData GetPlayerDataConfig()
    {
        try
        {
            return Resources.Load<PlayerData>("PlayerData");
        }
        catch (Exception e)
        {
            Debug.LogWarning("PlayerData file in Resources folder deleted or misplaced. Right click and create from Quiz menu.");
            throw;
        }
        
    }

    private static QuizData GetQuizDataConfig()
    {
        try
        {
            return Resources.Load<QuizData>("QuizData");
        }
        catch (Exception e)
        {
            Debug.LogWarning("QuizData file in Resources folder deleted or misplaced. Right click and create from Quiz menu.");
            throw;
        }
        
    }

    public static void InitConfig()
    {
        quizData = GetQuizDataConfig();
        playerData = GetPlayerDataConfig();
        categoryData = GetCategoryDataConfig();

        InitPlayerPrefs();
    }

    private static void InitPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(nameKey))
        {
            playerData.playerName = PlayerPrefs.GetString(nameKey);
        }

        if (PlayerPrefs.HasKey(scoreKey))
        {
            playerData.playerScore = PlayerPrefs.GetInt(scoreKey);
        }

        if (PlayerPrefs.HasKey(moneyKey))
        {
            playerData.playerMoney = PlayerPrefs.GetInt(moneyKey);
        }
        
        if (PlayerPrefs.HasKey(categoryKey))
        {
            string categories = PlayerPrefs.GetString(categoryKey);
            
            //get saved string as an array of string first
            // ',' used as a separator when save process
            var ids = categories.Split(',');
            //remove last item because it's a empty string coming from split
            ids = ids.Take(ids.Count() - 1).ToArray();

            foreach (var id in ids)
            {
                var convertedInt = Convert.ToInt32(id);
                var category = playerData.playerCategoryInventories.Find(x => x.categoryId == convertedInt);
                category.isPurchesed = true;
            }
            
        }
    }

    public static void SavePlayerDataToPlayerPref()
    {
        PlayerPrefs.SetString(nameKey, playerData.playerName);
        PlayerPrefs.SetInt(moneyKey, playerData.playerMoney);
        PlayerPrefs.SetInt(scoreKey, playerData.playerScore);
        
        //first find purchesed categories
        //make a collection of them
        string purchesedCategories = "";
        foreach (var category in playerData.playerCategoryInventories)
        {
            //separate them with a special symbol
            if (category.isPurchesed)
            {
                purchesedCategories += (category.categoryId.ToString() + "," );
            }
        }
        
        //save as a string to playerprefs
        PlayerPrefs.SetString(categoryKey, purchesedCategories);
        
    }

    /// <summary>
    /// Get prefab from resources folder
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject GetPrefab(string name)
    {
        return Resources.Load<GameObject>("Prefabs/" + name);
    }

    public static void IncrementPlayerScore(int score)
    {
        playerData.playerScore += score;
    }
    
    public static void IncrementPlayerMoney(int money)
    {
        playerData.playerMoney += money;
    }

    public static void SetTokenToPlayer(string token)
    {
        playerData.token = token;
    }
    
    
    /// <summary>
    /// Save the time that token requested.
    /// </summary>
    public static void SaveRequestedTokenTime()
    {
        playerData.tokenRequestTime = DateTime.Now;
    }

    /// <summary>
    /// Since opentdb api token is only valid for 6 six hours, check if the difference more than 6 hours.
    /// </summary>
    /// <returns></returns>
    public static bool IsTokenValid()
    {
        if (playerData.token.Equals(null) || playerData.token.Equals(""))
        {
            return false;
        }
        var currentTime = DateTime.Now;
        var lastSavedTokenTime = playerData.tokenRequestTime;

        Debug.Log(currentTime.Subtract(lastSavedTokenTime).Minutes);
        return (currentTime.Subtract(lastSavedTokenTime).Minutes < 360);
    }
}
