using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DailyTimer : MonoBehaviour
{
    public Text textTimer;
    private int maxTimer = 2;
    private int totalTimer = 0;
    public static DateTime nextTime;
    public static DateTime lastAddTime;
    public int restoreDuration;
    public bool isReady = false;
    [HideInInspector] public bool zapusk = false;
    void Start()
    {
        isReady = PlayerPrefs.GetInt("isReady") == 1 ? true : false;
        zapusk = PlayerPrefs.GetInt("zapusk") == 1 ? true : false;

        if (zapusk)
        {
            Load();
            StartCoroutine(RestoreRoutine());
        }
        else
        {

        }
        if (!isReady)
        {

        }
        Zapusk();
    }

    public void Zapusk()
    {
        if (!zapusk)
        {
            Load();
            StartCoroutine(RestoreRoutine());
            zapusk = true;
            PlayerPrefs.SetInt("zapusk", (zapusk ? 1 : 0));

        }
        else if (isReady)
        {
            textTimer.text = "Ready!";
        }
    }
    public void ClearDailyRewardData()
    {
        isReady = false;
        PlayerPrefs.SetInt("isReady", (isReady ? 1 : 0));
        zapusk = false;
        PlayerPrefs.SetInt("zapusk", (zapusk ? 1 : 0));
        PlayerPrefs.SetInt("totalTimer", 0);
        PlayerPrefs.DeleteKey("nextTime");
        PlayerPrefs.DeleteKey("lastAddTime");
        Zapusk();
    }

    private IEnumerator RestoreRoutine()
    {

        //UpdateTimer();
        //restoring = true;

        while (!isReady)
        {
            DateTime currentTime = DateTime.Now;
            DateTime counter = nextTime;
            while (currentTime > counter)
            {
                DateTime timeToAdd = lastAddTime > counter ? lastAddTime : counter;
                counter = AddDuration(timeToAdd, restoreDuration);
                lastAddTime = DateTime.Now;
                nextTime = counter;
                totalTimer++;
            }
            if (!isReady)
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
        if (totalTimer >= maxTimer)
        {

            isReady = true;
            textTimer.text = "Ready!";
            PlayerPrefs.SetInt($"isReady", (isReady ? 1 : 0));
            return;
        }
        TimeSpan t = nextTime - DateTime.Now;
        string value = String.Format("{0:D2}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);
        textTimer.text = value;
    }
    public static DateTime AddDuration(DateTime time, int duration)
    {
        return time.AddSeconds(duration);
    }
    private void Load()
    {
        totalTimer = PlayerPrefs.GetInt($"totalTimer");
        nextTime = StringToDate(PlayerPrefs.GetString($"nextTime"));
        lastAddTime = StringToDate(PlayerPrefs.GetString($"lastAddedTime"));
    }
    private void Save()
    {
        PlayerPrefs.SetInt($"totalTimer", totalTimer);
        PlayerPrefs.SetString($"nextTime", nextTime.ToString());
        PlayerPrefs.SetString($"lastAddedTime", lastAddTime.ToString());
    }
    private DateTime StringToDate(string date)
    {
        if (String.IsNullOrEmpty(date))
            return DateTime.Now;

        return DateTime.Parse(date);
    }
}

