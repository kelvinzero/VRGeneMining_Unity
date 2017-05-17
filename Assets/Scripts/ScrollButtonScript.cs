using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollButtonScript : MonoBehaviour {

    public bool upArrow = true;
    public Scrollbar scrollbar;
    bool buttonDown = false;
    float timeDown;

    public void ArrowButtonClicked()
    {  
            // Debug.Log("Button down: " + name);
            timeDown = Time.time;
            buttonDown = true;
            scrollbar.value += upArrow ? 0.3f : -0.3f;
    }

}
