using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public AudioSource UIAudioSource;
    public CivCamera CameraControls;
    public Texture2D PointerNormal;
    public AudioClip Select1;
    public AudioClip Click1;
    public AudioClip MenuMusic;

    public List<BuildItem> AllBuildItems;

    public GameObject TileValueDisplayPrefab;

    public List<BuildItem> AvailableBuildItems
    {
        get;
        private set;
    }

    public Sprite[] ResearchImages;

    public GameObject ScaffoldPrefab;

    public List<Phase1TileImprovement> Phase1TileImprovements;

    public Research Research;

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
        AvailableBuildItems = new List<BuildItem>(AllBuildItems);

        // background music
        UIAudioSource.PlayOneShot(MenuMusic, 0.8f);

        // assign tile improvements to worker
        Worker.Actions.Add(Phase1TileImprovements[0]);
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

    public void AddTileImprovement(Phase1TileImprovement improvement)
    {
        GridManager.instance.allTileImprovements.Add(improvement);
        // TODO: Change yield of affected tiles
    }

    public void RemoveTileImprovement(Phase1TileImprovement improvement)
    {
        throw new NotImplementedException();
    }
}
