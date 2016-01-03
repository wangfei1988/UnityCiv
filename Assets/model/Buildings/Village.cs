using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class Village : IGameBuilding
{
    public float ProductionOutput
    {
        get;
        private set;
    }

    private UnityAction listener;

    // Available items that can be built
    private List<BuildItem> buildItems = new List<BuildItem>();
    public override BuildItem[] Items
    {
        get
        {
            return buildItems.ToArray();
        }

        protected set
        {
            buildItems = new List<BuildItem>(value);
        }
    }

    public override Tile Location
    {
        get;
        set;
    }

    public BuildOrder Producing
    {
        get;
        private set;
    }

    public override void Produce(BuildItem item)
    {
        Producing = new BuildOrder(item, 0);
        Debug.Log("Producing now " + item.Title);
    }

    public override void Select()
    {
        base.Select();
        BuildingPanelUI.instance.SetBuildItems(buildItems.ToArray(), (int)ProductionOutput);
    }

    protected override void Start()
    {
        base.Start();
        buildItems.Add(GameManager.instance.AvailableBuildItems.First());
    }

    void Awake()
    {
        listener = new UnityAction(NextRound);
        ProductionOutput = 10;
    }

    void OnEnable()
    {
        EventManager.StartListening("NextRound", listener);
    }

    void OnDisable()
    {
        EventManager.StopListening("NextRound", listener);
    }

    void NextRound()
    {
        if (Producing != null)
        {
            Producing.Produced += ProductionOutput;
            // is the item completed?
            if (Producing.Produced >= Producing.Item.ProductionCosts)
            {
                var movement = Producing.Item.Produces.GetComponent<CharacterMovement>();
                if (movement != null)
                {
                    GridManager.instance.Spawn(Producing.Item.Produces, new Vector2(Location.X + Location.Y/2, Location.Y));
                }
            }
        }
    }
}
