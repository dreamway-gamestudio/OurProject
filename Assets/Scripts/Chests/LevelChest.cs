using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelChest : MonoBehaviour
{
    public Image LevelChestBar;
    public Text MaxLevelText;
    public static float maxCountToOpen = 15;
    public static float totalLevelCount = 0;
    public static bool _isLevelChestDone = false;
    
    void Start()
    {
        totalLevelCount = PlayerPrefs.GetFloat("LevelChestCount");
    }

    void Update()
    {
        PlayerPrefs.SetFloat("LevelChestCount", totalLevelCount);
        if (Input.GetKeyDown(KeyCode.L))
        {
            totalLevelCount += 1;
        }

        if (totalLevelCount >= maxCountToOpen)
        {
            _isLevelChestDone = true;
        }
        else
        {
            _isLevelChestDone = false;
        }

        LevelChestBar.fillAmount = totalLevelCount / maxCountToOpen;
        MaxLevelText.text = $"{totalLevelCount} / {maxCountToOpen}";
    }
}
