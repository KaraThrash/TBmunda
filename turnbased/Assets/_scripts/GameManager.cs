using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject cam;
    public List<Soldier> team0, team1;
    public int activeTeam, soldierInList;
    public Soldier selected;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) { ChangeFocus(); }
        if (Input.GetKeyDown(KeyCode.LeftShift)) { ChangeTeam(); }
       
    }
    public void ChangeFocus()
    {
        if (selected != null) { selected.ToggleFocus(); }
        soldierInList++;
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

    public void ChangeTeam()
    {
        if (selected != null) { selected.ToggleFocus(); }
       
        soldierInList = 0;
        if (activeTeam == 0)
        {
            activeTeam = 1;


          
            team1[soldierInList].ToggleFocus();
            selected = team1[soldierInList];

        }
        else
        {

            activeTeam = 0;
           
            team0[soldierInList].ToggleFocus();
            selected = team0[soldierInList];
        }
        cam.GetComponent<ThirdPersonOrbitCam>().player = selected.transform;

    }

}
