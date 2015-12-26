using UnityEngine;
using System.Collections;

public class TextureSplatPainter : MonoBehaviour {

    Terrain t;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Steepness()
    {
        var map = new float[t.terrainData.alphamapWidth, t.terrainData.alphamapHeight, 2];

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

        t.terrainData.SetAlphamaps(0, 0, map);
    }
}
