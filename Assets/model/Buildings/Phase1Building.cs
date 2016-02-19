using UnityEngine;
using System.Collections;
using System;

public abstract class Phase1Building : IEntity {

    [HideInInspector]
    public int Range;

    public override bool NeedsOrders()
    {
        return false;
    }

    public override void Select()
    { }

    public enum ColorPresets
    {
        Green,
        Red,
        Transparent
    }

    public void SetColor(ColorPresets colorPreset)
    {
        Renderer renderer = GetComponentInChildren<Renderer>();

        Color color;

        switch (colorPreset)
        {
            case ColorPresets.Green:
                color = Color.green;
                break;
            case ColorPresets.Red:
                color = Color.red;
                break;
            default:
                color = new Color(0, 0, 0, 0);
                break;
        }

        //float emission = Mathf.PingPong(Time.time, 1.0f);
        //Color baseColor = Color.yellow; //Replace this with whatever you want for your base color at emission level '1'

        //Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

        foreach (var mat in renderer.materials)
        {
            mat.EnableKeyword("_EmissionColor");
            mat.SetColor("_EmissionColor", color);
        }
    }
}
