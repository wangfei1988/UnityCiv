using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LevelCreator : MonoBehaviour {
    
    public GameObject Settler;
    private float treePlacementTolerance = 0.01f;
    public StrategicResource[] StrategicResources;

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

        // keep check on strategic resources
        Dictionary<StrategicResource, int> placedResources = new Dictionary<StrategicResource, int>();
        foreach (var res in StrategicResources)
            placedResources.Add(res, rnd.Next(res.MinAmount, res.MaxAmount));

        foreach (var tile in boardValues)
        {
            if (tile.Type == Tile.TerrainType.UNASSIGNED)
            {
                var tilecenter = gm.calcWorldCoord(new Vector2(tile.Location.X + tile.Location.Y / 2, tile.Location.Y));

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

                        PlaceTreesOnTile(gm, rnd, t, worldpos, terrainSize, tilecenter);
                    }
                    else
                    {
                        tile.Type = Tile.TerrainType.GRASS;

                    }
                }

                var matchingResource = placedResources.Keys.FirstOrDefault(f => placedResources[f] > 0 && f.RequiredType == tile.Type);
                if (matchingResource != null)
                {
                    GameObject res = Instantiate(matchingResource.Model);
                    res.transform.position = tilecenter;
                    tile.StrategicResource = matchingResource;
                    placedResources[matchingResource]--;
                }
            }
        }

        Debug.Log("Tree x min max:" + t.terrainData.treeInstances.Min(ti => ti.position.x) + " " + t.terrainData.treeInstances.Max(ti => ti.position.x));
        Debug.Log("Tree z min max:" + t.terrainData.treeInstances.Min(ti => ti.position.z) + " " + t.terrainData.treeInstances.Max(ti => ti.position.z));

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

        CreateSettler();
    }

    private void PlaceTreesOnTile(GridManager gm, System.Random rnd, Terrain t, Vector3 worldpos, Vector3 terrainSize, Vector3 tilecenter)
    {
        var amountOfTreesOnTile = rnd.Next(9, 14);
        float r = Math.Min(gm.hexWidth, gm.hexHeight) / 2;
        List<float[]> existingOffsets = new List<float[]>();
        for (int i = 0; i < amountOfTreesOnTile; i++)
        {
            // find a place to plant the tree
            float xoffset = -99;
            float zoffset = -99;
            for (int placeTry = 0; placeTry < 10; placeTry++)
            {
                var rot = rnd.NextDouble() * 2 * Math.PI;
                float dist = (float)rnd.NextDouble() * r;
                var xo = (float)Math.Cos(rot) * dist;
                var zo = (float)Math.Sin(rot) * dist;
                // if there's not a tree already
                if (!existingOffsets.Any(e => (e[0] - xo) * (e[0] - xo) + (e[1] - zo) * (e[1] - zo) <= treePlacementTolerance))
                {
                    xoffset = xo;
                    zoffset = zo;
                    break;
                }
            }
            // no suitable tree position found
            if (xoffset == -99)
            {
                Debug.Log("aborted");
                break;
            }

            TreeInstance ti = new TreeInstance();
            ti.prototypeIndex = 0;
            ti.heightScale = 0.02f;
            ti.widthScale = 0.02f;
            ti.color = Color.white;
            ti.position = new Vector3((tilecenter.x - worldpos.x + xoffset) / terrainSize.x, 0, (tilecenter.z - worldpos.z + zoffset) / terrainSize.z);
            //ti.position = new Vector3((tilecenter.x - worldpos.x) / terrainSize.x, 0, (tilecenter.z - worldpos.z) / terrainSize.z);
            if (ti.position.x < 0)
            {
                Debug.Log(tilecenter.x + " " + worldpos.x);
            }
            //Debug.Log("Tree: " + ti.position.x + " " + ti.position.z);
            existingOffsets.Add(new float[] { xoffset, zoffset });
            t.AddTreeInstance(ti);
        }
    }

    private void CreateSettler()
    {
        GridManager.instance.Spawn(Settler, new Vector2(25, 25));
    }
}
