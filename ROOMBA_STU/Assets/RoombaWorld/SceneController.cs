using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public GameObject dustParticle;
    private float maxDustSpawnTimer = 5f;
    private float dustSpawnTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        dustSpawnTimer += Time.deltaTime;
        if (dustSpawnTimer >= maxDustSpawnTimer){
            dustSpawn();

        }
    }

    public void dustSpawn()
    {
        Instantiate(dustParticle, RandomLocationGenerator.RandomWalkableLocation(), Quaternion.identity);
        dustSpawnTimer = 0;
    }
}
