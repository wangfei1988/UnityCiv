using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public AudioSource UIAudioSource;

    public List<BuildItem> AllBuildItems;

    public List<BuildItem> AvailableBuildItems
    {
        get;
        private set;
    }

    public static GameManager instance = null;

    void Awake()
    {
        instance = this;
        AvailableBuildItems = new List<BuildItem>(AllBuildItems);
    }
}
