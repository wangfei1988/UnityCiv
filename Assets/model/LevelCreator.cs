using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelCreator : MonoBehaviour {

	public void Create(Dictionary<Point, Tile> board)
    {
        int testcnt = 0;
        foreach (var tile in board.Values)
        {
            tile.Type = (Tile.TerrainType)(testcnt % 4);
            testcnt++;
        }
    }
}
