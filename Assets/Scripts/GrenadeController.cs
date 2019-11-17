using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class GrenadeController : MonoBehaviour {

    public SpriteRenderer sprite;
    public Sprite onSprite;
    Sprite offSprite;
    public float radius;
    public float blinkSpeed = 0.1f;
    float blinkTimer;
    public float explodeTime = 2f;
    float explodeTimer;
    bool isOn;

	void Start () {
		offSprite = sprite.sprite;
	}
	
	void Update () {
        explodeTimer += Time.deltaTime;
        blinkTimer += Time.deltaTime;

        if(blinkTimer > blinkSpeed)
        {
            blinkTimer = 0;
            isOn = !isOn;

            sprite.sprite = isOn ? onSprite : offSprite;
        }

        if(explodeTimer > explodeTime)
        {
            explodeTimer = 0;
            explode();
        }
	}

    public void explode()
    {
        CameraShaker.Instance.ShakeOnce(1.2f, 10, 0, 1f);
        ParticleManager.main.play(transform.position,4);
        AudioManager.main.Play("explosion");

        GameManager.main.explosion(transform.position, radius);
        Destroy(gameObject);
    }
}
