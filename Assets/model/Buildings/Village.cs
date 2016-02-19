using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class Village : IGameBuilding
{
    public override float ProductionOutput
    {
        get;
        protected set;
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

    public override BuildOrder Producing
    {
        get;
        protected set;
    }

    public override void Produce(BuildItem item)
    {
        Producing = new BuildOrder(item, 0);
        TimeManager.instance.NoMoreOrdersNeeded(this);
        Debug.Log("Producing now " + item.Title);
    }

    public override void Select()
    {
        base.Select();
        audioSource.PlayOneShot(GameManager.instance.Select1, 0.2f);
        BuildingPanelUI.instance.SetBuildItems(buildItems.ToArray(), (int)ProductionOutput);
    }

    protected override void Start()
    {
        base.Start();
        buildItems.Add(GameManager.instance.AvailableBuildItems.First());
    }

    protected override void Awake()
    {
        base.Awake();
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
                GameObject produces = (GameObject)Producing.Item.Produces;
                var movement = produces.GetComponent<CharacterMovement>();
                if (movement != null)
                {
                    GridManager.instance.Spawn(produces, new Vector2(Location.X + Location.Y / 2, Location.Y));
                }
                Producing = null;
                TimeManager.instance.NeedNewOrders(this);
            }

            // update the building display if this building is currently selected
            if (GridManager.instance.selectedBuilding == gameObject)
                BuildingPanelUI.instance.SetCurrentlyBuilding();
        }
    }

    public override bool NeedsOrders()
    {
        return Producing == null;
    }
}
