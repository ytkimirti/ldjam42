using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour {

    public float turnSpeed;
    public float lerpSpeed;
    [Space]
    public float accuracyOverDistance;
    public float playerAccuracyMult;

    PlayerInput player;
    Camera cam;

	void Start () {
        player = GameManager.main.player;
        cam = Camera.main;
    }
	
	void Update () {
        transform.Rotate(0, 0, turnSpeed * Time.deltaTime);

        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = Vector2.Lerp(transform.position, mousePos, lerpSpeed * Time.deltaTime);

        transform.localScale = Vector3.one * (player.currAccuracy / playerAccuracyMult) * accuracyOverDistance * Vector2.Distance(transform.position, player.transform.position);

        if (transform.localScale.x > 1)
            transform.localScale = Vector3.one;
    }
}
