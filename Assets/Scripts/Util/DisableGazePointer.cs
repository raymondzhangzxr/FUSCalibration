using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableGazePointer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
