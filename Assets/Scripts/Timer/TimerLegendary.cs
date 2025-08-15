using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerLegendary : MonoBehaviour
{
    public Text textTimer, StatusText;
    [SerializeField]
    private int maxTimer;
    private int totalTimer_Lg = 0;
    public static DateTime nextTime_Lg;
    public static DateTime lastAddTime_Lg;
    public int restoreDuration;
    [HideInInspector] public bool isEnd_Lg = false;
    [HideInInspector] public bool zapusk_Lg = false;
    public void Start()
    {
        isEnd_Lg = PlayerPrefs.GetInt("isEnd_Lg") == 1 ? true : false;
        zapusk_Lg = PlayerPrefs.GetInt("zapusk_Lg") == 1 ? true : false;

        if (zapusk_Lg)
        {
            Load();
            StartCoroutine(RestoreRoutine());
        }
        else
        {

        }
        if (isEnd_Lg)
        {
            textTimer.text = "Open";
        }

    }
    public void Zapusk()
    {
        Load();
        StartCoroutine(RestoreRoutine());
        zapusk_Lg = true;
        PlayerPrefs.SetInt("zapusk_Lg", (zapusk_Lg ? 1 : 0));

    }
    private IEnumerator RestoreRoutine()
    {
        //UpdateTimer();
        //restoring = true;

        while (!isEnd_Lg)
        {
            DateTime currentTime = DateTime.Now;
            DateTime counter = nextTime_Lg;
            while (currentTime > counter)
            {
                DateTime timeToAdd = lastAddTime_Lg > counter ? lastAddTime_Lg : counter;
                counter = AddDuration(timeToAdd, restoreDuration);
                lastAddTime_Lg = DateTime.Now;
                nextTime_Lg = counter;
                totalTimer_Lg++;
            }
            if (!isEnd_Lg)
            {
                UpdateTimer();
            }
            Save();


            yield return null;
        }
        //restoring = false;
    }
    private void UpdateTimer()
    {
        if (totalTimer_Lg >= maxTimer)
        {
            isEnd_Lg = true;
            textTimer.text = "Open";
            PlayerPrefs.SetInt("isEnd_Lg", (isEnd_Lg ? 1 : 0));
            return;
        }
        TimeSpan t = nextTime_Lg - DateTime.Now;
        string value = String.Format("{0:D2}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);

        textTimer.text = value;
    }
    public static DateTime AddDuration(DateTime time, int duration)
    {
        return time.AddSeconds(duration);
    }
    private void Load()
    {
        totalTimer_Lg = PlayerPrefs.GetInt("totalTimer_Lg");
        nextTime_Lg = StringToDate(PlayerPrefs.GetString("nextTime_Lg"));
        lastAddTime_Lg = StringToDate(PlayerPrefs.GetString("lastAddTime_Lg"));
    }
    private void Save()
    {
        PlayerPrefs.SetInt("totalTimer_Lg", totalTimer_Lg);
        PlayerPrefs.SetString("nextTime_Lg", nextTime_Lg.ToString());
        PlayerPrefs.SetString("lastAddTime_Lg", lastAddTime_Lg.ToString());
    }
    private DateTime StringToDate(string date)
    {
        if (String.IsNullOrEmpty(date))
            return DateTime.Now;

        return DateTime.Parse(date);
    }
}

