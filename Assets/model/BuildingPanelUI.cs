using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuildingPanelUI : MonoBehaviour
{
    public static BuildingPanelUI instance = null;

    public RectTransform buildPanelContainer;
    public Graphic buildItemTemplate;

    // Use this for initialization
    void Start ()
    {
        instance = this;
        //gameObject.SetActive(false);
        buildItemTemplate.GetComponent<RectTransform>().SetParent(null);

        SetBuildItems(new BuildItem[] { null, null, null, null, null, null, null, null, null, null });
    }

    public void SetBuildItems(BuildItem[] items)
    {
        if (items != null)
        {
            float rectHeight = buildItemTemplate.GetComponent<RectTransform>().rect.height;

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
            /*for (int btn = 0; btn < buttons.Length; btn++)
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
}
