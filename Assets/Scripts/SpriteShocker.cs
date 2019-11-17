using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteShocker : MonoBehaviour {

    public SpriteRenderer sprite;
    public float shockTime = 0.1f;
    public Color shockColor = Color.red;
    Color defColor;
    float timer;

	void Start () {
        defColor = sprite.color;
	}
	
	void Update () {
        timer += Time.deltaTime;


        if (timer > shockTime)
            sprite.color = defColor;
	}

    public void loopTime(int times)
    {
        StartCoroutine(loopEnum(times));
    }

    IEnumerator loopEnum(int times)
    {
        for (int i = 0; i < times / 2; i++)
        {
            shock();
            yield return new WaitForSeconds(shockTime * 2);
        }
    }

    public void shock()
    {
        timer = 0;
        sprite.color = shockColor;
    }
}
