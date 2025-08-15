using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMove : MonoBehaviour
{
    Waypoints Waypoints;
    Main Main;
    public float speed;
    private int waypointIndex;
    [HideInInspector] public int startHP;
    [HideInInspector] public int hp;
    [HideInInspector] public static int damage;
    //public Transform BulletTarget;
    public Vector3 UpdatePosition;
    public GameObject FlyingTextPrefab;
    GameObject FlyTest;
    private TMPro.TextMeshPro textHP;
    void Start()
    {

        Waypoints = GameObject.FindObjectOfType<Waypoints>();
        Main = GameObject.FindObjectOfType<Main>();
        hp = PlayerPrefs.GetInt("hp");
        startHP = hp;
        var Index = Camera.main.transform.GetChild(0).GetComponent<Main>().EnemyIndex;
        GetComponent<SpriteRenderer>().sortingOrder = Index;
        textHP = transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
    }
    void Awake()
    {

    }
    void Update()
    {

        transform.position = Vector2.MoveTowards(transform.position, Waypoints.waypoints[waypointIndex].position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, Waypoints.waypoints[waypointIndex].position) < 0.000001f)
        {
            waypointIndex++;
        }
        if (transform.position == Waypoints.waypoints[Waypoints.waypoints.Length - 1].position)
        {
            Destroy(gameObject);
        }
        textHP.text = hp.ToString();


    }
    void GetDiceByName()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            string current_DiceName = collision.transform.parent.GetChild(0).GetComponent<Dice>().DiceName;
            int current_DiceLevel = PlayerPrefs.GetInt($"PowerUp{current_DiceName}");
            int current_DiceAttack = DicePlayerPrefs.GetAttack(current_DiceName);

            PlayerPrefs.SetString("flyingText", "true");
            UpdatePosition = collision.gameObject.transform.position;
            if (current_DiceLevel==1)
            {
                damage = current_DiceAttack;
            } else
            {
                // damage = current_DiceAttack+(current_DiceAttack/10*current_DiceLevel);
                damage = PlayerPrefs.GetInt($"GameAttack{current_DiceName}");
            }

            // xndir_1 --> tokosov avelacni voch te skzbnakan attackin ayl yntacikin
            

            ShowFlyingText();
            //Destroy(collision.gameObject);
            transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = hp.ToString();
        }
        if (hp <= 0)
        {
            PlayerPrefs.SetString("hp_null", "true");
            Main.Score += 50;
            Camera.main.GetComponentInChildren<Main>().UpdateScore();
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Bullet").Length; i++)
            {
                Destroy(GameObject.FindGameObjectsWithTag("Bullet")[i]);
            }
            GamePlayCoins.Coins += 20;
            Destroy(gameObject);

        }
        else
        {
            PlayerPrefs.SetString("hp_null", "false");
        }

    }
    
    void ShowFlyingText()
    {
        FlyTest = GameObject.Find("FlyText");

        var go = Instantiate(FlyingTextPrefab, transform.position, Quaternion.identity, FlyTest.transform);
        go.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = damage.ToString();

        go.transform.position = transform.position;

    }
}
