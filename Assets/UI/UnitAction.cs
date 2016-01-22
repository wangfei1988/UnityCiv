using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System;

public class UnitAction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private static UnitPanelUI unitPanelUI;
    private int buttonIndex;

    void Start()
    {
        unitPanelUI = UnitPanelUI.instance;
        var btn = gameObject.GetComponent<Button>();
        buttonIndex = new List<Button>(unitPanelUI.Buttons).IndexOf(btn);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        unitPanelUI.HoverUnitAction(buttonIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        unitPanelUI.LeaveUnitAction(buttonIndex);
    }
}
