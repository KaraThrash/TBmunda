using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct actionData
{
    public Soldier ownerpublic;
    public GameObject target;
    public int actionNumber,team,reactType,targetType;
    
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
    public bool CheckActionPoints(int pts,int action, TurnManager turnManager)
    {
        bool enoughPoints = false; 
        switch (action)
        {
            case 0:
                if (pts > 1) { enoughPoints = true; }
                break;
            case 1:
                if (pts > 1)
                {
                    uiManager.UpdateScrollingText(" jump ");
                    SetCurrentAction(turnManager.currentSoldier.team, turnManager.currentSoldier.gameObject, 1, 1, 0);
                    turnManager.CheckReactions();
                    turnManager.watchActionTimer = 1.0f;
                    turnManager.currentSoldier.GetComponent<Rigidbody>().AddForce(Vector3.up * 510.0f * Time.deltaTime, ForceMode.Impulse);
                }
                break;
            case 2:
                if (pts > 1) { enoughPoints = true; }
                break;
            default:
                break;
        }return enoughPoints;
    }

    //second press
    public float ConfirmAction(int action, TurnManager turnManager)
    {
        float actionTimer = 0.0f;
        switch (action)
        {
            case 0:
     
                SetCurrentAction(turnManager.currentSoldier.team, turnManager.lookTarget, 0, 0, 0);

                turnManager.CheckReactions();
   
                break;
            case 1:
          

                break;
            case 2:

                break;
            default:
                break;
        }

        return actionTimer;
    }

    public float PerformAction(TurnManager turnManager)
    {
        float actionTimer = 0.0f;
    
        switch (currentAction.actionNumber)
        {
            case 0:
              
                uiManager.UpdateScrollingText(" fire gun ");
 
                FireGun(turnManager);
                break;
            case 1:
                Debug.Log("jump");
                SetCurrentAction(turnManager.currentSoldier.team, turnManager.currentSoldier.gameObject, 1, 1, 0);


                break;
            case 2:
                
                break;
            default:
                break;
        }

        return actionTimer;
    }
    public void SetCurrentAction(int teamnum, GameObject tar,int actionnumber,int reactnum,int targetnum)
    {
        currentAction.team = teamnum;
        currentAction.target = tar;
        currentAction.actionNumber = actionnumber;
        currentAction.reactType = reactnum;
        currentAction.targetType = targetnum;
    }

    public reaction ConfirmReaction(int action, Soldier reactingSoldier,TurnManager turnManager)
    {
       
        reactToReturn.actor = reactingSoldier.transform;
        reactToReturn.target = reactingSoldier.transform;
        reactToReturn.action = 0;
        reactToReturn.waittime = 0;
      
        switch (action)
        {
            case 0:
                if (currentAction.team != reactingSoldier.team  && reactingSoldier.usedReaction == false) {



                    reactingSoldier.transform.LookAt(turnManager.lookTarget.transform);
                    reactToReturn.actor = reactingSoldier.transform;
                    reactToReturn.target = turnManager.lookTarget.transform;
                    reactToReturn.action = 0;
                    reactToReturn.waittime = 1.0f;
                    // turnManager.cam.GetComponent<ThirdPersonOrbitCam>().player = reactingSoldier.transform;
                   
                }

              
                break;
            case 1:
             
                break;
            case 2:
               
                break;
            default:
                break;
        }
        return reactToReturn;
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

    public void FireGun(TurnManager turnManager)
    {
        if (turnManager.lookTarget == turnManager.currentSoldier.gameObject) { turnManager.CheckRange(5); turnManager.resultdisplay.text = "looking at self "; }

        if (turnManager.lookTarget != turnManager.currentSoldier.gameObject)
        {
            turnManager.actionRemaining--;
            turnManager.actionpointstext.text = turnManager.actionRemaining.ToString();

            int tohit = turnManager.CalculateToHit();
            int roll = Random.Range(1, 101);
            if (tohit > roll)
            {
                turnManager.lookTarget.GetComponent<Soldier>().TakeDamage(1);
                turnManager.lastActionPress = -1;
                GameObject clone = Instantiate(turnManager.bulletPrefab2, turnManager.gun.transform.position, transform.rotation) as GameObject;

                clone.GetComponent<Bullet>().holdvel = (turnManager.lookTarget.transform.position - turnManager.gun.transform.position) + transform.up;
                turnManager.resultdisplay.text = "hit : " + turnManager.lookTarget.transform.name + " :hp: " + turnManager.lookTarget.GetComponent<Soldier>().currenthp.ToString();
            }
            else
            {

                //check to see if miss destroys something
                Vector3 newdir = new Vector3(turnManager.lookTarget.transform.position.x + (Random.Range(-0.3f, 1.0f)), turnManager.lookTarget.transform.position.y + (Random.Range(-0.3f, 1.0f)), turnManager.lookTarget.transform.position.z + (Random.Range(-0.3f, 1.0f))) - turnManager.gun.transform.position;
                turnManager.resultdisplay.text = "missed " + turnManager.lookTarget.transform.name;
                RaycastHit hit;
                GameObject clone = Instantiate(turnManager.bulletPrefab, turnManager.gun.transform.position, transform.rotation) as GameObject;

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
    }
}
