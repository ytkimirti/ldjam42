using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HeartBarController : MonoBehaviour {

    public SpriteRenderer[] hearts;
    public Sprite heartSprite;
    public Sprite emptyHeartSprite;
    public Vector2 offset;
    public float shakeMagnitude;

    Camera cam;

	void Start () {
        cam = Camera.main;
	}
	
	void Update () {
        transform.position = offset + (Vector2)cam.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
	}

    public void updateHearts(int lifes)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = i < lifes ? heartSprite : emptyHeartSprite;
            hearts[i].transform.DOShakePosition(0.1f,shakeMagnitude);
        }
    }
}
