using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{

    public Text TimeLabel;
    public int Round
    {
        get;
        private set;
    }

    void Awake()
    {
        Round = -4000;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Round++;
            DisplayTime();
            EventManager.TriggerEvent("NextRound");
        }
    }

    private void DisplayTime()
    {
        if (Round < 0)
        {
            TimeLabel.text = Round + " BC";
        }
    }
}
