using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ElixirTimer : MonoBehaviour
{
    [SerializeField]
    private Text textElixir;
    [SerializeField]
    private Text textTimer;
    [SerializeField]
    public static int maxElixir = 5;
    public static int totalElixir = 5;
    public static DateTime nextElixirTime;
    public static DateTime lastAddedTime;
    public static int restoreDuration = 10;
    public static bool restoring = false;

    private Image UnlimElixirImage;

    void Start()
    {
        Load();
        StartCoroutine(RestoreRoutine());
        UnlimElixirImage = GameObject.Find("ElixirField/UnlimImage").GetComponent<Image>();
    }

    void Update()
    {
        totalElixir = PlayerPrefs.GetInt("totalElixir");
        CheckUnlimitedStatus();
        if (totalElixir >= maxElixir)
        {
            textTimer.text = "Full";
            textElixir.text = $"{totalElixir}/{maxElixir}";
            Elixir.SetElixir(maxElixir);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            UseElixir();
        }


        if (totalElixir <= 0)
        {
            Elixir.SetElixir(0);
            return;
        }


    }
    public void CheckUnlimitedStatus()
    {
        if ((PlayerPrefs.GetString("unlimited_elixir") == "false"))
        {

            Elixir.SetElixir(maxElixir);
            textElixir.enabled = true;
            UnlimElixirImage.enabled = false;
            textElixir.enabled = true;
            textElixir.text = $"{totalElixir}/{maxElixir}";


            return;
        }
        else if ((PlayerPrefs.GetString("unlimited_elixir") == "true"))
        {
            textElixir.enabled = false;
            UnlimElixirImage.enabled = true;
            textElixir.fontSize = 40;
            return;
        }

    }
    public void UseElixir()
    {
        if (totalElixir > 0)
        {
            PlayerPrefs.SetInt("totalElixir", totalElixir -= 1);
        }
        if (totalElixir == 0)
        {
            return;
        }


        UpdateElixir();

        if (!restoring)
        {
            if (totalElixir + 1 == maxElixir)
            {

                nextElixirTime = AddDuration(DateTime.Now, restoreDuration);
            }

            StartCoroutine(RestoreRoutine());
        }
    }
    private IEnumerator RestoreRoutine()
    {
        UpdateTimer();
        UpdateElixir();
        restoring = true;

        while (totalElixir < maxElixir)
        {
            DateTime currentTime = DateTime.Now;
            DateTime counter = nextElixirTime;
            bool isAdding = false;
            while (currentTime > counter)
            {
                if (totalElixir < maxElixir)
                {
                    isAdding = true;
                    totalElixir++;
                    DateTime timeToAdd = lastAddedTime > counter ? lastAddedTime : counter;
                    counter = AddDuration(timeToAdd, restoreDuration);
                }
                else
                    break;
            }

            if (isAdding)
            {
                lastAddedTime = DateTime.Now;
                nextElixirTime = counter;
            }

            UpdateTimer();
            UpdateElixir();
            Save();
            yield return null;

        }
        restoring = false;
    }
    private void UpdateTimer()
    {

        if (totalElixir >= maxElixir)
        {

            textTimer.text = "Full";
            return;
        }
        TimeSpan t = nextElixirTime - DateTime.Now;
        string value = String.Format("{0:D2}:{1:D2}", (int)t.Minutes, t.Seconds);

        textTimer.text = value;
    }
    private void UpdateElixir()
    {
        textElixir.text = $"{totalElixir}/{maxElixir}";
    }
    public static DateTime AddDuration(DateTime time, int duration)
    {
        return time.AddSeconds(duration);
    }
    private void Load()
    {
        totalElixir = PlayerPrefs.GetInt("totalElixir");
        nextElixirTime = StringToDate(PlayerPrefs.GetString("nextElixirTime"));
        lastAddedTime = StringToDate(PlayerPrefs.GetString("lastAddedTime"));
    }
    private void Save()
    {
        PlayerPrefs.SetInt("totalElixir", totalElixir);
        PlayerPrefs.SetString("nextElixirTime", nextElixirTime.ToString());
        PlayerPrefs.SetString("lastAddedTime", lastAddedTime.ToString());
    }
    private DateTime StringToDate(string date)
    {
        if (String.IsNullOrEmpty(date))
            return DateTime.Now;

        return DateTime.Parse(date);
    }
}

