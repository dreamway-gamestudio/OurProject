using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerChest : MonoBehaviour
{
    public Text textTimer, StatusText;
    private int maxTimer = 2;
    private int totalTimer = 0;
    public static DateTime nextTime;
    public static DateTime lastAddTime;
    public int restoreDuration;
    public bool isEnd = false;
    [HideInInspector] public bool zapusk = false;
    public string ChestType, CellName;
    Chests Chests;
    public static GameObject CurrentChest;
    GiveGameChests GiveGameChests;
    [HideInInspector]
    public int remainingTime;
    public void Start()
    {


    }
    void OnEnable()
    {
        Chests = GameObject.FindObjectOfType<Chests>();
        GiveGameChests = GameObject.FindObjectOfType<GiveGameChests>();
        GiveGameChests.LoadChests();
        isEnd = PlayerPrefs.GetInt($"isEnd{ChestType}{CellName}") == 1 ? true : false;
        zapusk = PlayerPrefs.GetInt($"zapusk{ChestType}{CellName}") == 1 ? true : false;

        if (zapusk)
        {
            Load();
            StartCoroutine(RestoreRoutine());
        }
        else
        {

        }
        if (!isEnd)
        {

            textTimer.text = restoreDuration + " h";
            SaveStatusText("Start");
        }
        else
        {
            gameObject.GetComponent<Animator>().Play("ChestDone");
        }
    }
    public void SaveStatusText(string status)
    {
        PlayerPrefs.SetString($"ChestStatus{ChestType}{CellName}", status);
        StatusText.text = GetStatusText();
    }
    public string GetStatusText()
    {
        return PlayerPrefs.GetString($"ChestStatus{ChestType}{CellName}");
    }
    public void ClearChestData(bool isActive) // jogenq inchy petq chi hanenq es funkciayic
    {
        gameObject.SetActive(isActive);
        isEnd = false;
        PlayerPrefs.SetInt($"isEnd{ChestType}{CellName}", (isEnd ? 1 : 0));
        PlayerPrefs.SetInt("Zapusk", 0);
        zapusk = false;
        PlayerPrefs.SetInt($"zapusk{ChestType}{CellName}", (zapusk ? 1 : 0));
        PlayerPrefs.SetString(transform.parent.name, "erased");
        PlayerPrefs.SetInt($"totalTimer{ChestType}{CellName}", 0);
        PlayerPrefs.DeleteKey($"nextTime{ChestType}{CellName}");
        PlayerPrefs.DeleteKey($"lastAddTime{ChestType}{CellName}");
        StatusText.enabled = false;
    }
    public void Zapusk()
    {

        CurrentChest = gameObject;
        if (PlayerPrefs.GetInt("TimerStatus") == 0)
        {

            if (!zapusk)
            {

                //PlayerPrefs.SetString(transform.parent.name, $"{ChestType}_Chest");
                Load();
                StartCoroutine(RestoreRoutine());
                zapusk = true;
                PlayerPrefs.SetInt("Zapusk", 1);
                PlayerPrefs.SetInt($"zapusk{ChestType}{CellName}", (zapusk ? 1 : 0));
                PlayerPrefs.SetInt("TimerStatus", 1);
                print("bacec");

            }
            else if (isEnd)
            {

                Chests.OpenChestPanel(transform.name);

            }
        }
        else if (zapusk)
        {
            Chests.OpenChestPanel(transform.name);
        }
        else
        {
            Chests.OpenChestPanel(transform.name);
        }
    }
    void Update()
    {

        //print("Timer Status: " + PlayerPrefs.GetInt("TimerStatus"));
    }
    private IEnumerator RestoreRoutine()
    {

        //UpdateTimer();
        //restoring = true;

        while (!isEnd)
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
            if (!isEnd)
            {
                SaveStatusText("Open now");
                UpdateTimer();
            }
            Save();


            yield return null;
        }
        //restoring = false;
    }
    public void OutOfTimeClearTimerChestData() // ures?
    {
        isEnd = true;
        textTimer.text = "00:00:00";
        textTimer.enabled = false;
        SaveStatusText("Open");
        PlayerPrefs.SetInt($"isEnd{ChestType}{CellName}", (isEnd ? 1 : 0));
        gameObject.GetComponent<Animator>().Play("ChestDone");
    }
    private void UpdateTimer()
    {
        if (totalTimer >= maxTimer)
        {

            isEnd = true;
            PlayerPrefs.SetInt("TimerStatus", 0);
            textTimer.text = "00:00:00";
            textTimer.enabled = false;
            SaveStatusText("Open");
            PlayerPrefs.SetInt($"isEnd{ChestType}{CellName}", (isEnd ? 1 : 0));
            gameObject.GetComponent<Animator>().Play("ChestDone");
            return;
        }
        TimeSpan t = nextTime - DateTime.Now;
        string value = String.Format("{0:D2}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);
        remainingTime = (int)t.TotalHours * 3600 + t.Minutes * 60 + t.Seconds;
        textTimer.text = value;
    }
    public static DateTime AddDuration(DateTime time, int duration)
    {
        return time.AddSeconds(duration);
    }
    private void Load()
    {
        totalTimer = PlayerPrefs.GetInt($"totalTimer{ChestType}{CellName}");
        nextTime = StringToDate(PlayerPrefs.GetString($"nextTime{ChestType}{CellName}"));
        lastAddTime = StringToDate(PlayerPrefs.GetString($"lastAddTime{ChestType}{CellName}"));
    }
    private void Save()
    {
        PlayerPrefs.SetInt($"totalTimer{ChestType}{CellName}", totalTimer);
        PlayerPrefs.SetString($"nextTime{ChestType}{CellName}", nextTime.ToString());
        PlayerPrefs.SetString($"lastAddTime{ChestType}{CellName}", lastAddTime.ToString());
    }
    private DateTime StringToDate(string date)
    {
        if (String.IsNullOrEmpty(date))
            return DateTime.Now;

        return DateTime.Parse(date);
    }
}

