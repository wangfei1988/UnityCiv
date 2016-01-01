using UnityEngine;
using System.Collections;

public abstract class IGameBuilding : IEntity
{

    public abstract BuildItem[] Items
    {
        get;
        protected set;
    }

    protected virtual void Start()
    {
        GridManager.instance.allBuildings.Add(gameObject);
    }

    /// <summary>
    /// Remove a building from the game with this method instead of directly calling "Destroy"
    /// </summary>
    public void RemoveBuilding()
    {
        Destroy(gameObject);
        GridManager.instance.allBuildings.Remove(gameObject);
    }
}
