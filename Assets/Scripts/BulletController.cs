using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class BulletController : MonoBehaviour {

    public bool isRocket;
    public SpriteRenderer sprite;

    public Sprite growingSprite;
    [Space]
    public float moveSpeed;
    public float growSpeed;
    public float growScale;
    public float growScaleRandomChange;

    public GameObject rocketParticle;
    public Color greenColor;

    bool isMoving;
    bool isGrowing;
    float growTimer;

    CircleCollider2D col;
    Rigidbody2D rb;

	void Start () {
        col = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        isMoving = true;
        isGrowing = false;

        rb.velocity = transform.right * moveSpeed;
    }

    public void beRocket()
    {
        isRocket = true;
        moveSpeed = moveSpeed / 5;
        //rocketParticle.SetActive(true);
        sprite.color = Color.white;
    }
	
	void Update () {


        if (isGrowing)
        {
            growTimer += Time.deltaTime * growSpeed;

            sprite.transform.localScale = growScale * growTimer * Vector3.one;

            if(growTimer > 1)
            {
                growTimer = 0;
                isGrowing = false;
            }
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isMoving)
            return;

        if(other.tag == "enemy")
        {
            if (isRocket)
            {
                GameManager.main.explosion(transform.position, 1.9f);
                CameraShaker.Instance.ShakeOnce(1.2f, 10, 0, 1f);
                ParticleManager.main.play(transform.position, 4);
                AudioManager.main.Play("explosion");

                die();
            }
            else
            {
                other.GetComponent<EnemyInput>().getDamage(1);
                die();
            }
        }

        if(other.tag == "environment" || other.tag == "bullet")
        {
            isMoving = false;
            isGrowing = true;
            growTimer = 0;

            tag = "bullet";
            gameObject.layer = 10;

            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;

            sprite.sprite = growingSprite;
            sprite.color = greenColor;

            col.isTrigger = false;
            col.offset = Vector2.zero;
            col.radius = 0.5f;

            if (isRocket)
                growScale = growScale * 3f;
            growScale = Random.Range(growScale - growScaleRandomChange, growScale + growScaleRandomChange);
        }
    }

    public void die()
    {
        rocketParticle.transform.parent = GameManager.main.player.bulletHolder;
        Destroy(gameObject);
    }
}
