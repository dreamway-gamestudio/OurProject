using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardChest : MonoBehaviour
{
    public Image CardChestBar;
    public Text MaxCardText;
    public static float maxCountToOpen = 25;
    public static float totalCardCount = 0;
    public static bool _isCardChestDone = false;
    void Start()
    {
        totalCardCount = PlayerPrefs.GetFloat("CardChestCount");
    }

    void Update()
    {
        PlayerPrefs.SetFloat("CardChestCount", totalCardCount);
        if (Input.GetKeyDown(KeyCode.K))
        {
            totalCardCount += 1;
        }

        if (totalCardCount >= maxCountToOpen)
        {
            _isCardChestDone = true;
        }
        else
        {
            _isCardChestDone = false;
        }
        
        CardChestBar.fillAmount = totalCardCount / maxCountToOpen;
        MaxCardText.text = $"{totalCardCount} / {maxCountToOpen}";
    }
}
