using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInput : MonoBehaviour {

    public bool isKnight;
    public bool isBoss;
    int skinID;
    public Sprite[] idleSprites;
    public Sprite[] walkSprites;
    public float walkSpeed;
    float walkTimer;
    [Space]
    public SpriteRenderer sprite;
    public int hp = 3;

    Transform target;
    CharacterMotor motor;
    SpriteShocker shocker;

	void Start () {
        motor = GetComponent<CharacterMotor>();
        target = GameManager.main.player.transform;
        shocker = GetComponent<SpriteShocker>();


        if (!isBoss)
        {
            GameManager.main.currEnemy.Add(this);
            skinID = Random.Range(0, idleSprites.Length);
        }
    }

    public void getDamage(int damage)
    {

        GameManager.main.addScore(15);
        AudioManager.main.Play("hit");

        hp -= damage;
        shocker.shock();

        if(isBoss || isKnight)
            ParticleManager.main.play(transform.position, 5);
        else
            ParticleManager.main.play(transform.position, 0);
        if (hp <= 0)
            die();

        if(isBoss && hp <= 10)
        {
            skinID = 1;
        }
    }

    public void respawnBoss()
    {
        hp = 26;
        skinID = 0;
    }

    public void die()
    {
        GameManager.main.addScore(125);

        AudioManager.main.Play("explosion");


        if (isBoss)
            ParticleManager.main.play(transform.position, 6);
        else
            ParticleManager.main.play(transform.position, 2);
        hp = 0;

        if (!isBoss)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerInput>().getDamage(1);
        }
    }
	
	void Update () {
        if(GameManager.main.gameEnded || target == null)
        {
            sprite.sprite = idleSprites[skinID];
            motor.inp = Vector2.zero;
            return;
        }

        motor.inp = (target.transform.position - transform.position).normalized;

        if (motor.inp.x != 0)
            sprite.flipX = motor.inp.x < 0;


        walkTimer += Time.deltaTime;
        if(walkTimer > walkSpeed)
        {
            walkTimer = 0;
            if (sprite.sprite == idleSprites[skinID])
                sprite.sprite = walkSprites[skinID];
            else
                sprite.sprite = idleSprites[skinID];
        }
    }
}
