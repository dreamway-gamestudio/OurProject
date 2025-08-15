using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System;

public class Dice : MonoBehaviour
{
    public int Grade;
    public string DiceName;
    private string DraggedDiceName;
    public GameObject Bullet;
    public float ReloadTime;
    public int Attack;
    public float shootSpeed;
    public int priceWithDiamond;

    private bool _shooting;
    private bool _drag = false;
    private bool _canCombine = false;
    private GameObject _dice;
    private bool _returning;
    private int dotindex;
    public int pinCount;
    private List<Vector3> positions;
    [HideInInspector]
    public Vector3 _currentPosition;

    [HideInInspector]
    public Vector3 target;
    bool isTriggerStay = false;

    private SpriteRenderer DiceSprite;

    // Scripts
    private Main Main;
    public GameObject Field;
    ChangeDicePosition changeDicePosition;

    int index, index_1, index_2;
    public enum Target_Types
    {
        Front,
        End,
        Random,
        Strongest
    }

    public enum Rarity_Types
    {
        Standard,
        Exclusive,
        Legendary
    }

    public Target_Types Target;
    public Rarity_Types Rarity;
    Color color;
    string parentName;


    private void Start()
    {
        
        

        if(transform.parent.childCount>1)
        {
            Destroy(transform.parent.GetChild(0).gameObject);
        }
        changeDicePosition = FindObjectOfType<ChangeDicePosition>();
        Main = FindObjectOfType<Main>();
        parentName = transform.parent.name;
        gameObject.GetComponent<Animator>().Play("DiceCreate");
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.5f, 1.5f);
        positions = new List<Vector3>(); // create new empty list
        Field = GameObject.Find("Field");
        DiceSprite = GetComponent<SpriteRenderer>();
        // or with for loop syntax:

