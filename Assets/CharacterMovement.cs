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

    public List<Tile> RemainingPath
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
            RemainingPath = new List<Tile>() { curTile };
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

            // TODO: Assuming 1 tile = 1 movement
            RemainingPath = pathlist;
            StartMoving();
            return true;
        }
        return false;
    }

    //method argument is a list of tiles we got from the path finding algorithm
    void StartMoving()
    {
        if (MovementPointsRemaining > 0)
        {
            var movement = Math.Min(MovementPointsRemaining, RemainingPath.Count - 1);
            var path = RemainingPath.Take(movement + 1).ToList();
            RemainingPath = RemainingPath.Skip(movement).ToList();
            ExpendMovementPoints(movement);
            path.Reverse();
            if (path.Count <= 1)
                return;

            TimeManager.instance.PerformingAction();
            TimeManager.instance.NoMoreOrdersNeeded(gameObject.GetComponent<IEntity>());

            //the first tile we need to reach is actually in the end of the list just before the one the character is currently on
            curTile = path[path.Count - 2];
            curTilePos = getWorld(curTile);
            // Rotate towards the target
            transform.rotation = Quaternion.LookRotation(curTilePos - transform.position);

            IsMoving = true;
            this.path = path;
        }
    }

    void Update()
    {
        if (!IsMoving)
            return;
        //if the distance between the character and the center of the next tile is short enough
        if ((curTilePos - transform.position).sqrMagnitude < MinNextTileDist * MinNextTileDist)
        {
            // Remove the way marker for the reached tile
            RemoveNextWayMarker();

            //if we reached the destination tile
            if (path.IndexOf(curTile) == 0)
            {
                IsMoving = false;
                anim.Play("idle", -1);
                TimeManager.instance.FinishedAction();
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
        //TODO: depends on fps..
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
        //Vector3 forwardDir = transform.forward;
        //forwardDir = forwardDir * speed;

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
        EventManager.StartListening("NextRoundRequest", nextRoundRequestListener);
    }

    void OnDisable()
    {
        EventManager.StopListening("NextRound", nextRoundListener);
        EventManager.StopListening("NextRoundRequest", nextRoundRequestListener);
    }

    private void nextRoundRequestListener()
    {
        // perform remaining movement
        if (MovementPointsRemaining > 0 && RemainingPath.Count > 1)
        {
            StartMoving();
        }
    }

    private void nextRoundListener()
    {
        MovementPointsRemaining = (int)MovementPointsMax;
        // draw the next path's reachability for this round if the unit's selected
        if (GridManager.instance.selectedUnit == gameObject)
            DrawPath(RemainingPath);
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

    public void DrawPath()
    {
        DrawPath(RemainingPath);
    }

    private void DrawPath(List<Tile> pathlist)
    {
        // starting at 1 because the first tile in the list is the current tile itself
        for (int t = 1; t < pathlist.Count; t++)
        {
            var tile = pathlist[t];
            GameObject point = null;
            if (suggestedMovePathLineObjects.Count > t - 1)
                point = suggestedMovePathLineObjects[t - 1];
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
        if (suggestedMovePathLineObjects.Count >= pathlist.Count)
        {
            for (int s = Math.Max(pathlist.Count - 1, 0); s < suggestedMovePathLineObjects.Count; s++)
            {
                suggestedMovePathLineObjects[s].SetActive(false);
            }
        }
    }

    public void CancelSuggestedMove()
    {
        RemainingPath = new List<Tile>();
        RemoveWayMarkers();
    }

    private void RemoveNextWayMarker()
    {
        if (GridManager.instance.selectedUnit == gameObject && suggestedMovePathLineObjects[0].activeSelf)
        {
            var removeWayPoint = suggestedMovePathLineObjects[0];
            removeWayPoint.SetActive(false);
            suggestedMovePathLineObjects = suggestedMovePathLineObjects.Skip(1).ToList();
            suggestedMovePathLineObjects.Add(removeWayPoint);
        }
    }

    /// <summary>
    /// Hides the unit's path
    /// </summary>
    public void RemoveWayMarkers()
    {
        foreach (var o in suggestedMovePathLineObjects)
            o.SetActive(false);
    }
}