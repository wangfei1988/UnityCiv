using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LevelCreator : MonoBehaviour {

	public void Create(Dictionary<Point, Tile> board, Vector2 gridSize)
    {
        var gm = GridManager.instance;
        var rnd = new System.Random();
        var t = GetComponent<Terrain>();
        var worldpos = transform.position;
        var terrainSize = t.terrainData.size;

        t.terrainData.treeInstances = new TreeInstance[] { };

        // initialize 1 - 2 redstone fields
        for (var i = 0; i < rnd.Next(2,3); i++)
        {
            var el = board.ElementAt(rnd.Next(0, board.Values.Count - 1));
            el.Value.Type = Tile.TerrainType.REDSTONE;
            el.Value.AllNeighbours.ElementAt(rnd.Next(0, el.Value.AllNeighbours.Count() - 1)).Type = Tile.TerrainType.REDSTONE;
        }

        int amountRedstone = board.Values.Count(tl => tl.Type == Tile.TerrainType.REDSTONE);

        var boardValues = new List<Tile>(board.Values);
        boardValues.Shuffle();

        foreach (var tile in board.Values)
        {
            if (tile.Type == Tile.TerrainType.UNASSIGNED)
            {
                var tilecenter = gm.calcWorldCoord(new Vector2(tile.Location.X, tile.Location.Y));

                var surroundingTypes = tile.Neighbours.Select(tl => tl.Type).ToArray();

                int amountSurroundingRedstone = surroundingTypes.Count(tl => tl == Tile.TerrainType.REDSTONE);
                // max 30 redstone
                if (amountSurroundingRedstone >= 2 && amountRedstone <= 30 && rnd.NextDouble() <= 0.5d + 1d / amountRedstone)
                {
                    tile.Type = Tile.TerrainType.REDSTONE;
                    amountRedstone++;
                }
                else
                {
                    if (rnd.NextDouble() < 0.1)
                    {
                        tile.Type = Tile.TerrainType.DRYGRASS;

                        var amountOfTresOnTile = rnd.Next(2, 4);
                        float r = Math.Min(gm.hexWidth, gm.hexHeight) / 2;
                        for (int i = 0; i < amountOfTresOnTile; i++)
                        {
                            var rot = rnd.NextDouble() * 2 * Math.PI;

                            TreeInstance ti = new TreeInstance();
                            ti.prototypeIndex = 0;
                            ti.heightScale = 0.02f;
                            ti.widthScale = 0.02f;
                            ti.color = Color.white;
                            //ti.position = new Vector3((tilecenter.x - worldpos.x + (float)Math.Cos(rot) * r) / terrainSize.x, 0.0f, (tilecenter.z - worldpos.z + (float)Math.Sin(rot) * r) / terrainSize.z);
                            ti.position = new Vector3((tilecenter.x - worldpos.x) / terrainSize.x, 0, (tilecenter.z - worldpos.z) / terrainSize.z);
                            t.AddTreeInstance(ti);
                        }
                    }
                    else
                    {
                        tile.Type = Tile.TerrainType.GRASS;
                    }
                }
            }
        }


        // Like this: Iterate through every row
        /*for (int y = 0; y < gridSize.y; y++)
        {
            float sizeX = gridSize.x;
            //if the offset row sticks up, reduce the number of hexes in a row
            if (y % 2 != 0 && (gridSize.x + 0.5) * gm.hexWidth > gm.groundWidth)
                sizeX--;
            for (float x = 0; x < sizeX; x++)
            {

            }
        }*/



        var amountUnassigned = board.Values.Count(tl => tl.Type == Tile.TerrainType.UNASSIGNED);
        if (amountUnassigned > 0)
        {
            throw new UnassignedReferenceException(amountUnassigned + " tiles have no type assigned!");
        }
    }
}
