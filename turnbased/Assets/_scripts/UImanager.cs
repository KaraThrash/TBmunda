using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UImanager : MonoBehaviour
{
    public List<Transform> redTeamChars,blueTeamChars,turnList;
    public Transform topOfList;
    public Transform scrollingText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSoldierDisplayerText(int whichteam,int whichsolider,int hp, int ap, int mv)
    {
        if (whichteam == 0) {
            redTeamChars[whichsolider].GetChild(0).GetComponent<Text>().text = mv.ToString();
            redTeamChars[whichsolider].GetChild(1).GetComponent<Text>().text = ap.ToString();
            redTeamChars[whichsolider].GetChild(2).GetComponent<Text>().text = hp.ToString();
        }
        else {
            blueTeamChars[whichsolider].GetChild(0).GetComponent<Text>().text = mv.ToString();
            blueTeamChars[whichsolider].GetChild(1).GetComponent<Text>().text = ap.ToString();
            blueTeamChars[whichsolider].GetChild(2).GetComponent<Text>().text = hp.ToString();
        }

    }
    public void SetSoldierDisplayerText(Soldier soldier)
    {
        if (soldier.team == 0)
        {
            redTeamChars[soldier.teamnumber].GetChild(0).GetComponent<Text>().text = soldier.movepoints.ToString();
            redTeamChars[soldier.teamnumber].GetChild(1).GetComponent<Text>().text = soldier.actionpoints.ToString();
            redTeamChars[soldier.teamnumber].GetChild(2).GetComponent<Text>().text = soldier.currenthp.ToString();
        }
        else
        {
            blueTeamChars[soldier.teamnumber].GetChild(0).GetComponent<Text>().text = soldier.movepoints.ToString();
            blueTeamChars[soldier.teamnumber].GetChild(1).GetComponent<Text>().text = soldier.actionpoints.ToString();
            blueTeamChars[soldier.teamnumber].GetChild(2).GetComponent<Text>().text = soldier.currenthp.ToString();
        }

    }
    public void UpdateScrollingText(string newtext)
    {
        int count = scrollingText.childCount - 1;
        while (count > 0)
        {
            scrollingText.GetChild(count).GetComponent<Text>().text = scrollingText.GetChild(count - 1).GetComponent<Text>().text;
            count--;
        }
        scrollingText.GetChild(count).GetComponent<Text>().text = newtext;
    }

    public void SetTurnList(List<Soldier> soldierList)
    {
        soldierList.Reverse();
        int count = 0;
        int count2 = 0;
        while (count2 < turnList.Count)
        {
            while (count < soldierList.Count)
            {


                Soldier tempSoldier = soldierList[count];
                if (tempSoldier.spentTurn == false)
                {
                    Transform placeInTurnList = turnList[count2];
                    placeInTurnList.gameObject.active = true;

                    SetTurnListElementTextColor(placeInTurnList, tempSoldier.team);

                    placeInTurnList.GetChild(0).GetComponent<Text>().text = tempSoldier.movepoints.ToString();
                    placeInTurnList.GetChild(1).GetComponent<Text>().text = tempSoldier.actionpoints.ToString();
                    placeInTurnList.GetChild(2).GetComponent<Text>().text = tempSoldier.currenthp.ToString();
                    placeInTurnList.GetChild(3).GetComponent<Text>().text = tempSoldier.transform.name;
                    count2++;

                }
                count++;

              
            }
            turnList[count2].gameObject.active = false;
            count2++;
        }
      
        soldierList.Reverse();
    }

    public void SetTurnListElementTextColor(Transform turnlistelement, int redOrBlue)
    {
        Color tempColor = Color.blue;
        Color tempColor2 = Color.cyan;

        if (redOrBlue == 0)
        {
             tempColor = Color.red;
             tempColor2 = Color.magenta;
        }

        turnlistelement.GetChild(0).GetComponent<Text>().color = tempColor;
            turnlistelement.GetChild(0).GetChild(0).GetComponent<Text>().color = tempColor2;
        turnlistelement.GetChild(2).GetComponent<Text>().color = tempColor;
        turnlistelement.GetChild(2).GetChild(0).GetComponent<Text>().color = tempColor2;
        turnlistelement.GetChild(1).GetComponent<Text>().color = tempColor;
        turnlistelement.GetChild(1).GetChild(0).GetComponent<Text>().color = tempColor2;


    }
}
