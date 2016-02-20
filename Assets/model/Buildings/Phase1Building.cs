using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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
        Renderer renderer = GetComponentInChildren<MeshRenderer>();

        Color color;

        switch (colorPreset)
        {
            case ColorPresets.Green:
                color = new Color(0, 0, 0, 1);
                break;
            case ColorPresets.Red:
                color = new Color(0.3f, 0, 0, 1);
                break;
            default:
                color = new Color(0, 0, 0, 1);
                break;
        }

        for (int i = 0; i < renderer.materials.Length; i++)
        {
            renderer.materials[i].EnableKeyword("_EMISSION");
            renderer.materials[i].SetColor("_EmissionColor", color);
        }
    }
}
