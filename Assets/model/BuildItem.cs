using UnityEngine;
using System.Collections;

[System.Serializable]
public class BuildItem {
    public string Title;
    public Sprite Image;
    public string Tooltip;
    public float ProductionCosts;
    public float PurchaseCosts;
    public Object Produces;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="image"></param>
    /// <param name="tooltip"></param>
    /// <param name="productionCosts"></param>
    /// <param name="purchaseCosts">Amount of gold the item can be purchased for. 0 = item can't be purchased.</param>
    public BuildItem(string title, Sprite image, string tooltip, float productionCosts, float purchaseCosts, Object produces)
    {
        Title = title;
        Image = image;
        Tooltip = tooltip;
        ProductionCosts = productionCosts;
        PurchaseCosts = purchaseCosts;
        Produces = produces;
    }
}
