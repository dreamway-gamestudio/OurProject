using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ElixirStore : MonoBehaviour
{
    //public Text textTimer;
    private int maxTimer = 2;
    private int totalTimerUnlim = 0;
    public static DateTime nextTimerUnlim;
    public static DateTime lastAddTime;
    private int restoreDuration;
    public static bool isEndUnlim = false;
    [HideInInspector] public bool zapuskUnlim = false;
    public static string value;
    int maxElixir = 5;
    ElixirTimer ElixirTimer;
    void Start()
    {
        ElixirTimer = GameObject.FindObjectOfType<ElixirTimer>();
        isEndUnlim = PlayerPrefs.GetInt("isEndUnlim") == 1 ? true : false;
        zapuskUnlim = PlayerPrefs.GetInt("zapuskUnlim") == 1 ? true : false;

        if (zapuskUnlim)
        {
            Load();
            StartCoroutine(RestoreRoutine());
        }
    }
    void Update()
    {
        try
        {
            if (PlayerPrefs.GetString("unlimited_elixir") == "true")
            {
                ChangeUnlimTimerText(value);
                UnlimButtonsInteractible(false);
                if(!GameObject.Find("RechargeButton"))
                {
                    GameObject.Find("RechargeButton").GetComponent<Button>().interactable = false;
                    print("foooo e");
                }
                
            }
            else if (PlayerPrefs.GetString("unlimited_elixir") == "false")
            {
                ChangeUnlimTimerText(PlayerPrefs.GetString("CurrentNeedTimerText"));
                UnlimButtonsInteractible(true);
            }
        }catch{}

        if(Elixir.GetElixir() < maxElixir)
        {
            GameObject.Find("RechargeButton").GetComponent<Button>().interactable = true;
        } else {
            GameObject.Find("RechargeButton").GetComponent<Button>().interactable = false;
        }
    }
 
    public void RechargeElixir(int price)
    {
        if (Coin.Coins >= price && Elixir.GetElixir() < maxElixir)
        {
            Coin.Coins -= price;
            Elixir.AddElixir(maxElixir);
        }
        else { print("No coins"); }
    }
    void ChangeUnlimTimerText(string timerText)
    {
        GameObject ButtonParent = GameObject.Find(PlayerPrefs.GetString("CurrentUnlimButton"));
        for (int i = 0; i <= ButtonParent.transform.childCount - 1; i++)
        {
            if (ButtonParent.transform.GetChild(i).name == "NeedTimerText")
            {
                ButtonParent.transform.GetChild(i).GetComponent<Text>().text = timerText;
                break;
            }
        }
    }
    void UnlimButtonsInteractible(bool isActive)
    {
        for (int i = 1; i <= 4; i++)
        {
            GameObject EP_Unlimited = GameObject.Find($"EP_Unlimited_{i}");
            for (int j = 0; j <= EP_Unlimited.transform.childCount - 1; j++)
            {

                if (EP_Unlimited.transform.GetChild(j).name == $"BuyButton_{i}")
                {
                    EP_Unlimited.transform.GetChild(j).GetComponent<Button>().interactable = isActive;
                }
            }
        }
    }
    public void Zapusk(int restoreDurationButton)
    {
        restoreDuration = restoreDurationButton;
        if (!zapuskUnlim)
        {
            PlayerPrefs.SetString("CurrentUnlimButton", EventSystem.current.currentSelectedGameObject.transform.parent.name);
            PlayerPrefs.SetString("CurrentNeedTimerText", GameObject.Find($"{EventSystem.current.currentSelectedGameObject.transform.parent.name}/NeedTimerText").GetComponent<Text>().text);
            PlayerPrefs.SetString("unlimited_elixir", "true");
            Elixir.SetElixir(maxElixir);
            Load();
            StartCoroutine(RestoreRoutine());
            zapuskUnlim = true;
            PlayerPrefs.SetInt("zapuskUnlim", (zapuskUnlim ? 1 : 0));

        }
    }
    private IEnumerator RestoreRoutine()
    {

        //UpdateTimer();
        //restoring = true;

        while (!isEndUnlim)
        {
            DateTime currentTime = DateTime.Now;
            DateTime counter = nextTimerUnlim;
            while (currentTime > counter)
            {
                DateTime timeToAdd = lastAddTime > counter ? lastAddTime : counter;
                counter = AddDuration(timeToAdd, restoreDuration);
                lastAddTime = DateTime.Now;
                nextTimerUnlim = counter;
                totalTimerUnlim++;
            }
            if (!isEndUnlim)
            {
                UpdateTimer();
            }
            Save();


            yield return null;
        }
        ClearDailyRewardData();
        //restoring = false;
    }
    private void UpdateTimer()
    {
        if (totalTimerUnlim >= maxTimer)
        {

            isEndUnlim = true;
            // textTimer.text = "End!";

            PlayerPrefs.SetInt($"isEndUnlim", (isEndUnlim ? 1 : 0));
            return;
        }
        else
        {

        }
        TimeSpan t = nextTimerUnlim - DateTime.Now;
        value = String.Format("{0:D2}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);
        // textTimer.text = value;
    }
    public void ClearDailyRewardData()
    {
        print("maqruma");
        isEndUnlim = false;
        Elixir.SetElixir(maxElixir);
        PlayerPrefs.SetString("unlimited_elixir", "false");
        PlayerPrefs.SetInt("isEndUnlim", (isEndUnlim ? 1 : 0));
        zapuskUnlim = false;
        PlayerPrefs.SetInt("zapuskUnlim", (zapuskUnlim ? 1 : 0));
        PlayerPrefs.SetInt("totalTimerUnlim", 0);
        PlayerPrefs.DeleteKey("nextTimerUnlim");
        PlayerPrefs.DeleteKey("lastAddedTimeUnlim");
    }
    public static DateTime AddDuration(DateTime time, int duration)
    {
        return time.AddHours(duration);
    }
    private void Load()
    {
        totalTimerUnlim = PlayerPrefs.GetInt($"totalTimerUnlim");
        nextTimerUnlim = StringToDate(PlayerPrefs.GetString($"nextTimerUnlim"));
        lastAddTime = StringToDate(PlayerPrefs.GetString($"lastAddedTimeUnlim"));
    }
    private void Save()
    {
        PlayerPrefs.SetInt($"totalTimerUnlim", totalTimerUnlim);
        PlayerPrefs.SetString($"nextTimerUnlim", nextTimerUnlim.ToString());
        PlayerPrefs.SetString($"lastAddedTimeUnlim", lastAddTime.ToString());
    }
    private DateTime StringToDate(string date)
    {
        if (String.IsNullOrEmpty(date))
            return DateTime.Now;

        return DateTime.Parse(date);
    }
}
