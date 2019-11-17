using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMotor : MonoBehaviour {

    public float speed;

	void Start () {
		
	}
	
	void Update () {
        transform.position += new Vector3(Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime, 0, Input.GetAxisRaw("Vertical") * speed * Time.deltaTime);

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float angle = (pos.toVector2() - transform.position.toVector2()).Angle();

        transform.localEulerAngles = new Vector3(0, -angle + 90, 0);
    }
}
