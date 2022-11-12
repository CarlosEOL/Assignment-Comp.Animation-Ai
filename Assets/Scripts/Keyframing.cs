using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Keyframing : SphereHolder
{
    private float distance;
    private Vector3 playerPos;
    private void Update()
    {
        distance = Vector3.Distance(transform.position, SpherePosition[0].position);

        transform.position = Lerp(playerPos, SpherePosition[0].position, tt);
    }

    void Awake()
    {
        playerPos = transform.position;
    }
}
