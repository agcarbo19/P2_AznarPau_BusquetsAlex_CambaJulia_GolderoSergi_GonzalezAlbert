using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_roombaPatrol", menuName = "Finite State Machines/FSM_roombaPatrol", order = 1)]
public class FSM_roombaPatrol : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private roombaBlackboard blackboard;

    private GoToTarget goToTarget;          //Genera un path
    private PathFollowing pathFollowing;    //Sigue un path
    private GameObject currentWanderpoint;
    public override void OnEnter()
    {
        blackboard = GetComponent<roombaBlackboard>();
        goToTarget = GetComponent<GoToTarget>();
        pathFollowing = GetComponent<PathFollowing>();
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        base.DisableAllSteerings();
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
        State CHOOSE = new State("CHOOSE",
            () => {
                currentWanderpoint = blackboard.GetRandomWanderPoint();
                goToTarget.target = currentWanderpoint;
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );
        State REACHING = new State("REACHING",
            () => {
                goToTarget.enabled = true;
                pathFollowing.enabled = true;
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => {
                goToTarget.enabled = false;
                pathFollowing.enabled = false;
            }  // write on exit logic inisde {}  
        );

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        Transition wayPointChosen = new Transition("wayPointChosen",
             () => {

                 return currentWanderpoint != null;
             }, // write the condition checkeing code in {}
             () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
         );
        Transition wayPointReached = new Transition("wayPointReached",
                () => { return SensingUtils.DistanceToTarget(gameObject, currentWanderpoint) <= blackboard.placeReached; }, // write the condition checkeing code in {}
                () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(CHOOSE, REACHING);


        AddTransition(CHOOSE, wayPointChosen, REACHING);
        AddTransition(REACHING, wayPointReached, CHOOSE);

        initialState = CHOOSE;

        /* STAGE 4: set the initial state
         
        initialState = ... 

         */

    }
}
