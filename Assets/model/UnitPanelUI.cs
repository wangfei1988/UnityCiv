using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitPanelUI : MonoBehaviour
{
    public static UnitPanelUI instance = null;
    
    public Button[] Buttons;

    void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
        Buttons = GetComponentsInChildren<Button>();
    }

    public class UnitInfo {
        public Sprite image;
        public string tooltip;
        public UnitInfo(Sprite image, string tooltip)
        {
            this.image = image;
            this.tooltip = tooltip;
        }
    }

    public void SetUnitPanelInfo(UnitInfo[] infos)
    {
        if (infos != null)
        {
            gameObject.SetActive(true);
            for (int btn = 0; btn < Buttons.Length; btn++)
            {
                // the button will be used
                if (btn < infos.Length)
                {
                    var button = Buttons[btn];
                    button.gameObject.SetActive(true);
                    var btnImg = (Image)button.targetGraphic;
                    btnImg.sprite = infos[btn].image;
                }
                else
                {
                    Buttons[btn].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Linked in editor UI
    /// </summary>
    /// <param name="action"></param>
    public void UseUnitAction(int action)
    {
        var unit = GridManager.instance.selectedUnit;
        if (unit != null)
            unit.GetComponent<IGameUnit>().UseAction(action);
    }

    public void HoverUnitAction(int action)
    {
        var unit = GridManager.instance.selectedUnit;
        if (unit != null)
            unit.GetComponent<IGameUnit>().HoverAction(action);
    }

    public void LeaveUnitAction(int action)
    {
        var unit = GridManager.instance.selectedUnit;
        if (unit != null)
            unit.GetComponent<IGameUnit>().LeaveAction(action);
    }
}
