using UnityEngine;
using System.Collections;
using System;

public abstract class Phase1Building : IEntity {

    public int Range;

    public override bool NeedsOrders()
    {
        return false;
    }

    public override void Select()
    { }
}
