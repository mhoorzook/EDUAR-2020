using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class JsonHelper
{
    public static T[] FromResultJson<T>(string json)
    {
        ResultsWrapper<T> wrapper = JsonUtility.FromJson<ResultsWrapper<T>>(json);
        return wrapper.results;
    }

    public static T[] FromCategoryJson<T>(string json)
    {
        CategoriesWrapper<T> wrapper = JsonUtility.FromJson<CategoriesWrapper<T>>(json);
        return wrapper.trivia_categories;
    }

    public static string RepairResultForQuizContract(string json)
    {
        json = json.Remove(0, 19);
        return "{" + json;
    }

    [Serializable]
    private class ResultsWrapper<T>
    {
        public T[] results;
    }

    [Serializable]
    private class CategoriesWrapper<T>
    {
        public T[] trivia_categories;
    }

    
}
