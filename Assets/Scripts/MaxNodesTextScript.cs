using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxNodesTextScript : MonoBehaviour {

    public Text thisText;

    public void ChangeCounter(int changeBy)
    {
        int lastNumber = int.Parse(thisText.text);
        
        lastNumber += changeBy;
        if(lastNumber >= 0)
            thisText.text = lastNumber.ToString();
    }
}
