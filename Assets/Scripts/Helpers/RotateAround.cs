using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{

    [SerializeField] private GameObject target;
    [SerializeField] private float degreesPerSecond = 45;
    [SerializeField] private float maxRandomness = 1.0f;
    [SerializeField] private bool sideWays = false;

    // Update is called once per frame
    void Update()
    {
        maxRandomness = maxRandomness > 0 ? maxRandomness : 1;
        var speedRandomness = (float)(Random.Range(1, maxRandomness));
        Debug.Log(speedRandomness);
        transform.RotateAround(target.transform.position, sideWays ?  Vector3.forward : Vector3.right, (degreesPerSecond + speedRandomness) * Time.deltaTime);
    }
}