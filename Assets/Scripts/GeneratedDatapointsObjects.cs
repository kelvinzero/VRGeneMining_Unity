using Assets.Scripts.DataHandling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratedDatapointsObjects : MonoBehaviour {


    public GameObject           GenAttributeContentHolder;
    public GameObject           GenAttributeContentItem;
    public GameObject           MyRecordObject;
    public string[]             Record;
    public List<DataTypes>[]    DataTypes;
    public int                  RecordNumber;

    public void AddAttributesToScroller()
    {
        
        for(int i = 0; i < Record.Length; i++)
        {
            GameObject newAttributeBox = Instantiate(GenAttributeContentItem, GenAttributeContentHolder.transform);
            newAttributeBox.GetComponentInChildren<Text>().text = Record[i];
            newAttributeBox.SetActive(true);
        }
    }
}
