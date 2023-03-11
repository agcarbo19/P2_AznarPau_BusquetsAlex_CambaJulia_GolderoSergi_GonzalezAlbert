using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public GameObject dustParticle;
    private float maxDustSpawnTimer = 5f;
    private float dustSpawnTimer = 0f;

    /* private Random random1 = new Random();
    private Random random2 = new Random();
    private Random random3 = new Random();
    */
    // Start is called before the first frame update
    void Start()
    {
        /*
        int n1 = random1.Next(256);
        int n2 = random2.Next(256);
        int n3 = random3.Next(256);
        */
    }
    


    // Update is called once per frame
    void Update()
    {
        dustSpawnTimer += Time.deltaTime;
        if (dustSpawnTimer >= maxDustSpawnTimer){
            //GetComponent<SpriteRenderer>().color = new Color(n1/256, n2/256, n3/256);
            dustSpawn();
        }
    }

    public void dustSpawn()
    {
        Instantiate(dustParticle, RandomLocationGenerator.RandomWalkableLocation(), Quaternion.identity);
        dustSpawnTimer = 0;
    }
}