        for (int dotindex = 0; dotindex < transform.childCount; dotindex++)
        {
            positions.Add(transform.GetChild(dotindex).transform.position);
            //print(positions[dotindex]);
        }
        _currentPosition = transform.position;
        DiceSorting();
    }

    int getIndex(GameObject Dice)
    {
        
        // Получаем имя текущего объекта
        string objectName = Dice.transform.parent.name;

        // Разделяем имя объекта по символу "_"
        string[] parts = objectName.Split('_');

        string numberAsString = parts[1];

        // Преобразуем строку в целое число
        if (int.TryParse(numberAsString, out int number))
        {
            index = number;
        }
        return index;
    }
 
    public void DiceSorting()
    {
        string[] splitArray = transform.parent.name.Split(char.Parse("_"));
        int parentNumber = int.Parse(splitArray[1]);

        if (_drag)
        {
            SetSortingOrder(6, 7);
        }
        else
        {
            if (parentNumber < 6)
            {
                SetSortingOrder(0, 1);
            }
            else if (parentNumber < 11)
            {
                SetSortingOrder(2, 3);
            }
            else
            {
                SetSortingOrder(4, 5);
            }
        }
    }
    private void SetSortingOrder(int parentSortingOrder, int childSortingOrder)
    {
        DiceSprite.sortingOrder = parentSortingOrder;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = childSortingOrder;
        }
    }


    void ChangeDiceDotColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            //gameObject.GetComponent<SpriteRenderer>().color = color;
            for (int i = 0; i <= transform.childCount - 1; i++)
            {
                gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().color = color;
            }
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            
            //changeDicePosition.ChangePosition(13, 1);


        }

        if (GameObject.FindGameObjectWithTag("Enemy") && !_shooting)
        {
            StartCoroutine(Shoot());
            _shooting = true;
        }
        if (_returning)
        {
            ReturnPosition();
        }
    }

    private IEnumerator Shoot()
    {
        int rnd = Random.Range(0, transform.childCount); // Patahakan tiv doteri qanakov
        Instantiate(Bullet, transform.parent); // Bulleti stexcum parenti tochkic

        GameObject rb = transform.parent.GetChild(transform.parent.childCount - 1).gameObject; // Amen nor stexcvac bulleti rb stanal

        rb.transform.position = positions[rnd];

        transform.GetChild(rnd).GetComponent<Transform>().localScale = new Vector2(1.2f, 1.2f); // Doteri size i mecacum krakelu jamanak
        StartCoroutine(ReturnDotSize(rnd)); // Doteri resize
        // print($"{DiceName} ReloadTime: {DicePlayerPrefs.GetReloadTime(DiceName)}");
        yield return new WaitForSeconds(DicePlayerPrefs.GetReloadTime(DiceName)); // Bulletneri zarijat
        _shooting = false; // Chkrakel
    }

    private IEnumerator ReturnDotSize(int rnd)
    {
        yield return new WaitForSeconds(0.2f);
        transform.GetChild(rnd).GetComponent<Transform>().localScale = new Vector2(1, 1);
    }

    private void OnMouseDrag()
    {

        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = objPosition;
        DiceSorting();

        _drag = true;
        if (!isTriggerStay)
        {
            ResizeCollider(0.8f, 0.8f);
        }
        gameObject.GetComponent<Animator>().Play("DiceReSizeIn");

        GameObject parent = transform.parent.parent.gameObject;
        for (int i = 0; i <= parent.transform.childCount - 1; i++)
        {
            if (parent.transform.GetChild(i).gameObject.name == parentName)
            {
                //print("vrov ancav: " + parent.transform.GetChild(i).gameObject.name);

                continue;
            }
            if (parent.transform.GetChild(i).childCount > 0)
            {
                GameObject Dice = parent.transform.GetChild(i).transform.GetChild(0).gameObject;

                if (
                    Dice.GetComponent<Dice>().DiceName != DiceName
                        && Dice.GetComponent<Dice>().Grade == Grade
                    || Dice.GetComponent<Dice>().DiceName == DiceName
                        && Dice.GetComponent<Dice>().Grade != Grade
                    || Dice.GetComponent<Dice>().DiceName != DiceName
                )
                {
                    Dice.GetComponent<Animator>().Play("DiceFadeIn");
                    Dice.GetComponent<Dice>().ChangeDiceDotColor("#C8C8C8");
                }
            }
        }
    }

    private void OnMouseUp()
    {
        
        DiceSorting();
        _drag = false;
        if (_canCombine)
        {
            if(DiceName != "Monkey")
            {
                Destroy(_dice.gameObject);
            }
            
            Combine();
        }
        else
        {
            _returning = true;
        }
        isTriggerStay = false;
        if (!isTriggerStay)
        {
            ResizeCollider(1.5f, 1.5f);
        }
        gameObject.GetComponent<Animator>().Play("DiceReSizeOut");

        GameObject parent = transform.parent.parent.gameObject;
        for (int i = 0; i <= parent.transform.childCount - 1; i++)
        {
            if (parent.transform.GetChild(i).gameObject.name == parentName)
            {
                continue;
            }
            if (parent.transform.GetChild(i).childCount > 0)
            {
                GameObject Dice = parent.transform.GetChild(i).transform.GetChild(0).gameObject;
                
                if (
                    Dice.GetComponent<Dice>().DiceName != DiceName
                        && Dice.GetComponent<Dice>().Grade == Grade
                    || Dice.GetComponent<Dice>().DiceName == DiceName
                        && Dice.GetComponent<Dice>().Grade != Grade
                    || Dice.GetComponent<Dice>().DiceName != DiceName
                )
                {
                    Dice.GetComponent<Animator>().Play("DiceFadeOut");
                    Dice.GetComponent<Dice>().ChangeDiceDotColor("#FFFFFF");
                }
            }
        }

    }

    void ResizeCollider(float x, float y)
    {
        GameObject parent = transform.parent.parent.gameObject;
        for (int i = 0; i <= parent.transform.childCount - 1; i++)
        {
            if (parent.transform.GetChild(i).childCount > 0)
            {
                GameObject Dice = parent.transform.GetChild(i).transform.GetChild(0).gameObject;
                Dice.GetComponent<BoxCollider2D>().size = new Vector2(x, y);
            }
        }
    }

    void ResizeScale(float x, float y)
    {
        transform.localScale = new Vector2(x, y);
    }

    private void ReturnPosition() // Dice i plavni het gnal
    {
        if (Vector2.Distance(transform.position, _currentPosition) > 0.1f)
        {
            transform.Translate((_currentPosition - transform.position) * 0.3f);
        }
        else
        {
            transform.position = _currentPosition;
            _returning = false;
            DiceSorting();
        }
    }

    private void OnTriggerStay2D(Collider2D collision) // Combine
    {
        if (collision.gameObject.CompareTag("Dice"))
        {
            if (_drag
                && collision.GetComponent<Dice>().Grade == GetComponent<Dice>().Grade
                && GetComponent<Dice>().DiceName == "Monkey"
                && Grade != 7)
            {
                _dice = collision.gameObject;
                DraggedDiceName = collision.GetComponent<Dice>().DiceName;
                _canCombine = true;
                isTriggerStay = true;
                collision.GetComponent<BoxCollider2D>().size = new Vector2(1.1f, 1.1f);
                return;
            }
            if (
                _drag
                && collision.GetComponent<Dice>().Grade == GetComponent<Dice>().Grade
                && collision.GetComponent<Dice>().DiceName == GetComponent<Dice>().DiceName
                && Grade != 7
            )
            {
                _dice = collision.gameObject;
                DraggedDiceName = collision.GetComponent<Dice>().DiceName;
                _canCombine = true;
                isTriggerStay = true;
                collision.GetComponent<BoxCollider2D>().size = new Vector2(1.1f, 1.1f);
            }
        }
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Physics2D.IgnoreLayerCollision(8, 9);
        }

        //0,-1,-2   -10
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Dice CollisionDice = collision.GetComponent<Dice>();
        _canCombine = false;
        if (
            isTriggerStay
            && CollisionDice.Grade == GetComponent<Dice>().Grade
            && CollisionDice.DiceName == GetComponent<Dice>().DiceName
            && Grade != 7
        )
        {
            collision.GetComponent<BoxCollider2D>().size = new Vector2(0.8f, 0.8f);
        }

        isTriggerStay = false;
    }

    private void Combine()
    {
        index_1 = getIndex(gameObject);
        index_2 = getIndex(_dice);

        
        int currentDiceIndexInArray = Main.GetCurrentDiceIindexFromArray(DiceName);
        if (DiceName == "Wolf")
        {
            print("mtav ste");
            int wolfIndex;
            wolfIndex = currentDiceIndexInArray + 5 * Grade;

            GenerateDiceWithCombine(wolfIndex);
            return;
        }
        if(DiceName == "Monkey" && DraggedDiceName != "Monkey")
        {
            _dice.GetComponent<Animator>().Play("DiceFadeOut");
            changeDicePosition.ChangePosition(index_1, index_2, _currentPosition);
            SortingOrderAmenSharqiHamar();
            return;
        }

        print("hasav ste");
        int lowerBound = 5 * Grade; // Grade եթե 4 անոց ըլնի կանի 5*4 այսինքն ռանդոմի առաջի թիվը կանի 20-ից
        int upperBound = 5 * (Grade + 1); // հետո կանի 4+1=5 -> 5*5=25 այսինքն 25 կանի ռանդոմից երկրորդ թիվը 20-25 միջակայք

        int d_rnd = Random.Range(lowerBound, upperBound);

        GenerateDiceWithCombine(d_rnd);


    }

    void GenerateDiceWithCombine(int index)
    {
        Main main = Camera.main.GetComponentInChildren<Main>();
        Instantiate(main.CurrentDices[index], _dice.transform.parent);

        Destroy(gameObject);

        UpdateSpriteSortingOrder();
    }

    public void UpdateSpriteSortingOrder()
    {
        // Обновите сортировку спрайтов для текущего объекта
        GetComponent<SpriteRenderer>().sortingOrder = 0;

        // Обновите сортировку спрайтов для дочерних объектов
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
    }

    void SortingOrderAmenSharqiHamar()
    {
        for (int i = 0; i < 15; i++)
        {
            try
            {
                Transform child = Field.transform.GetChild(i).GetChild(0);

                if (child && child.childCount > 0)
                {
                    int parentSortingOrder = i / 5 * 2;
                    int childSortingOrder = parentSortingOrder + 1;

                    child.GetComponent<SpriteRenderer>().sortingOrder = parentSortingOrder;
                    for(int j = 0; j < child.transform.childCount; j++)
                    {
                        child.GetChild(j).GetComponent<SpriteRenderer>().sortingOrder = childSortingOrder;
                    }
                    
                }
            }
            catch { }
        }
    }

}
