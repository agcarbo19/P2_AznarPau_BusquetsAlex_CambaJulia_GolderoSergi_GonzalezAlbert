using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_mouseMain", menuName = "Finite State Machines/FSM_mouseMain", order = 1)]
public class FSM_mouseMain : FiniteStateMachine
{
    private mouseBlackboard blackboard;

    private GoToTarget goToTarget;          //Genera un path
    private PathFollowing pathFollowing;    //Sigue un path
    private GameObject pooPoint;
    private GameObject poo;

    public override void OnEnter()
    {
        blackboard = GetComponent<mouseBlackboard>();
        goToTarget = GetComponent<GoToTarget>();
        pathFollowing = GetComponent<PathFollowing>();
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        //GoingToRandomLocation, Pooping, GoingToExitLocation, Scared

        State GOINGTOPOO = new State("GOINGTOPOO",
            () => {
                poo = Instantiate(blackboard.pooTarget, RandomLocationGenerator.RandomWalkableLocation(), Quaternion.identity);
                //pooPoint = poo;
                goToTarget.target = poo;
               // goToTarget.enabled = true;
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
        State POOPING = new State("POOPING",
            () => {
                Destroy(poo);
                Instantiate(blackboard.pooPrefab, this.transform.position, Quaternion.identity);
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => {
                

            }  // write on exit logic inisde {}  
        );
        State LEAVING = new State("LEAVING",
            () => {
                goToTarget.target = blackboard.hideout;
                goToTarget.enabled = true;
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { goToTarget.enabled = false; }  // write on exit logic inisde {}  
        );
        State GONE = new State("GONE",
            () => {
                Destroy(this.gameObject);
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { goToTarget.enabled = false; }  // write on exit logic inisde {}  
        );
         Transition pooPointChosen = new Transition("Poo point chosen",
             () => {

                 return poo != null;
             }, // write the condition checkeing code in {}
             () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
         );
        Transition pooPointReached = new Transition("Poo point reached",
             () => {

                 return
                 SensingUtils.DistanceToTarget(gameObject, poo) <= blackboard.pooPlaceRadius; ;
             }, // write the condition checkeing code in {}
             () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
         );
        Transition lookingForHideout = new Transition("Hideout reached",
             () => {

                 return
                 true ;
             }, // write the condition checkeing code in {}
             () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
         );
        Transition hideoutReached = new Transition("Hideout reached",
             () => {

                 return
                 SensingUtils.DistanceToTarget(gameObject, blackboard.hideout) <= blackboard.hideoutReachedRadius; ;
             }, // write the condition checkeing code in {}
             () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
         );
        AddStates(GOINGTOPOO, REACHING, POOPING, LEAVING, GONE);
        AddTransition(GOINGTOPOO, pooPointChosen, REACHING);
        AddTransition(REACHING, pooPointReached, POOPING);
        AddTransition(POOPING, lookingForHideout, LEAVING);
        AddTransition(LEAVING, hideoutReached, GONE);

        initialState = GOINGTOPOO;
    }
}
