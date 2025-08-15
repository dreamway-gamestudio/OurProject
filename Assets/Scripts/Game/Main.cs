using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class Main : MonoBehaviour
{
    public GameObject[] AllDices;
    public GameObject[] CurrentDices;
    public GameObject Enemy;
    public Button CreatingDiceButton;
    public Text Score_txt;
    public static int Score;
    public float FrequancyOfSpawning;
    public int EnemyIndex;
    private int _count;
    public bool _spawning;
    

    private void Start()
    {
        InitDicePrefabs();


        Enemy.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().sortingOrder = -1;
        EnemyIndex = 0;
        _spawning = false;
        Score = 0;
        PlayerPrefs.SetInt("hp", 100);
        // InvokeRepeating("Enemytext", 5f, 5f);
        StartCoroutine(ChangeHP());
    }
    void Update()
    {
        CheckCells(15);
    }
    public int GetCurrentDiceIindexFromArray(string diceName)
    {
        int index = 0;
        for(int i = 0; i<= 4; i++)
        {
            if (CurrentDices[i].GetComponent<Dice>().DiceName == diceName)
            {
                index = i;
                break;
            }
        }
        return index;
    }
    void InitDicePrefabs()
    {
        int index = 0;
        for (int i = 1; i <= 5; i++)
        {
            string cellDice = PlayerPrefs.GetString($"Dice{i}");
            string[] arrayStr = cellDice.Split(char.Parse("_"));
            string diceName = arrayStr[1];

            for (int j = 0; j <= AllDices.Length - 1; j++)
            {
                if (AllDices[j].GetComponent<Dice>().DiceName == diceName)
                {
                    CurrentDices[index] = AllDices[j];
                    index++;
                }
            }
        }
        CurrentDices = CurrentDices.OrderBy(obj => obj.name, new AlphanumComparatorFast()).ToArray(); // sorting currentDices by name
    }
    public void UpdateScore()
    {
        Score_txt.text = Score.ToString();
    }

    void Enemytext()
    {

    }
    private IEnumerator ChangeHP()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            if (_spawning)
            {
                
                PlayerPrefs.SetInt("hp", PlayerPrefs.GetInt("hp") + 100);
            }


        }
    }
    private IEnumerator SpawnEnemy(float freq)
    {
        while (_spawning)
        {
            Instantiate(Enemy, transform.parent);
            EnemyIndex--;
            Enemy.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().sortingOrder -= 1;
            yield return new WaitForSeconds(freq);
        }
    }

    public void CreateDice()
    {
        var d_rnd = Random.Range(0, 5);
        var c_rnd = Random.Range(0, 15);
        if (transform.GetChild(c_rnd).childCount == 0)
        {
            Instantiate(CurrentDices[d_rnd], transform.GetChild(c_rnd));
        }
        else
        {
            while (transform.GetChild(c_rnd).childCount != 0)
                c_rnd = Random.Range(0, 15);
            Instantiate(CurrentDices[d_rnd], transform.GetChild(c_rnd));
        }
        CheckCells(15);
    }

    public void CheckCells(int n)
    {
        for (int i = 0; i < n; i++)
        {
            if (transform.GetChild(i).childCount > 0)
                _count++;
        }
        if (_count == n)
            CreatingDiceButton.interactable = false;
        else
            CreatingDiceButton.interactable = true;
        _count = 0;
    }

    public void StartSpawn()
    {
        _spawning = true;
        StartCoroutine(SpawnEnemy(FrequancyOfSpawning));
    }

    public void StopSpawn()
    {
        _spawning = false;
    }
}
public class AlphanumComparatorFast : IComparer<string> // CurrentDices-i sorting
{
    public int Compare(string x, string y)
    {
        string s1 = x as string;
        if (s1 == null)
        {
            return 0;
        }
        string s2 = y as string;
        if (s2 == null)
        {
            return 0;
        }

        int len1 = s1.Length;
        int len2 = s2.Length;
        int marker1 = 0;
        int marker2 = 0;

        // Walk through two the strings with two markers.
        while (marker1 < len1 && marker2 < len2)
        {
            char ch1 = s1[marker1];
            char ch2 = s2[marker2];

            // Some buffers we can build up characters in for each chunk.
            char[] space1 = new char[len1];
            int loc1 = 0;
            char[] space2 = new char[len2];
            int loc2 = 0;

            // Walk through all following characters that are digits or
            // characters in BOTH strings starting at the appropriate marker.
            // Collect char arrays.
            do
            {
                space1[loc1++] = ch1;
                marker1++;

                if (marker1 < len1)
                {
                    ch1 = s1[marker1];
                }
                else
                {
                    break;
                }
            } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

            do
            {
                space2[loc2++] = ch2;
                marker2++;

                if (marker2 < len2)
                {
                    ch2 = s2[marker2];
                }
                else
                {
                    break;
                }
            } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

            // If we have collected numbers, compare them numerically.
            // Otherwise, if we have strings, compare them alphabetically.
            string str1 = new string(space1);
            string str2 = new string(space2);

            int result;

            if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
            {
                int thisNumericChunk = int.Parse(str1);
                int thatNumericChunk = int.Parse(str2);
                result = thisNumericChunk.CompareTo(thatNumericChunk);
            }
            else
            {
                result = str1.CompareTo(str2);
            }

            if (result != 0)
            {
                return result;
            }
        }
        return len1 - len2;
    }
}