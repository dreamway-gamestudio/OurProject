using UnityEngine;
public class ScreenSize : MonoBehaviour
{
    [SerializeField]
    public static float staticSize;
    public float Size;
    void Awake()
    {
        staticSize = Size;
    }
    void Update()
    {
        float width = ScreenSize.GetScreenToWorldWidth;
        transform.localScale = Vector3.one * width;
    }
    public static float GetScreenToWorldHeight
    {
        get
        {
            Vector2 topRightCorner = new Vector2(1, 1);
            Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
            var height = edgeVector.y * staticSize;  // 1   - 0,3555555555555556f
            return height;                                    // 0.9 - 0.32
        }                                                     // 0.8 - 0,2844444444444444
    }                                                         // 0.5 - 0,1777777777777778
    public static float GetScreenToWorldWidth
    {
        get
        {
            Vector2 topRightCorner = new Vector2(1, 1);
            Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
            var width = edgeVector.x * staticSize;
            return width;
        }
    }
}