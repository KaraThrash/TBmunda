using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject cam;
    public List<Soldier> team0, team1,activeSoldiers,spentTurnSoldiers, masterlist; // using speed instead of by team
    public int activeTeam, soldierInList;
    public Soldier selected;
    public TurnManager turnManager;
    void Start()
    {
       
       // soldierInList = activeSoldiers.Count - 1;
    }

    // Update is called once per frame
    void Update()
    {
       // if (Input.GetKeyDown(KeyCode.Tab)) { ChangeFocus(); }
        //if (Input.GetKeyDown(KeyCode.LeftShift)) { SortSoldierList(); }
        //if (Input.GetKeyUp(KeyCode.T) )
        //{
        //    Debug.Log("who is this:  " + transform.name);
            

        //   TakeTurn();
        //}

    }
    public void ChangeFocus()
    {
        if (selected != null) { selected.ToggleFocus(); }
        //soldierInList++;
        if (activeTeam == 0)
        {

            
            
            if (soldierInList >= team0.Count) { soldierInList = 0; }
            team0[soldierInList].ToggleFocus();
            selected = team0[soldierInList];

        }
        else {
           
           
            if (soldierInList >= team1.Count) { soldierInList = 0; }
            team1[soldierInList].ToggleFocus();
            selected = team1[soldierInList];
        }
        cam.GetComponent<ThirdPersonOrbitCam>().player = selected.transform;
    }
    public void AddSoldierToActiveList(Soldier newSolider)
    {
        activeSoldiers.Add(newSolider);
        SortSoldierList();


    }

    public void SortSoldierList()
    {
        
            activeSoldiers.Sort((s1, s2) => (s1.loadout.speed * 10 + (s1.loadout.subspeed)).CompareTo(s2.loadout.speed * 10 + (s2.loadout.subspeed)));
       // if (selected != null) { selected.ToggleFocus(); }
        selected = activeSoldiers[activeSoldiers.Count - 1];
        selected.ToggleFocus();
        cam.GetComponent<ThirdPersonOrbitCam>().player = selected.transform;
        soldierInList = activeSoldiers.Count - 1;
    }

   

    public void ChangeTeam()
    {
        //if (selected != null) { selected.ToggleFocus(); }
       
        //soldierInList = 0;
        //if (activeTeam == 0)
        //{
        //    activeTeam = 1;


          
        //    team1[soldierInList].ToggleFocus();
        //    selected = team1[soldierInList];

        //}
        //else
        //{

        //    activeTeam = 0;
           
        //    team0[soldierInList].ToggleFocus();
        //    selected = team0[soldierInList];
        //}
        //cam.GetComponent<ThirdPersonOrbitCam>().player = selected.transform;

    }

    public void TakeTurn( )
    {


        Debug.Log("soldiers in list: " + soldierInList.ToString());
        soldierInList = soldierInList - 1 ;
        selected.ToggleFocus();
        selected.SpentTurn();
        if (soldierInList < 0) { EndOfRound(); }
        else
        {
            selected = activeSoldiers[soldierInList];
            selected.ToggleFocus();
            cam.GetComponent<ThirdPersonOrbitCam>().player = selected.transform;
        }
      





    }
    public void EndOfRound()
    {
       // soldierInList = activeSoldiers.Count - 1;
        foreach (Soldier el in activeSoldiers)
        { el.RefreshTurn(); }
       // selected = activeSoldiers[soldierInList];
      //  selected.ToggleFocus();
     
       // cam.GetComponent<ThirdPersonOrbitCam>().player = selected.transform;
    }

}
