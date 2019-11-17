using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using DG.Tweening;

public class PlayerInput : MonoBehaviour {

    [Header("Accuracy")]

    public float currAccuracy;
    public float accuracyRegen;
    public float accuracyLose;

    [Header("Sprites")]
    [Space]
    public Sprite playerDefaultSprite;
    public Sprite playerIdleSprite;
    public Sprite playerWalkSprite;
    bool currSprite;
    public float walkAnimationSpeed;
    public float idleAnimationSpeed;
    float spriteTimer;

    [Header("Variables")]
    [Space]

    public int grenadeCount = 3;
    public int hp = 3;
    public Transform bulletHolder;
    public ParticleSystem fireParticle;
    public GameObject bulletPrefab;
    public GameObject grenadePrefab;
    public Transform armTransform;
    public SpriteRenderer armSprite;
    public SpriteRenderer sprite;
    public Transform shieldTrans;
    public Sprite rocketSprite;

    public float rifleFireRate;
    float rifleFireTimer;
    public float rocketFireRate;
    float rocketFireTimer;
    public int weaponCount;
    public int currWeapon = 3;
    int memWeapon;

    SpriteShocker shocker;

    bool isInvicible = false;
    float invicibleTimer;

    Camera cam;

    CharacterMotor motor;

	void Start () {
        shocker = GetComponent<SpriteShocker>();
        cam = Camera.main;
        motor = GetComponent<CharacterMotor>();

        grenadeCount = 3;
        GameManager.main.grenadeBar.updateGrenades(grenadeCount);
    }

    

