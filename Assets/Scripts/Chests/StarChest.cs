using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarChest : MonoBehaviour
{
    public Image StarChestBar;
    public Text MaxStarText;
    public static float maxCountToOpen = 20;
    public static float totalStarCount = 0;
    public static bool _isStarChestDone = false;
    void Start()
    {
        totalStarCount = PlayerPrefs.GetFloat("StarChestCount");
    }

    void Update()
    {
        PlayerPrefs.SetFloat("StarChestCount", totalStarCount);
        if (Input.GetKeyDown(KeyCode.S))
        {
            totalStarCount += 1;
        }

        if (totalStarCount >= maxCountToOpen)
        {
            _isStarChestDone = true;
        }
        else
        {
            _isStarChestDone = false;
        }
        
        StarChestBar.fillAmount = totalStarCount / maxCountToOpen;
        MaxStarText.text = $"{totalStarCount} / {maxCountToOpen}";
    }
}
