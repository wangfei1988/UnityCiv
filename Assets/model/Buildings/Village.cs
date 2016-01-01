using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class Village : IGameBuilding {

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

    public BuildOrder Producing
    {
        get;
        private set;
    }

    public override void Select()
    {
        BuildingPanelUI.instance.SetBuildItems(buildItems.ToArray());
    }

    protected override void Start()
    {
        base.Start();
        buildItems.Add(GameManager.instance.AvailableBuildItems.First());
    }
}
