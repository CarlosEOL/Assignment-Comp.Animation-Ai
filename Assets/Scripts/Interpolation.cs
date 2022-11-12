using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Interpolation : SphereHolder
{
    private float distance;
    private float elapsedTime;
    private float duration = 3.0f;

    private int i = 0;
    private void Update()
    {
        float t = elapsedTime / duration;
        
        distance = Vector3.Distance(plrPos, SpherePosition[i].position);

        InterpolationMethods(plrPos, SpherePosition[i].position, t);
        
        elapsedTime += Time.smoothDeltaTime;
        if (elapsedTime > duration)
        {
            plrPos = SpherePosition[i].position;
            elapsedTime = 0f;
            i++;

            if (i >= SpherePosition.Length)
            {
                i = 0;
            }
        }
    }

    private void Start()
    {
        plrPos = transform.position;
    }
}
