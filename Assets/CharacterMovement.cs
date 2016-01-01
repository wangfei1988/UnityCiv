using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterMovement : MonoBehaviour
{
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

    //gets tile position in world space
    /*Vector3 calcTilePos(Tile tile)
    {
        //y / 2 is added to convert coordinates from straight axis coordinate system to squiggly axis system
        Vector2 tileGridPos = new Vector2(tile.X + tile.Y / 2, tile.Y);
        Vector3 tilePos = GridManager.instance.calcWorldCoord(tileGridPos);
        //y coordinate is disregarded
        tilePos.y = myTransform.position.y;
        return tilePos;
    }*/

    public void MoveTo(Vector2 dest)
    {
        if (!IsMoving)
        {
            var GM = GridManager.instance;
            var pathlist = new List<Tile>(GM.generatePath(new Vector2(curTile.X, curTile.Y), dest));
            /*Debug.Log("Moving from: " + curTile.Location.X + "," + curTile.Location.Y);
            Debug.Log("To: " + dest);
            Debug.Log("Via: " + String.Join(" | ", pathlist.Select(t => t.X + "," + t.Y).ToArray()));*/
            StartMoving(pathlist);
        }
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
        /*if (!GetComponent<Animation>()["HumanoidIdle"].enabled)
            GetComponent<Animation>().CrossFade("HumanoidIdle");*/
    }

    private Vector3 getWorld(Tile t)
    {
        return GridManager.instance.calcWorldCoord(new Vector2(t.X + t.Y / 2, t.Y));
    }
}