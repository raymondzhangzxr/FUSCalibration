// May 2023
// Augmented Mirror: Data logging utility file
// Ray Zhang xzhan227@jh.ede

// This script help to log mirror/syringe/spine/HMD/eye tracking data
// for every frame to corresponding csv files


using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;


public class ObjectsLogger : MonoBehaviour
{
    
    // the game objects to log
    public GameObject toolToLog; 
    public GameObject targetToLog;

    // Show the user the logging status
    public GameObject recordingStatusText;
    public float flushPeriod = 1.0f;


    // Welcome text
    public string welcomeText = "Please start recording!";

    // Flush once per second
    private float nextFlushTime = 0.0f;
    
    private bool isLogging = false;

    // To display recording status
    private TextMesh t;

    // Const strings
    private string toolFileName = "ToolTipLogging";
    private string targetFileName = "TargetLogging";
    private string posHeader = "ObjectLogging,TransX,TransY,TransZ,RotX,RotY,RotZ,RotW";

    // Using the logger
    private MyCSVUtils.CSVLogger toolLogger;
    private MyCSVUtils.CSVLogger targetLogger;
    
    // Update function for position/Orientation logging 
    void PosLoggerUpdate(GameObject go, MyCSVUtils.CSVLogger logger)
    {
        if (go != null)
        {
            Vector3 position = go.transform.position;
            Quaternion rotation = go.transform.rotation;

            // Create a new row of data to log
            List<string> rowData = logger.RowWithStartData();
            rowData.Add(go.name); // Add the name of the game object
            rowData.Add(position.x.ToString()); // Add the x-coordinate of the position
            rowData.Add(position.y.ToString()); // Add the y-coordinate of the position
            rowData.Add(position.z.ToString()); // Add the z-coordinate of the position
            rowData.Add(rotation.x.ToString()); // Add the x-component of the rotation quaternion
            rowData.Add(rotation.y.ToString()); // Add the y-component of the rotation quaternion
            rowData.Add(rotation.z.ToString()); // Add the z-component of the rotation quaternion
            rowData.Add(rotation.w.ToString()); // Add the w-component of the rotation quaternion

            logger.AddRow(rowData); // Add the row of data to the CSV file
        }

        if (isLogging && Time.time > nextFlushTime)
        {
            nextFlushTime = Time.time + flushPeriod;
            logger.FlushData();
        }
    }



    void Start()
    {

        // Set up three loggers using CSV logger base fucntion

        toolLogger = gameObject.AddComponent<MyCSVUtils.CSVLogger>();
        toolLogger.DataSuffix = toolFileName;
        toolLogger.CSVHeader = "Timestamp," + posHeader;

        targetLogger = gameObject.AddComponent<MyCSVUtils.CSVLogger>();
        targetLogger.DataSuffix = targetFileName;
        targetLogger.CSVHeader = "Timestamp," + posHeader;

        t = recordingStatusText.GetComponent<TextMesh>();
        t.text = welcomeText;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLogging)
        {
            PosLoggerUpdate(toolToLog, toolLogger);
            PosLoggerUpdate(targetToLog,targetLogger);
            
        }

    }

    public void ChangeLoggingStatus()
    {
        // if it's logging and button pressed
        if (isLogging)
        {
            StopLogging();
        }
        else
        {
            StartLogging();
        }
    }

    public void StartLogging()
    {
        
        t.text = "Logging started!";
        
        isLogging = true;
        Debug.Log("Tool logging started");
        toolLogger.StartNewCSV();
        toolLogger.FlushData();

        Debug.Log("Target logging started");
        targetLogger.StartNewCSV();
        targetLogger.FlushData();

        
    }

    public void StopLogging()
    {
        
        t.text = "Logging ended!";
        isLogging = false;

        Debug.Log("Tool logging ended");
        toolLogger.EndCSV();

        Debug.Log("Target logging ended");
        targetLogger.EndCSV();
    }



}
