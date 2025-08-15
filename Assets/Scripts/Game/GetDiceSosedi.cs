using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDiceSosedi : MonoBehaviour
{
    public GameObject[,] grid; // Двумерный массив для представления игрового поля

    void Start()
    {
        // Инициализация grid, например:
        grid = new GameObject[3, 5]; // Ваши размеры игрового поля
                                     // Заполнение grid вашими GameObjects
    }
    public List<GameObject> GetNeighbors(int playerIndex)
    {
        List<GameObject> neighbors = new List<GameObject>();

        int numRows = grid.GetLength(0);
        int numColumns = grid.GetLength(1);

        // Находим координаты игрока по его индексу
        int row = playerIndex / numColumns;
        int column = playerIndex % numColumns;

        // Проверяем соседние позиции слева, справа, сверху и снизу
        int[] neighborRows = { row - 1, row + 1, row, row };
        int[] neighborColumns = { column, column, column - 1, column + 1 };

        for (int i = 0; i < 4; i++)
        {
            int neighborRow = neighborRows[i];
            int neighborColumn = neighborColumns[i];

            // Проверяем, находятся ли соседи в пределах игрового поля
            if (neighborRow >= 0 && neighborRow < numRows && neighborColumn >= 0 && neighborColumn < numColumns)
            {
                GameObject neighbor = grid[neighborRow, neighborColumn];
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            print("esr");
            List<GameObject> neighbors = GetNeighbors(5);

            // Пройдитесь по всем соседям и выведите их имена
            foreach (var neighbor in neighbors)
            {
                if (neighbor != null)
                {
                    Debug.Log("Сосед: " + neighbor.name);
                }
            }
        }
    }

}
