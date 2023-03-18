using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_roombaPatrol", menuName = "Finite State Machines/FSM_roombaPatrol", order = 1)]
public class FSM_roombaPatrol : FiniteStateMachine
{
    private roombaBlackboard blackboard;
    private GoToTarget goToTarget;
    private GameObject currentWanderpoint;

    public override void OnEnter()
    {
        blackboard = GetComponent<roombaBlackboard>();
        goToTarget = GetComponent<GoToTarget>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------  
         */

        State CHOOSE = new State("CHOOSE",
            () =>
            {
                currentWanderpoint = blackboard.GetRandomWanderPoint();
                goToTarget.target = currentWanderpoint;
            },
            () => { },
            () => { }
        );

        State REACHING = new State("REACHING",
            () =>
            {
                goToTarget.enabled = true;

            },
            () => { },
            () =>
            {
                goToTarget.enabled = false;

            }
        );

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------  
        */

        Transition wayPointChosen = new Transition("wayPointChosen",
             () =>
             { 
                 return currentWanderpoint != null;
             },  
             () => { }   
         );

        Transition wayPointReached = new Transition("wayPointReached",
                () => { return SensingUtils.DistanceToTarget(gameObject, currentWanderpoint) <= blackboard.placeReached; },  
                () => { }  
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------  
         */

        AddStates(CHOOSE, REACHING);


        AddTransition(CHOOSE, wayPointChosen, REACHING);
        AddTransition(REACHING, wayPointReached, CHOOSE);


        /* STAGE 4: set the initial state  
         */

        initialState = CHOOSE;
    }
}
