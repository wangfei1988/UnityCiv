using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterMovement : MonoBehaviour
{
    private float _maxpoints = 0;
    public float MovementPointsMax
    {
        get
        {
            return _maxpoints;
        }
        set
        {
            _maxpoints = value;
            MovementPointsRemaining = (int)value;
        }
    }

    public int MovementPointsRemaining
    {
        get;
        private set;
    }

    public bool ExpendMovementPoints(int points)
    {
        if (!IsMoving && MovementPointsRemaining >= points)
        {
            MovementPointsRemaining -= points;
            return true;
        }
        return false;
    }

    //speed in meters per second
    private float speed = 0.025f;
    //distance between character and tile position when we assume we reached it and start looking for the next. Explained in detail later on
    public static float MinNextTileDist = 0.005f;
    //position of the tile we are heading to
    public Vector3 curTilePos
    {
        get;
        private set;
    }
    public Tile curTile
    {
        get;
        private set;
    }
    List<Tile> path;
    public bool IsMoving { get; private set; }

    public List<Tile> Path
    {
        get;
        private set;
    }

    private Animator anim;

    void Awake()
    {
        //singleton pattern here is used just for the sake of simplicity. Messenger (http://goo.gl/3Okkh) should be used in cases when this script is attached to more than one character
        //instance = this;
        IsMoving = false;
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void setPos(Vector2 pos)
    {
        Tile cTile;
        if (GridManager.instance.board.TryGetValue(new Point((int)pos.x - (int)pos.y/2, (int)pos.y), out cTile))
        {
            curTile = cTile;
            curTilePos = GridManager.instance.calcWorldCoord(pos);
            gameObject.transform.position = curTilePos;
        }
        else
        {
            throw new UnityException("No valid start pos!");
        }
    }

    public bool MoveTo(Vector2 dest)
    {
        if (!IsMoving)
        {
            var GM = GridManager.instance;
            var pathlist = new List<Tile>(GM.generatePath(new Vector2(curTile.X, curTile.Y), dest));
            pathlist.Reverse();
            /*Debug.Log("Moving from: " + curTile.Location.X + "," + curTile.Location.Y);
            Debug.Log("To: " + dest);
            Debug.Log("Via: " + String.Join(" | ", pathlist.Select(t => t.X + "," + t.Y).ToArray()));*/
            // TODO: Assuming 1 tile = 1 movement
            Path = pathlist;
            if (MovementPointsRemaining > 0) {
                var movement = Math.Min(MovementPointsRemaining, pathlist.Count - 1);
                var pathNow = pathlist.Take(movement + 1).ToList();
                Path = pathlist.Skip(movement).ToList();
                ExpendMovementPoints(movement);
                pathNow.Reverse();
                StartMoving(pathNow);
            }
            return true;
        }
        return false;
    }

    //method argument is a list of tiles we got from the path finding algorithm
    void StartMoving(List<Tile> path)
    {
        if (path.Count == 0)
            return;
        //the first tile we need to reach is actually in the end of the list just before the one the character is currently on
        curTile = path[path.Count - 2];
        curTilePos = getWorld(curTile);
        // Rotate towards the target
        transform.rotation = Quaternion.LookRotation(curTilePos - transform.position);

        IsMoving = true;
        this.path = path;
    }

    void Update()
    {
        if (!IsMoving)
            return;
        //if the distance between the character and the center of the next tile is short enough
        if ((curTilePos - transform.position).sqrMagnitude < MinNextTileDist * MinNextTileDist)
        {
            //if we reached the destination tile
            if (path.IndexOf(curTile) == 0)
            {
                IsMoving = false;
                anim.Play("idle", -1);
                return;
            }
            //curTile becomes the next one
            curTile = path[path.IndexOf(curTile) - 1];
            curTilePos = getWorld(curTile);

            // Rotate towards the target
            transform.rotation = Quaternion.LookRotation(curTilePos - transform.position);
        }

        MoveTowards(curTilePos);
    }

    void MoveTowards(Vector3 position)
    {
        //mevement direction
        Vector3 dir = position - transform.position;
        if (dir.sqrMagnitude > speed * speed)
        {
            dir.Normalize();
            dir *= speed;
        }

        //float commonGround = Vector3.Dot(dir.normalized, transform.forward);

        /*Vector3 forwardDir = transform.forward;
        forwardDir = forwardDir * speed;
        float speedModifier = Vector3.Dot(dir.normalized, transform.forward);
        forwardDir *= speedModifier;*/

        //var idlehash = Animator.StringToHash("idle");
        Vector3 forwardDir = transform.forward;
        forwardDir = forwardDir * speed;

        //var positionstr = position.ToString("F5");
        //var transformdir = transform.position.ToString("F5");
        //var dirstr = dir.ToString("F5");

        if (dir.sqrMagnitude > MinNextTileDist * MinNextTileDist)
        {
            transform.Translate(dir, Space.World);
            //controller.SimpleMove(forwardDir);
            anim.Play("walk", -1);
        }
        else {
            anim.Play("idle", -1);
        }
    }

    private Vector3 getWorld(Tile t)
    {
        return GridManager.instance.calcWorldCoord(new Vector2(t.X + t.Y / 2, t.Y));
    }

    void OnEnable()
    {
        EventManager.StartListening("NextRound", nextRoundListener);
    }

    void OnDisable()
    {
        EventManager.StopListening("NextRound", nextRoundListener);
    }

    private void nextRoundListener()
    {
        MovementPointsRemaining = (int)MovementPointsMax;
    }

    private static List<GameObject> suggestedMovePathLineObjects = new List<GameObject>();
    private static Color thisRoundMoveColor = new Color(0.31f, 0.27f, 0.1f, 1);
    private static Color nextRoundsMoveColor = new Color(0.65f, 0.65f, 0.65f, 1);

    internal void SuggestMove(Vector3 dest)
    {
        if (!IsMoving)
        {
            var pathlist = new List<Tile>(GridManager.instance.generatePath(new Vector2(curTile.X, curTile.Y), dest));
            pathlist.Reverse();
            DrawPath(pathlist);
        }
    }

    private void DrawPath(List<Tile> pathlist)
    {
        for (int t = 0; t < pathlist.Count; t++)
        {
            var tile = pathlist[t];
            GameObject point = null;
            if (suggestedMovePathLineObjects.Count > t)
                point = suggestedMovePathLineObjects[t];
            else
            {
                point = Instantiate(GridManager.instance.MovementLineObject);
                suggestedMovePathLineObjects.Add(point);
            }
            point.transform.position = GridManager.instance.calcWorldCoord(new Vector2(tile.X + tile.Y / 2, tile.Y));
            // TODO: assuming every tile movement costs 1 points
            point.GetComponent<MeshRenderer>().material.color = MovementPointsRemaining - t >= 0 ? thisRoundMoveColor : nextRoundsMoveColor;
            point.SetActive(true);
            // TODO: display a number for each movement point used on tile
        }

        // hide the rest of the (still visible) movement points
        if (suggestedMovePathLineObjects.Count > pathlist.Count)
        {
            for (int s = pathlist.Count; s < suggestedMovePathLineObjects.Count; s++)
            {
                suggestedMovePathLineObjects[s].SetActive(false);
            }
        }
    }
}