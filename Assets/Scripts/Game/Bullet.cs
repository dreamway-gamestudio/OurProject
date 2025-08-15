using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Dice Dice;
    EnemyMove EnemyMove;
    FlyingTextPrefab FlyingTextPrefab;
    Vector3 target, target_front, target_end, target_random;
    int random;
    Vector3 UpdatePosition;
    
    //public GameObject FlyingTextPrefab;
    void Start()
    {
        Dice = GameObject.FindObjectOfType<Dice>();
        EnemyMove = GameObject.FindObjectOfType<EnemyMove>();
        FlyingTextPrefab = GameObject.FindObjectOfType<FlyingTextPrefab>();
        //target = Dice.target;
        random = Random.Range(1, Camera.main.transform.childCount);


    }
    void Update()
    {
        try
        {
            if (gameObject.transform.parent.transform.GetChild(0).GetComponent<Dice>().Target == Dice.Target_Types.Front)
            {
                target = Camera.main.transform.GetChild(1).transform.position; // shooting target to enemy
            }
            else if (gameObject.transform.parent.transform.GetChild(0).GetComponent<Dice>().Target == Dice.Target_Types.End)
            {
                target = Camera.main.transform.GetChild(Camera.main.transform.childCount - 1).transform.position; // shooting target to enemy
            }
            else if (gameObject.transform.parent.transform.GetChild(0).GetComponent<Dice>().Target == Dice.Target_Types.Random)
            {
                try
                {
                    target = Camera.main.transform.GetChild(random).transform.position;// shooting target to enemy
                }
                catch { }
            }
            string diceName = gameObject.transform.parent.GetChild(0).GetComponent<Dice>().DiceName;
            // print($"{diceName} Shoot Speed: {DicePlayerPrefs.GetShootSpeed(diceName)}");
            transform.position = Vector2.MoveTowards(transform.position, target, DicePlayerPrefs.GetShootSpeed(diceName) * Time.deltaTime);
        }
        catch
        {
            Destroy(gameObject); // ete enymyn destroy lini odi dotery satken
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Enemy") /*&& collision.transform.position == target*/)
        {
            if (Vector2.Distance(collision.transform.position, target) < 0.1f)
            {
                GameObject Enemy = collision.gameObject;
               // 
                //FlyTest.transform.position = Enemy.transform.position;
                try
                {
                    string diceName = gameObject.transform.parent.GetChild(0).GetComponent<Dice>().DiceName;
                    // print($"{diceName} Attack: {DicePlayerPrefs.GetAttack(diceName)}");
                    // EnemyMove.damage = DicePlayerPrefs.GetAttack(diceName);
                }
                catch {}

                Enemy.GetComponent<EnemyMove>().hp -= EnemyMove.damage /*+ ((DiceLVL.levelStartFrom/10)+EnemyMove.damage)*/;
                Destroy(gameObject);
            }
        }
    }
    void ShowFlyingText()
    {

    }
}
