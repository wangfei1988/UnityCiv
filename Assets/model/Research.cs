using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Research {

    // X: 0 - 6, Y: 0 - ?
    public readonly ResearchItem[] ResearchItems = new ResearchItem[]
    {
        new ResearchItem()
        {
            Title = "Animal Husbandry",
            Tooltip = "Research Animal Husbandry",
            ProductionCosts = 300,
            X = 0,
            Y = 1
        },
        new ResearchItem()
        {
            Title = "Stone Tools",
            Tooltip = "Research Stone Tools",
            ProductionCosts = 300,
            X = 0,
            Y = 2
        },
        new ResearchItem()
        {
            Title = "Wood Working",
            Tooltip = "Research Wood Working",
            ProductionCosts = 300,
            X = 1,
            Y = 2
        }
    };
    

    public Research(Sprite[] images)
    {
        // set images
        for (int i = 0; i < ResearchItems.Length; i++)
            ResearchItems[i].Image = images[i];
        // connect researches
        ResearchItems[0].Children = new List<ResearchItem>() { ResearchItems[2] };
        ResearchItems[1].Children = new List<ResearchItem>() { ResearchItems[2] };
    }

    public void FinishedResearching(string item)
    {
        // find ResearchItem
        ResearchItems.First(ri => ri.Title == item);
    }
}
