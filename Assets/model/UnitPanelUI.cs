using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitPanelUI : MonoBehaviour
{
    public static UnitPanelUI instance = null;
    
    private Button[] buttons;

    void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
        buttons = GetComponentsInChildren<Button>();
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
            for (int btn = 0; btn < buttons.Length; btn++)
            {
                // the button will be used
                if (btn < infos.Length)
                {
                    var button = buttons[btn];
                    button.gameObject.SetActive(true);
                    var btnImg = (Image)button.targetGraphic;
                    btnImg.sprite = infos[btn].image;
                }
                else
                {
                    buttons[btn].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /*void OnGUI()
    {
        if (GridManager.instance.selectedUnit != null)
        {
            //gameObject.SetActive(true);
            int buttoncount = 0;
            foreach (var content in this.infos)
            {
                if (GUI.Button(new Rect((buttonWidth + 20) * buttoncount, 0, buttonWidth, buttonHeight), content, buttonStyle))
                {
                    //button was clicked
                    var unit = GridManager.instance.selectedUnit.GetComponent<IGameUnit>();
                    unit.UseAction(unit.Actions[buttoncount]);
                }
                buttoncount++;
            }
        }
        else
        {
            //gameObject.SetActive(false);
        }
    }*/

    public void UseUnitAction(int action)
    {
        if (action == 0)
        {
            var unit = GridManager.instance.selectedUnit;
            if (unit != null)
                unit.GetComponent<IGameUnit>().UseAction("Expand");
        }
    }
}
