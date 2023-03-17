using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseBlackboard : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject[] exitPoints;
    public GameObject[] wayPoints;
    public GameObject pooPrefab;
    public GameObject hideout;
    public GameObject pooTarget;
    public float roombaDetectionRadius = 50;
    public float pooPlaceRadius = 15; // reachability radius
    public float hideoutReachedRadius = 15; // reachability radius

    public float speedBooster = 2f;
    public float accBooster = 4f;
    void Awake()
    {
        // let's get all the exit&entry points
        wayPoints = GameObject.FindGameObjectsWithTag("PATROLPOINT");
        exitPoints = GameObject.FindGameObjectsWithTag("EXIT");
        pooPrefab = Resources.Load<GameObject>("POO");
        hideout = GetRandomExitPoint();

    }

    public GameObject GetRandomExitPoint()
    {
        return exitPoints[Random.Range(0, exitPoints.Length)];
    }
    public GameObject GetRandomPooPoint()
    {
        return wayPoints[Random.Range(0, wayPoints.Length)];
    }

    public GameObject GetClosestExitPoint()
    {
        GameObject closestExit = null;
        for (int i = 0; i < exitPoints.Length-1; i++)
        {
            if (closestExit == null)
                closestExit = exitPoints[i];

            if (SensingUtils.DistanceToTarget(gameObject, exitPoints[i]) < SensingUtils.DistanceToTarget(gameObject, closestExit))
                closestExit = exitPoints[i];

        }
        return closestExit;
    }
}
