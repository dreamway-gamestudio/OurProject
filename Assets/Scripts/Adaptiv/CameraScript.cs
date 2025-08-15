using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public SpriteRenderer AdaptiveScreen;

    void Update()
    {
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = AdaptiveScreen.bounds.size.x / AdaptiveScreen.bounds.size.y;
        
        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = AdaptiveScreen.bounds.size.y * 0.5f;
            //Camera.main.orthographicSize = Line.bounds.size.y * 0.7f;
        }
        else
        {
            float diferenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = AdaptiveScreen.bounds.size.y * 0.5f * diferenceInSize;
            //Camera.main.orthographicSize = Field.bounds.size.y * 0.3f * diferenceInSize;
        }
    }
}
