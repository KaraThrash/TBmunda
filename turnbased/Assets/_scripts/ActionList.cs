using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct actionData
{
    public Soldier actor, target;
    
    public int actionNumber,team,reactType,targetType;
    public int range, damage;
    public float actionTime;
    public Vector3 loc;
}
public struct reaction
{
    public int action, targettype, reacttype, teamtype, range,turnsactive,cost;
    public Soldier actor, target;
    public float waittime;
    public string anim;
}
public class ActionList : MonoBehaviour
{
    public reaction reactToReturn;
    public UImanager uiManager;
    public actionData currentAction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //first press
    public bool CheckActionPoints(int action, int currentActionPoints,Soldier actor, Soldier actiontarget, TurnManager turnManager)
    {
        bool enoughPoints = false; 
        switch (action)
        {
            case 0:
                if (currentActionPoints > 1 && actor.team != actiontarget.team) { enoughPoints = true; }
                break;
            case 1:
                if (currentActionPoints > 1)
                {
                    uiManager.UpdateScrollingText(" jump ");
                    SetCurrentAction(turnManager.currentSoldier.team, actor,actiontarget, 1, 1, 0,actor.transform.position);
                   
                    //turnManager.watchActionTimer = 1.0f;
                    turnManager.currentSoldier.GetComponent<Rigidbody>().AddForce(Vector3.up * 510.0f * Time.deltaTime, ForceMode.Impulse);
                }
                break;
            case 2:
               
                break;
            default:
                break;
        }return enoughPoints;
    }

    //second press
    public actionData ConfirmAction(int action, Soldier actor, Soldier actiontarget, TurnManager turnManager)
    {
      
        switch (action)
        {
            case 0: //shoot
                turnManager.SpendActionPoints(1);
                return SetCurrentAction(actor.team, actor,actiontarget, 0, 0, 0, actor.transform.position);

               // turnManager.CheckReactions();
   
                break;
            case 1: //jump
                turnManager.SpendActionPoints(1);
                return SetCurrentAction(actor.team, actor, actor, 1, 1, 0,actor.transform.position);
                //turnManager.CheckReactions();
                break;
            case 2:

                break;
            default:
                break;
        }

        return SetCurrentAction(actor.team, actor, actor, 1, 1, 0, actor.transform.position);
    }

    public float PerformAction(actionData act,  TurnManager turnManager)
    {
        float actionTimer = 0.0f;
    
        switch (act.actionNumber)
        {
            case 0:
              
              //  uiManager.UpdateScrollingText(" fire gun ");
                actionTimer = act.actionTime;
                FireGun(act,turnManager);
                break;
            case 1:
                Debug.Log("jump"); 
                //SetCurrentAction(currentAction.team, currentAction.ownerpublic, 1, 1, 0);
                actionTimer = act.actionTime;

                break;
            case 2://take damage


                break;
            default:
                break;
        }

        return actionTimer;
    }
    public actionData SetCurrentAction(int teamnum, Soldier owner,Soldier tar,int actionnumber,int reactnum,int targetnum,Vector3 loc)
    {
        actionData tempAction = new actionData();
        tempAction.actor = owner;
        tempAction.team = owner.team;
        tempAction.target = tar;
        tempAction.actionTime = 1.0f;
        tempAction.actionNumber = actionnumber;
        tempAction.reactType = reactnum;
        tempAction.targetType = targetnum;
        tempAction.loc = loc;
        return tempAction;
    }

    //to add to the list the turnmanager checks 
    public reaction ConfirmReaction(int action, Soldier reactingSoldier,TurnManager turnManager)
    {
        reaction tempReact = new reaction();

        tempReact.actor = reactingSoldier;
        tempReact.target = reactingSoldier;
        tempReact.action = 0;
        tempReact.waittime = 0;


    //       public int action, targettype, reacttype, teamtype, range;
    //public Soldier actor, target;
    //public float waittime;
    //public string anim;
        switch (action)
        {
            case 0:// when teammate shoots, assist them
                tempReact.action = 0;//shoot
                tempReact.actor = reactingSoldier;
                tempReact.reacttype = 0;
                tempReact.turnsactive = -1;
                tempReact.teamtype = 1;
                tempReact.cost = 1;
               // if (reactingSoldier.team == 0) { tempReact.teamtype = 1; } else { tempReact.teamtype = 0; }
                
                tempReact.range = reactingSoldier.loadout.range;


              
                break;
            case 1:
             
                break;
            case 2:
               
                break;
            default:
                break;
        }
        return tempReact;
    }

