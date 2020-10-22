using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class QuizConfigManager : MonoBehaviour
{
    public static QuizConfigManager instance;

    private int questionAmount = 10;

    [Header("Canvas Elements")]
    public GameObject categoryPanel;
    public GameObject categoryParent;
    public GameObject animPanel;
    public Transform mainMenuParent;
    
    [Header("Dropdown Elements")]
    public Dropdown typeDropdown;
    public Dropdown difficultyDropdown;
    public Dropdown amountDropdown;

    private PlayerData   playerData;
    private CategoryData categoryData;

    //configure this string to make use get http url
    private string url;
    
    //base urls
    private const string baseURL = "https://opentdb.com/api.php?";
    private const string categoryUrl = "https://opentdb.com/api_category.php";
    private const string tokenRequestUrl = "https://opentdb.com/api_token.php?command=request";
    
    //ingredients to use opentdb api
    private const string categoryTypeUrlAdd = "&category=";
    private const string typeTrueFalseURLadd = "&type=boolean";
    private const string typeMultipleURLadd = "&type=multiple";
    private const string diffEasyURLadd = "&difficulty=easy";
    private const string diffMediumURLadd = "&difficulty=medium";
    private const string diffHardURLadd = "&difficulty=hard";
    private const string amountURLadd = "amount=";
    private const string tokenURLadd = "&token=";

    private void Awake()
    {
        instance = this;
        //make utils configuration from here
        Utils.InitConfig();
    }

    private void Start()
    {
        //get references
        playerData = Utils.playerData;
        categoryData = Utils.categoryData;
        //
        RequestCategories();
        RequestToken();
    }


    private void InitQuizRequest()
    {
        //configuration of url based on user's quiz configuration on scene.
        SetConfigByAmountOfQuestion();
        SetConfigbyDifficulty();
        SetConfigByCategory();
        SetConfigbyType();
        SetConfigbyToken();

        GetResponseByConfig();

        Debug.Log(url);
    }
    
    private void SetConfigByAmountOfQuestion()
    {
        switch (amountDropdown.value)
        {
            case 1:
                questionAmount = 15;
                break;

            case 2:
                questionAmount = 20;
                break;

            default:
                questionAmount = 10;
                break;
        }

        url = baseURL + amountURLadd + questionAmount.ToString();
    }

    private void SetConfigbyType()
    {
        switch (typeDropdown.value)
        {
            case 1:
                url = url + typeMultipleURLadd;

                break;

            case 2:
                url = url + typeTrueFalseURLadd;

                break;
            default:
                break;
        }
    }

    private void SetConfigbyDifficulty()
    {
        switch (difficultyDropdown.value)
        {
            case 1:
                url = url + diffEasyURLadd;

                break;

            case 2:
                url = url + diffMediumURLadd;

                break;

            case 3:
                url = url + diffHardURLadd;
                break;

            default:
                break;
        }
    }

    private void SetConfigByCategory()
    {
        if (playerData.currentCategory == null)
        {
            return;
        }
        
        url = url + categoryTypeUrlAdd +
                    playerData.currentCategory.categoryId.ToString();
    }
    
    private void SetConfigbyToken()
    {
        if (playerData.useToken)
        {
            url = url + tokenURLadd + playerData.token;
        }
    }
    
    //make rest call by user configured settings
    private void GetResponseByConfig()
    {
        StartCoroutine(RestClientAPI.instance.GetQuiz(url, QuizGetResponseCompleteCallback));
    }

    private void QuizGetResponseCompleteCallback()
    {
        if (Utils.quizData.quizzes.Length > 0)
        {
            StartCoroutine(SceneTransition(5.5f, "InGame"));
        }
        else
        {
            var errorPopup = Resources.Load<GameObject>("Prefabs/StartError");
            Instantiate(errorPopup, mainMenuParent);
        }
    }

    //start new scene transition
    private IEnumerator SceneTransition(float transitionTime, string sceneName)
    {
        animPanel.SetActive(true);
        
        //wait for anim complete
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }

    

    //get categories if category data corrupted or deleted
    public void PopulateCategoryDataThenInitPlayer()
    {
        if(!playerData.isCategoriesCreated)
        {
            foreach (var item in categoryData.categories)
            {
                PlayerCategoryInventory pci = new PlayerCategoryInventory(item.id, item.name);
                playerData.playerCategoryInventories.Add(pci);
            }

            playerData.isCategoriesCreated = true;
            PlayerManager.instance.FirstInitPlayer();
        }
    }

   
    //make rest call for categories
    private void RequestCategories()
    {
        StartCoroutine(RestClientAPI.instance.GetCategories(categoryUrl, PopulateCategoryDataThenInitPlayer));
    }
    
    private void RequestToken()
    {
        if (!Utils.IsTokenValid())
        {
            StartCoroutine(RestClientAPI.instance.GetToken(tokenRequestUrl, (() => {})));
        }
    }

    //sets current category
    public void OnUserSelectedCategory(PlayerCategoryInventory pci)
    {
        playerData.currentCategory = pci;

        var categoriesToDestroy = categoryPanel.GetComponentsInChildren<CategoryButton>();

        foreach (var item in categoriesToDestroy)
        {
            Destroy(item.gameObject);
        }

        categoryPanel.SetActive(false);
        PlayerManager.instance.UpdateUI();
    }
    
    //UI Method. Hooks up start button
    public void UIMStartButton()
    {
        InitQuizRequest();
    }
    
    //UI Method. Hooks up category button
    public void UIMSetCategoryData()
    {
        //open panel
        categoryPanel.SetActive(true);
        
        //create category items in panel based on inventory
        foreach (var item in playerData.playerCategoryInventories)
        {
            var categoryGO = Instantiate(Resources.Load<GameObject>("Prefabs/CategoryContainer"));
            
            CategoryButton categoryButton = categoryGO.GetComponent<CategoryButton>();
            
            // make configuration of button.
            categoryButton.InitCategoryInfo(item.categoryName, item.categoryId);
            categoryButton.transform.parent = categoryParent.transform;
            
            //after instantiation, reset scale
            categoryButton.transform.localScale = Vector3.one;
        }
    }
}