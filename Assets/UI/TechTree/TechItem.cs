using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TechItem : MonoBehaviour {

    public ResearchItem ResearchItem;
    public Image Icon;
    public Text Name;

    public void SetResearchItem(ResearchItem item)
    {
        ResearchItem = item;
        Icon.sprite = item.Image;
        Name.text = item.Title;
    }
}
