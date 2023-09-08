using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NavigationLineRendererByCylinder : MonoBehaviour
{
 
    [Header("Offset")]
    public Vector3 toolTipOffset = Vector3.zero;  // This will be the offset from the c.g. of the shperes to the actual tip
    public Vector3 targetOffset = Vector3.zero;  

    [Header("Appearance")]
    public Color lineColor = Color.green;
    public float thickness = 0.01f;
    [Header("References")]
    public Transform toolTip;
    public Transform target;
    public GameObject distanceText;

    private GameObject cylinder;
    private TextMesh textPrint;

    void Start()
    {
        // Create a cylinder to act as the line
        cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.localScale = new Vector3(thickness, 1.0f, thickness);
        cylinder.GetComponent<Renderer>().material.color = lineColor;
        textPrint = distanceText.GetComponent<TextMesh>(); // text
    }

    void Update()
    {
        // Update the line position and scale based on the distance between the toolTip and target
        UpdateLine();
    }

    void UpdateLine()
    {
        if (toolTip && target)
        {
            Vector3 startPos = toolTip.position + toolTip.TransformDirection(toolTipOffset) ;
            Vector3 endPos = target.position + target.TransformDirection(targetOffset);
            
            Vector3 midPoint = (startPos + endPos) * 0.5f;

            cylinder.transform.position = midPoint;
            cylinder.transform.LookAt(startPos);
            cylinder.transform.Rotate(90, 0, 0);
            float distance = Vector3.Distance(startPos, endPos);
            // Show the distance to the textprint in cm with 2 significant values
            if(distance < 0.01)
            {
                textPrint.text = $"You are close to the Target: {(distance * 1000):F2} mm";
            }
            else
            {
                textPrint.text = $"Distance to Target: {(distance * 100):F2} cm";
            }
            

            cylinder.transform.localScale = new Vector3(thickness, distance * 0.5f, thickness); // Multiply by 0.5 because the cylinder's default height is 2
        }
    }
}

