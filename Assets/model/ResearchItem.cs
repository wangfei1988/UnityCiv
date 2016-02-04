using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ResearchItem : Object {

    public string Title;
    public Sprite Image;
    public string Tooltip;
    public float ProductionCosts;
    public List<Object> LeadsTo;
}
