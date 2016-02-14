using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TechItem : MonoBehaviour {

    public ResearchItem ResearchItem;
    public Image Icon;
    public Text Name;
    public Image Background;
    public TechTree TechTreeInstance;

    public void SetResearchItem(ResearchItem item)
    {
        ResearchItem = item;
        Icon.sprite = item.Image;
        Name.text = item.Title;
    }

    public void OnClick()
    {
        TechTreeInstance.SelectItem(this);
    }
}