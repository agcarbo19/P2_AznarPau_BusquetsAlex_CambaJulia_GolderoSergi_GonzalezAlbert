using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSMmouse", menuName = "Finite State Machines/FSMmouse", order = 1)]
public class FSMmouse : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private mouseBlackboard blackboard;
    private GameObject roomba;
    private GoToTarget goToTarget;          //Genera un path
    private PathFollowing pathFollowing;    //Sigue un path
    private SteeringContext steeringContext;
    private GameObject closestExit;
    public override void OnEnter()
    {
        blackboard = GetComponent<mouseBlackboard>();
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

        FiniteStateMachine FSM_mouseMain = ScriptableObject.CreateInstance<FSM_mouseMain>();
        FSM_mouseMain.Name = "MOUSE MAIN";
        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        State RANAWAY = new State("RAN AWAY",
            () => { Destroy(goToTarget.target); goToTarget.target = blackboard.GetClosestExitPoint(); goToTarget.enabled = true; pathFollowing.enabled = true; steeringContext.maxSpeed *= blackboard.speedBooster; steeringContext.maxAcceleration *= blackboard.accBooster; }, // write on enter logic inside {}
            () => { GetComponent<SpriteRenderer>().color = new Color(3f / 256, 120f / 256, 7f / 256); }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );
        State EXITREACHED = new State("EXIT",
            () => { Destroy(this.gameObject); }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { goToTarget.enabled = false; pathFollowing.enabled = false; }  // write on exit logic inisde {}  
        );
        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        Transition scareMe = new Transition("Scare Me",
            () => {
                roomba = SensingUtils.FindInstanceWithinRadius(gameObject, "ROOMBA", blackboard.roombaDetectionRadius);
                return roomba != null;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition inRangeOfScape = new Transition("In Range of Scape",
            () => {
               
                return
                SensingUtils.DistanceToTarget(gameObject, goToTarget.target) <= blackboard.hideoutReachedRadius;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        AddStates(FSM_mouseMain, RANAWAY, EXITREACHED);

        AddTransition(FSM_mouseMain, scareMe, RANAWAY);
        AddTransition(RANAWAY, inRangeOfScape, EXITREACHED);

        initialState = FSM_mouseMain;

    }
}
