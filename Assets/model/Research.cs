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
            Title = "From Nomads to Farmers",
            Tooltip = "When Nomads became Farmers...",
            ProductionCosts = 0,
            X = 0,
            Y = 2,
            Completed = true
        },
        new ResearchItem()
        {
            Title = "Animal Husbandry",
            Tooltip = "Research Animal Husbandry",
            ProductionCosts = 300,
            X = 1,
            Y = 1,
            LeadsTo = new List<UnityEngine.Object>()
            {

            }
        },
        new ResearchItem()
        {
            Title = "Stone Tools",
            Tooltip = "Research Stone Tools",
            ProductionCosts = 300,
            X = 1,
            Y = 2
        },
        new ResearchItem()
        {
            Title = "Wood Working",
            Tooltip = "Research Wood Working",
            ProductionCosts = 300,
            X = 2,
            Y = 2
        }
    };
    
    // How much Research does the player complete per round?
    public int ResearchProduction;

    private ResearchItem CurrentResearch;

    public void SetCurrentResearch(ResearchItem item)
    {
        CurrentResearch = item;
    }

    public Research(Sprite[] images)
    {
        // base research production
        ResearchProduction = 3;

        // set images
        for (int i = 0; i < ResearchItems.Length; i++)
            ResearchItems[i].Image = images[i];
        // connect researches
        ResearchItems[0].Children = new List<ResearchItem>() { ResearchItems[1], ResearchItems[2] };
        ResearchItems[1].Children = new List<ResearchItem>() { ResearchItems[3] };
        ResearchItems[2].Children = new List<ResearchItem>() { ResearchItems[3] };
    }

    public void Start()
    {
        // complete first research by artificially calling the next round listener
        CurrentResearch = ResearchItems.First();
        CurrentResearch.ProductionCosts += ResearchProduction;
        NextRoundListener();
    }

    public bool HasResearchSelected()
    {
        return CurrentResearch != null;
    }

    void OnEnable()
    {
        EventManager.StartListening("NextRound", NextRoundListener);
    }

    void OnDisable()
    {
        EventManager.StopListening("NextRound", NextRoundListener);
    }

    private void NextRoundListener()
    {
        CurrentResearch.ProductionCosts -= ResearchProduction;
        if (CurrentResearch.ProductionCosts <= 0)
        {
            CurrentResearch.Completed = true;
            TechTree.instance.SetFinished(CurrentResearch);
            if (CurrentResearch.LeadsTo != null)
            {
                foreach (var lt in CurrentResearch.LeadsTo)
                {
                    ResearchConsequence(lt);
                }
            }
            CurrentResearch = null;
        }
    }

    private void ResearchConsequence(UnityEngine.Object lead)
    {
        // TODO: Use "LeadsTo"
    }
}
