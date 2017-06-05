using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExclusiveToggle : MonoBehaviour {

    public Toggle otherToggle;
    Toggle thisToggle;

    private void Start()
    {
        thisToggle = GetComponent<Toggle>();
    }

    public void ToggleChanged()
    {
        if (thisToggle.isOn)
            otherToggle.isOn = true;
        else
            otherToggle.isOn = false;
    }

}
