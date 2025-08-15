using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Vector2 StartPosition;
    public Vector2 UpdatePosition;
    public float MoveSpeed;

    private Vector2 TargetPosition;
    private string _vector;
    private int hp;
    private int demage;
    Animator animka;
    public GameObject FlyingTextPrefab;

    private TMPro.TextMeshPro hpText;
    private void Start()
    {
 
        animka = GetComponent<Animator>();
        TargetPosition = new Vector2(-2.4f, 0.6f);
        transform.position = StartPosition;
        hp = PlayerPrefs.GetInt("hp");
        MoveUp();
        var Index = Camera.main.transform.GetChild(0).GetComponent<Main>().EnemyIndex;
        GetComponent<SpriteRenderer>().sortingOrder = Index;
        hpText = transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
    }

    
    private void MoveUp()
    {
        _vector = "up";
        var rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.up * MoveSpeed;
        print("MoveUp " + TargetPosition);
    }

    private void MoveRight()
    {
        _vector = "right";
        TargetPosition = new Vector2(2.9f, 0.1f);
        var rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.right * MoveSpeed;
        print("MoveRight " + TargetPosition);
    }

    private void MoveDown()
    {
        _vector = "down";
        TargetPosition = new Vector2(2.9f, -3.3f);
        var rb = GetComponent<Rigidbody2D>();
        print("esh");
        rb.velocity = Vector2.down * MoveSpeed;
        print("MoveDown " + TargetPosition);
    }

    private void Update()
    {
        
        UpdatePosition = transform.position;
        hpText.text = hp.ToString();
        if (Vector2.Distance(transform.position, TargetPosition) <= 0.5f)
        {
            if (_vector == "up")
                MoveRight();
            else if (_vector == "right")
                MoveDown();
            else if (_vector == "down")
            {
                for (int i = 0; i < GameObject.FindGameObjectsWithTag("Bullet").Length; i++)
                    Destroy(GameObject.FindGameObjectsWithTag("Bullet")[i]);
                Destroy(gameObject);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            PlayerPrefs.SetString("flyingText", "true");
            try
            {
                demage = collision.transform.parent.GetChild(0).GetComponent<Dice>().Attack;
                hp -= demage;
            }
            catch
            {
                hp -= demage;
            }
            
            Destroy(collision.gameObject);
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
            
            Destroy(gameObject);
            
        }
        else
        {
            PlayerPrefs.SetString("hp_null", "false");
        }
    }
    void ShowFlyingText()
    {
        var go = Instantiate(FlyingTextPrefab, UpdatePosition, Quaternion.identity, transform);
        go.GetComponent<TMPro.TextMeshPro>().text = demage.ToString();
    }
}
