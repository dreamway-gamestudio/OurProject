using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDicePosition : MonoBehaviour
{
    public GameObject Field;
    GameObject Dice_1, Dice_2;
    Transform dice_1_transform, dice_2_transform;
    Transform originalParentDice1, originalParentDice2;

    Vector3 originalPositionDice1;
    Dice dice_script;
    private void Start()
    {
        dice_script = FindObjectOfType<Dice>();
    }

    //bool HasDiceInPosition(int index)
    //{
    //    if (index >= 1 && index <= 15)
    //    {
    //        Transform positionTransform = Field.transform.GetChild(index - 1);
    //        if (positionTransform.childCount > 0 && positionTransform.GetChild(0).CompareTag("Dice"))
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}
    public void ChangePosition(int index_1, int index_2, Vector3 dice1Pos)
    {
        //bool hasDiceInPosition1 = HasDiceInPosition(index_1);
        //bool hasDiceInPosition13 = HasDiceInPosition(index_2);

        //if (!hasDiceInPosition1 || !hasDiceInPosition13)
        //{
        //    Debug.Log("Не хватает Dice для перемещения.");
        //    return;
        //}

        int childCount = Field.transform.childCount;

        // Проверяем, достаточно ли дочерних объектов для выполнения операции
        if (childCount >= Mathf.Max(index_1, index_2))
        {
            for (int i = 0; i < childCount; i++)
            {
                if (i == index_1 - 1)
                {
                    if (Field.transform.GetChild(i).childCount > 0)
                    {
                        Dice_1 = Field.transform.GetChild(i).GetChild(0).gameObject;
                        dice_1_transform = Dice_1.transform;
                        originalParentDice1 = Dice_1.transform.parent;
                        originalPositionDice1 = Dice_1.transform.position;
                    }
                }
                if (i == index_2 - 1)
                {
                    if (Field.transform.GetChild(i).childCount > 0)
                    {
                        Dice_2 = Field.transform.GetChild(i).GetChild(0).gameObject;
                        dice_2_transform = Dice_2.transform;
                        originalParentDice2 = Dice_2.transform.parent;
                    }
                }
            }

            // Поменять местами родительские объекты
            if (originalParentDice1 != null && originalParentDice2 != null)
            {
                if (dice_1_transform != null)
                {
                    // Сохранить позиции
                    Vector3 tempPosition = dice_1_transform.position;

                    // Переместить Dice_1 на позицию Dice_2
                    dice_1_transform.position = dice_2_transform.position;
                    Dice_1.transform.SetParent(originalParentDice2);
                    Dice_1.GetComponent<Dice>()._currentPosition = dice_2_transform.position;

                    // Восстановить позицию Dice_2
                    if (dice_2_transform != null)
                    {
                        dice_2_transform.position = dice1Pos;
                        Dice_2.transform.SetParent(originalParentDice1);
                        Dice_2.GetComponent<Dice>()._currentPosition = dice1Pos;

                    }
                }
            }
        }
        else
        {
            // Если не хватает дайсов, создаем дополнительные
            while (childCount < Mathf.Max(index_1, index_2))
            {
                // Создаем новый дайс
                GameObject newDice = new GameObject("Dice");
                newDice.transform.SetParent(Field.transform.GetChild(0)); // Возможно, вам нужно изменить эту логику
                childCount++;
            }
        }
    }

    private void Update()
    {
       
    }
}
