using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerExclusive : MonoBehaviour
{
    public Text textTimer, StatusText;
    [SerializeField]
    private int maxTimer;
    private int totalTimer_Ex = 0;
    public static DateTime nextTime_Ex;
    public static DateTime lastAddTime_Ex;
    public int restoreDuration;
    [HideInInspector] public bool isEnd_Ex = false;
    [HideInInspector] public bool zapusk_Ex = false;
    public void Start()
    {
        isEnd_Ex = PlayerPrefs.GetInt("isEnd_Ex") == 1 ? true : false;
        zapusk_Ex = PlayerPrefs.GetInt("zapusk_Ex") == 1 ? true : false;

        if (zapusk_Ex)
        {
            Load();
            StartCoroutine(RestoreRoutine());
        }
        else
        {

        }
        if (isEnd_Ex)
        {
            textTimer.text = "Open";
        }

    }
    public void Zapusk()
    {
        Load();
        StartCoroutine(RestoreRoutine());
        zapusk_Ex = true;
        PlayerPrefs.SetInt("zapusk_Ex", (zapusk_Ex ? 1 : 0));

    }
    private IEnumerator RestoreRoutine()
    {
        //UpdateTimer();
        //restoring = true;

        while (!isEnd_Ex)
        {
            DateTime currentTime = DateTime.Now;
            DateTime counter = nextTime_Ex;
            while (currentTime > counter)
            {
                DateTime timeToAdd = lastAddTime_Ex > counter ? lastAddTime_Ex : counter;
                counter = AddDuration(timeToAdd, restoreDuration);
                lastAddTime_Ex = DateTime.Now;
                nextTime_Ex = counter;
                totalTimer_Ex++;
            }
            if (!isEnd_Ex)
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
        if (totalTimer_Ex >= maxTimer)
        {
            isEnd_Ex = true;
            textTimer.text = "Open";
            PlayerPrefs.SetInt("isEnd_Ex", (isEnd_Ex ? 1 : 0));
            return;
        }
        TimeSpan t = nextTime_Ex - DateTime.Now;
        string value = String.Format("{0:D2}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);

        textTimer.text = value;
    }
    public static DateTime AddDuration(DateTime time, int duration)
    {
        return time.AddSeconds(duration);
    }
    private void Load()
    {
        totalTimer_Ex = PlayerPrefs.GetInt("totalTimer_Ex");
        nextTime_Ex = StringToDate(PlayerPrefs.GetString("nextTime_Ex"));
        lastAddTime_Ex = StringToDate(PlayerPrefs.GetString("lastAddTime_Ex"));
    }
    private void Save()
    {
        PlayerPrefs.SetInt("totalTimer_Ex", totalTimer_Ex);
        PlayerPrefs.SetString("nextTime_Ex", nextTime_Ex.ToString());
        PlayerPrefs.SetString("lastAddTime_Ex", lastAddTime_Ex.ToString());
    }
    private DateTime StringToDate(string date)
    {
        if (String.IsNullOrEmpty(date))
            return DateTime.Now;

        return DateTime.Parse(date);
    }
}

