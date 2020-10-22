using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    
    public Text scoreText;
    public Text earnedMoneyText;
    public Text answerCountText;

    // Start is called before the first frame update
    public virtual void Start()
    {
        scoreText.text = InGameManager.instance.GetUserScore().ToString();
        earnedMoneyText.text = InGameManager.instance.GetRewardedMoney().ToString();
        answerCountText.text = InGameManager.instance.GetQuizIndex().ToString();
    }

    public void UIMReturnMenu()
    {
        //calls OnQuizEnd to return to main menu
        InGameManager.instance.OnQuizEnd();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
       
    }
}
