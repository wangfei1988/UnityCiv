using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

//Basic skeleton of Tile class which will be used as grid node
public class Tile : GridObject, IHasNeighbours<Tile>
{
    public bool Passable;
    public TerrainType Type;
    public static Sprite[] Sprites;
    public StrategicResource StrategicResource;
    public bool InPlayerTerritory = true;

    public enum TerrainType
    {
        DRYEARTH,
        DRYGRASS,
        REDSTONE,
        GRASS,
        UNASSIGNED = 99
    }

    public Tile(int x, int y)
        : base(x, y)
    {
        Passable = true;
        Type = TerrainType.UNASSIGNED;
    }

    public Dictionary<Resource, int> GetTileResources()
    {
        if (!InPlayerTerritory)
            return null;

        return new Dictionary<Resource, int>()
        {
            { Food.i, 3 },
            { Production.i, 2 }
        };
    }

    /// <summary>
    /// Display the tile resource icons
    /// </summary>
    public void DisplayTileResources()
    {
        if (!InPlayerTerritory)
            return;

        if (tileResourceDisplay == null)
        {
            tileResourceDisplay = UnityEngine.Object.Instantiate(GameManager.instance.TileValueDisplayPrefab);
            tileResourceDisplay.transform.position = Representation.transform.position;
        }

        var trd = tileResourceDisplay.GetComponent<TileValueDisplay>();
        trd.SetTile(this);
        tileResourceDisplay.gameObject.SetActive(true);
    }
    private GameObject tileResourceDisplay = null;

    /// <summary>
    /// Hide the tile resource icons
    /// </summary>
    public void HideTileResources()
    {
        if (tileResourceDisplay != null)
            tileResourceDisplay.SetActive(false);
    }

    public GameObject Representation { get; set; }

    public IEnumerable<Tile> AllNeighbours { get; set; }
    public IEnumerable<Tile> Neighbours
    {
        get { return AllNeighbours.Where(o => o.Passable); }
    }

    //change of coordinates when moving in any direction
    public static List<Point> NeighbourShift
    {
        get
        {
            return new List<Point>
                {
                    new Point(0, 1),
                    new Point(1, 0),
                    new Point(1, -1),
                    new Point(0, -1),
                    new Point(-1, 0),
                    new Point(-1, 1),
                };
        }
    }

    public void FindNeighbours(Dictionary<Point, Tile> Board,
        Vector2 BoardSize, bool EqualLineLengths)
    {
        List<Tile> neighbours = new List<Tile>();

        foreach (Point point in NeighbourShift)
        {
            int neighbourX = X + point.X;
            int neighbourY = Y + point.Y;
            //x coordinate offset specific to straight axis coordinates
            int xOffset = neighbourY / 2;

            //If every second hexagon row has less hexagons than the first one, just skip the last one when we come to it
            if (neighbourY % 2 != 0 && !EqualLineLengths &&
                neighbourX + xOffset == BoardSize.x - 1)
                continue;
            //Check to determine if currently processed coordinate is still inside the board limits
            if (neighbourX >= 0 - xOffset &&
                neighbourX < (int)BoardSize.x - xOffset &&
                neighbourY >= 0 && neighbourY < (int)BoardSize.y)
                neighbours.Add(Board[new Point(neighbourX, neighbourY)]);
        }

        AllNeighbours = neighbours;
    }

    public void SetLooks(int sprite, TileColorPresets color)
    {
        var spr = Representation.GetComponent<SpriteRenderer>();
        spr.sprite = Sprites[sprite];
        spr.color = tileColors[(int)color];
    }

    public enum TileColorPresets
    {
        WhiteTransparent = 0,
        Area = 1,
    }

    private List<Color> tileColors = new List<Color>()
    {
        new Color(1f, 1f, 1f, 0.3f),
        new Color(0.7f, 0.3f, 0.4f, 0.3f)
    };
}