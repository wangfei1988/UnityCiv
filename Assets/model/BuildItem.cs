using UnityEngine;
using System.Collections;

public class BuildItem {
    public Sprite Image;
    public string Tooltip;
    public float ProductionCosts;
    public float PurchaseCosts;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="image"></param>
    /// <param name="tooltip"></param>
    /// <param name="productionCosts"></param>
    /// <param name="purchaseCosts">Amount of gold the item can be purchased for. 0 = item can't be purchased.</param>
    public BuildItem(Sprite image, string tooltip, float productionCosts, float purchaseCosts)
    {
        Image = image;
        Tooltip = tooltip;
        ProductionCosts = productionCosts;
        PurchaseCosts = purchaseCosts;
    }
}
