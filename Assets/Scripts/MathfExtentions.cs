using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathfExtentions : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		
	}
}

public static class ArrayExtension
{
    public static T[] ShuffleArray<T>(this T[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            T tmp = arr[i];
            arr[i] = arr[r];
            arr[r] = tmp;
        }
        return arr;
    }
}

public static class FloatExtension
{
    public static float increment(this float a, float b, float speed)
    {
        if (a > b)
        {
            a -= speed;
            if (a < b)
                a = b;
        }
        if (a < b)
        {
            a += speed;
            if (a > b)
                a = b;
        }

        return a;
    }

    public static Vector2 toVector(this float angle)
    {
        return new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}

public static class Vector2Extension
{
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }

    public static float Angle(this Vector2 p_vector2)
    {
        if (p_vector2.x < 0)
        {
            return (360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg)) + 90;
        }
        else
        {
            return (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1) + 90;
        }
    }

    public static Vector2 toVector2(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector3 toVector3(this Vector2 v)
    {
        return new Vector3(v.x,0, v.y);
    }

    public static float increase(this float a, float b, float speed)
    {
        float change = b - a;

        if (change == 0)
            return a;

        if (change > 0)
            change = speed;
        else
            change = -speed;

        return a + change;
    }
}
