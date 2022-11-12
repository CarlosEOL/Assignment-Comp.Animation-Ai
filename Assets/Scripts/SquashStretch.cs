using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashStretch : SphereHolder
{
    private float distance;
    private float elapsedTime;
    private float duration = 3.0f;

    private Vector3 plrScale;
    private Vector3 targetScale = new Vector3(5,5,5);

    private int i = 0;
    private void Update()
    {
        float t = elapsedTime / duration;

        InterpolationForScale(plrScale, targetScale, t);
        
        elapsedTime += Time.smoothDeltaTime;
        if (elapsedTime > duration)
        {
            elapsedTime = 0f;
            
            if (targetScale == new Vector3(5,5,5))
            {
                plrScale = targetScale;
                targetScale = new Vector3(1, 1, 1);
                print(targetScale);
            }
            else if (targetScale == new Vector3(1, 1, 1))
            {
                plrScale = targetScale;
                targetScale = new Vector3(5, 5, 5);
                print(targetScale);
            }
        }
    }

    private void Start()
    {
        plrScale = transform.localScale;
    }
}
