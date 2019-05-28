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
    public float PerformAction(int action,TurnManager turnManager)
    {
        float actionTimer = 0.0f;
        switch (action)
        {
            case 0:
                actionTimer = 1.0f;
                uiManager.UpdateScrollingText(" fire gun ");
                currentAction.team = turnManager.currentSoldier.team;
                currentAction.target = turnManager.lookTarget;
                turnManager.CheckReactions();
                FireGun(turnManager);
                break;
            case 1:
                Debug.Log("jump");
               
                break;
            case 2:
                
                break;
            default:
                break;
        }

        return actionTimer;
    }


    public bool ConfirmReaction(int action, Soldier reactingSoldier,TurnManager turnManager)
    {
        bool enoughPoints = false;
        switch (action)
        {
            case 0:
                if (currentAction.team != reactingSoldier.team  ) {
                    uiManager.UpdateScrollingText(reactingSoldier.transform.name + " react fire gun ");
                    ReactFireGun(reactingSoldier,currentAction.target,turnManager);
                }

              
                break;
            case 1:
             
                break;
            case 2:
               
                break;
            default:
                break;
        }
        return enoughPoints;
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
