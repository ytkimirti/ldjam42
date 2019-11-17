using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GrenadeBarController : MonoBehaviour
{

    public SpriteRenderer[] grenades;
    public Vector2 offset;
    public float shakeMagnitude;

    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        transform.position = offset + (Vector2)cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
    }

    public void updateGrenades(int nades)
    {
        for (int i = 0; i < grenades.Length; i++)
        {
            grenades[i].gameObject.SetActive(i < nades);

            grenades[i].transform.DOShakePosition(0.1f, shakeMagnitude);
        }
    }
}
