using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public AudioSource UIAudioSource;

    public AudioClip Select1;
    public AudioClip Click1;
    public AudioClip MenuMusic;

    public List<BuildItem> AllBuildItems;

    public GameObject TileValueDisplayPrefab;

    public Research Research;

    public List<BuildItem> AvailableBuildItems
    {
        get;
        private set;
    }

    public Sprite[] ResearchImages;

    public void PlayUIClick()
    {
        UIAudioSource.PlayOneShot(Click1, 0.3f);
    }

    public static GameManager instance = null;

    void Awake()
    {
        instance = this;

        Research = new Research(ResearchImages);
    }

    void Start()
    {
        // add all researches as build items
        for (int i = 0; i < Research.ResearchItems.Length; i++)
        {
            var research = Research.ResearchItems[i];
            AllBuildItems.Add(new BuildItem(research.Title, research.Image, research.Tooltip, research.ProductionCosts, 0, research));
        }

        AvailableBuildItems = new List<BuildItem>(AllBuildItems);


        UIAudioSource.PlayOneShot(MenuMusic, 0.8f);
        
        //testwise
        Tile.TileValuesContainer = new GameObject("TileValues");
        //ToggleResourceDisplay();
    }

    private bool tileResourcesDisplayed = false;
    public void ToggleResourceDisplay()
    {
        foreach (var t in GridManager.instance.board.Values)
        {
            if (!tileResourcesDisplayed)
                t.DisplayTileResources();
            else
                t.HideTileResources();
        }
        tileResourcesDisplayed = !tileResourcesDisplayed;
    }
}
