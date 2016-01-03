using UnityEngine;
using System.Collections;
using System;

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

    public override void UseAction(string action)
    {
        if (!movement.IsMoving)
        {
            if (action == "Expand")
            {
                GameObject village = Instantiate(VillagePrefab);
                village.transform.position = transform.position;
                village.GetComponent<IGameBuilding>().Location = movement.curTile;
                RemoveCharacter();
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

    // Use this for initialization
    protected override void Start () {
        base.Start();
    }
}
