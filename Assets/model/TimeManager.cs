using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

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

    public static TimeManager instance = null;

    private DateTime nextRoundStartsAt = DateTime.MaxValue;
    private List<IEntity> entitiesAwaitingOrders = new List<IEntity>();

    // TODO: Gather event handles from "PerformingAction", continue with round if all of them completed.
    private int actionsStillBeingPerformed = 0;

    void Awake()
    {
        instance = this;
        Round = -4000;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && nextRoundStartsAt == DateTime.MaxValue)
        {
            nextRoundStartsAt = DateTime.Now.AddSeconds(0.5);
            actionsStillBeingPerformed = 0;
            EventManager.TriggerEvent("NextRoundRequest");
            //test
            //GameManager.instance.CameraControls.FlyToTarget(GridManager.instance.allUnits.First().transform.position);
        }

        if (nextRoundStartsAt <= DateTime.Now && actionsStillBeingPerformed <= 0)
        {
            nextRoundStartsAt = DateTime.MaxValue;
            actionsStillBeingPerformed = 0;
            Round++;
            DisplayTime();
            GameManager.instance.UIAudioSource.PlayOneShot(NextRoundClip, 0.7f);
            EventManager.TriggerEvent("NextRound");
        }
    }

    /// <summary>
    /// Listeners to "NextRoundRequest" call this if the next round is requested and they perform an action
    /// </summary>
    public void PerformingAction()
    {
        actionsStillBeingPerformed++;
    }

    public void FinishedAction()
    {
        actionsStillBeingPerformed--;
    }

    /// <summary>
    /// Listeners to "NextRoundRequest" call this if they can't go to the next round because e.g. because a user input is still required
    /// </summary>
    public void DenyNextRoundRequest(IEntity entity)
    {
        entitiesAwaitingOrders.Add(entity);
        nextRoundStartsAt = DateTime.MaxValue;
    }

    private void DisplayTime()
    {
        if (Round < 0)
        {
            TimeLabel.text = Round < 0 ? -Round + " BC" : Round + " AD";
        }
    }
}