    public void getDamage(int damage)
    {
        if (isInvicible)
            return;

        AudioManager.main.Play("hit");

        shocker.loopTime(10);
        isInvicible = true;
        invicibleTimer = 1;

        hp -= damage;

        CameraShaker.Instance.ShakeOnce(1, 10, 0, 1);

        GameManager.main.heartBar.updateHearts(hp);

        ParticleManager.main.play(transform.position, 1);
        if (hp <= 0)
            die();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "bullet")
        {
            getDamage(1);
        }
    }

    public void getShield()
    {
        shieldTrans.gameObject.SetActive(true);
        invicibleTimer = 10;
        isInvicible = true;
    }

    public void getGrenade(int amount)
    {
        grenadeCount += amount;

        GameManager.main.grenadeBar.updateGrenades(grenadeCount);


    }

    public bool getHeart(int amount)
    {
        if (hp >= 3)
            return false;

        hp += amount;

        GameManager.main.heartBar.updateHearts(hp);

        ParticleManager.main.play(transform.position, 1);

        return true;
    }

    public void die()
    {
        AudioManager.main.Play("explosion");
        GameManager.main.endGame();
        ParticleManager.main.play(transform.position, 3);
        gameObject.SetActive(false);
    }
	
    void updateSpriteAnimation(bool isWalking)
    {
        spriteTimer += Time.deltaTime;

        if (isWalking)
        {
            if(spriteTimer >= walkAnimationSpeed)
            {
                spriteTimer = 0;
                currSprite = !currSprite;

                if (currSprite)
                    sprite.sprite = playerDefaultSprite;
                else
                    sprite.sprite = playerWalkSprite;
            }
        }
        else
        {
            if (sprite.sprite == playerWalkSprite)
                sprite.sprite = playerDefaultSprite;

            if (spriteTimer >= idleAnimationSpeed)
            {
                spriteTimer = 0;
                currSprite = !currSprite;

                if (currSprite)
                    sprite.sprite = playerDefaultSprite;
                else
                    sprite.sprite = playerIdleSprite;
            }
        }
    }

    void Update() {


        if (isInvicible)
        {
            shieldTrans.Rotate(0, 0, 1000 * Time.deltaTime);
            invicibleTimer -= Time.deltaTime;
            if (invicibleTimer <= 0)
            {
                isInvicible = false;
                shieldTrans.gameObject.SetActive(false);
            }
        }


        motor.inp = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        float mouseAngle = (mousePos - (Vector2)transform.position).Angle();

        bool facingRight = mouseAngle < 90 && mouseAngle > -90;

        bool isWalking = (motor.inp.x != 0 || motor.inp.y != 0);

        updateSpriteAnimation(isWalking);

        sprite.flipX = !facingRight;
        armSprite.flipY = !facingRight;

        armTransform.eulerAngles = new Vector3(0, 0, mouseAngle);

        

        //Ateş etme
        if (currWeapon == 0) {
            currAccuracy = Mathf.Lerp(currAccuracy, 0, accuracyRegen);
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                fire(mouseAngle, accuracyLose, false);
            }
        }else if(currWeapon == 1){
            currAccuracy = Mathf.Lerp(currAccuracy, 0, accuracyRegen * 0.6f);
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                AudioManager.main.Play("explosion");
                currAccuracy += 4;

                if (currAccuracy > 90)
                    currAccuracy = 90;

                fire(mouseAngle,0,false);
                fire(mouseAngle,0, false);
                fire(mouseAngle,0, false);
                fire(mouseAngle,0, false);
                fire(mouseAngle,0, false);

                currAccuracy -= 4;
                currAccuracy += accuracyLose * 6;
            }
        }else if (currWeapon == 2)
        {
            currAccuracy = Mathf.Lerp(currAccuracy, 0, accuracyRegen * 1.3f);
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                rifleFireTimer = rifleFireRate;
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                rifleFireTimer += Time.deltaTime;
                if (rifleFireTimer > rifleFireRate)
                {
                    rifleFireTimer = 0;
                    fire(mouseAngle, accuracyLose * 0.5f, false);
                }
            }
        }else if(currWeapon == 3)
        {
            currAccuracy = Mathf.Lerp(currAccuracy, 0, accuracyRegen * 0.7f);
            rocketFireTimer += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Mouse0) && rocketFireTimer > rocketFireRate)
            {
                rocketFireTimer = 0;
                fire(mouseAngle + Random.Range(-3,3), 50, true);
                AudioManager.main.Play("explosion");
            }

        }

        if (Input.GetMouseButtonDown(1))
        {
            throwGrenade(mousePos);
        }

        //Weapon değiştirme
        memWeapon = currWeapon;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currWeapon++;
            if (currWeapon > weaponCount)
                currWeapon = 0;
        }
        else if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                currWeapon++;
            }
            else
            {
                currWeapon--;
            }

            if (currWeapon > weaponCount)
                currWeapon = 0;
            else if (currWeapon < 0)
                currWeapon = weaponCount;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            currWeapon = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            currWeapon = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            currWeapon = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            currWeapon = 3;

        if (currWeapon > weaponCount)
            currWeapon = weaponCount;

        if (memWeapon != currWeapon)
            changeWeapon();
    }

    public void changeWeapon()
    {
        if (currWeapon == 0)
            armSprite.sprite = GameManager.main.defaultWeaponSprite;
        else if (currWeapon == 1)
            armSprite.sprite = GameManager.main.pumpbubleSprite;
        else if (currWeapon == 2)
            armSprite.sprite = GameManager.main.machinebubleSprite;
        else if (currWeapon == 3)
            armSprite.sprite = GameManager.main.rocketlauncherSprite;
    }

    void throwGrenade(Vector2 mousePos)
    {
        grenadeCount--;
        if (grenadeCount < 0)
        {
            grenadeCount = 0;
            return;
        }

        GameManager.main.grenadeBar.updateGrenades(grenadeCount);

        Vector2 vec = mousePos - (Vector2)transform.position;

        vec = Vector2.ClampMagnitude(vec, 4);

        AudioManager.main.Play("fire");

        GameObject grenade = Instantiate(grenadePrefab, transform.position, Quaternion.identity);

        grenade.transform.DOMove((Vector2)transform.position + vec, 0.3f).SetEase(Ease.OutQuad);
    }

    void fire(float angle,float accLose,bool isRocket)
    {
        AudioManager.main.Play("fire");

        float newAngle = angle + Random.Range(-currAccuracy, currAccuracy);

        currAccuracy += accLose;

        CameraShaker.Instance.ShakeOnce(1, 10, 0, 0.5f);

        fireParticle.Play();

        //Ateş edince knockback efekti vermek için kolu bir piksel geri alıyoruz
        armSprite.transform.localPosition = new Vector3(0, 0, 0);

        //Kol eski konumuna geri gelsin diye 0.1 saniyelik invoke koyuyoruz, o da geri getiriyor
        Invoke("getArmDefPosition", 0.1f);

        //Mermi doğar (Pooling yok)
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, newAngle));

        GameManager.main.currBullets.Add(bullet.transform);

        bullet.transform.parent = bulletHolder;

        if (isRocket)
        {
            BulletController bulletScript = bullet.GetComponent<BulletController>();

            bulletScript.beRocket();
            bulletScript.sprite.sprite = rocketSprite;
        }
    }

    void getArmDefPosition()
    {
        armSprite.transform.localPosition = new Vector3(0.1f, 0, 0);
    }
}
