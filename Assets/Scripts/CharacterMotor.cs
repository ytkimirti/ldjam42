using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotor : MonoBehaviour {

    public float moveSpeed;
    public float maxSpeed;

    Rigidbody2D rb;
    public Vector2 inp;

	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate () {
        

        rb.AddForce(inp * moveSpeed);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
	}
}
