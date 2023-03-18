using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSMmouse", menuName = "Finite State Machines/FSMmouse", order = 1)]
public class FSMmouse : FiniteStateMachine
{

    private mouseBlackboard blackboard;
    private GameObject roomba;
    private GoToTarget goToTarget;

    private SteeringContext steeringContext;
    private GameObject closestExit;
    public override void OnEnter()
    {
        blackboard = GetComponent<mouseBlackboard>();
        goToTarget = GetComponent<GoToTarget>();

        steeringContext = GetComponent<SteeringContext>();

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

        FiniteStateMachine FSM_mouseMain = ScriptableObject.CreateInstance<FSM_mouseMain>();
        FSM_mouseMain.Name = "MOUSE MAIN";



        State RANAWAY = new State("RAN AWAY",
            () =>
            {
                Destroy(goToTarget.target); goToTarget.target = blackboard.GetClosestExitPoint();
                goToTarget.enabled = true;
                steeringContext.maxSpeed *= blackboard.speedBooster;
                steeringContext.maxAcceleration *= blackboard.accBooster;
            },
            () => { GetComponent<SpriteRenderer>().color = new Color(3f / 256, 120f / 256, 7f / 256); },
            () => { }
        );
        State EXITREACHED = new State("EXIT",
            () => { Destroy(this.gameObject); },
            () => { },
            () => { goToTarget.enabled = false; }
        );

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------
 
        */

        Transition scareMe = new Transition("Scare Me",
            () =>
            {
                roomba = SensingUtils.FindInstanceWithinRadius(gameObject, "ROOMBA", blackboard.roombaDetectionRadius);
                return roomba != null;
            },
            () => { }
        );
        Transition inRangeOfScape = new Transition("In Range of Scape",
            () =>
            {

                return
                SensingUtils.DistanceToTarget(gameObject, goToTarget.target) <= blackboard.hideoutReachedRadius;
            },
            () => { }
        );


        /* STAGE 3: add states and transitions to the FSM 
     * ----------------------------------------------

     */
        AddStates(FSM_mouseMain, RANAWAY, EXITREACHED);

        AddTransition(FSM_mouseMain, scareMe, RANAWAY);
        AddTransition(RANAWAY, inRangeOfScape, EXITREACHED);

        /* STAGE 4: set the initial state


        */
        initialState = FSM_mouseMain;

    }
}
