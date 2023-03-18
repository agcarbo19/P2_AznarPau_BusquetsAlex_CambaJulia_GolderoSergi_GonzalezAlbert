using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_mouseMain", menuName = "Finite State Machines/FSM_mouseMain", order = 1)]
public class FSM_mouseMain : FiniteStateMachine
{
    private mouseBlackboard blackboard;

    private GoToTarget goToTarget;
    private GameObject pooPoint;
    private GameObject poo;

    public override void OnEnter()
    {
        blackboard = GetComponent<mouseBlackboard>();
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


        State GOINGTOPOO = new State("GOINGTOPOO",
            () =>
            {
                poo = Instantiate(blackboard.pooTarget, RandomLocationGenerator.RandomWalkableLocation(), Quaternion.identity);

                goToTarget.target = poo;

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
        State POOPING = new State("POOPING",
            () =>
            {
                Destroy(poo);
                Instantiate(blackboard.pooPrefab, this.transform.position, Quaternion.identity);
            },
            () => { },
            () =>
            {


            }
        );
        State LEAVING = new State("LEAVING",
            () =>
            {
                goToTarget.target = blackboard.hideout;
                goToTarget.enabled = true;
            },
            () => { },
            () => { goToTarget.enabled = false; }
        );
        State GONE = new State("GONE",
            () =>
            {
                Destroy(this.gameObject);
            },
            () => { },
            () => { goToTarget.enabled = false; }
        );
        Transition pooPointChosen = new Transition("Poo point chosen",
            () =>
            {

                return poo != null;
            },
            () => { }
        );
        Transition pooPointReached = new Transition("Poo point reached",
             () =>
             {

                 return
                 SensingUtils.DistanceToTarget(gameObject, poo) <= blackboard.pooPlaceRadius; ;
             },
             () => { }
         );
        Transition lookingForHideout = new Transition("Hideout reached",
             () =>
             {

                 return
                 true;
             },
             () => { }
         );
        Transition hideoutReached = new Transition("Hideout reached",
             () =>
             {

                 return
                 SensingUtils.DistanceToTarget(gameObject, blackboard.hideout) <= blackboard.hideoutReachedRadius; ;
             },
             () => { }
         );
        AddStates(GOINGTOPOO, REACHING, POOPING, LEAVING, GONE);
        AddTransition(GOINGTOPOO, pooPointChosen, REACHING);
        AddTransition(REACHING, pooPointReached, POOPING);
        AddTransition(POOPING, lookingForHideout, LEAVING);
        AddTransition(LEAVING, hideoutReached, GONE);

        initialState = GOINGTOPOO;
    }
}
