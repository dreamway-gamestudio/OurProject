using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerStandard : MonoBehaviour
{
    public Text textTimer, StatusText;
    [SerializeField]
    private int maxTimer;
    private int totalTimer_St = 0;
    public static DateTime nextTime_St;
    public static DateTime lastAddTime_St;
    public int restoreDuration;
    [HideInInspector] public bool isEnd_St = false;
    [HideInInspector] public bool zapusk_St = false;
    public void Start()
    {
        isEnd_St = PlayerPrefs.GetInt("isEnd_St") == 1 ? true : false;
        zapusk_St = PlayerPrefs.GetInt("zapusk_St") == 1 ? true : false;

        if (zapusk_St)
        {
            Load();
            StartCoroutine(RestoreRoutine());
        }
        else
        {

        }
        if (!isEnd_St)
        {
            StatusText.text = "Start";
        }

    }
    public void Zapusk()
    {
        if(!zapusk_St)
        {
        Load();
        StartCoroutine(RestoreRoutine());
        zapusk_St = true;
        PlayerPrefs.SetInt("zapusk_St", (zapusk_St ? 1 : 0));
        StatusText.text = "Open now";
        } else {
            print("infopanel");
        }

    }
    private IEnumerator RestoreRoutine()
    {
        //UpdateTimer();
        //restoring = true;

        while (!isEnd_St)
        {
            DateTime currentTime = DateTime.Now;
            DateTime counter = nextTime_St;
            while (currentTime > counter)
            {
                DateTime timeToAdd = lastAddTime_St > counter ? lastAddTime_St : counter;
                counter = AddDuration(timeToAdd, restoreDuration);
                lastAddTime_St = DateTime.Now;
                nextTime_St = counter;
                totalTimer_St++;
            }
            if (!isEnd_St)
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
        if (totalTimer_St >= maxTimer)
        {
            isEnd_St = true;
            textTimer.text = "00:00:00";
            StatusText.text = "Open";
            PlayerPrefs.SetInt("isEnd_St", (isEnd_St ? 1 : 0));
            return;
        }
        TimeSpan t = nextTime_St - DateTime.Now;
        string value = String.Format("{0:D2}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);

        textTimer.text = value;
    }
    public static DateTime AddDuration(DateTime time, int duration)
    {
        return time.AddSeconds(duration);
    }
    private void Load()
    {
        totalTimer_St = PlayerPrefs.GetInt("totalTimer_St");
        nextTime_St = StringToDate(PlayerPrefs.GetString("nextTime_St"));
        lastAddTime_St = StringToDate(PlayerPrefs.GetString("lastAddTime_St"));
    }
    private void Save()
    {
        PlayerPrefs.SetInt("totalTimer_St", totalTimer_St);
        PlayerPrefs.SetString("nextTime_St", nextTime_St.ToString());
        PlayerPrefs.SetString("lastAddTime_St", lastAddTime_St.ToString());
    }
    private DateTime StringToDate(string date)
    {
        if (String.IsNullOrEmpty(date))
            return DateTime.Now;

        return DateTime.Parse(date);
    }
}

