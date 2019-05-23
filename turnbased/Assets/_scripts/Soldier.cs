using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    // Start is called before the first frame update
    public bool focused;
        public bool alert,spentTurn;
    public Transform hpvisual,exhausted;
    public int actionpoints, movepoints ,variablepoints;
    public Vector3 posmark,lastpos; //location to track movement spent
    public Loadout loadout;
    public GameManager gameManager;
  //  public int speed,subspeed;
    void Start()
    {
        loadout = GetComponent<Loadout>();
        gameManager.activeSoldiers.Add(this);
    }

    // Update is called once per frame
    void Update()
    {

        if (focused == true )
        {
            if (Vector3.Distance(posmark, transform.position) > 1)
            { SpendMove(); }
            //if (Input.GetKeyUp(KeyCode.T) && spentTurn == false) {
            //    Debug.Log("who is this:  " + transform.name);
            //    spentTurn = true;
                
            //    gameManager.TakeTurn();
            //}
        }

    }
    public void SpentTurn()
    { exhausted.gameObject.active = true; }

    public void RefreshTurn()
    { exhausted.gameObject.active = false; movepoints = 3; }
    public void SpendMove()
    {
        movepoints--;
        lastpos = posmark;
        posmark = transform.position;

        if (movepoints <= 0 && variablepoints <= 0)
        {
            //TODO: aiming movement should stop too
            GetComponent<Animator>().Play("Change");
            
        }
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
