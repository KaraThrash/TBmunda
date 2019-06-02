using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    // Start is called before the first frame update
    public bool focused;
        public bool alert,spentTurn,usedReaction,inOverwatch;
    public Transform eyes,hpvisual,exhausted,aimPoints,statDisplay;
    public int team,teamnumber,actionpoints, movepoints ,variablepoints, currentreactPoints,currenthp, accuracy;
    public Vector3 posmark,lastpos; //location to track movement spent
   
    public Loadout loadout;

    public GameManager gameManager;
    public TurnManager turnManager;

  //  public int speed,subspeed;
    void Start()
    {
        usedReaction = false;
        movepoints = 12;
        currenthp = 5;// Random.Range(1,6);
        accuracy = Random.Range(1, 30);
        actionpoints = Random.Range(11, 16);
        
        loadout = GetComponent<Loadout>();
        RefreshTurn();
        TakeDamage(0);
        gameManager.activeSoldiers.Add(this);
        // ConfirmReaction(int action, Soldier reactingSoldier, TurnManager turnManager)
        
       
      //  turnManager.activeSoldiers.Add(this);
    }

    // Update is called once per frame
    void Update()
    {

        if (focused == true )
        {
            //if (Vector3.Distance(posmark, transform.position) > 1)
            //{ SpendMove(); }
            //if (Input.GetKeyUp(KeyCode.T) && spentTurn == false) {
            //    Debug.Log("who is this:  " + transform.name);
            //    spentTurn = true;
                
            //    gameManager.TakeTurn();
            //}
        }

    }
    public int TakeDamage(int dmg)
    {
        int count = 0;
        currenthp -= dmg;
        while (count < hpvisual.childCount)
        {
            if (count < currenthp)
            { hpvisual.GetChild(count).gameObject.active = true; }
            else { hpvisual.GetChild(count).gameObject.active = false; }
            count++;
        }

        return currenthp;
    }

    public void SpentTurn()
    {
        exhausted.gameObject.active = true;
        focused = false;
        spentTurn = true;
        GetComponent<Animator>().Play("Change");
        GetComponent<BasicBehaviour>().enabled = false;
        GetComponent<MoveBehaviour>().enabled = false;
        GetComponent<AimBehaviour>().enabled = false;
        GetComponent<FlyBehaviour>().enabled = false;

    }

    public void RefreshTurn()
    {
        spentTurn = false;
        usedReaction = false;
        exhausted.gameObject.active = false;
        actionpoints = loadout.actionpoints;
        movepoints = loadout.movepoints;
        currentreactPoints = loadout.reactPoints;
        inOverwatch = false;
    }


    public int SpendMove(int moveSpent)
    {
        movepoints -= moveSpent;
        lastpos = posmark;
        posmark = transform.position;

        if (movepoints <= 0 )
        {
            //TODO: aiming movement should stop too
            GetComponent<Animator>().Play("Change");
            
        }

        return movepoints;
    }
    public void Focus()
    {
      
            focused = true;
            posmark = transform.position;
            GetComponent<Animator>().Play("Idle");
            GetComponent<BasicBehaviour>().enabled = true;
            GetComponent<MoveBehaviour>().enabled = true;
            GetComponent<AimBehaviour>().enabled = true;
            GetComponent<FlyBehaviour>().enabled = true;
        
    }
    public void FocusOff()
    {
       
        //posmark = transform.position;
        GetComponent<Animator>().Play("Change");
        GetComponent<BasicBehaviour>().enabled = false;
        GetComponent<MoveBehaviour>().enabled = false;
        GetComponent<AimBehaviour>().enabled = false;
        GetComponent<FlyBehaviour>().enabled = false;

    }
    public void ToggleFocus()
    {
        if (focused == true) {
            focused = false;
           
            
            GetComponent<Animator>().Play("Change");
            GetComponent<BasicBehaviour>().enabled = false ;
            GetComponent<MoveBehaviour>().enabled = false;
            GetComponent<AimBehaviour>().enabled = false;
            GetComponent<FlyBehaviour>().enabled = false;
        }
        else { focused = true;
           
            posmark = transform.position;
            GetComponent<Animator>().Play("Idle");
            GetComponent<BasicBehaviour>().enabled = true;
            GetComponent<MoveBehaviour>().enabled = true;
            GetComponent<AimBehaviour>().enabled = true;
            GetComponent<FlyBehaviour>().enabled = true;
        }
    }

}
