using UnityEngine;
using System.Collections;

public abstract class IGameUnit : IEntity {

    public abstract string[] Actions
    {
        get;
        protected set;
    }

    public abstract void UseAction(int action);

    public abstract void HoverAction(int action);

    public abstract void LeaveAction(int action);

    public override void Select()
    {
        GridManager.instance.selectedUnit = gameObject;
        BuildingPanelUI.instance.SetBuildItems(null, 0);
        var movement = gameObject.GetComponent<CharacterMovement>();
        if (movement != null)
            movement.DrawPath();
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
        if (GridManager.instance.selectedUnit == gameObject)
        {
            GridManager.instance.selectedUnit = null;
            UnitPanelUI.instance.SetUnitPanelInfo(null);
            var movement = gameObject.GetComponent<CharacterMovement>();
            if (movement != null)
                movement.RemoveWayMarkers();
        }
        GridManager.instance.allUnits.Remove(gameObject);
        Destroy(gameObject);
    }
}
