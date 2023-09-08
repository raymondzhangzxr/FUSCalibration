using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NavigationLineRenderer : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public GameObject distanceText;

    [Header("Appearance")]
    public Color lineStartColor = Color.green;
    public Color lineEndColor = Color.red;
    public float lineWidth = 0.1f;

    private LineRenderer lineRenderer;
    private TextMesh textPrint;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        // Set initial properties for the LineRenderer
        lineRenderer.startColor = lineStartColor;
        lineRenderer.endColor = lineEndColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 2; // We're drawing a line between two points
        textPrint = distanceText.GetComponent<TextMesh>(); // text
    }

    void Update()
    {
        DrawLine();
        UpdateDistance();
    }

    private void DrawLine()
    {
        if (startPoint && endPoint)
        {
            lineRenderer.SetPosition(0, startPoint.position);
            lineRenderer.SetPosition(1, endPoint.position);
        }
    }

    private void UpdateDistance()
    {
        Vector3 directionToTarget = startPoint.position - endPoint.position;
        float distance = directionToTarget.magnitude;
        textPrint.text = "Distance: " + distance.ToString("F2") + " units";
    }
       
        

}
