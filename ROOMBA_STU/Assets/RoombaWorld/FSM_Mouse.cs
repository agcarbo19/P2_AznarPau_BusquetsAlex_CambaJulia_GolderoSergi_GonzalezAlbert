using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Mouse", menuName = "Finite State Machines/FSM_Mouse", order = 1)]
public class FSM_Mouse : FiniteStateMachine
{
    private MOUSE_Blackboard mouseBlackboard;
    private RandomLocationGenerator randomLocationGenerator;


    public override void OnEnter()
    {   
        mouseBlackboard = GetComponent<MOUSE_Blackboard>();
        randomLocationGenerator = GetComponent<RandomLocationGenerator>();

        base.OnEnter();
    }

    public override void OnExit()
    {        
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        //GoingToRandomLocation, Pooping, GoingToExitLocation, Scared

        State GoingToRandomLocation = new State("GoingToRandomLocation",
            () => { },
            () => { },
            () => { }
            );
    }
}