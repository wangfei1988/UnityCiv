﻿using UnityEngine;
using System.Collections;

public abstract class IGameUnit : IEntity {

    public abstract string[] Actions
    {
        get;
        protected set;
    }

    public abstract void UseAction(string action);

    public override void Select()
    {
        GridManager.instance.selectedUnit = gameObject;
    }

    protected virtual void Start()
    {
        GridManager.instance.allUnits.Add(gameObject);
    }

    /// <summary>
    /// Remove a unit/character from the game with this method instead of directly calling "Destroy"
    /// </summary>
    public void RemoveCharacter()
    {
        Destroy(this.gameObject);
        if (GridManager.instance.selectedUnit == gameObject)
        {
            GridManager.instance.selectedUnit = null;
            UnitPanelUI.instance.SetUnitPanelInfo(null);
        }
        GridManager.instance.allUnits.Remove(gameObject);
    }
}
