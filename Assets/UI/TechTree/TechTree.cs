using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TechTree : MonoBehaviour {

    public Sprite NormalBackground;
    public Sprite FinishedBackground;
    public Sprite AvailableBackground;
    public Sprite SelectedBackground;
    public GameObject TechItemsContainer;
    public GameObject TechItemDisplayPrefab;
    public GameObject TechLineHorizontal;
    public GameObject TechLineVertical;
    public GameObject TechLineHorizontalUp;
    public GameObject TechLineHorizontalDown;
    public GameObject TechLineUpHorizontal;
    public GameObject TechLineDownHorizontal;

    private float gridY = -100;
    private float gridX = 340;
    private float startX = 200;
    private float startY = -100;

    private Dictionary<ResearchItem, TechItem> ItemDisplays;
    private TechItem SelectedItem;

	// Use this for initialization
	void Start () {
        ItemDisplays = new Dictionary<ResearchItem, TechItem>();
        BuildTree(GameManager.instance.Research.ResearchItems);
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void SetFinished(ResearchItem item)
    {
        ItemDisplays[item].Background.sprite = FinishedBackground;
    }

    public void SelectItem(TechItem techItem)
    {
        if (techItem.Background.sprite == AvailableBackground)
        {
            if (SelectedItem != null)
            {
                SelectedItem.Background.sprite = AvailableBackground;
            }
            techItem.Background.sprite = SelectedBackground;
            SelectedItem = techItem;
        }
    }

    private void BuildTree(ResearchItem[] researchItems)
    {
        Debug.Log("Build Research Tree");
        foreach (var item in researchItems)
        {
            var displayItem = Instantiate(TechItemDisplayPrefab);
            displayItem.transform.SetParent(TechItemsContainer.transform);
            displayItem.transform.position = new Vector3(startX + item.X * gridX, startY + item.Y * gridY, 0);
            var ti = displayItem.GetComponent<TechItem>();
            ti.SetResearchItem(item);
            ti.TechTreeInstance = this;
            ItemDisplays.Add(item, ti);
            // create connections to children
            if (item.Children != null)
                foreach (var child in item.Children)
                    CreateLine(item.X, item.Y, child.X, child.Y);
            // show as available if on first layer
            if (item.X == 0)
                ti.Background.sprite = AvailableBackground;
        }
    }

    /// <summary>
    /// Creates a UI connection between a parent and a child position
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    private void CreateLine(int x1, int y1, int x2, int y2)
    {
        float distance1 = x2 - x1 - 1;
        var connectionHorizontal1 = Instantiate(TechLineHorizontal);
        var transform = connectionHorizontal1.GetComponent<RectTransform>();
        transform.SetParent(TechItemsContainer.transform);
        transform.position = new Vector3(startX + x1 * gridX + 137, startY + y1 * gridY, 0);

        // case: just horizontal
        if (y2 - y1 == 0)
        {
            transform.localScale = new Vector3(66 + distance1 * gridX, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(23 + distance1 * gridX, 1, 1);
            // create a curve towards up or down
            if (y2 - y1 > 0)
            {
                var connectionToDown = Instantiate(TechLineHorizontalDown);
                var transformToDown = connectionToDown.GetComponent<RectTransform>();
                transformToDown.SetParent(TechItemsContainer.transform);
                transformToDown.position = new Vector3(transform.position.x + transform.localScale.x, transform.position.y + 2);
                // create vertical path
                var connectionVertical = Instantiate(TechLineVertical);
                var transformVertical = connectionVertical.GetComponent<RectTransform>();
                transformVertical.SetParent(TechItemsContainer.transform);
                float verticalPosX = transform.position.x + transform.localScale.x + 18;
                float verticalPosY = transform.position.y - 18;
                transformVertical.position = new Vector3(verticalPosX, verticalPosY);
                transformVertical.localScale = new Vector3(1, -gridY - 2*18, 1);
                verticalPosY -= -gridY - 2 * 18;
                // create second curve
                var connectionToHorizontal = Instantiate(TechLineDownHorizontal);
                var transformToHorizontal = connectionToHorizontal.GetComponent<RectTransform>();
                transformToHorizontal.SetParent(TechItemsContainer.transform);
                transformToHorizontal.position = new Vector3(verticalPosX - 2, verticalPosY);
                // create second horizontal part
                verticalPosX += 18;
                verticalPosY -= 18;
                var connectionHorizontal2 = Instantiate(TechLineHorizontal);
                var transformHorizontal2 = connectionHorizontal2.GetComponent<RectTransform>();
                transformHorizontal2.SetParent(TechItemsContainer.transform);
                transformHorizontal2.position = new Vector3(verticalPosX, verticalPosY, 0);
                transformHorizontal2.localScale = new Vector3(23, 1, 1);
            }
            else
            {
                var connectionToUp = Instantiate(TechLineHorizontalUp);
                var transformToUp = connectionToUp.GetComponent<RectTransform>();
                transformToUp.SetParent(TechItemsContainer.transform);
                transformToUp.position = new Vector3(transform.position.x + transform.localScale.x, transform.position.y + 18);
                // create vertical path
                var connectionVertical = Instantiate(TechLineVertical);
                var transformVertical = connectionVertical.GetComponent<RectTransform>();
                transformVertical.SetParent(TechItemsContainer.transform);
                float verticalSizeY = -gridY - 2 * 18;
                float verticalPosX = transform.position.x + transform.localScale.x + 18;
                float verticalPosY = transform.position.y + 18 + verticalSizeY;
                transformVertical.position = new Vector3(verticalPosX, verticalPosY);
                transformVertical.localScale = new Vector3(1, verticalSizeY, 1);
                // create second curve
                var connectionToHorizontal = Instantiate(TechLineUpHorizontal);
                var transformToHorizontal = connectionToHorizontal.GetComponent<RectTransform>();
                transformToHorizontal.SetParent(TechItemsContainer.transform);
                verticalPosY += 20;
                transformToHorizontal.position = new Vector3(verticalPosX - 2, verticalPosY);
                // create second horizontal part
                verticalPosX += 18;
                verticalPosY -= 2;
                var connectionHorizontal2 = Instantiate(TechLineHorizontal);
                var transformHorizontal2 = connectionHorizontal2.GetComponent<RectTransform>();
                transformHorizontal2.SetParent(TechItemsContainer.transform);
                transformHorizontal2.position = new Vector3(verticalPosX, verticalPosY, 0);
                transformHorizontal2.localScale = new Vector3(23, 1, 1);
            }
        }
    }
}
