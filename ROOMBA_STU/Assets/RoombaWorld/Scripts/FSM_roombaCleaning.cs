using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_roombaCleaning", menuName = "Finite State Machines/FSM_roombaCleaning", order = 1)]
public class FSM_roombaCleaning : FiniteStateMachine
{

    private roombaBlackboard blackboard;

    private GoToTarget goToTarget;          
    private GameObject otherPoo;
    private GameObject poo;
    private GameObject dust;

    private SteeringContext steeringContext;

    public override void OnEnter()
    {
        blackboard = GetComponent<roombaBlackboard>();
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

        FiniteStateMachine PATROL = ScriptableObject.CreateInstance<FSM_roombaPatrol>();
        PATROL.Name = "PATROL";

        State REACHINGPOO = new State("reachingPoo",
            () =>
            {
                goToTarget.target = poo;
                goToTarget.enabled = true; steeringContext.maxSpeed *= blackboard.speedBooster;
                steeringContext.maxAcceleration *= blackboard.accBooster;

            },  
            () => { },  
            () =>
            {
                goToTarget.target = null;
                goToTarget.enabled = false;
                steeringContext.maxSpeed /= blackboard.speedBooster;
                steeringContext.maxAcceleration /= blackboard.accBooster;
            }   
        );
        State REACHINGDUST = new State("reachingDust",
            () =>
            {
                goToTarget.target = dust;
                goToTarget.enabled = true;
            },  
            () => { }, 
            () =>
            {
                goToTarget.target = null;
                goToTarget.enabled = false;
            }   
        );


        /* STAGE 2: create the transitions with their logic(s)
         * --------------------------------------------------- 
        */

        Transition pooFound = new Transition("goToPoo",
            () =>
            {
                poo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", blackboard.pooDetectionRadius);
                return poo != null;
            }, 
            () => { }   
        );

        Transition pooBumpedDustTargetMemory = new Transition("PooBumpedDustTargetMemory",
            () =>
            {
                poo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", blackboard.pooDetectionRadius);
                return (poo != null && dust != null);
            },  
            () => { blackboard.AddToMemory(dust); }  
        );

        Transition reachingPooRememberDust = new Transition("Reaching Poo Remember Dust",
            () =>
            {
                dust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", blackboard.dustDetectionRadius);
                return (dust != null && poo != null);
            },  
            () => { blackboard.AddToMemory(dust); }  
        ); 

        Transition dustFound = new Transition("Go to dust",
            () =>
            {
                dust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", blackboard.dustDetectionRadius);
                return dust != null;
            },  
            () => { }   
        );

        Transition pooCloser = new Transition("Poo Closer",
            () =>
            {
                otherPoo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", blackboard.pooDetectionRadius);

                return
                    otherPoo != null && SensingUtils.DistanceToTarget(gameObject, otherPoo) < SensingUtils.DistanceToTarget(gameObject, poo);
            },
            () => { poo = otherPoo; }
        );

        Transition checkMemo = new Transition("Check Memo",
            () => { return  blackboard.somethingInMemory() && (SensingUtils.DistanceToTarget(gameObject, poo) <= blackboard.pooReachedRadius); },  
            () => { Destroy(poo); dust = blackboard.RetrieveFromMemory(); }  
        );

        Transition pooRemoved = new Transition("Poo removed",
            () => { return (SensingUtils.DistanceToTarget(gameObject, poo) <= blackboard.pooReachedRadius) && !(blackboard.somethingInMemory()); }, 
            () => { Destroy(poo); }   
        );

        Transition dustRemoved = new Transition("Dust removed",
            () => { return SensingUtils.DistanceToTarget(gameObject, dust) <= blackboard.dustReachedRadius; },  
            () => { Destroy(dust); }   
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ---------------------------------------------- 
         */

        AddStates(PATROL, REACHINGPOO, REACHINGDUST);

        AddTransition(REACHINGDUST, dustRemoved, PATROL);
        AddTransition(REACHINGPOO, pooCloser, REACHINGPOO);
        AddTransition(REACHINGPOO, reachingPooRememberDust, REACHINGPOO);
        AddTransition(REACHINGDUST, pooBumpedDustTargetMemory, REACHINGPOO);
        AddTransition(PATROL, pooFound, REACHINGPOO);
        AddTransition(PATROL, dustFound, REACHINGDUST);
        AddTransition(REACHINGPOO, checkMemo, REACHINGDUST);
        AddTransition(REACHINGPOO, pooRemoved, PATROL);

        /* STAGE 4: set the initial state 
         */
        initialState = PATROL;

    }
}