    public void PerformReaction( reaction raction, TurnManager turnManager)
    {

    
        int reactnum = raction.action;
        switch (reactnum)
        {
            case 0:
               
                    uiManager.UpdateScrollingText(raction.actor.name + " react fire gun ");
                    ReactFireGun(raction.actor.GetComponent<Soldier>(), raction.target.gameObject, turnManager);
  
                break;
            case 1:

                break;
            case 2:

                break;
            default:
                break;
        }
      
    }


    public void ReactFireGun(Soldier shooting,GameObject target,TurnManager turnManager)
    {
       

            int tohit = turnManager.CalculateToHit();
            int roll = Random.Range(1, 101);
            if (tohit > roll)
            {

                target.GetComponent<Soldier>().TakeDamage(1);
              
                GameObject clone = Instantiate(turnManager.bulletPrefab2, shooting.transform.position + shooting.transform.forward, transform.rotation) as GameObject;

                clone.GetComponent<Bullet>().holdvel = (target.transform.position - shooting.transform.position) + transform.up;
            uiManager.UpdateScrollingText(shooting.transform.name + ":Hit:" + target.transform.name);
        }
            else
            {
            uiManager.UpdateScrollingText(shooting.transform.name + ":missed:" + target.transform.name);
            //check to see if miss destroys something
            Vector3 newdir = new Vector3(turnManager.lookTarget.transform.position.x + (Random.Range(-0.3f, 1.0f)), turnManager.lookTarget.transform.position.y + (Random.Range(-0.3f, 1.0f)), turnManager.lookTarget.transform.position.z + (Random.Range(-0.3f, 1.0f))) - turnManager.gun.transform.position;
               
                RaycastHit hit;
                GameObject clone = Instantiate(turnManager.bulletPrefab, turnManager.gun.transform.position, transform.rotation) as GameObject;

                clone.GetComponent<Bullet>().holdvel = newdir;
                if (Physics.Raycast(shooting.transform.position + shooting.transform.forward, newdir, out hit, 15.0f))
                {


                    if (hit.transform.tag == "destructable")
                    {
                        //  Destroy(hit.transform.gameObject);
                    }
                }
            }
        
    }

    public void FireGun(actionData act,TurnManager turnManager)
    {
        // if (act.target == act.actor) { turnManager.CheckRange(5); turnManager.resultdisplay.text = "looking at self "; Debug.Log("dont shoot im you "); }

        //uiManager.UpdateScrollingText(act.actor.transform.name + " shoots at " + act.target.transform.name);
        if (act.target.team != act.actor.team)
        {
            act.actor.transform.LookAt(act.target.transform);
            Debug.Log("hit: Fire gun ");
            //  turnManager.actionRemaining--;
            turnManager.actionpointstext.text = turnManager.actionRemaining.ToString();

            int tohit = turnManager.CalculateToHit();
            int roll = Random.Range(1, 101);
            if (tohit > roll)
            {
                act.target.TakeDamage(act.damage + 1);
                // turnManager.lastActionPress = -1;
                GameObject clone = Instantiate(turnManager.bulletPrefab2, act.actor.transform.position + act.actor.transform.forward, transform.rotation) as GameObject;

                clone.GetComponent<Bullet>().holdvel = (act.target.transform.position - act.actor.transform.position);
                turnManager.resultdisplay.text = " HIT : " + act.target.transform.name + " :hp: " + act.target.currenthp.ToString();
                uiManager.UpdateScrollingText(act.actor.transform.name + " : HIT : " + act.target.transform.name + " :hp: " + act.target.currenthp.ToString());
            }
            else
            {
                Debug.Log("miss: Fire gun ");
                //check to see if miss destroys something
                Vector3 newdir = new Vector3(act.target.transform.position.x + (Random.Range(-0.3f, 1.0f)), act.target.transform.position.y +  (Random.Range(-0.3f, 1.0f)), act.target.transform.position.z + (Random.Range(-0.3f, 1.0f))) - act.actor.transform.position;
                turnManager.resultdisplay.text = " ::MISSED:: " + act.target.transform.name;
                RaycastHit hit;
                GameObject clone = Instantiate(turnManager.bulletPrefab, act.actor.transform.position + act.actor.transform.forward, transform.rotation) as GameObject;
                uiManager.UpdateScrollingText(act.actor.transform.name + " ::MISSED:: " + act.target.transform.name );
                clone.GetComponent<Bullet>().holdvel = newdir;
                if (Physics.Raycast(turnManager.gun.transform.position, newdir, out hit, 15.0f))
                {


                    if (hit.transform.tag == "destructable")
                    {
                        //  Destroy(hit.transform.gameObject);
                    }
                }
            }
        }
        else { Debug.Log("sameteam"); uiManager.UpdateScrollingText(act.actor.transform.name + " doesnt shoot at " + act.target.transform.name); }

    }
}
