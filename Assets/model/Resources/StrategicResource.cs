using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class StrategicResource : Resource
{
    public string Name;
    public GameObject Model;
    public int MinAmount;
    public int MaxAmount;
    public Tile.TerrainType RequiredType;
    public int BaseYieldFood;
    public int BaseYieldProduction;
}
