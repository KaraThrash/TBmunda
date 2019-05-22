using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    // Start is called before the first frame update
    public bool focused;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleFocus()
    {
        if (focused == true) {
            focused = false;
           
            //GetComponent<Animator>().speed = 0;
            GetComponent<Animator>().Play("Change");
            GetComponent<BasicBehaviour>().enabled = false ;
            GetComponent<MoveBehaviour>().enabled = false;
            GetComponent<AimBehaviour>().enabled = false;
            GetComponent<FlyBehaviour>().enabled = false;
        }
        else { focused = true;
            GetComponent<Animator>().Play("Idle");
            GetComponent<BasicBehaviour>().enabled = true;
            GetComponent<MoveBehaviour>().enabled = true;
            GetComponent<AimBehaviour>().enabled = true;
            GetComponent<FlyBehaviour>().enabled = true;
        }
    }

}
