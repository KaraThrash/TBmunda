﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loadout : MonoBehaviour
{
    // Start is called before the first frame update
    public int hp, speed, subspeed,range;
    public int currenthp,accuracy;
    public int team, actionpoints, movepoints, variablepoints,reactPoints;
    public bool usedReaction;
    public List<int> availableActions,reactionActions;
    public List<Vector3> actions;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
