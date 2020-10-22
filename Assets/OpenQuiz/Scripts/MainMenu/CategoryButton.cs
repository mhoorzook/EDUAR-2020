using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CategoryButton : MonoBehaviour
{
    public PlayerData playerData;
    public Text categoryNameText;
    public Text unlockPriceText;
    public GameObject lockImage;

    private int id;

    private void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(OnCategoryButtonClicked);
    }

    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Sets the category button's lock image and price if it's not purchased yet.
    /// </summary>
    /// <param name="categoryName"></param>
    /// <param name="categoryId"></param>
    public void InitCategoryInfo(string categoryName, int categoryId)
    {
        categoryNameText.text = categoryName;
        id = categoryId;

        var data = GetPlayerCategoryInventoryById(id);
        if (id == 9)
        {
            data.isPurchesed = true;
        }

        if (data.isPurchesed)
        {
            lockImage.SetActive(false);
            unlockPriceText.text = "Unlocked";
        }
        else
        {
            lockImage.SetActive(true);
            unlockPriceText.text = "Purchese for " + playerData.purcheseCategoryPrice;
        }
    }

    
    private void OnCategoryButtonClicked()
    {
        AudioManager.instance.UIMButtonSound();
        var data = GetPlayerCategoryInventoryById(id);
        if (!data.isPurchesed && playerData.playerMoney >= playerData.purcheseCategoryPrice)
        {
            data.isPurchesed = true;
            
            playerData.SpendMoney(playerData.purcheseCategoryPrice);
            
            Utils.SavePlayerDataToPlayerPref();
            lockImage.SetActive(false);
        }
        else if (data.isPurchesed)
        {
            QuizConfigManager.instance.OnUserSelectedCategory(data);
        }
    }

    /// <summary>
    /// returns category from inventory by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private PlayerCategoryInventory GetPlayerCategoryInventoryById(int id)
    {
        return playerData.playerCategoryInventories.Find(x => x.categoryId == id);
    }

   
}
