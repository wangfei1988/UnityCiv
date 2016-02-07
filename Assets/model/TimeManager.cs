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
    public Image NextRoundButtonImage;
    public Sprite NextRoundNextSprite;
    public Sprite NextRoundUnitSprite;
    public Sprite NextRoundBuildingSprite;

    public int Round
    {
        get;
        private set;
    }

    public static TimeManager instance = null;

    private bool nextRoundRequested = false;

    private DateTime nextRoundStartsAt = DateTime.MaxValue;
    private List<IEntity> entitiesAwaitingOrders = new List<IEntity>();

    // TODO: Gather event handles from "PerformingAction", continue with round if all of them completed.
    private int actionsStillBeingPerformed = 0;

    void Awake()
    {
        instance = this;
        Round = -4000;
    }

    void Start()
    {
        Cursor.SetCursor(GameManager.instance.PointerNormal, new Vector2(1, 3), CursorMode.Auto);
        
    }
    
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || nextRoundRequested) && nextRoundStartsAt == DateTime.MaxValue)
        {
            nextRoundRequested = false;

            if (entitiesAwaitingOrders.Count == 0)
            {
                UpdateEntitiesAwaitingOrders();
            }

            if (entitiesAwaitingOrders.Count == 0)
            {
                nextRoundStartsAt = DateTime.Now.AddSeconds(0.5);
                actionsStillBeingPerformed = 0;
                EventManager.TriggerEvent("NextRoundRequest");
            }
            else
            {
                // select the first unit from the list that still requires action
                SelectNextAwaitingOrder();
            }
        }

        if (nextRoundStartsAt <= DateTime.Now && actionsStillBeingPerformed <= 0)
        {
            nextRoundStartsAt = DateTime.MaxValue;
            actionsStillBeingPerformed = 0;
            Round++;
            DisplayTime();
            NextRoundButtonImage.sprite = NextRoundNextSprite;
            //NextRoundButtonImage.CrossFadeColor(new Color(0f / 255, 0f / 255, 0), 1, true, false);
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
    /// Call this if an entity needs no more orders for this round
    /// </summary>
    /// <param name="entity"></param>
    public void NoMoreOrdersNeeded(IEntity entity)
    {
        if (entitiesAwaitingOrders.Contains(entity))
        {
            entitiesAwaitingOrders.Remove(entity);
            IEntity next = GetNextEntityAwaitingOrders();
            if (next == null)
            {
                NextRoundButtonImage.sprite = NextRoundNextSprite;
            }
            else
            {
                NextRoundButtonImage.sprite = next is IGameUnit ? NextRoundUnitSprite : NextRoundBuildingSprite;
            }
        }
    }

    /// <summary>
    /// Call this if an entity requires orders which it didn't already require at the beginning of the round. Example: Newly created entity
    /// </summary>
    /// <param name="entity"></param>
    public void NeedNewOrders(IEntity entity)
    {
        if (!entitiesAwaitingOrders.Contains(entity))
        {
            entitiesAwaitingOrders.Add(entity);
            if (entitiesAwaitingOrders.Count == 1)
            {
                NextRoundButtonImage.sprite = entity is IGameUnit ? NextRoundUnitSprite : NextRoundBuildingSprite;
            }
        }
    }

    private void UpdateEntitiesAwaitingOrders()
    {
        // set entitiesAwaitingOrders
        var allEntities = GridManager.instance.allUnits.Union(GridManager.instance.allBuildings);
        entitiesAwaitingOrders = allEntities.Select(u => u.GetComponent<IEntity>()).Where(ue => ue.NeedsOrders()).ToList();
    }

    private void SelectNextAwaitingOrder()
    {
        IEntity e = GetNextEntityAwaitingOrders();

        if (e == null)
        {
            NextRoundButtonImage.sprite = NextRoundNextSprite;
        }
        else
        {
            NextRoundButtonImage.sprite = e is IGameUnit ? NextRoundUnitSprite : NextRoundBuildingSprite;
            GameManager.instance.CameraControls.FlyToTarget(e.transform.position);
            e.Select();
        }
    }

    private IEntity GetNextEntityAwaitingOrders()
    {
        IEntity e = null;
        while (e == null && entitiesAwaitingOrders.Count > 0)
        {
            e = entitiesAwaitingOrders.First();
            if (!e.NeedsOrders())
            {
                entitiesAwaitingOrders.Remove(e);
                e = null;
            }
        }
        return e;
    }

    private void DisplayTime()
    {
        if (Round < 0)
        {
            TimeLabel.text = Round < 0 ? -Round + " BC" : Round + " AD";
        }
    }

    public void RequestNextRound()
    {
        nextRoundRequested = true;
    }
}
