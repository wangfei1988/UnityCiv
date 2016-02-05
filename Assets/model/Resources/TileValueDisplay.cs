using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System;

public class TileValueDisplay : MonoBehaviour {

    public Image StrategicResourceDisplay;
    public RectTransform BasicResourceDisplay;
    public Image FoodIconPrefab;
    public Image ProductionIconPrefab;
    public int DistanceBetweenBasicResources;
    public int DistanceBetweenFoods;
    public int DistanceBetweenProductions;

    void Start()
    {
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 0);
    }

    public void SetTile(Tile tile)
    {
        for (var i = 0; i < BasicResourceDisplay.childCount; i++)
            Destroy(BasicResourceDisplay.GetChild(i));

        var resources = tile.GetTileResources();
        // if there's a strategic resource, display its image
        var strategic = resources.FirstOrDefault(r => r.Key is StrategicResource);
        if (strategic.Value == 0)
        {
            StrategicResourceDisplay.color = new Color(1, 1, 1, 0);
        }
        else
        {
            StrategicResourceDisplay.color = new Color(1, 1, 1, 1);
        }

        var food = resources.FirstOrDefault(r => r.Key is Food);
        var production = resources.FirstOrDefault(r => r.Key is Production);
        int totalSize = Math.Max(0, food.Value - 1) * DistanceBetweenFoods + Math.Min(1, food.Value * production.Value) * DistanceBetweenBasicResources + Math.Max(0, production.Value - 1) * DistanceBetweenProductions;
        int currentPos = (0 - totalSize) / 2;
        //int currentPos = 0;
        for (int f = 0; f < food.Value; f++)
        {
            if (f > 0)
                currentPos += DistanceBetweenFoods;
            Image ficon = Instantiate(FoodIconPrefab);
            ficon.transform.position = new Vector3(currentPos, 0, 0);
            ficon.transform.SetParent(BasicResourceDisplay, false);
        }

        currentPos += Math.Min(1, food.Value * production.Value) * DistanceBetweenBasicResources;

        for (int p = 0; p < production.Value; p++)
        {
            if (p > 0)
                currentPos += DistanceBetweenProductions;
            Image picon = Instantiate(ProductionIconPrefab);
            picon.transform.position = new Vector3(currentPos, 0, 0);
            picon.transform.SetParent(BasicResourceDisplay, false);
        }

    }
}
