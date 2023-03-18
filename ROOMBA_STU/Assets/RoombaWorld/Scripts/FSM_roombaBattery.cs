using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_roombaBattery", menuName = "Finite State Machines/FSM_roombaBattery", order = 1)]
public class FSM_roombaBattery : FiniteStateMachine
{ 
    private roombaBlackboard blackboard;

    private GoToTarget goToTarget;         
    private GameObject closestCharger;

    public override void OnEnter()
    {
        blackboard = GetComponent<roombaBlackboard>();
        goToTarget = GetComponent<GoToTarget>();  

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    { 
        DisableAllSteerings();

        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *----------------------------------------------- 
         */
        FiniteStateMachine CLEANING = ScriptableObject.CreateInstance<FSM_roombaCleaning>();
        CLEANING.Name = "CLEANING";

        State GOTOCHARGE = new State("Go To Charge",
            () =>
            {
                closestCharger = blackboard.whereToRecharge();
                goToTarget.target = closestCharger;
                goToTarget.enabled = true;
            },  
            () => { }, 
            () =>
            {
                goToTarget.target = null;
                goToTarget.enabled = false;
            }   
        );
        State CHARGING = new State("Charging",
            () => { },  
            () => { blackboard.Recharge(Time.deltaTime); }, 
            () => { }  
        );
        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------  
        */

        Transition lowBattery = new Transition("Low Battery",
            () =>
            {
                return blackboard.currentCharge <= blackboard.minCharge;
            }, 
            () => { }   
        );
        Transition inCharger = new Transition("In Charger",
            () =>
            {
                return
                    SensingUtils.DistanceToTarget(gameObject, closestCharger)
                    <= blackboard.chargingStationReachedRadius;
            }, 
            () => { }   
        );
        Transition fullBattery = new Transition("Full Battery",
            () =>
            {
                return blackboard.currentCharge >= blackboard.maxCharge;
            },  
            () => { }   
        );
        /* STAGE 3: add states and transitions to the FSM 
         * ---------------------------------------------- 
         */

        AddStates(CLEANING, GOTOCHARGE, CHARGING);

        AddTransition(CLEANING, lowBattery, GOTOCHARGE);
        AddTransition(GOTOCHARGE, inCharger, CHARGING);
        AddTransition(CHARGING, fullBattery, CLEANING);

        /* STAGE 4: set the initial state 
         */
        initialState = CLEANING;


    }
}
