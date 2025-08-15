using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    Slider Slider;
    public EnemyMove EnemyMove;
    void Start()
    {
        Slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        Slider.value = (float)EnemyMove.hp/(float)EnemyMove.startHP;
    }
}
