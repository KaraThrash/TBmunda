using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TurnManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject cam,activeMarker,gun,targetMarker,lookTarget,moveLimiter,bulletPrefab, bulletPrefab2;
    public Transform accRaycastHitIndicatorParent;
    public Soldier currentSoldier;
    public int actionRemaining, moveRemaining, wildRemaining,lastActionPress;
    public Vector3 posmark, lastpos; //location to track movement spent
    public List<Soldier>  activeSoldiers;
    public List<Soldier> soldiersInRange;
    public Text movepointsText, actionpointstext, tohittext,resultdisplay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSoldier != null)
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
                AbilityButtonPress(0);
            }
            if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                AbilityButtonPress(0);
            }
        }
       
             if (Input.GetKeyUp(KeyCode.Y))
        {
            SortSoldierList();
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
           // Debug.Log("who is this:  " + transform.name);
            EndTurn();
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            CheckRange(5);
        }

    }

    public void AbilityButtonPress(int abilityNumber)
    {

        if (actionRemaining > 0)
        {
            if (lastActionPress != abilityNumber) { StartUseAbility(abilityNumber); }
            else { EndUseAbility(abilityNumber); }
        }
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
        if (ability == 0 )
        {
            if (lookTarget == currentSoldier.gameObject) { CheckRange(5); resultdisplay.text = "looking at self "; }

            if (lookTarget != currentSoldier.gameObject)
            {
                actionRemaining--;
                actionpointstext.text = actionRemaining.ToString();

                int tohit = CalculateToHit();
                int roll = Random.Range(1, 101);
                if (tohit > roll)
                {
                    lookTarget.GetComponent<Soldier>().TakeDamage(1);
                    lastActionPress = -1;
                    GameObject clone = Instantiate(bulletPrefab2, gun.transform.position, transform.rotation) as GameObject;
  
                    clone.GetComponent<Bullet>().holdvel = (lookTarget.transform.position - gun.transform.position) + transform.up;
                    resultdisplay.text = "hit : " + lookTarget.transform.name + " :hp: " + lookTarget.GetComponent<Soldier>().currenthp.ToString();
                }
                else {
                   
                    //check to see if miss destroys something
                    Vector3 newdir = new Vector3(lookTarget.transform.position.x + (  Random.Range(-0.3f,1.0f)), lookTarget.transform.position.y + (Random.Range(-0.3f, 1.0f)), lookTarget.transform.position.z + (Random.Range(-0.3f, 1.0f))) - gun.transform.position;
                    resultdisplay.text = "missed " + lookTarget.transform.name;
                    RaycastHit hit;
                    GameObject clone = Instantiate(bulletPrefab,gun.transform.position,transform.rotation) as GameObject;
                 
                    clone.GetComponent<Bullet>().holdvel = newdir;
                    if (Physics.Raycast(gun.transform.position, newdir, out hit, 15.0f))
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

    public void CheckRange(int maxDistance)
    {
        //if the list has elements toggle through them
        if (soldiersInRange.Count != 0)
        {
            soldiersInRange.RemoveAt(soldiersInRange.Count - 1);
          
        }

        if (soldiersInRange.Count != 0)
        {
         
                lookTarget = soldiersInRange[soldiersInRange.Count - 1].gameObject;
            targetMarker.transform.position = lookTarget.transform.position;
            tohittext.text = ( CalculateToHit()).ToString();
        }
        else
        {
            foreach (Soldier el in activeSoldiers)
            {
                if (el != currentSoldier && Vector3.Distance(currentSoldier.transform.position, el.transform.position) <= maxDistance)
                { soldiersInRange.Add(el); }

            }
            if (soldiersInRange.Count != 0)
            {
                lookTarget = soldiersInRange[soldiersInRange.Count - 1].gameObject;
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
        foreach (Transform el in accRaycastHitIndicatorParent)
        { el.transform.localPosition = Vector3.zero; }
        soldiersInRange.Clear();
        currentSoldier.SpentTurn();

        if (currentSoldier == activeSoldiers[0])
        {
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
                    lookTarget = currentSoldier.gameObject;
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

        currentSoldier = activeSoldiers[activeSoldiers.Count - 1];
        currentSoldier.Focus();
        lookTarget = currentSoldier.gameObject;
        movepointsText.text = currentSoldier.movepoints.ToString();
        actionRemaining = currentSoldier.actionpoints;
        actionpointstext.text = currentSoldier.actionpoints.ToString();
        lastpos = currentSoldier.transform.position;
        posmark = currentSoldier.transform.position;
        cam.GetComponent<ThirdPersonOrbitCam>().player = currentSoldier.transform;
       
    }
}
