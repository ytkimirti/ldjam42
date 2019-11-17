using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public Transform enemyHolder;
    public GameObject enemyPrefab;
    public float horizontalSize;
    public float verticalSize;
    [Space]
    public float spawnTime;
    public float spawnRandomChange;

    float spawnTimer;
    float spawnTimeMult;

	void Start () {
        spawnTimer = Random.Range(spawnTime - spawnRandomChange, spawnTime + spawnRandomChange);
	}
	
	void Update () {
        if (GameManager.main.gameEnded)
            return;

        spawnTime -= Time.deltaTime / 120;
        if (spawnTime - spawnRandomChange < 0)
            spawnTime = spawnRandomChange;

        spawnTimer -= Time.deltaTime;
        
        if(spawnTimer <= 0)
        {
            spawnTimeMult = GameManager.main.isBossLevel ? 2 : 1;
            
            spawnTimer = Random.Range((spawnTime * spawnTimeMult) - spawnRandomChange, (spawnTime * spawnTimeMult) + spawnRandomChange);
            spawnEnemy();
        }
	}

    public void spawnEnemy()
    {
        Vector3 pos = Vector3.zero;

        int randomNum = Random.Range(0, 4);
        
        if(randomNum == 0)
            pos = new Vector3(Random.Range(-horizontalSize, horizontalSize), verticalSize, 0);
        else if(randomNum == 1)
            pos = new Vector3(Random.Range(-horizontalSize, horizontalSize), -verticalSize, 0);
        else if (randomNum == 2)
            pos = new Vector3(horizontalSize,Random.Range(-verticalSize, verticalSize), 0);
        else if (randomNum == 3)
            pos = new Vector3(-horizontalSize, Random.Range(-verticalSize, verticalSize), 0);


        GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);

        enemy.transform.parent = enemyHolder;
    }
}
