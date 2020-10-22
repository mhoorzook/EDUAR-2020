using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FailPopUp : PopUp
{
    public Text moneyButtonText;
    public Button moneyButton;
    public Button adButton;

 
    public override void Start()
    {
        base.Start();
        moneyButtonText.text = InGameManager.instance.GetContinueWithMoneyCost().ToString();
        
        if (InGameManager.instance.GetPlayerReturnStatus())
        {
            moneyButtonText.text = "Not available!";
            moneyButton.interactable = false;
            adButton.interactable = false;
            
        }
    }

    void UIMContinueWithWatchVideo()
    {
        // ad managers handles show video
    }

    public void UIMContinueWithMoney()
    {
        
        
        var cost = InGameManager.instance.GetContinueWithMoneyCost();

        //check if there is enough money to continue
        if (Utils.playerData.playerMoney < cost)
        {
            moneyButton.interactable = false;
            moneyButtonText.text = "Not enough money!";
        }
        else
        {
            //return to game and spend money 
            Utils.playerData.SpendMoney(InGameManager.instance.GetContinueWithMoneyCost());
            
            InGameManager.instance.OnPlayerReturn();
            Destroy(gameObject);
        }
        
    }
}
