using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_ROOMBA", menuName = "Finite State Machines/FSM_ROOMBA", order = 1)]
public class FSM_ROOMBA : FiniteStateMachine
{

    
  //  private ROOMBA_Blackboard blackboard;
    //private GameObject target;
    

    public override void OnEnter()
    {

  //      blackboard = GetComponent<ROOMBA_Blackboard>();
        


        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {


        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
/*
        State ToPoint = new State ("GoingToPoint",
        
            () => { },
            () => { },
            () => { }
        );


        Transition changePoint = new Transition("ChangePoint",
            () => { },
            () => { }
        );

        AddStates (ToPoint);

        AddTransition(ToPoint, changePoint, ToPoint);


        initialState = ToPoint;
*/
    }
}