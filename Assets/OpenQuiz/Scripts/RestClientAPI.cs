using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class RestClientAPI : MonoBehaviour
{
    public static RestClientAPI instance;

    private void Awake()
    {
        instance = this;
    }
    
    private IEnumerator Get(string url, System.Action<string> jsonResponse)
    {
        using(UnityWebRequest www = UnityWebRequest.Get(url)){
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    //get string from reult
                    string result = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    
                    //since there is no return option by type in coroutine
                    //we are using an action to make it so
                    jsonResponse(result);
                }
                else
                {
                    Debug.Log("wwww problem");
                }
            }
        }
        
    }

    public IEnumerator GetCategories(string url, Action callback)
    {
        
        string jsonResponse = string.Empty;
        
        //get response from api
        yield return StartCoroutine(Get(url, json => jsonResponse = json));
        
        // create and init data based on json
        Category[] categories = JsonHelper.FromCategoryJson<Category>(jsonResponse);
        
        //get reference
        var categoryData = Utils.categoryData;
        
        //inject to object
        categoryData.categories = categories;

        //if it takes time to response from service wait until 
        while (categoryData.categories.Length <= 0)
        {
            yield return null;
        }
        
        //do stuff after complete
        callback();
    }

    public IEnumerator GetQuiz(string url, System.Action callback)
    {

        string jsonResponse = string.Empty;
        yield return StartCoroutine(Get(url, json => jsonResponse = json));
        jsonResponse = JsonHelper.RepairResultForQuizContract(jsonResponse);

        Quiz[] quiz = JsonHelper.FromResultJson<Quiz>(jsonResponse);

        //get object from resources and populate data
        var data = Utils.quizData;
        data.quizzes = quiz;
        DecodeHtmlElementsInQuestionStringFields(data);
        
        callback();
    }

    public IEnumerator GetToken(string url, System.Action callback)
    {
        string jsonResponse = string.Empty;
        yield return StartCoroutine(Get(url, json => jsonResponse = json));

        Token token = JsonUtility.FromJson<Token>(jsonResponse);
        Utils.SetTokenToPlayer(token.token);
        Utils.SaveRequestedTokenTime();
    }
    
    

    //decode all html elements
    private void DecodeHtmlElementsInQuestionStringFields(QuizData quizData)
    {
        foreach (var quiz in quizData.quizzes)
        {
            quiz.question =quiz.question.DecodeHtmlWithHttpUtils();
            quiz.correct_answer = quiz.correct_answer.DecodeHtmlWithHttpUtils();
            for (int i = 0; i < quiz.incorrect_answers.Length - 1; i++)
            {
                quiz.incorrect_answers[i] = quiz.incorrect_answers[i].DecodeHtmlWithHttpUtils();
            }
            
        }
    }
}
