using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Category", menuName = "Quiz/Data/CategoryData")]
public class CategoryData : ScriptableObject
{
    public Category[] categories;
}
