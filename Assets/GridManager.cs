using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Hex coming from:
/// https://tbswithunity3d.wordpress.com/2012/02/23/hexagonal-grid-path-finding-using-a-algorithm/
/// http://keekerdc.com/2011/03/hexagon-grids-coordinate-systems-and-distance-calculations/
/// http://www.redblobgames.com/grids/hexagons/
/// </summary>
public class GridManager : MonoBehaviour
{
    public GameObject Hex;
    //This time instead of specifying the number of hexes you should just drop your ground game object on this public variable
    public GameObject Ground;

    //selectedTile stores the tile mouse cursor is hovering on
    public Tile selectedTile = null;
    //tile which is the start of the path

    //board is used to store tile locations
    public Dictionary<Point, Tile> board = new Dictionary<Point, Tile>();
    //Line should be initialised to some 3d object that can fit nicely in the center of a hex tile and will be used to indicate the path. For example, it can be just a simple small sphere with some material attached to it. Initialise the variable using inspector pane.
    public GameObject Line;
    //List to hold "Lines" indicating the path
    List<GameObject> path;

    public GameObject Settler;
    private GameObject selectedUnit = null;

    public static GridManager instance = null;

    private float hexWidth;
    private float hexHeight;
    private float groundWidth;
    private float groundHeight;

    void Awake()
    {
        instance = this;
    }

    void setSizes()
    {
        hexWidth = Hex.GetComponent<Renderer>().bounds.size.x;
        hexHeight = Hex.GetComponent<Renderer>().bounds.size.z;
        groundWidth = Ground.GetComponent<Renderer>().bounds.size.x;
        groundHeight = Ground.GetComponent<Renderer>().bounds.size.z;
    }

    //The method used to calculate the number hexagons in a row and number of rows
    //Vector2.x is gridWidthInHexes and Vector2.y is gridHeightInHexes
    Vector2 calcGridSize()
    {
        //According to the math textbook hexagon's side length is half of the height
        float sideLength = hexHeight / 2;
        //the number of whole hex sides that fit inside inside ground height
        int nrOfSides = (int)(groundHeight / sideLength);
        //I will not try to explain the following calculation because I made some assumptions, which might not be correct in all cases, to come up with the formula. So you'll have to trust me or figure it out yourselves.
        int gridHeightInHexes = (int)(nrOfSides * 2 / 3);
        //When the number of hexes is even the tip of the last hex in the offset column might stick up.
        //The number of hexes in that case is reduced.
        if (gridHeightInHexes % 2 == 0
            && (nrOfSides + 0.5f) * sideLength > groundHeight)
            gridHeightInHexes--;
        //gridWidth in hexes is calculated by simply dividing ground width by hex width
        return new Vector2((int)(groundWidth / hexWidth), gridHeightInHexes);
    }

    //Method to calculate the position of the first hexagon tile
    //The center of the hex grid is (0,0,0)
    Vector3 calcInitPos()
    {
        Vector3 initPos;
        initPos = new Vector3(-groundWidth / 2 + hexWidth / 2, 0,
            groundHeight / 2 - hexWidth / 2);

        return initPos;
    }

    public Vector3 calcWorldCoord(Vector2 gridPos)
    {
        Vector3 initPos = calcInitPos();
        float offset = 0;
        if (gridPos.y % 2 != 0)
            offset = hexWidth / 2;

        float x = initPos.x + offset + gridPos.x * hexWidth;
        float z = initPos.z - gridPos.y * hexHeight * 0.75f;
        //If your ground is not a plane but a cube you might set the y coordinate to sth like groundDepth/2 + hexDepth/2
        return new Vector3(x, 0.01f, z);
    }

    public Vector2 calcGridCoord(Vector3 worldPos)
    {
        Vector3 initPos = calcInitPos();

        float y = -(float)(worldPos.z - initPos.z) / (hexHeight * 0.75f);
        float x = (worldPos.x - initPos.x) / hexWidth - y/2; // - y / 2
        //col = x + (z - (z & 1)) / 2             + (y - ((int)y & 1)) / 2
        //row = z
        return hex_round(new Vector2(x, y));
    }

    Vector2 hex_round(Vector2 h)
    {
        return cube_to_hex(cube_round(hex_to_cube(h)));
    }

    Vector3 cube_round(Vector3 cubeInput) {
        var rx = Math.Round(cubeInput.x);
        var ry = Math.Round(cubeInput.y);
        var rz = Math.Round(cubeInput.z);

        var x_diff = Math.Abs(rx - cubeInput.x);
        var y_diff = Math.Abs(ry - cubeInput.y);
        var z_diff = Math.Abs(rz - cubeInput.z);

        if (x_diff > y_diff && x_diff > z_diff) {
            rx = -ry - rz;
        } else if (y_diff > z_diff) {
            ry = -rx - rz;
        } else {
            rz = -rx - ry;
        }

        return new Vector3((float)rx, (float)ry, (float)rz);
    }

