using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenController : MonoBehaviour {

    public bool isWeapon = false;
    public float spawnTime;
    public float spawnTimeChange;
    float spawnTimer;
    [Space]
    public float sinHeight;
    public float sinSpeed;
    [Space]
    public string type;
    Vector3 pos;
    [Space]
    public Vector2 offset;
    SpriteRenderer sprite;
    bool isOn = false;

	void Start () {
        sprite = GetComponent<SpriteRenderer>();
        spawnTimer = 5;
    }
	
	void Update () {

        transform.position = pos + (Vector3.up * Mathf.Sin(Time.time * sinSpeed) * sinHeight);

        if (isWeapon)
            return;

        spawnTimer -= Time.deltaTime;
        if(spawnTimer <= 0)
        {
            spawnTimer = Random.Range(spawnTime - spawnTimeChange, spawnTime + spawnTimeChange);
            respawn();
        }
        
	}

    public void respawn()
    {
        sprite.enabled = true;

        if(!isWeapon)
            transform.position = new Vector3(Random.Range(-offset.x, offset.x), Random.Range(-offset.y, offset.y), 0);

        pos = transform.position;
        isOn = true;

        if(!isWeapon)
            pickType();
    }

    public void pickType()
    {
        if (!isWeapon)
        {
            int randomNum = Random.Range(0, 4);

            if (randomNum == 0)
                type = "heart";
            else if (randomNum == 1)
                type = "saw";
            else if (randomNum == 2)
                type = "grenade";
            else if (randomNum == 3)
                type = "shield";

            if (type == "heart")
                sprite.sprite = GameManager.main.heartTokenSprite;
            if (type == "saw")
                sprite.sprite = GameManager.main.sawTokenSprite;
            if (type == "grenade")
                sprite.sprite = GameManager.main.grenadeTokenSprite;
            if (type == "shield")
                sprite.sprite = GameManager.main.shieldTokenSprite;
        }
        else
        {
            if (type == "pumpbuble")
                sprite.sprite = GameManager.main.pumpbubleSprite;
            if (type == "machinebuble")
                sprite.sprite = GameManager.main.machinebubleSprite;
            if (type == "rocketlauncher")
                sprite.sprite = GameManager.main.rocketlauncherSprite;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!isOn)
            return;

        if(col.gameObject.tag == "Player")
        {
            if (type == "heart")
            {
                if(GameManager.main.player.getHeart(1))
                    collect();
            }
            else if (type == "saw")
            {
                GameManager.main.shaveBullets(GameManager.main.shaveAmount);
                collect();
            }
            else if (type == "grenade")
            {
                GameManager.main.player.getGrenade(2);
                collect();
            }else if(type == "shield")
            {
                GameManager.main.player.getShield();
                collect();
            }


            if (isWeapon)
            {
                if (type == "pumpbuble")
                {
                    print("picking up " + type);
                }
                else if (type == "machinebuble")
                {
                    print("picking up" + type);
                }
                else if (type == "rocket launcher")
                {
                    print("picking up " + type);
                }

                collect();
            }
        }
    }

    void collect()
    {
        if (isWeapon)
        {
            GameManager.main.player.weaponCount++;

            if (GameManager.main.player.weaponCount > 3)
                GameManager.main.player.weaponCount = 3;


            GameManager.main.player.currWeapon = GameManager.main.player.weaponCount;
            GameManager.main.player.changeWeapon();
        }

        AudioManager.main.Play("pickup");
        GameManager.main.addScore(175);
        sprite.enabled = false;
        spawnTimer = Random.Range(spawnTime - spawnTimeChange, spawnTime + spawnTimeChange);
        isOn = false;
    }
}
