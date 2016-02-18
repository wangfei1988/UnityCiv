using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitPanelUI : MonoBehaviour
{
    public static UnitPanelUI instance = null;
    
    public Button[] Buttons;
    public Image[] ButtonSubImages;

    void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
        //Buttons = GetComponentsInChildren<Button>();
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
            for (int i = 0; i < Buttons.Length; i++)
            {
                // the button will be used
                if (i < infos.Length)
                {
                    var button = Buttons[i];
                    button.gameObject.SetActive(true);
                    ButtonSubImages[i].sprite = infos[i].image;
                }
                else
                {
                    Buttons[i].gameObject.SetActive(false);
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
