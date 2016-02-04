using UnityEngine;
using System.Collections.Generic;
using System;

public class Research : MonoBehaviour {

    public Sprite[] Images;

    public static readonly ResearchItem[] ResearchItems = new ResearchItem[]
    {
        new ResearchItem()
        {
            Title = "Animal Husbandry",
            Tooltip = "Research Animal Husbandry",
            ProductionCosts = 300
        }
    };

    public static Research instance = null;

    void Awake()
    {
        instance = this;

        // set images
        for (int i = 0; i < ResearchItems.Length; i++)
            ResearchItems[i].Image = Images[i];
    }

	// Use this for initialization
	void Start () {
	
	}

    public void FinishedResearching(String item)
    {
        // find ResearchItem
        //ResearchItems.First(ri => ri)
    }
}
