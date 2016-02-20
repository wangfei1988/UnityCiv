using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class Village : IGameBuilding
{
    public GameObject Scaffold;

    public static bool InBuildMode;
    public static Tile BuildModeHoveredTile;
    private Phase1Building BuildModeBuilding;
    private GameObject BuildModeScaffold;
    private BuildItem BuildModeBuildItem;

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
        item.LoadObject();
        var b = item.Produces.GetComponent<Phase1Building>();

        if (b != null)
        {
            // first the player has to select a location where to build
            InBuildMode = true;
            BuildModeBuilding = Instantiate(b);
            BuildModeBuildItem = item;
        }
        else
        {
            Producing = new BuildOrder(item, 0);
            TimeManager.instance.NoMoreOrdersNeeded(this);
        }
        Debug.Log("Producing now " + item.Title);
    }

    private Tile lastTile;
    void Update()
    {
        if (BuildModeBuilding != null)
        {
            if (lastTile != BuildModeHoveredTile)
            {
                var adjacencyCheck = GridManager.instance.GetHexArea(BuildModeHoveredTile, BuildModeBuilding.Range);
                bool buildable = adjacencyCheck.Any(a => a.InPlayerTerritory);
                BuildModeBuilding.SetColor(buildable ? Phase1Building.ColorPresets.Green : Phase1Building.ColorPresets.Red);
                lastTile = BuildModeHoveredTile;
                BuildModeBuilding.transform.position = GridManager.instance.calcWorldCoord(new Vector2(lastTile.X + lastTile.Y / 2, lastTile.Y));
            }

            if (Input.GetMouseButtonDown(0))
            {
                var adjacencyCheck = GridManager.instance.GetHexArea(BuildModeHoveredTile, BuildModeBuilding.Range);
                bool buildable = adjacencyCheck.Any(a => a.InPlayerTerritory);
                // if at least one tile in or near the area is already player territory
                if (buildable)
                {
                    Producing = new BuildOrder(BuildModeBuildItem, 0);
                    BuildModeScaffold = Instantiate(Scaffold);
                    BuildModeScaffold.transform.position = BuildModeBuilding.transform.position;
                    Destroy(BuildModeBuilding.gameObject);
                    BuildModeBuilding = null;
                    TimeManager.instance.NoMoreOrdersNeeded(this);
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Destroy(BuildModeBuilding.gameObject);
                BuildModeBuilding = null;
                InBuildMode = false;
            }
        }
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
        for (int i = 0; i < 2; i ++)
            buildItems.Add(GameManager.instance.AvailableBuildItems[i]);

        foreach (var item in buildItems)
            item.LoadIcon();
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
                if (Producing.Item.Produces == null)
                    Producing.Item.LoadObject();

                GameObject produces = (GameObject)Producing.Item.Produces;
                var building = produces.GetComponent<Phase1Building>();
                if (building != null)
                {
                    var go = Instantiate(building);
                    go.transform.position = BuildModeScaffold.transform.position;
                    lastTile.Building = go;
                    Destroy(BuildModeScaffold.gameObject);
                }
                else
                {
                    var movement = produces.GetComponent<CharacterMovement>();
                    if (movement != null)
                    {
                        GridManager.instance.Spawn(produces, new Vector2(Location.X + Location.Y / 2, Location.Y));
                    }

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
