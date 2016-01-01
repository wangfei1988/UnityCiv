using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public List<BuildItem> AvailableBuildItems
    {
        get;
        private set;
    }

    public static GameManager instance = null;

    void Awake()
    {
        instance = this;

        AvailableBuildItems = new List<BuildItem>()
        {
            new BuildItem(null, "build worker", 5, 250)
        };
    }
}
