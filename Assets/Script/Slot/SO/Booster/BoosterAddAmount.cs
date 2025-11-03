using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Booster", menuName = "Booster/Logic/AddAmount")]

public class BoosterAddAmount : BoosterLogicSO
{
    public CharacterStateType stateType;
    public override void BoosterAttack()
    {
        base.BoosterAttack();
        if (topicType == TopicType.Self)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            switch (stateType)
            {
                case CharacterStateType.Fight:
                    if(keepTime>1)
                        player.GetComponent<Player>().AddFightAmount(amount, keepTime);
                    else if(keepTime>0)
                    {
                        int time = (int)(keepTime * 10);
                        player.GetComponent<Player>().AddFightAmountFake(amount,tempType, time);
                    }
                    else if(keepTime==0)
                        player.GetComponent<Player>().SetFightAmount(amount);
                    break;
                case CharacterStateType.Protect:
                    if(keepTime>0)
                        player.GetComponent<Player>().AddProtectAmount(amount, keepTime);
                    else
                        player.GetComponent<Player>().SetProtectAmount(amount);
                    break;
            }
        }
    }

    public override void BoosterAfterWin()
    {
        base.BoosterAfterWin();
        if (topicType == TopicType.Self)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Player playerScript = player.GetComponent<Player>();
            switch (stateType)
            {
                case CharacterStateType.Fight:
                    if(keepTime>1)
                        playerScript.AddFightAmount(amount, keepTime);
                    else if(keepTime>0)
                    {
                        int time = (int)(keepTime * 10);
                        playerScript.AddFightAmountFake(amount,tempType, time);
                    }
                    else if(keepTime==0)
                        playerScript.SetFightAmount(amount);
                    break;
                case CharacterStateType.Protect:
                    if(keepTime>0)
                        playerScript.AddProtectAmount(amount, keepTime);
                    else if (tempType == TempType.GetDamage)
                        playerScript.AddProtectAmountFake(amount,tempType, 0);
                    else
                        playerScript.SetProtectAmount(amount);
                    break;
                case CharacterStateType.HPCurrent:
                    playerScript.AddHPCurrent(amount);
                    break;
            }
        }
    }
}
