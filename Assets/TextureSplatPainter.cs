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

                var pos_x = (float)normX * terrainSize.x + worldpos.x;
                var pos_y = (float)normY * terrainSize.z + worldpos.z;

                /*float grid_y = -(pos_y - initPos.z) / (gm.hexHeight * 0.75f);
                float grid_x = (pos_x - initPos.x) / gm.hexWidth - y / 2;*/
                var grid_pos = gm.calcGridCoord(new Vector3(pos_x, 0, -pos_y));
                float grid_y = grid_pos.y;
                float grid_x = grid_pos.x;


                //floor and ceil both to get the two relevant values
                int y_sign = Math.Sign(grid_y);
                var y1 = y_sign * Math.Ceiling(Math.Abs(grid_y));
                var y2 = (int)grid_y; //floor

                int x_sign = Math.Sign(grid_x);
                var x1 = x_sign * Math.Ceiling(Math.Abs(grid_x));
                var x2 = (int)grid_x; //floor

                var point1 = new Point((int)x1, (int)y1);
                var point2 = new Point(x2, y2);

                Tile tile1, tile2;

                board.TryGetValue(point1, out tile1);
                board.TryGetValue(point2, out tile2);

                if (tile1 != null && tile2 != null)
                {
                    map[x, y, 1] = 1;
                    map[x, y, 0] = 0;
                    map[x, y, 3] = 0;
                    map[x, y, 2] = 0;
                } else
                {
                    map[x, y, 0] = 0;
                    map[x, y, 1] = 0;
                    map[x, y, 2] = 0;
                    map[x, y, 3] = 1;
                }
            }
        }
        t.terrainData.SetAlphamaps(0, 0, map);
    }

    void Steepness()
    {
        /*var map = new float[t.terrainData.alphamapWidth, t.terrainData.alphamapHeight, 2];

        // For each point on the alphamap...
        for (var y = 0; y < t.terrainData.alphamapHeight; y++)
        {
            for (var x = 0; x < t.terrainData.alphamapWidth; x++)
            {
                // Get the normalized terrain coordinate that
                // corresponds to the the point.
                var normX = x * 1.0 / (t.terrainData.alphamapWidth - 1);
                var normY = y * 1.0 / (t.terrainData.alphamapHeight - 1);

                // Get the steepness value at the normalized coordinate.
                var angle = t.terrainData.GetSteepness((float)normX, (float)normY);

                // Steepness is given as an angle, 0..90 degrees. Divide
                // by 90 to get an alpha blending value in the range 0..1.
                var frac = angle / 90.0;
                map[x, y, 0] = (float)frac;
                map[x, y, 1] = 1 - (float)frac;
            }
        }

        t.terrainData.SetAlphamaps(0, 0, map);*/
    }
}
