using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_roombaCleaning", menuName = "Finite State Machines/FSM_roombaCleaning", order = 1)]
public class FSM_roombaCleaning : FiniteStateMachine
{

    private roombaBlackboard blackboard;

    private GoToTarget goToTarget;          //Genera un path
    private PathFollowing pathFollowing;    //Sigue un path
    private GameObject otherPoo;
    private GameObject poo;
  //  private GameObject otherDust;
    private GameObject dust;

    private SteeringContext steeringContext;

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
        FiniteStateMachine PATROL = ScriptableObject.CreateInstance<FSM_roombaPatrol>();
        PATROL.Name = "PATROL";

        State REACHINGPOO = new State("reachingPoo",
            () => { goToTarget.target = poo; goToTarget.enabled = true; steeringContext.maxSpeed *= blackboard.speedBooster; steeringContext.maxAcceleration *= blackboard.accBooster; }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { goToTarget.target = null; goToTarget.enabled = false; steeringContext.maxSpeed /= blackboard.speedBooster; steeringContext.maxAcceleration /= blackboard.accBooster; }  // write on exit logic inisde {}  
        );
        State REACHINGDUST = new State("reachingDust",
            () => { goToTarget.target = dust; goToTarget.enabled = true; }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { goToTarget.target = null; goToTarget.enabled = false; }  // write on exit logic inisde {}  
        );
        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        Transition pooFound = new Transition("goToPoo",
            () => { poo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", blackboard.pooDetectionRadius);
                return poo != null;
            }, // write the condition checkeing code in {}
            () => {  }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition pooBumpedDustTargetMemory = new Transition("PooBumpedDustTargetMemory",
            () => {
                poo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", blackboard.pooDetectionRadius);
                return (poo != null && dust != null);
            }, // write the condition checkeing code in {}
            () => { blackboard.AddToMemory(dust); }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition reachingPooRememberDust = new Transition("Reaching Poo Remember Dust",
            () => { 
                dust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", blackboard.dustDetectionRadius);
                return (dust != null && poo != null);
            }, // write the condition checkeing code in {}
            () => { blackboard.AddToMemory(dust); }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );//
        Transition dustFound = new Transition("Go to dust",
            () => { dust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", blackboard.dustDetectionRadius);
                    return dust != null;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition pooCloser = new Transition("Poo Closer",
            () => {
                otherPoo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO",
                                                                   blackboard.pooDetectionRadius);
                return
                    otherPoo != null &&
                    SensingUtils.DistanceToTarget(gameObject, otherPoo)
                    < SensingUtils.DistanceToTarget(gameObject, poo);
            },
            () => { poo = otherPoo; }
        );
        /*
        Transition dustCloser = new Transition("Dust Closer",
            () => {
                otherDust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST",
                                                                   blackboard.dustDetectionRadius);
                return
                    otherDust != null &&
                    SensingUtils.DistanceToTarget(gameObject, otherDust)
                    <= SensingUtils.DistanceToTarget(gameObject, dust);
            },
            () => { dust = otherDust; }
        );*/
        Transition checkMemo = new Transition("Check Memo",
            () => { return (blackboard.somethingInMemory()) && (SensingUtils.DistanceToTarget(gameObject, poo) <= blackboard.pooReachedRadius); }, // write the condition checkeing code in {}
            () => { Destroy(poo); dust = blackboard.RetrieveFromMemory(); }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition pooRemoved = new Transition("Poo removed",
            () => { return (SensingUtils.DistanceToTarget(gameObject, poo) <= blackboard.pooReachedRadius) && !(blackboard.somethingInMemory()); }, // write the condition checkeing code in {}
            () => { Destroy(poo); }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition dustRemoved = new Transition("Dust removed",
            () => { return SensingUtils.DistanceToTarget(gameObject, dust) <= blackboard.dustReachedRadius; }, // write the condition checkeing code in {}
            () => { Destroy(dust); }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(PATROL,REACHINGPOO, REACHINGDUST);

        AddTransition(REACHINGDUST, dustRemoved, PATROL);
        AddTransition(REACHINGPOO, pooCloser, REACHINGPOO);
        AddTransition(REACHINGPOO, reachingPooRememberDust, REACHINGPOO);
        AddTransition(REACHINGDUST, pooBumpedDustTargetMemory, REACHINGPOO);
        AddTransition(PATROL, pooFound, REACHINGPOO);
        AddTransition(PATROL, dustFound, REACHINGDUST);
        AddTransition(REACHINGPOO, checkMemo, REACHINGDUST);
        AddTransition(REACHINGPOO, pooRemoved, PATROL);


        // AddTransition(REACHINGDUST, dustCloser, REACHINGDUST);
        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        initialState = PATROL;

    }
}
