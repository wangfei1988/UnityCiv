using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public AudioSource UIAudioSource;

    public AudioClip Select1;
    public AudioClip Click1;
    public AudioClip MenuMusic;

    public List<BuildItem> AllBuildItems;

    public List<BuildItem> AvailableBuildItems
    {
        get;
        private set;
    }

    public void PlayUIClick()
    {
        UIAudioSource.PlayOneShot(Click1, 0.3f);
    }

    public static GameManager instance = null;

    void Awake()
    {
        instance = this;
        AvailableBuildItems = new List<BuildItem>(AllBuildItems);
    }

    void Start()
    {
        UIAudioSource.PlayOneShot(MenuMusic, 0.8f);
    }
}
