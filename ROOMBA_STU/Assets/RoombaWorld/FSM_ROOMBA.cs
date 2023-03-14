using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_ROOMBA", menuName = "Finite State Machines/FSM_ROOMBA", order = 1)]
public class FSM_ROOMBA : FiniteStateMachine
{


    private ROOMBA_Blackboard blackboard;
    private GameObject target;
    private GoToTarget goToTarget;
    private GameObject poo;
    private GameObject dust;
    private GameObject chargingStation;


    public override void OnEnter()
    {

        blackboard = GetComponent<ROOMBA_Blackboard>();
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


        State Patrolling = new State("Patrolling",

            () =>
            {
                target.transform.position = RandomLocationGenerator.RandomPatrolLocation() ;
                goToTarget.target = target;
                Debug.Log(RandomLocationGenerator.RandomPatrolLocation());
                // goToTarget.enabled = true;
                Debug.Log("sigue");

                //  Debug.Log(target);
                //Debug.Log(target.transform.position);    

                goToTarget.enabled = true;

                Debug.Log("llega");
            },
            () => { },
            () => { }
            // RandomLocationGenerator.RandomPatrolLocation()
            );
 

        State GoingToPoo = new State("GoingToPoo",

           () =>
           {
               if (dust !=null)
               {
                   blackboard.AddToMemory(dust);
               }

               goToTarget.target = poo;
           },
           () => { },
           () => { }
           
           );


        State GoingToDust = new State("GoingToDust",

           () =>
           {
               goToTarget.target = dust;
           },
           () => { },
           () => { }
          
           );

        State RemoveDust = new State("RemoveDust",

          () =>
          {
              Destroy(dust);
          },
          () => { },
          () => { }

          );

        State RemovePoo  = new State("RemovePoo",

          () =>
          {
              Destroy(poo);
          },
          () => { },
          () => { }

          );

        State LowBattery  = new State("LowBattery",

          () =>
          {
              goToTarget.target = chargingStation;
          },
          () => { },
          () => { }

          );

        State ChargingPointReached = new State("ChargingPointReached",

         () =>
         {   
         },
         () => { blackboard.Recharge(blackboard.energyRechargePerSecond); },
         () => { }

         );



         



        Transition ReachingPoo = new Transition("ReachingPoo",
            () => { poo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", blackboard.pooDetectionRadius);
                return poo != null;

            },
            () => { }
        );

        Transition ReachingDust = new Transition("ReachingDust",
            () => {
                dust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", blackboard.dustDetectionRadius);
                return dust != null;

            },
            () => { }
        );

        Transition PooReached = new Transition("PooReached",
          () => {
              return SensingUtils.DistanceToTarget(gameObject, poo) <= blackboard.pooReachedRadius;

          },
          () => { }
      );



        Transition DustReached = new Transition("DustReached",
         () => {
             return SensingUtils.DistanceToTarget(gameObject, dust) <= blackboard.dustReachedRadius;

         },
         () => { }
     );




        Transition DustInMemory = new Transition("DustInMemory",
            () => {
                dust = blackboard.RetrieveFromMemory();
                return blackboard.somethingInMemory() == true;
            },
            () => { }
        );

        Transition IsDicharged = new Transition("IsDischarged",
            () => {
                return blackboard.currentCharge <= blackboard.minCharge;
            },
            () => { }
        );

        Transition IsRecharged = new Transition("IsRecharged",
            () => {
                return blackboard.currentCharge >  blackboard.maxCharge;
            },
            () => { }
        );

        Transition IsInChargingStation = new Transition("IsInChargingStation",
            () => {
                return SensingUtils.DistanceToTarget(gameObject, chargingStation) <  blackboard.chargingStationReachedRadius ;
            },
            () => { }
        );




        AddStates(Patrolling, GoingToPoo, GoingToDust,RemoveDust, RemovePoo, LowBattery, ChargingPointReached);

        AddTransition(Patrolling, ReachingPoo, GoingToPoo);
        AddTransition(GoingToPoo, DustInMemory, GoingToDust);
        AddTransition(GoingToDust, ReachingPoo, GoingToPoo);
        AddTransition(GoingToPoo, PooReached, RemovePoo);
        AddTransition(Patrolling, ReachingDust, GoingToDust);
        AddTransition(GoingToDust, DustReached, RemoveDust);
        AddTransition(Patrolling, IsDicharged,LowBattery);
        AddTransition(LowBattery, IsInChargingStation, ChargingPointReached);
        AddTransition(ChargingPointReached, IsRecharged, Patrolling);
        AddTransition(GoingToPoo, IsDicharged, LowBattery);
        AddTransition(GoingToDust, IsDicharged, LowBattery);
        AddTransition(RemovePoo, PooReached, RemovePoo);

        initialState = Patrolling;
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