using Assets.Scripts.DataHandling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateAttributeItemController : MonoBehaviour {

    public int                  AttributeNumber;
    public Text                 AttributeName;
    public Text                 AttributeValue;
    public List<DataTypes>      AttributeType;
    public GameObject           UpArrow;
    public GameObject           DownArrow;
    public GameObject           RandomButton;
    public double[]             MinMax;
   

    private void Start()
    {
        
    }
    public void ArrowChangeAttributeValuePressed(bool upArrow)
    {
        if(AttributeType.Contains(DataTypes.NUMERIC))
        {
            double currentValue = double.Parse(AttributeValue.text);
            Debug.Log("Current value text " + currentValue.ToString());
            double increment = (MinMax[1] - MinMax[0]) / 20.000d;
            Debug.Log("Current value increment " + increment.ToString());

            if (upArrow)
            {
                Debug.Log("Uparrow pressed " + MinMax[0] + " : " + MinMax[1]);
                currentValue += increment;
                if (currentValue > MinMax[1])
                    currentValue = MinMax[1];
            }
            else
            {
                Debug.Log("Downarrow pressed " + MinMax[0] + " : " + MinMax[1]);
                currentValue -= increment;
                if (currentValue < MinMax[0])
                    currentValue = MinMax[0];
            }
            AttributeValue.text = currentValue.ToString();
        }
    }

    public void RandomizeAttributeValuePressed()
    {
        if(AttributeType.Contains(DataTypes.NUMERIC))
        {
            double randomVal = Random.Range((float)MinMax[0], (float)MinMax[1]);
            AttributeValue.text = randomVal.ToString();
        }
    }
}
