using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class StrategicResource {

    public string Title;
    public GameObject Model;
    public int MinAmount;
    public int MaxAmount;
    public Tile.TerrainType RequiredType;

}
