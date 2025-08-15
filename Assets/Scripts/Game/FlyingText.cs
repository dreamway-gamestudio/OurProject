using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingText : MonoBehaviour
{
    public Vector3 RandomizeIntensity = new Vector3(0.5f, 0, 0);
    void Start()
    {
        Destroy(gameObject, 1.5f);
        transform.localPosition += new Vector3(Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x),
        Random.Range(-RandomizeIntensity.y, RandomizeIntensity.y));
    }
}
