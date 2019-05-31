using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject cam,reactCam,activeMarker,gun,targetMarker,moveLimiter,bulletPrefab, bulletPrefab2;
    public Transform accRaycastHitIndicatorParent;
    public Soldier currentSoldier,lookTarget;
    public int actionRemaining, moveRemaining, wildRemaining,lastActionPress;
    public Vector3 posmark, lastpos; //location to track movement spent
    public List<Soldier>  activeSoldiers;
    public List<Soldier> soldiersInRange;
    public List<reaction> reactsOnStack;
    public List<actionData> actionsOnStack;
    public Text movepointsText, actionpointstext, tohittext,resultdisplay,confirmaction;
    public UImanager uiManager;
    public ActionList actionManager;
    public float watchActionTimer;
    public bool reacting, focusTeam;
    // Start is called before the first frame update
    void Start()
    {
        reactsOnStack = new List<reaction>();
        actionsOnStack = new List<actionData>();
    }

    // Update is called once per frame
    void Update()
    {

        if (actionsOnStack.Count > 0 && reacting == true)
        {
           
                if (watchActionTimer > 0)
                {
               
                    watchActionTimer -= Time.deltaTime;
                }
                else
                {
                //reactions are kept in a list going backwards: LiFO //trade off for going first -> reacting last
                    LoopThroughactions();
                  
                
                }
              
           }
            else
            {
            if (reacting == true)
            {
                if (watchActionTimer > 0)
                {
                    cam.active = true;
                    reactCam.active = false;
                    watchActionTimer -= Time.deltaTime;
                }
                else
                { reacting = false;  }
            }


            if (currentSoldier != null)
            { ControlSoldier(); }

         }


        if (Input.GetKeyUp(KeyCode.Y))
        {
            SortSoldierList();
        }

    }
    public void LoopThroughactions()
    {
        Debug.Log("action loop: " + actionsOnStack.Count.ToString());
        watchActionTimer = actionsOnStack[actionsOnStack.Count - 1].actionTime;
        reactCam.transform.position = actionsOnStack[actionsOnStack.Count - 1].actor.transform.position;
        reactCam.transform.LookAt(actionsOnStack[actionsOnStack.Count - 1].target.transform);
        actionManager.PerformAction(actionsOnStack[actionsOnStack.Count - 1],GetComponent<TurnManager>());
        actionsOnStack.RemoveAt(actionsOnStack.Count - 1);
        uiManager.SetTurnList(activeSoldiers);
    }
    public void SpendActionPoints(int spent)
    {
        actionRemaining -= spent;
    }
    public void ControlSoldier()
    {
            if (Input.GetMouseButton(1)) { moveLimiter.active = true; } else { moveLimiter.active = false; }
            activeMarker.transform.position = currentSoldier.transform.position;
            // activeMarker.transform.eulerAngles = new Vector3(0,currentSoldier.transform.eulerAngles.y,0);
            gun.transform.LookAt(cam.transform.GetChild(0));
            activeMarker.transform.LookAt(lookTarget.transform);
            moveLimiter.transform.position = posmark;
            if (Vector3.Distance(posmark, currentSoldier.transform.position) > 1)
            { IncrementMove(1); }

            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                AbilityButtonPress(0);
            }
            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                AbilityButtonPress(1);
            }
            if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                AbilityButtonPress(2);
            }
   

        if (Input.GetKeyUp(KeyCode.T))
        {
            // Debug.Log("who is this:  " + transform.name);
            EndTurn();
        }
        if (Input.GetKeyUp(KeyCode.R) && lastActionPress != -1)
        {
            CheckRange(15);
        }
    }
    public void CheckReactions(actionData act)
    {
       
        foreach (reaction el in reactsOnStack)
        {
            Debug.Log("check react");
            if (act.reactType == el.reacttype && Vector3.Distance(el.actor.transform.position, act.loc) < el.range && el.actor.currentreactPoints >= el.cost )
            {
                //  if((act.team != el.actor.team && el.teamtype == 1))
                //   Debug.Log("diff team:  confirmed react");
                // actionsOnStack.Add(actionManager.ConfirmAction(el.action, el.actor,act.target, this.GetComponent<TurnManager>()));

                if ((act.team == el.actor.team && el.teamtype == 1) && el.actor != act.actor)
                {
                    Debug.Log("same team : confirmed react");
                    actionsOnStack.Add(actionManager.ConfirmAction(el.action, el.actor, act.target, this.GetComponent<TurnManager>()));
                    el.actor.usedReaction = true;
                    el.actor.currentreactPoints -= el.cost;
                }
            }
        }
        if (actionsOnStack.Count > 1)
        {
            cam.active = false;
            reactCam.active = true;
            
        }
        reacting = true;
    }
  

    public void AbilityButtonPress(int abilityNumber)
    {

        if (lastActionPress != abilityNumber)
        {

            if (focusTeam == true && (int)currentSoldier.loadout.actions[abilityNumber].y == 1) { soldiersInRange.Clear(); focusTeam = false; }
            if (focusTeam == false && (int)currentSoldier.loadout.actions[abilityNumber].y == 0) { soldiersInRange.Clear(); focusTeam = true; }


          CheckRange(15);
            if (actionManager.CheckActionPoints(abilityNumber,actionRemaining,currentSoldier ,lookTarget, GetComponent<TurnManager>()) == true)
            { StartUseAbility(abilityNumber); confirmaction.text = "Action: " + abilityNumber.ToString(); lastActionPress = abilityNumber; }
            else { lastActionPress = -1; confirmaction.text = "not enough AP"; uiManager.UpdateScrollingText(" no ap "); }
          
        }
        else { EndUseAbility(abilityNumber); }

    }

    public void StartUseAbility(int ability)
    {
        resultdisplay.text = "ability: " + ability.ToString();
        if (ability == 0)
        {
            lastActionPress = 0;
           
           // CheckRange(5);
           // currentSoldier
        }
    }
    public void EndUseAbility(int ability)
    {


        actionsOnStack.Clear();
        actionData tempAction = actionManager.ConfirmAction(ability, currentSoldier, lookTarget, this.GetComponent<TurnManager>());
        actionsOnStack.Add(tempAction);
        lastActionPress = -1;
        if (actionsOnStack.Count > 0) { CheckReactions(tempAction); }
    }

    public void CheckRange(int maxDistance)
    {
        //if the list has elements toggle through them
      
        if (soldiersInRange.Count > 0 )
        {
            soldiersInRange.RemoveAt(soldiersInRange.Count - 1);
           
        }

        if (soldiersInRange.Count != 0)
        {
         
                lookTarget = soldiersInRange[soldiersInRange.Count - 1];
            targetMarker.transform.position = lookTarget.transform.position;
            tohittext.text = ( CalculateToHit()).ToString();
        }
        else
        {
            foreach (Soldier el in activeSoldiers)
            {
                //el != currentSoldier &&
                if ( Vector3.Distance(currentSoldier.transform.position, el.transform.position) <= maxDistance)
                    {

                        if ((el.team != currentSoldier.team && focusTeam == false) || (el.team == currentSoldier.team && focusTeam == true)) { soldiersInRange.Add(el); }


                    }
                
                         else { soldiersInRange.Add(el); }
            }
            if (soldiersInRange.Count != 0)
            {
                lookTarget = soldiersInRange[soldiersInRange.Count - 1];
                targetMarker.transform.position = lookTarget.transform.position;
                tohittext.text = ( CalculateToHit()).ToString();
            }
        }

       // if (lookTarget != null && lookTarget != currentSoldier) { targetMarker.transform.position = lookTarget.transform.position; }
    }
    public int CalculateToHit()
    {
        int accmod = 0;

        

            RaycastHit hit;
        var ray = gun.transform.position;
        int count = 0;
        foreach (Transform el in lookTarget.GetComponent<Soldier>().aimPoints.transform)
        {
           
            if (Physics.Raycast(gun.transform.position, (el.transform.position - gun.transform.position), out hit))
            {
                accRaycastHitIndicatorParent.GetChild(count).transform.position = hit.point;
                //Debug.Log("hit: " + hit.transform.name);
                if (hit.transform == el)
                {
                    accmod += 10;
                    //Debug.Log("acc mod +10");
                }
            }
            count++;
        }
        int acc = (20 + accmod + currentSoldier.accuracy) - ((int)Vector3.Distance(currentSoldier.transform.position, lookTarget.transform.position) * 2);
        if (accmod <= 0) { acc = 0; }
       // Debug.Log("To Hit: " + acc.ToString());
        return acc;
    }
    public void EndTurn()
    {
        lastActionPress = -1;
        soldiersInRange.Clear(); focusTeam = false;
        foreach (Transform el in accRaycastHitIndicatorParent)
        { el.transform.localPosition = Vector3.zero; }
        soldiersInRange.Clear();
        currentSoldier.SpentTurn();

        if (currentSoldier == activeSoldiers[0])
        {
            //int count2 = reactsOnStack.Count - 1;
            //while (count2 > 0)
            //{

            //    if (reactsOnStack[count2].turnsactive != -1) { reactsOnStack.RemoveAt(count2); }
            //    count2--;
            //}
            gameManager.EndOfRound();
            SortSoldierList();
        }
        else
        {


         
            int count = activeSoldiers.Count - 1;
            while (count >= 0)
            {
                if (activeSoldiers[count].spentTurn == false)
                {
                    currentSoldier = activeSoldiers[count];
                   
                    moveRemaining = currentSoldier.SpendMove(0);

                    lastpos = currentSoldier.transform.position;
                    posmark = currentSoldier.transform.position;
                    currentSoldier.Focus();

                    uiManager.SetTurnList(activeSoldiers);


                    uiManager.SetSoldierDisplayerText(currentSoldier);
                    lookTarget = currentSoldier; 
                    cam.GetComponent<ThirdPersonOrbitCam>().player = currentSoldier.transform;
                    actionRemaining = currentSoldier.actionpoints;
                    movepointsText.text = currentSoldier.movepoints.ToString();
                    actionpointstext.text = currentSoldier.actionpoints.ToString();
                    return;
                }
                count--;
            }

        }
        

    }

    public void IncrementMove(int amount)
    {
        moveRemaining = currentSoldier.SpendMove(amount);
        
        lastpos = posmark;
        posmark = currentSoldier.transform.position;
        //if move clear the range list so out of reange dont remain, and in range enemies get added
        soldiersInRange.Clear();
        movepointsText.text = moveRemaining.ToString();
    }
    public void SortSoldierList()
    {

        activeSoldiers.Sort((s1, s2) => (s1.loadout.speed * 10 + (s1.loadout.subspeed)).CompareTo(s2.loadout.speed * 10 + (s2.loadout.subspeed)));
        reactsOnStack.Clear();
        foreach (Soldier el in activeSoldiers)
        {
            if (el.loadout.reactionActions.Count > 0)
            {

                reactsOnStack.Add(actionManager.ConfirmReaction(el.loadout.reactionActions[0], el, GetComponent<TurnManager>()));
                Debug.Log("adding react");
            }

        }
        currentSoldier = activeSoldiers[activeSoldiers.Count - 1];
        currentSoldier.Focus();
       // uiManager.SetSoldierDisplayerText(currentSoldier);
        uiManager.SetTurnList(activeSoldiers);
        lookTarget = currentSoldier;
        movepointsText.text = currentSoldier.movepoints.ToString();
        actionRemaining = currentSoldier.actionpoints;
        actionpointstext.text = currentSoldier.actionpoints.ToString();
        lastpos = currentSoldier.transform.position;
        posmark = currentSoldier.transform.position;
        cam.GetComponent<ThirdPersonOrbitCam>().player = currentSoldier.transform;
       
    }
}
