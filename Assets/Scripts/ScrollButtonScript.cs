using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollButtonScript : MonoBehaviour {

    public bool upArrow = true;
    public Scrollbar scrollbar;
    public float scale = 0.3f;
 
    public void ArrowButtonClicked()
    {
        scrollbar.value += upArrow ? scale : -scale;
    }


}
