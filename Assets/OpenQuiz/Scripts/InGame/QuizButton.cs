using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QuizButton : MonoBehaviour
{
    [SerializeField]private bool isTrue;
    private Button button;
    private Image image;

    private Color wrongColor = Color.red;
    private Color correctColor = Color.green;

    private void Start()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }

    private bool CheckAnswer()
    {
        return isTrue;
    }

    /// <summary>
    /// Set the button's data and is true or false
    /// </summary>
    /// <param name="buttonText"></param>
    /// <param name="answer"></param>
    public void SetButtonData(string buttonText, bool answer)
    {
        gameObject.GetComponentInChildren<Text>().text = buttonText;
        isTrue = answer;
    }

    private void SetButtonColorToWrongAnswerColor()
    {
        image.color = wrongColor;
    }
    
    public void SetButtonColorToCorrectAnswerColor()
    {
        image.color = correctColor;
    }

    public void SetButtonColorsToDefault()
    {
        image.color = Color.white;
    }

    //UIM stands for UI Method. Hooked up in editor
    public void UIMOnUserAnswer()
    {
        if (CheckAnswer())
        {
            InGameManager.instance.OnCorrectAnswer();
        }
        else
        {
            SetButtonColorToWrongAnswerColor();
            InGameManager.instance.OnWrongAnswer();
        }
    }
}
