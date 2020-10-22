using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;

    [Header("UI Elements")]
    public Text question;
    public Text score;
    public Text indexText;

    public List<QuizButton> buttons = new List<QuizButton>();
    public List<QuizButton> buttonsToHide = new List<QuizButton>();

    [Header("Canvas Elements")]
    public Transform canvasParent;
    public GameObject feedbackPanel;

    private int index = 0;
    private int playerScore;

    private int baseMultipleScorePoint;
    private int baseTrueFalseScorePoint;

    private bool playerUsedReturn;

    private QuizButton correctButton;
    private QuizButton wrongButton;
    private QuizData quizData;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerUsedReturn = false;
        quizData = Utils.quizData;
        InitQuiz();
    }

    private void InitQuiz()
    {
        StartCoroutine(CheckQuizTypeThenPushNextQuestion(false));
        
        var player = Utils.playerData;
        baseMultipleScorePoint = player.multipleBaseScore;
        baseTrueFalseScorePoint = player.trueFalseBaseScore;
    }
    
    /// <summary>
    /// Pushes available next question
    /// </summary>
    /// <param name="isReturn"></param>
    /// <returns></returns>
    private IEnumerator CheckQuizTypeThenPushNextQuestion(bool isReturn)
    { 
        //checks if quiz ended.
        if (index == quizData.quizzes.Length)
        {
            Victory();
            yield break;
        }

        //checks if this is first question or returning
        //from failed screen 
        if (index != 0 && !isReturn)
        {
            //correct animation feedback to player
            yield return StartCoroutine(FeedbackDuration());
        }
        
        DisplayQuestionFromData();

        //Configuration on screen multiple or true false format
        if (quizData.quizzes[index].type == "multiple")
        {
            MultipleButtonsToShow(true);
            NextMultipleQuestion();
        }
        else
        {
            MultipleButtonsToShow(false);
            NextTrueFalseQuestion();
        }
        
        AudioManager.instance.UIMNewQuestionSound();
        
        //Updates other UI elements
        DisplayScoreAndIndex();
        
        //Since quiz starts from 0, we are incrementing
        // after quiz displayed
        index++;
    }

    private void MultipleButtonsToShow(bool hide)
    {
        buttonsToHide[0].gameObject.SetActive(hide);
        buttonsToHide[1].gameObject.SetActive(hide);
    }
    
    private void NextTrueFalseQuestion()
    {
        bool answer = bool.Parse(quizData.quizzes[index].correct_answer);
        buttons[0].SetButtonData("True", answer);
        buttons[1].SetButtonData("False", !answer);

        if (answer)
        {
            correctButton = buttons[0];
            wrongButton = buttons[1];
        }
        else
        {
            correctButton = buttons[1];
            wrongButton = buttons[0];
        }
    }

    private void NextMultipleQuestion()
    {
        //first take a random number to place quiz button
        int randomTrueIndex = UnityEngine.Random.Range(0, 4);
        int falseAnswerIndex = 0;

        for (int i = 0; i < buttons.Count; i++)
        {
            //place the true answer
            if (i == randomTrueIndex)
            {
                buttons[i].SetButtonData(quizData.quizzes[index].correct_answer, true);
                correctButton = buttons[i];
            }
            else //push other false answers
            {
                buttons[i].SetButtonData(quizData.quizzes[index].incorrect_answers[falseAnswerIndex], false);
                falseAnswerIndex++;
            }
        }
    }

    private void DisplayQuestionFromData()
    {
        question.text = quizData.quizzes[index].question;
    }

    private void DisplayScoreAndIndex()
    {
        score.text = "Score: " + playerScore.ToString();
        indexText.text = "" + GetQuizIndex() + " / " + quizData.quizzes.Length;
    }

    /// <summary>
    /// Sequence of correct answer feedback to player
    /// </summary>
    /// <returns></returns>
    IEnumerator FeedbackDuration()
    {
        feedbackPanel.SetActive(true);
        AudioManager.instance.UIMCorrectSound();
        yield return new WaitForSeconds(2.1f);
        feedbackPanel.SetActive(false);
    }
    
    private IEnumerator Fail()
    {
        AudioManager.instance.UIMFailSound();
        correctButton.SetButtonColorToCorrectAnswerColor();
        
        //give some time to player to show wrong and correct answer
        yield return new WaitForSeconds(2f);
        
        Debug.Log("Quiz fail");
        var failPanel = Instantiate(Utils.GetPrefab("FailPanel"), canvasParent);
    }

    private void Victory()
    {
        AudioManager.instance.UIMVictorySound();
        Debug.Log("Quiz victory");
        var victoryPanel = Instantiate(Utils.GetPrefab("VictoryPanel"), canvasParent);
    }
    
    //score and costs are based on quiz type and difficulty
    private void CalculateScore()
    {
        int currentIndex = index - 1; //we have to check previous item 
        if (currentIndex < 9)
        {
            ProcessScore(quizData.quizzes[currentIndex].type);
        }
        else if (currentIndex < 14)
        {
            ProcessScore(quizData.quizzes[currentIndex].type);
        }
        else
        {
            ProcessScore(quizData.quizzes[currentIndex].type);
        }
    }

    private void ProcessScore(string type)
    {
        int score;
        int currentIndex = index - 1; //we have to check previous item 

        if (type == "multiple")
        {
            score = baseMultipleScorePoint;
        }
        else
        {
            score = baseTrueFalseScorePoint;
        }
        
        if (quizData.quizzes[currentIndex].difficulty == "easy")
        {
            playerScore = playerScore + score;
        }
        else if (quizData.quizzes[currentIndex].difficulty == "medium")
        {
            playerScore = playerScore + score;
        }
        else if (quizData.quizzes[currentIndex].difficulty == "hard")
        {
            playerScore = playerScore + score;
        }
    }
    
    //resets buttons colors to default after fail screen
    private void ResetButtonsColors()
    {
        foreach (var button in buttons)
        {
            button.SetButtonColorsToDefault();
        }
        
        foreach (var button in buttonsToHide)
        {
            button.SetButtonColorsToDefault();
        }
    }

    #region Events
    public void OnCorrectAnswer() 
    {
        CalculateScore();
        StartCoroutine(CheckQuizTypeThenPushNextQuestion(false));
        DisplayScoreAndIndex();
    }

    public void OnWrongAnswer() 
    {
        StartCoroutine(Fail());
    }
    
    //calls from fail or victory panel before user going back to main menu
    public void OnQuizEnd()
    {
        Utils.IncrementPlayerMoney(GetRewardedMoney());
        Utils.IncrementPlayerScore(GetUserScore());
        Debug.Log(GetUserScore());
        Utils.SavePlayerDataToPlayerPref();
    }

    public void OnPlayerReturn()
    {
        ResetButtonsColors();
        StartCoroutine(CheckQuizTypeThenPushNextQuestion(true));
        playerUsedReturn = true;
    }
    
    #endregion

    

    #region Getters

    public int GetUserScore()
    {
        return playerScore;
    }

    public int GetQuizIndex()
    {
        return index;
    }

    public int GetRewardedMoney()
    {
        return playerScore / 2;
    }

    public int GetContinueWithMoneyCost()
    {
        return  50 + playerScore / 4;
    }

    public bool GetPlayerReturnStatus()
    {
        return playerUsedReturn;
    }

    #endregion
    

    

    

    

}
