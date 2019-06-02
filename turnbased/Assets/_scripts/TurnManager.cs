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
    public int actionRemaining, moveRemaining, wildRemaining,lastActionPress,targetInList;
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
                { reacting = false;
                    if (moveRemaining > 0) { currentSoldier.Focus(); }
                }
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
        if (actionsOnStack.Count > 1) { currentSoldier.FocusOff(); }
     
        Debug.Log("action loop: " + actionsOnStack.Count.ToString());
       
        watchActionTimer = actionsOnStack[actionsOnStack.Count - 1].actionTime;
        reactCam.transform.position = actionsOnStack[actionsOnStack.Count - 1].actor.transform.position;
        reactCam.transform.LookAt(actionsOnStack[actionsOnStack.Count - 1].target.transform);
        //cam.GetComponent<ThirdPersonOrbitCam>().player = actionsOnStack[actionsOnStack.Count - 1].actor.transform;

        actionManager.PerformAction(actionsOnStack[actionsOnStack.Count - 1],GetComponent<TurnManager>());
        actionsOnStack.RemoveAt(actionsOnStack.Count - 1);
        uiManager.SetTurnList(activeSoldiers);
        if (actionRemaining <= 0) { EndTurn(); }
    }
    public void SpendActionPoints(int spent)
    {
        if (spent == -1) {

            actionRemaining = 0;
         
        }
        else {

            actionRemaining -= spent;
        }
        currentSoldier.actionpoints = actionRemaining;
        actionpointstext.text = actionRemaining.ToString();
       
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
            {
            EndUseAbility(2);
            if (actionManager.CheckActionPoints(2, moveRemaining, currentSoldier, currentSoldier, GetComponent<TurnManager>()) == true)
                 { }
            
          //  IncrementMove(1);

                }

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
                AbilityButtonPress(3);
            }
   

        if (Input.GetKeyUp(KeyCode.T))
        {
            // Debug.Log("who is this:  " + transform.name);
            EndTurn();
        }
        if (Input.GetKeyUp(KeyCode.R) && lastActionPress != -1)
        {
           // CheckRange(currentSoldier.loadout.range);
            TabTarget(0,currentSoldier.team);
        }
        if (Input.GetKeyUp(KeyCode.F) && lastActionPress != -1)
        {
            //CheckRange(currentSoldier.loadout.range);
            TabTarget(1, currentSoldier.team);
        }
        if (Input.GetKeyUp(KeyCode.V) && lastActionPress != -1)
        {
            //CheckRange(currentSoldier.loadout.range);
            TabTarget(2, currentSoldier.team);
        }
    }
    public void CheckReactions(actionData act)
    {
       
        foreach (reaction el in reactsOnStack)
        {
            Debug.Log("check react");
            //overwatch is temporary
            if (el.actor.inOverwatch == false && el.action == 1) { }
            else
            {
                //confirm the action matches the reaction event
                if (act.reactType == el.reacttype && Vector3.Distance(el.actor.transform.position, act.loc) < el.range && el.actor.currentreactPoints >= el.cost)
                {

                    //confirm the target is the correct team
                    if (((act.team == el.actor.team && el.teamtype == 0) || (act.team != el.actor.team && el.teamtype == 1)) && el.actor != act.actor)
                    {
                        Debug.Log(" confirmed react");
                        actionsOnStack.Add(actionManager.ConfirmAction(el.action, el.actor, act.target, this.GetComponent<TurnManager>()));


                        el.actor.usedReaction = true;
                        el.actor.currentreactPoints -= el.cost;
                        if (el.actor.inOverwatch == true && el.action == 1) { el.actor.inOverwatch = false; }
                    }
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

            //  if (focusTeam == true && (int)currentSoldier.loadout.actions[abilityNumber].y == 1) { soldiersInRange.Clear(); focusTeam = false; }
            // if (focusTeam == false && (int)currentSoldier.loadout.actions[abilityNumber].y == 0) { soldiersInRange.Clear(); focusTeam = true; }

            // if (soldiersInRange.Count == 0) { CheckRange(currentSoldier.loadout.range); }
            UpdateTargetList(currentSoldier.loadout.range);
            if (abilityNumber < currentSoldier.loadout.actions.Count)
            {
                TabTarget((int)currentSoldier.loadout.actions[abilityNumber].y, currentSoldier.team);
            }

            if (actionManager.CheckActionPoints(abilityNumber,actionRemaining,currentSoldier ,lookTarget, GetComponent<TurnManager>()) == true)
            {
                StartUseAbility(abilityNumber);
                confirmaction.text = "Action: " + abilityNumber.ToString();
                lastActionPress = abilityNumber;
            }
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
        if (actionManager.CheckActionPoints(lastActionPress, actionRemaining, currentSoldier, lookTarget, GetComponent<TurnManager>()) == false) { lastActionPress = -1; }
       // lastActionPress = -1;
        if (actionsOnStack.Count > 0) { CheckReactions(tempAction); }
    }

    public bool CheckLineOfSight(Soldier actingSolider,Soldier targetSoldier)
    {
        RaycastHit hit;
        var ray = actingSolider.eyes.transform.position;
        foreach (Transform el in targetSoldier.aimPoints)
        {
                if (Physics.Raycast(gun.transform.position, (el.transform.position - gun.transform.position), out hit))
                {
                    if (hit.transform == el)
                    { return true; }
            
                }
        }

        return false;
    }

    public bool CheckRange(int maxDistance,Soldier actingSoldier, Soldier targetSoldier)
    {
        //if the list has elements toggle through them
        return maxDistance >= Vector3.Distance(actingSoldier.transform.position,targetSoldier.transform.position);

        //if (soldiersInRange.Count > 0 )
        //{
        //    soldiersInRange.RemoveAt(soldiersInRange.Count - 1);
           
        //}

        //if (soldiersInRange.Count != 0)
        //{
         
        //        lookTarget = soldiersInRange[soldiersInRange.Count - 1];
        //    targetMarker.transform.position = lookTarget.transform.position;
        //    tohittext.text = ( CalculateToHit()).ToString();
        //}
        //else
        //{
        //    foreach (Soldier el in activeSoldiers)
        //    {
        //        //el != currentSoldier &&
        //        if ( Vector3.Distance(currentSoldier.transform.position, el.transform.position) <= maxDistance)
        //            {
        //            soldiersInRange.Add(el);
        //            //note: can always target everyone.

        //        //    if ((el.team != currentSoldier.team && focusTeam == false) || (el.team == currentSoldier.team && focusTeam == true)) { soldiersInRange.Add(el); }
        //          //  else { soldiersInRange.Add(el); }

        //             }
                
                         
        //    }
        //    if (soldiersInRange.Count != 0)
        //    {
        //        lookTarget = soldiersInRange[soldiersInRange.Count - 1];
        //        targetMarker.transform.position = lookTarget.transform.position;
        //        tohittext.text = ( CalculateToHit()).ToString();
        //    }
        //}

       // if (lookTarget != null && lookTarget != currentSoldier) { targetMarker.transform.position = lookTarget.transform.position; }
    }

    public void UpdateTargetList(int maxDistance)
    {
        soldiersInRange.Clear();
        foreach (Soldier el in activeSoldiers)
        {
            if (Vector3.Distance(currentSoldier.transform.position, el.transform.position) <= maxDistance)
            {
                soldiersInRange.Add(el);
            }


        }
        soldiersInRange.Sort((s1, s2) => (Vector3.Distance(s1.transform.position,currentSoldier.transform.position)).CompareTo(Vector3.Distance(s2.transform.position, currentSoldier.transform.position)));
    }

    public Soldier TabTarget(int type,int teamtarget)// all, friendlies, enemies
    {

        targetInList++;
        int count = 0;//to never get stuck in an infinite target loop 
        while (count < soldiersInRange.Count) {
            if (targetInList > soldiersInRange.Count - 1)
            { targetInList = 0; }
           
                while ((soldiersInRange[targetInList].team != teamtarget && type == 0) || (soldiersInRange[targetInList].team == teamtarget && type == 1) && count < soldiersInRange.Count)
                {

                    targetInList++;
                    if (targetInList > soldiersInRange.Count - 1)
                    { targetInList = 0; }
                    count++;
                }
            
        


            
            count++;
        }

        lookTarget = soldiersInRange[targetInList];
        targetMarker.transform.position = lookTarget.transform.position;
        tohittext.text = (CalculateToHit()).ToString();

        //todo: note when no one is in range
         if (lookTarget != null ) { targetMarker.transform.position = lookTarget.transform.position; }


        return soldiersInRange[targetInList];
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
        targetInList = 0;
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
    
        if (moveRemaining > 0)
        {
            moveRemaining = currentSoldier.SpendMove(amount);
            lastpos = posmark;
            posmark = currentSoldier.transform.position;
            //if move clear the range list so out of reange dont remain, and in range enemies get added
            soldiersInRange.Clear();

            UpdateTargetList(currentSoldier.loadout.range);
        }
        actionpointstext.text = currentSoldier.actionpoints.ToString();
        movepointsText.text = moveRemaining.ToString();
        if (moveRemaining <= 0) { currentSoldier.FocusOff();Debug.Log("NoMoveLeft"); }
    }
    public void SortSoldierList()
    {

        activeSoldiers.Sort((s1, s2) => (s1.loadout.speed * 10 + (s1.loadout.subspeed)).CompareTo(s2.loadout.speed * 10 + (s2.loadout.subspeed)));
        reactsOnStack.Clear();
        foreach (Soldier el in activeSoldiers)
        {
            if (el.loadout.reactionActions.Count > 0)
            {
                //add all reactions
                foreach (int elr in el.loadout.reactionActions)
                {
                    if (el.inOverwatch == false && elr == 1)
                    { }
                    else { 
                        reactsOnStack.Add(actionManager.ConfirmReaction(elr, el, GetComponent<TurnManager>()));
                          }
                }
                Debug.Log("adding react");
            }

        }
        currentSoldier = activeSoldiers[activeSoldiers.Count - 1];
        currentSoldier.Focus();
        moveRemaining = currentSoldier.SpendMove(0);


        UpdateTargetList(15);
        TabTarget(0,0);
        posmark = currentSoldier.transform.position;
       // uiManager.SetSoldierDisplayerText(currentSoldier);
        uiManager.SetTurnList(activeSoldiers);
        lookTarget = currentSoldier;
        movepointsText.text = currentSoldier.movepoints.ToString();
        actionRemaining = currentSoldier.actionpoints;
        actionpointstext.text = currentSoldier.actionpoints.ToString();
        lastpos = currentSoldier.transform.position;
        posmark = currentSoldier.transform.position;
        moveRemaining = currentSoldier.SpendMove(0);


        





        cam.GetComponent<ThirdPersonOrbitCam>().player = currentSoldier.transform;
       
    }
}
