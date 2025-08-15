using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerStandard1 : MonoBehaviour
{
    public Text textTimer, StatusText;
    [SerializeField]
    private int maxTimer;
    private int totalTimer_St1 = 0;
    public static DateTime nextTime_St1;
    public static DateTime lastAddTime_St1;
    public int restoreDuration;
    [HideInInspector] public bool isEnd_St1 = false;
    [HideInInspector] public bool zapusk_St1 = false;
    public void Start()
    {
        isEnd_St1 = PlayerPrefs.GetInt("isEnd_St1") == 1 ? true : false;
        zapusk_St1 = PlayerPrefs.GetInt("zapusk_St1") == 1 ? true : false;

        if (zapusk_St1)
        {
            Load();
            StartCoroutine(RestoreRoutine());
        }
        else
        {

        }
        if (!isEnd_St1)
        {
            StatusText.text = "Start";
        }

    }
    public void Zapusk()
    {
        if(!zapusk_St1)
        {
        Load();
        StartCoroutine(RestoreRoutine());
        zapusk_St1 = true;
        PlayerPrefs.SetInt("zapusk_St1", (zapusk_St1 ? 1 : 0));
        StatusText.text = "Open now";
        } else {
            print("infopanel");
        }

    }
    private IEnumerator RestoreRoutine()
    {
        //UpdateTimer();
        //restoring = true;

        while (!isEnd_St1)
        {
            DateTime currentTime = DateTime.Now;
            DateTime counter = nextTime_St1;
            while (currentTime > counter)
            {
                DateTime timeToAdd = lastAddTime_St1 > counter ? lastAddTime_St1 : counter;
                counter = AddDuration(timeToAdd, restoreDuration);
                lastAddTime_St1 = DateTime.Now;
                nextTime_St1 = counter;
                totalTimer_St1++;
            }
            if (!isEnd_St1)
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
        if (totalTimer_St1 >= maxTimer)
        {
            isEnd_St1 = true;
            textTimer.text = "00:00:00";
            StatusText.text = "Open";
            PlayerPrefs.SetInt("isEnd_St1", (isEnd_St1 ? 1 : 0));
            return;
        }
        TimeSpan t = nextTime_St1 - DateTime.Now;
        string value = String.Format("{0:D2}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);

        textTimer.text = value;
    }
    public static DateTime AddDuration(DateTime time, int duration)
    {
        return time.AddSeconds(duration);
    }
    private void Load()
    {
        totalTimer_St1 = PlayerPrefs.GetInt("totalTimer_St1");
        nextTime_St1 = StringToDate(PlayerPrefs.GetString("nextTime_St1"));
        lastAddTime_St1 = StringToDate(PlayerPrefs.GetString("lastAddTime_St1"));
    }
    private void Save()
    {
        PlayerPrefs.SetInt("totalTimer_St1", totalTimer_St1);
        PlayerPrefs.SetString("nextTime_St1", nextTime_St1.ToString());
        PlayerPrefs.SetString("lastAddTime_St1", lastAddTime_St1.ToString());
    }
    private DateTime StringToDate(string date)
    {
        if (String.IsNullOrEmpty(date))
            return DateTime.Now;

        return DateTime.Parse(date);
    }
}

