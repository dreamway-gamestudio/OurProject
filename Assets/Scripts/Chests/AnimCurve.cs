using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class AnimCurve : MonoBehaviour
{
    GameObject LevelButton, TouchBlock;
    public AnimationCurve ChestAnimation;
    public Vector3 lerpOffset;
    public float lerpTime = 3f;
    public float _timer = 0f;
    bool _start_moving = false;
    Vector3 StartPos;

    /// <GetComponents>
    GameObject thisGameObject;
    Animator thisGOAnimator;
    Image thisGOImage;
    Canvas thisGOCanvas;
    TimerChest thisGOTimerChest;


    void OnEnable()
    {
        /// Find Objects
        thisGameObject = gameObject;

        thisGOAnimator = thisGameObject.GetComponent<Animator>();
        thisGOImage = thisGameObject.GetComponent<Image>();
        thisGOCanvas = thisGameObject.GetComponent<Canvas>();
        thisGOTimerChest = thisGameObject.GetComponent<TimerChest>();
        ///


        LevelButton = GameObject.Find("LevelButton");
        TouchBlock = GameObject.Find("TouchBlock");
        thisGOCanvas.overrideSorting = false;
        thisGOImage.enabled = false;
        StartPos = transform.position;
        if (PlayerPrefs.GetString(transform.parent.name) == transform.name)
        {
           
            thisGOImage.enabled = true;
            thisGOAnimator.enabled = false;
            if(thisGOTimerChest.isEnd)
            {
                thisGOAnimator.enabled = true;
                thisGOAnimator.Play("ChestDone");
            }
        }
        else
        {
            
            MoveChestAnim(); 
            thisGOAnimator.enabled = true;
        }
    }
    public void MoveChestAnim()
    {
        _timer = 0;
        lerpOffset = new Vector3(110f, 0f, 0f);
        _start_moving = true;
        TouchBlock.GetComponent<Image>().enabled = true;
        thisGOCanvas.overrideSorting = true;
    }
    
    void Update()
    {
        if (_start_moving)
        {
            _timer += Time.deltaTime;

            if (_timer > lerpTime)
            {
                _timer = lerpTime;
                lerpOffset = new Vector3(0f, 0f, 0f);
                _start_moving = false;
                TouchBlock.GetComponent<Image>().enabled = false;
                thisGOCanvas.overrideSorting = false;
                ReturnTextOfChild(0).enabled = true;
                ReturnTextOfChild(1).enabled = true;
                PlayerPrefs.SetString(transform.parent.name, transform.name);
            }
            float lerpRatio = _timer / lerpTime;
            Vector3 positionOffset = ChestAnimation.Evaluate(lerpRatio) * lerpOffset;
            transform.position = Vector3.Lerp(LevelButton.transform.position, StartPos, lerpRatio) + positionOffset;
            thisGOImage.enabled = true;
        }
    }

    Text ReturnTextOfChild(int child)
    {
        return thisGameObject.transform.GetChild(child).gameObject.GetComponent<Text>();
    }

}
