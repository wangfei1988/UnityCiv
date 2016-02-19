using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

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

    public GameObject MovementLineObject;

    public Sprite[] tileSprites;

    //selectedTile stores the tile mouse cursor is hovering on
    public Tile selectedTile = null;
    //tile which is the start of the path

    //board is used to store tile locations
    public Dictionary<Point, Tile> board = new Dictionary<Point, Tile>();
    //Line should be initialised to some 3d object that can fit nicely in the center of a hex tile and will be used to indicate the path. For example, it can be just a simple small sphere with some material attached to it. Initialise the variable using inspector pane.
    public GameObject Line;
    //List to hold "Lines" indicating the path
    List<GameObject> path;

    [HideInInspector]
    public GameObject selectedUnit = null;
    [HideInInspector]
    public GameObject selectedBuilding = null;
    [HideInInspector]
    public List<GameObject> allUnits = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> allBuildings = new List<GameObject>();
    [HideInInspector]
    public List<Phase1TileImprovement> allTileImprovements = new List<Phase1TileImprovement>();

    public List<RectTransform> uiElements = new List<RectTransform>();
    public List<Vector3[]> worldCorners = new List<Vector3[]>();

    public static GridManager instance = null;

    public float hexWidth
    {
        get;
        private set;
    }
    public float hexHeight
    {
        get;
        private set;
    }
    private float groundWidth;
    private float groundHeight;
    private Vector2 gridSize;

    void Awake()
    {
        instance = this;
        Tile.Sprites = tileSprites;
    }

    void setSizes()
    {
        hexWidth = Hex.GetComponent<Renderer>().bounds.size.x;
        hexHeight = Hex.GetComponent<Renderer>().bounds.size.z;
        var terrainComponent = Ground.GetComponent<Terrain>();
        if (terrainComponent != null)
        {
            groundWidth = terrainComponent.terrainData.size.x;
            groundHeight = terrainComponent.terrainData.size.z;
        }
        else
        {
            groundWidth = Ground.GetComponent<Renderer>().bounds.size.x;
            groundHeight = Ground.GetComponent<Renderer>().bounds.size.z;
        }
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
    public Vector3 calcInitPos()
    {
        Vector3 initPos;
        initPos = new Vector3(-groundWidth / 2 + hexWidth / 2, 0,
            groundHeight / 2 - hexWidth / 2);

        return initPos;
    }

    /// <summary>
    /// calcs world coord from wiggly axis points. If you have straight axis coordinate system, remember to add x+=y/2
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
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
        float x = (worldPos.x - initPos.x) / hexWidth - y/2;
        return hex_round(new Vector2(x, y));
    }

    public Vector2 hex_round(Vector2 h)
    {
        return cube_to_hex(cube_round(hex_to_cube(h)));
    }

    // See http://www.redblobgames.com/grids/hexagons/#rounding
    public Vector3 cube_round(Vector3 cubeInput) {
        var rx = Math.Round(cubeInput.x);
        var ry = Math.Round(cubeInput.y);
        var rz = Math.Round(cubeInput.z);

        var x_diff = Math.Abs(rx - cubeInput.x);
        var y_diff = Math.Abs(ry - cubeInput.y);
        var z_diff = Math.Abs(rz - cubeInput.z);

        //reset largest rounding change
        if (x_diff > y_diff && x_diff > z_diff) {
            rx = -ry - rz;
        } else if (y_diff > z_diff) {
            ry = -rx - rz;
        } else {
            rz = -rx - ry;
        }

        return new Vector3((float)rx, (float)ry, (float)rz);
    }

    // ranges from 0 to 1
    public float cube_distance(Vector3 a, Vector3 b)
    {
        return (Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z)) / 2;
    }

    public Vector2 cube_to_hex(Vector3 h)
    {
        return new Vector2(h.x, h.z);
    }

    public Vector3 hex_to_cube(Vector2 h)
    {
        return new Vector3(h.x, -h.x - h.y, h.y);
    }

    void createGrid()
    {
        gridSize = calcGridSize();
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


    private Color hex_default_color = new Color(1, 1, 1, 85f/255f);

    // Update is called once per frame
    void Update()
    {

        // if the mouse isn't hovering over the gui
        if (!worldCorners.Any(wc => isMouseOverUI(wc, Input.mousePosition)))
        {
            //get mouse pos
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Ground.GetComponent<Collider>().Raycast(ray, out hit, 100f))
            {
                var coord = calcGridCoord(hit.point);
                Tile newSelectedTile;
                if (board.TryGetValue(new Point((int)coord.x, (int)coord.y), out newSelectedTile))
                {
                    if (selectedTile != null && selectedTile != newSelectedTile) selectedTile.Representation.GetComponent<SpriteRenderer>().color = hex_default_color;
                    selectedTile = newSelectedTile;
                    selectedTile.Representation.GetComponent<SpriteRenderer>().color = Color.red;

                    if (!Village.InBuildMode)
                    {
                        if (Input.GetMouseButton(1))
                        {
                            if (selectedUnit != null) selectedUnit.GetComponent<CharacterMovement>().SuggestMove(coord);
                        }
                        // the user wants to move there
                        else if (Input.GetMouseButtonUp(1))
                        {
                            if (selectedUnit != null) selectedUnit.GetComponent<CharacterMovement>().MoveTo(coord);
                        }
                        // does the user want to select something?
                        else if (Input.GetMouseButtonDown(0))
                        {
                            GameObject selected = null;

                            //get all units from that tile
                            var entitiesOnTile = allUnits.Where(u => u.GetComponent<CharacterMovement>().curTile == selectedTile);
                            entitiesOnTile = entitiesOnTile.Union(allBuildings.Where(b => b.GetComponent<IGameBuilding>().Location == selectedTile));
                            if (entitiesOnTile.Count() > 0)
                            {
                                selected = entitiesOnTile.First();
                                entitiesOnTile.First().GetComponent<IEntity>().Select();
                            }

                            // If nothing is selected we'll leave the current unit selected, but we can get rid of the building!
                            // There will always be a unit selected, if possible
                            if (selected == null)
                            {
                                //UnitPanelUI.instance.SetUnitPanelInfo(null);
                                BuildingPanelUI.instance.SetBuildItems(null, 0);
                                selectedBuilding = null;
                                if (selectedUnit != null) selectedUnit.GetComponent<IEntity>().Select();
                            }
                            else if (allBuildings.Contains(selected))
                            {
                                UnitPanelUI.instance.SetUnitPanelInfo(null);
                            }
                            else
                            {
                                BuildingPanelUI.instance.SetBuildItems(null, 0);
                            }
                        }
                    }
                    else
                    {
                        Village.BuildModeHoveredTile = selectedTile;
                    }
                }
                else
                {
                    //Debug.Log("out of range");
                }
            }

        }
    }


    void Start()
    {
        setSizes();
        createGrid();
        
        worldCorners = uiElements.Select(uie => { Vector3[] wc = new Vector3[4];  uie.GetWorldCorners(wc); return wc; } ).ToList();

        Ground.GetComponent<LevelCreator>().Create(board, gridSize);
        Ground.GetComponent<TextureSplatPainter>().Paint(board);
    }

    public void Spawn(GameObject entity, Vector2 position)
    {
        GameObject PC = Instantiate(entity);
        var cm = PC.GetComponent<CharacterMovement>();
        cm.setPos(position);
    }

    /*public GameObject DrawHexLine(Vector3[] edgepoints)
    {
        GameObject line = Instantiate(LineRendererLine);
        var lineComp = line.GetComponent<LineRenderer>();
        lineComp.SetPositions(edgepoints);
        return line;
    }*/

    public List<Tile> GetHexArea(Tile center, int distance)
    {
        List<Tile> results = new List<Tile>();
        var cubecenter = hex_to_cube(new Vector2(center.X, center.Y));
        for (int dx = -distance; dx <= distance; dx++)
            for (int dy = Math.Max(-distance, -dx - distance); dy <= Math.Min(distance, -dx + distance); dy++)
            {
                var dz = -dx - dy;
                var pos = cube_to_hex(new Vector3(cubecenter.x + dx, cubecenter.y + dy, cubecenter.z + dz));
                Tile tile;
                if (board.TryGetValue(new Point((int)(pos.x + 0.1), (int)(pos.y + 0.1)), out tile))
                {
                    results.Add(tile);
                }
            }
        return results;
    }

    public List<Tile> DrawHexArea(Tile center, int distance, int sprite, Tile.TileColorPresets color)
    {
        var area = GetHexArea(center, distance);
        foreach (var tile in area)
            tile.SetLooks(sprite, color);
        return area;
    }

    private bool isMouseOverUI(Vector3[] worldCorners, Vector2 mousePosition)
    {
        if (mousePosition.x >= worldCorners[0].x && mousePosition.x < worldCorners[2].x
           && mousePosition.y >= worldCorners[0].y && mousePosition.y < worldCorners[2].y)
        {
            return true;
        }
        return false;
    }
}