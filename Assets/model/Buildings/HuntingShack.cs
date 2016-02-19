using UnityEngine;
using System.Collections;
using System;

public class HuntingShack : Phase1Building {

    protected override void Awake()
    {
        base.Awake();

        Range = 1;
    }

    // Use this for initialization
    void Start () {
	
	}
}
