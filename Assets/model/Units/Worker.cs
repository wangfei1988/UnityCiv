using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class Worker : IGameUnit {
    
    public GameObject HuntingShackPrefab;
    public Sprite IconHuntingShack;
    public AudioClip WorkerStartWork;
    
    protected CharacterMovement movement;

    private static List<Phase1Building> Actions = new List<Phase1Building>();

    public override void UseAction(int action)
    {
        Actions.ElementAt(action);
        /*if (action == 0)
        {
            if (movement.ExpendMovementPoints(2))
            {
                GameObject village = Instantiate(VillagePrefab);
                village.transform.position = transform.position;
                var gb = village.GetComponent<IGameBuilding>();
                gb.Location = movement.curTile;
                TimeManager.instance.NoMoreOrdersNeeded(this);
                RemoveCharacter();
                gb.audioSource.PlayOneShot(ExpandAudioClip, 1f);
                LeaveAction(_settlementHexArea);
            }
        }*/
    }

    public override void Select()
    {
        base.Select();
        // draw own panel
        UnitPanelUI.instance.SetUnitPanelInfo(Actions.Select(a => new UnitPanelUI.UnitInfo(a.Icon, a.Tooltip)).ToArray());
    }

    protected override void Awake()
    {
        base.Awake();
        movement = GetComponent<CharacterMovement>();
        movement.MovementPointsMax = 2;
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
    protected override void Start()
    {
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

    public override bool NeedsOrders()
    {
        if (movement.RemainingPath.Count > 1 || movement.MovementPointsRemaining == 0)
        {
            return false;
        }
        return true;
    }
}
