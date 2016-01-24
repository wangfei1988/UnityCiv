using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;
using System;

/// <summary>
/// Next round method chain:
/// - Trigger "NextRoundRequest"
/// - listeners will call TimeManager's "PerformingAction"
/// </summary>
public class TimeManager : MonoBehaviour
{
    public AudioClip NextRoundClip;

    public Text TimeLabel;
    public int Round
    {
        get;
        private set;
    }

    private DateTime nextRoundStartsAt = DateTime.MaxValue;

    void Awake()
    {
        Round = -4000;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            nextRoundStartsAt = DateTime.Now.AddSeconds(1);
            EventManager.TriggerEvent("NextRoundRequest");
        }

        if (nextRoundStartsAt != null && nextRoundStartsAt <= DateTime.Now)
        {
            nextRoundStartsAt = DateTime.MaxValue;
            Round++;
            DisplayTime();
            GameManager.instance.UIAudioSource.PlayOneShot(NextRoundClip, 0.4f);
            EventManager.TriggerEvent("NextRound");
        }
    }

    /// <summary>
    /// Listeners to "NextRoundRequest" call this if the next round is requested and they perform an action that takes a certain duration maximally
    /// </summary>
    public void PerformingAction()
    {

    }

    /// <summary>
    /// Listeners to "NextRoundRequest" call this if they can't go to the next round because e.g. because a user input is still required
    /// </summary>
    public void DenyNextRoundRequest()
    {

    }

    private void DisplayTime()
    {
        if (Round < 0)
        {
            TimeLabel.text = Round < 0 ? -Round + " BC" : Round + " AC";
        }
    }
}
