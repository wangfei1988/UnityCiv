using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TextureSplatPainter : MonoBehaviour {

    public void Paint(Dictionary<Point, Tile> board)
    {
        var gm = GridManager.instance;
        Vector3 initPos = gm.calcInitPos();
        var t = GetComponent<Terrain>();
        var worldpos = transform.position;
        var terrainSize = t.terrainData.size;

        var map = new float[t.terrainData.alphamapWidth, t.terrainData.alphamapHeight, 4];
        for (var y = 0; y < t.terrainData.alphamapHeight; y++)
        {
            for (var x = 0; x < t.terrainData.alphamapWidth; x++)
            {

                var normX = x * 1.0 / (t.terrainData.alphamapWidth - 1);
                var normY = y * 1.0 / (t.terrainData.alphamapHeight - 1);

                var pos_y = (float)normX * terrainSize.x + worldpos.x;
                var pos_x = (float)normY * terrainSize.z + worldpos.z;

                float grid_y = -(pos_y - initPos.z) / (gm.hexHeight * 0.75f);
                float grid_x = (pos_x - initPos.x) / gm.hexWidth - grid_y / 2;

                var cube_pos = gm.hex_to_cube(new Vector2(grid_x, grid_y));
                var cube_pos_rounded = cube_roundvals(cube_pos);
                float fract = gm.cube_distance(cube_pos, cube_pos_rounded[0]);

                //to figure out the other point we'll round in the other direction

                /*var grid_pos = gm.calcGridCoord(new Vector3(pos_y, 0, pos_x));
                float grid_y = grid_pos.y;
                float grid_x = grid_pos.x;

                var point1 = new Point((int)grid_x, (int)grid_y);
                var point2 = point1;*/

                var grid_pos1 = gm.cube_to_hex(cube_pos_rounded[0]);
                var grid_pos2 = gm.cube_to_hex(cube_pos_rounded[1]);

                var point1 = new Point((int)grid_pos1.x, (int)grid_pos1.y);
                var point2 = new Point((int)grid_pos2.x, (int)grid_pos2.y);

                Tile tile1, tile2;

                board.TryGetValue(point1, out tile1);
                board.TryGetValue(point2, out tile2);

                if (tile1 != null && tile2 != null)
                {
                    /*for (int i = 0; i < 4; i++)
                    {
                        map[x, y, i] = 0;
                    }*/
                    //map[x, y, 0] = 1;
                    map[x, y, (int)tile1.Type] += (float)1.0 - fract;
                    map[x, y, (int)tile2.Type] += fract;
                    /*var sldklksd = (int)tile1.Type;
                    var lskdis = (int)tile2.Type;
                    var sldk = map[x, y, (int)tile1.Type];
                    var sdlk = map[x, y, (int)tile2.Type];
                    var sldkjflsjkdf = 65;*/
                }
                else
                {
                    if (tile1 != null)
                        map[x, y, (int)tile1.Type] = 1;
                    else
                        map[x, y, (int)tile2.Type] = 1;
                }
            }
        }
        t.terrainData.SetAlphamaps(0, 0, map);
    }

    // See http://www.redblobgames.com/grids/hexagons/#rounding
    private Vector3[] cube_roundvals(Vector3 cubeInput)
    {
        var rx = Math.Round(cubeInput.x);
        var ry = Math.Round(cubeInput.y);
        var rz = Math.Round(cubeInput.z);

        var x_diff = Math.Abs(rx - cubeInput.x);
        var y_diff = Math.Abs(ry - cubeInput.y);
        var z_diff = Math.Abs(rz - cubeInput.z);

        var rx2 = (rx - cubeInput.x) + 1 + cubeInput.x;
        var ry2 = (ry - cubeInput.y) + 1 + cubeInput.y;
        var rz2 = (rz - cubeInput.z) + 1 + cubeInput.z;

        //reset largest rounding change
        if (x_diff > y_diff && x_diff > z_diff)
        {
            rx = -ry - rz;
            rx2 = rx;
        }
        else if (y_diff > z_diff)
        {
            ry = -rx - rz;
            ry2 = ry;
        }
        else {
            rz = -rx - ry;
            rz2 = rz;
        }

        return new[] { new Vector3((float)rx, (float)ry, (float)rz),
                       new Vector3((float)rx2, (float)ry2, (float)rz2)};
    }
}
