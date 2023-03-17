using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_roombaBattery", menuName = "Finite State Machines/FSM_roombaBattery", order = 1)]
public class FSM_roombaBattery : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private roombaBlackboard blackboard;

    private GoToTarget goToTarget;          //Genera un path
    private PathFollowing pathFollowing;    //Sigue un path
    private SteeringContext steeringContext;
    private GameObject closestCharger;
    public override void OnEnter()
    {
        blackboard = GetComponent<roombaBlackboard>();
        goToTarget = GetComponent<GoToTarget>();
        pathFollowing = GetComponent<PathFollowing>();
        steeringContext = GetComponent<SteeringContext>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        DisableAllSteerings();

        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------
         
        State varName = new State("StateName",
            () => { }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );

         */
        FiniteStateMachine CLEANING = ScriptableObject.CreateInstance<FSM_roombaCleaning>();
        CLEANING.Name = "CLEANING";
        State GOTOCHARGE = new State("Go To Charge",
            () => {
                closestCharger = blackboard.whereToRecharge();
                goToTarget.target = closestCharger; goToTarget.enabled = true; }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { goToTarget.target = null; goToTarget.enabled = false; }  // write on exit logic inisde {}  
        );
        State CHARGING = new State("Charging",
            () => {  }, // write on enter logic inside {}
            () => { blackboard.Recharge(Time.deltaTime); }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );
        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        
        Transition lowBattery = new Transition("Low Battery",
            () => {
                return blackboard.currentCharge <= blackboard.minCharge; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition inCharger = new Transition("In Charger",
            () => {
                return
                    SensingUtils.DistanceToTarget(gameObject, closestCharger)
                    <= blackboard.chargingStationReachedRadius;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition fullBattery = new Transition("Full Battery",
            () => {
                return blackboard.currentCharge >= blackboard.maxCharge;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(CLEANING, GOTOCHARGE, CHARGING);

        AddTransition(CLEANING, lowBattery, GOTOCHARGE);
        AddTransition(GOTOCHARGE, inCharger, CHARGING);
        AddTransition(CHARGING, fullBattery, CLEANING);

        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        initialState = CLEANING;


    }
}
