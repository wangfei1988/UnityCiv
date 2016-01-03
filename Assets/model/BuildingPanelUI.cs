using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildingPanelUI : MonoBehaviour
{
    public static BuildingPanelUI instance = null;

    public RectTransform buildPanelContainer;
    public GameObject buildItemTemplate;

    // Use this for initialization
    void Start ()
    {
        instance = this;
        //gameObject.SetActive(false);
    }

    public void SetBuildItems(BuildItem[] items, int ProductionOutput)
    {
        if (items != null)
        {
            foreach (Transform child in buildPanelContainer.transform)
            {
                Destroy(child.gameObject);
            }

            gameObject.SetActive(true);

            foreach (var item in items)
            {
                GameObject newItem = Instantiate<GameObject>(buildItemTemplate);
                BuildingItemPrefab newItem_vals = newItem.GetComponent<BuildingItemPrefab>();
                newItem_vals.title.text = item.Title;
                newItem_vals.icon.sprite = item.Image;
                newItem_vals.turns.text = ((int)item.ProductionCosts / ProductionOutput) + " Turns";
                newItem_vals.button.onClick.AddListener(new UnityEngine.Events.UnityAction(() => SelectItem(item)));
                newItem.transform.SetParent(buildPanelContainer);
            }

            /*float rectHeight = buildItemTemplate.GetComponent<RectTransform>().rect.height;

            gameObject.SetActive(true);
            for (int i = 0; i < 10; i++)
            {
                var bItem = Instantiate<Graphic>(buildItemTemplate);
                var rect = bItem.GetComponent<RectTransform>();
                rect.SetParent(buildPanelContainer);
                //rect.anchoredPosition.Set(rect.anchoredPosition.x, rect.anchoredPosition.y + i * rect.rect.height);
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - i * rectHeight);
                var sdlk = 5;
            }
            buildPanelContainer.offsetMin = new Vector2(0, -items.Length * rectHeight);
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
            }*/
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void SelectItem(BuildItem item)
    {
        if (GridManager.instance.selectedBuilding != null)
        {
            IGameBuilding building = GridManager.instance.selectedBuilding.GetComponent<IGameBuilding>();
            building.Produce(item);
        }
    }
}
