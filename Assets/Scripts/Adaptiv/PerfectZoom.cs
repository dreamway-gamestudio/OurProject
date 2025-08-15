using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfectZoom : MonoBehaviour
{
    public SpriteRenderer rink;
    void Update()
    {
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = rink.bounds.size.x / rink.bounds.size.y;

        if(screenRatio >= targetRatio)
        {
             Camera.main.orthographicSize = targetRatio / screenRatio;
        }
        else 
        {
            float diffrenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = rink.bounds.size.y / 2 * diffrenceInSize;
        }
    }
}
