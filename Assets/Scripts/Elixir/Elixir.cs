using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elixir : MonoBehaviour
{
    public static int Elixirs;
    void Start()
    {
        Elixirs = PlayerPrefs.GetInt("totalElixir");
    }
    
    public static void AddElixir(int count)
    {
        if (PlayerPrefs.GetInt("totalElixir") < 5)
        {
            PlayerPrefs.SetInt("totalElixir", PlayerPrefs.GetInt("totalElixir") + count);
        }
    }
    public static void SetElixir(int count)
    {
        PlayerPrefs.SetInt("totalElixir", count);
        
    }
    public static int GetElixir()
    {
        return PlayerPrefs.GetInt("totalElixir");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            PlayerPrefs.SetInt("totalElixir", PlayerPrefs.GetInt("totalElixir") -1);
        }
    }
}
