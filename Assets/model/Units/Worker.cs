using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class Worker : IGameUnit {

    public AudioClip WorkerStartWork;
    protected CharacterMovement movement;

    [HideInInspector]
    public Phase1TileImprovement Producing;
    [HideInInspector]
    public int ProducingRoundsLeft;
    private GameObject ProductionScaffold;

    public static List<Phase1TileImprovement> Actions = new List<Phase1TileImprovement>();

    public override void UseAction(int action)
    {
        Producing = Actions[action];
        ProducingRoundsLeft = Producing.BuildDurationRounds;
        movement.CancelSuggestedMove();
        ProductionScaffold = Instantiate(GameManager.instance.ScaffoldPrefab);
        ProductionScaffold.transform.position = new Vector3(movement.curTilePos.x, movement.curTilePos.y, movement.curTilePos.z);
        audioSource.PlayOneShot(WorkerStartWork);
    }

    public override void Select()
    {
        base.Select();
        // draw own panel
        UnitPanelUI.instance.SetUnitPanelInfo(Actions.Select(a => new UnitPanelUI.UnitInfo(a.Icon, a.Tooltip)).ToArray());
    }

    protected override void Awake()
    {
        base.Awake();
        movement = GetComponent<CharacterMovement>();
        movement.MovementPointsMax = 2;
    }

    void Update()
    {

    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    public override void HoverAction(int action)
    {

    }

    public override void LeaveAction(int action)
    {

    }

    public override bool NeedsOrders()
    {
        if (movement.RemainingPath.Count > 1 || Producing != null || movement.MovementPointsRemaining == 0)
        {
            return false;
        }
        return true;
    }

    void OnEnable()
    {
        EventManager.StartListening("NextRoundRequest", nextRoundRequestListener);
    }

    void OnDisable()
    {
        EventManager.StopListening("NextRoundRequest", nextRoundRequestListener);
    }

    private void nextRoundRequestListener()
    {
        if (Producing != null && movement.ExpendMovementPoints(2))
        {
            // perform some building action
            // TODO: Animation
            ProducingRoundsLeft--;
            if (ProducingRoundsLeft <= 0)
            {
                Destroy(ProductionScaffold);
                var newImprovement = Instantiate(Producing);
                newImprovement.transform.position = new Vector3(movement.curTilePos.x, movement.curTilePos.y, movement.curTilePos.z);
                newImprovement.Location = movement.curTile;
                GameManager.instance.AddTileImprovement(newImprovement);
                Producing = null;
                //if (GridManager.instance.selectedUnit == gameObject)
                    //TODO: Update icon progress display of production
            }
        }
    }
}
