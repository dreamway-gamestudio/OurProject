using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int numRows = 3;
    public int numColumns = 5;

    private void Start()
    {
        //for(int i = 0; i<= GetNeighbors(10).Length-1; i++)
        //{
        //    print(GetNeighbors(10)[i]);
        //}
        
    }
    public int[] GetNeighbors(int index)
    {
        int row = index / numColumns;
        int col = index % numColumns;

        int[] neighbors = new int[4];
        int count = 0;

        // Левый сосед
        if (col > 0)
            neighbors[count++] = index - 1;

        // Правый сосед
        if (col < numColumns - 1)
            neighbors[count++] = index + 1;

        // Верхний сосед
        if (row > 0)
            neighbors[count++] = index - numColumns;

        // Нижний сосед
        if (row < numRows - 1)
            neighbors[count++] = index + numColumns;

        // Обрезаем массив до количества соседей
        int[] result = new int[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = neighbors[i];
        }

        return result;
    }
}
