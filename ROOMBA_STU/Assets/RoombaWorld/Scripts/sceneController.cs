using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class sceneController : MonoBehaviour
{
    public GameObject dustParticle;
    public GameObject micePrefab;
    public mouseBlackboard mBlackboard;

    private float maxDustSpawnTimer = 5f;
    private float dustSpawnTimer = 0f;
    private float maxRandomMiceSpawnTimer = 50f;
    private float minRandomMiceSpawnTimer = 10f;
    private float randomMiceSpawnTimer;
    private float miceSpawnTimer = 0f;

    public GameObject[] entryAndExitPoints;
    public float random;


    // Start is called before the first frame update
    void Start()
    {
        entryAndExitPoints = GameObject.FindGameObjectsWithTag("EXIT");

        randomMiceSpawnTimer = Random.Range(minRandomMiceSpawnTimer, maxRandomMiceSpawnTimer);
        mBlackboard = micePrefab.GetComponent<mouseBlackboard>();
    }



    // Update is called once per frame
    void Update()
    {
        dustSpawnTimer += Time.deltaTime;
        if (dustSpawnTimer >= maxDustSpawnTimer)
        {
            DustSpawn();
        }

        miceSpawnTimer += Time.deltaTime;
        if (miceSpawnTimer >= randomMiceSpawnTimer)
        {
            MiceSpawner();
        }

    }



    public void MiceSpawner()
    {
        Instantiate(micePrefab, entryAndExitPoints[Random.Range(0, entryAndExitPoints.Length - 1)].transform.position, Quaternion.identity);
        miceSpawnTimer = 0;
        randomMiceSpawnTimer = Random.Range(minRandomMiceSpawnTimer, maxRandomMiceSpawnTimer);

    }


    public void DustSpawn()
    {
        dustParticle.GetComponent<SpriteRenderer>().color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        Instantiate(dustParticle, RandomLocationGenerator.RandomWalkableLocation(), Quaternion.identity);
        dustSpawnTimer = 0;
    }
}
