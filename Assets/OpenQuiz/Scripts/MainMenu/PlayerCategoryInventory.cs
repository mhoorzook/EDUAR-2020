using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerCategoryInventory
{
    public int categoryId;
    public string categoryName;
    public bool isPurchesed;

    public PlayerCategoryInventory(int id, string name)
    {
        categoryId = id;
        categoryName = name;
    }
}
