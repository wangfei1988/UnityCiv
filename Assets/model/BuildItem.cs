using UnityEngine;
using System.Collections;
using UnityEditor;

[System.Serializable]
public class BuildItem {
    public string Title;
    public Sprite Image;
    public string ImageAssetPath;
    public string Tooltip;
    public float ProductionCosts;
    public float PurchaseCosts;
    public GameObject Produces;
    public string ProducesAssetPath;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="image"></param>
    /// <param name="tooltip"></param>
    /// <param name="productionCosts"></param>
    /// <param name="purchaseCosts">Amount of gold the item can be purchased for. 0 = item can't be purchased.</param>
    public BuildItem(string title, string imageAssetPath, string tooltip, float productionCosts, float purchaseCosts, string producesAssetPath)
    {
        Title = title;
        ImageAssetPath = imageAssetPath;
        Tooltip = tooltip;
        ProductionCosts = productionCosts;
        PurchaseCosts = purchaseCosts;
        ProducesAssetPath = producesAssetPath;
    }

    public void LoadIcon()
    {
        if (Image == null && ImageAssetPath != null)
            Image = Resources.Load<Sprite>(ImageAssetPath);
    }

    public void LoadObject()
    {
        if (Produces == null && ProducesAssetPath != null)
            Produces = Resources.Load<GameObject>(ProducesAssetPath);
    }
}
