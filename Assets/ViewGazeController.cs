using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.VR;

public class ViewGazeController : MonoBehaviour {

    bool _vrActivated;
    bool _waitForButtonUp;
    bool _waitForDeselect;
    GameObject _targetSelected;
    GameObject _lastTargetSelected;
    public GameObject LayoutPanel;


    // Use this for initialization
    void Start()
    {
        _vrActivated = VRSettings.isDeviceActive;
        _waitForButtonUp = false;
        _waitForDeselect = false;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        List<RaycastResult> targetsHit = new List<RaycastResult>();

        if (_vrActivated)
            pointer.position = new Vector2(VRSettings.eyeTextureWidth / 2, VRSettings.eyeTextureWidth / 2);
        else
            pointer.position = new Vector2(Screen.width / 2, Screen.height / 2);
        
        pointer.button = PointerEventData.InputButton.Left;

        EventSystem.current.RaycastAll(pointer, targetsHit);
        if (targetsHit.Count > 0)
        {
            Debug.Log("Hit " + targetsHit[0].gameObject.name);
            _targetSelected = targetsHit[0].gameObject;
            if (Input.GetKeyDown("space"))
            {
                if (_targetSelected.GetComponentInParent<ISubmitHandler>() != null)
                    _targetSelected.GetComponentInParent<ISubmitHandler>().OnSubmit(pointer);      
            }
        }
        Ray ray = GetComponentInParent<Camera>().ScreenPointToRay(pointer.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "node")
            {
                NodePhysX nodescript = hit.collider.GetComponentInParent<NodePhysX>();
                LayoutPanel.GetComponent<DatapointInfoPanelScript>().SetValues(nodescript.RecordValues);
            }
        }
    }
}
