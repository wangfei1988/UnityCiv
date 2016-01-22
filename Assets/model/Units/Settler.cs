using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Settler : IGameUnit
{

    public GameObject VillagePrefab;

    public Sprite icon_expand;

    private static String[] actions = new String[]
    {
        "Expand"
    };

    public override string[] Actions
    {
        get
        {
            return actions;
        }

        protected set
        {
            Settler.actions = value;
        }
    }

    protected CharacterMovement movement;

    public override void UseAction(int action)
    {
        if (!movement.IsMoving)
        {
            if (action == 0)
            {
                GameObject village = Instantiate(VillagePrefab);
                village.transform.position = transform.position;
                village.GetComponent<IGameBuilding>().Location = movement.curTile;
                RemoveCharacter();
                LeaveAction(_settlementHexArea);
            }
        }
    }

    public override void Select()
    {
        base.Select();
        // draw own panel
        UnitPanelUI.instance.SetUnitPanelInfo(new UnitPanelUI.UnitInfo[] {
            new UnitPanelUI.UnitInfo(icon_expand, "testtooltip")
        });
    }

    void Awake()
    {
        movement = GetComponent<CharacterMovement>();
    }

    void Update()
    {
        if (_settlementHexArea > -1 && _settlementHexAreaPos != movement.curTile)
        {
            GridManager.instance.DrawHexArea(_settlementHexAreaPos, 2, 0, Tile.TileColorPresets.WhiteTransparent);
            HoverAction(_settlementHexArea);
        }


    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
    }

    private int _settlementHexArea = -1;
    private Tile _settlementHexAreaPos = null;

    public override void HoverAction(int action)
    {
        GridManager.instance.DrawHexArea(movement.curTile, 2, 1, Tile.TileColorPresets.Area);
        _settlementHexArea = action;
        _settlementHexAreaPos = movement.curTile;
    }

    public override void LeaveAction(int action)
    {
        GridManager.instance.DrawHexArea(movement.curTile, 2, 0, Tile.TileColorPresets.WhiteTransparent);
        _settlementHexArea = -1;
    }
}
