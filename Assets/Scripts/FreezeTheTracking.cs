// May 2023
// Augmented Mirror: Freeze/Unfreeze the tracking for spine and mirror to make the view stable
// Ray Zhang xzhan227@jh.ede


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeTheTracking : MonoBehaviour
{
    public GameObject[] objectsToFreeze = new GameObject[2]; // assign the objects to freeze here
    public GameObject freezeStatusText;
    private bool isTracking = true;

    void Start()
    {
        Debug.Log("Tracking freezer started!");

    }


    public void FreezeUnFreezeTracking()
    {
       
        foreach(GameObject ob in objectsToFreeze)
        {
            IRToolTrack.IRToolController script = ob.GetComponent<IRToolTrack.IRToolController>();
            if (script != null)
            {
                if (isTracking)
                {
                    script.StopTracking();
                    Debug.Log("Tracking Stopped for" + ob.name);
                }
                else
                {
                    script.StartTracking();
                    Debug.Log("Tracking Started for" + ob.name);
                }
                
            }
            else
            {
                Debug.LogError("IRToolController is not found on" + ob.name);
            }
        }

        isTracking = !isTracking;
        UpdateText();
        
    }

    private void UpdateText()
    {
        TextMesh t = freezeStatusText.GetComponent<TextMesh>();
        if (isTracking)
        {
            t.text = "Mirror and Spine are Tracked";
        }
        else
        {
            t.text = "Tracking is Freezed!";
        }
        
    }
}