    Vector2 cube_to_hex(Vector3 h)
    {
        return new Vector2(h.x, h.z);
    }

    Vector3 hex_to_cube(Vector2 h)
    {
        return new Vector3(h.x, -h.x - h.y, h.y);
    }

    void createGrid()
    {
        Vector2 gridSize = calcGridSize();
        GameObject hexGridGO = new GameObject("HexGrid");

        for (float y = 0; y < gridSize.y; y++)
        {
            float sizeX = gridSize.x;
            //if the offset row sticks up, reduce the number of hexes in a row
            if (y % 2 != 0 && (gridSize.x + 0.5) * hexWidth > groundWidth)
                sizeX--;
            for (float x = 0; x < sizeX; x++)
            {
                GameObject hex = Instantiate(Hex);
                Vector2 gridPos = new Vector2(x, y);
                hex.transform.position = calcWorldCoord(gridPos);
                hex.transform.parent = hexGridGO.transform;

                var tile = new Tile((int)x - (int)(y / 2), (int)y);
                tile.Representation = hex;
                //y / 2 is subtracted from x because we are using straight axis coordinate system
                board.Add(tile.Location, tile);
            }
        }
        //variable to indicate if all rows have the same number of hexes in them
        //this is checked by comparing width of the first hex row plus half of the hexWidth with groundWidth
        bool equalLineLengths = (gridSize.x + 0.5) * hexWidth <= groundWidth;
        //Neighboring tile coordinates of all the tiles are calculated
        foreach (Tile tile in board.Values)
            tile.FindNeighbours(board, gridSize, equalLineLengths);
    }

    //Distance between destination tile and some other tile in the grid
    double calcDistance(Tile tile, Tile destTile)
    {
        //Formula used here can be found in Chris Schetter's article
        float deltaX = Mathf.Abs(destTile.X - tile.X);
        float deltaY = Mathf.Abs(destTile.Y - tile.Y);
        int z1 = -(tile.X + tile.Y);
        int z2 = -(destTile.X + destTile.Y);
        float deltaZ = Mathf.Abs(z2 - z1);

        return Mathf.Max(deltaX, deltaY, deltaZ);
    }

    private void DrawPath(IEnumerable<Tile> path)
    {
        if (this.path == null)
            this.path = new List<GameObject>();
        //Destroy game objects which used to indicate the path
        this.path.ForEach(Destroy);
        this.path.Clear();

        //Lines game object is used to hold all the "Line" game objects indicating the path
        GameObject lines = GameObject.Find("Lines");
        if (lines == null)
            lines = new GameObject("Lines");
        foreach (Tile tile in path)
        {
            var line = (GameObject)Instantiate(Line);
            //calcWorldCoord method uses squiggly axis coordinates so we add y / 2 to convert x coordinate from straight axis coordinate system
            Vector2 gridPos = new Vector2(tile.X + tile.Y / 2, tile.Y);
            line.transform.position = calcWorldCoord(gridPos);
            this.path.Add(line);
            line.transform.parent = lines.transform;
        }
    }

    public Path<Tile> generatePath(Vector2 start, Vector2 end)
    {
        //We assume that the distance between any two adjacent tiles is 1
        //If you want to have some mountains, rivers, dirt roads or something else which might slow down the player you should replace the function with something that suits better your needs
        Func<Tile, Tile, double> distance = (node1, node2) => 1;

        Tile startTile;
        Tile endTile;
        if (board.TryGetValue(new Point((int)start.x, (int)start.y), out startTile) && board.TryGetValue(new Point((int)end.x, (int)end.y), out endTile))
        {
            var path = PathFinder.FindPath(startTile, endTile, distance, calcDistance);
            //DrawPath(path)
            return path;
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {

        //get mouse pos
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Ground.GetComponent<Collider>().Raycast(ray, out hit, 100f))
        {
            var coord = calcGridCoord(hit.point);
            Debug.Log(coord);
            if (selectedTile != null) selectedTile.Representation.GetComponent<SpriteRenderer>().color = Color.white;
            if (board.TryGetValue(new Point((int)coord.x, (int)coord.y), out selectedTile))
            {
                selectedTile.Representation.GetComponent<SpriteRenderer>().color = Color.red;
                if (Input.GetMouseButtonDown(1))
                {
                    selectedUnit.GetComponent<CharacterMovement>().MoveTo(coord);
                }
            }
            else
            {
                Debug.Log("out of range");
            }
        }

    }

    void Start()
    {
        setSizes();
        createGrid();

        GameObject PC = (GameObject)Instantiate(Settler);
        PC.GetComponent<CharacterMovement>().setPos(new Vector2(0, 0));
        selectedUnit = PC;
    }
}