// May 2023
// Augmented Mirror: Log objects pos/orientation at current timestamp
// Ray Zhang xzhan227@jh.ede


using System.Collections.Generic;
using UnityEngine;



public class InstanceLogger : MonoBehaviour
{
    // Using the logger
    private MyCSVUtils.CSVLogger logger;

    public bool isLogging = false;

    public GameObject[] objectsToLog = new GameObject[3]; // assign your targets here in Unity Inspector
    // each target we save 3 instance so 1,2,3 for target 1 and so on
    private int instanceNum = 1;
    private int targetNum = 1;
    public int totalTargetNum = 10;
    public GameObject displayText;

    void Start()
    {
        logger = GetComponent<MyCSVUtils.CSVLogger>();
        if (logger == null)
        {
            logger = gameObject.AddComponent<MyCSVUtils.CSVLogger>();
            logger.DataSuffix = "Target" + targetNum + "Instance" + instanceNum;
            logger.CSVHeader = "Timestamp," +
                                        "ObjectLogging,TransX,TransY,TransZ,RotX,RotY,RotZ,RotW";
        }

    }


    // Update the logging file name
    void UpdateLogger()
    {

        logger.DataSuffix = "Target" + targetNum + "Instance" + instanceNum;
            
    }

    // Log all objects we want 
    void LogObjects()
    {

        logger.StartNewCSV();
        foreach (GameObject ob in objectsToLog)
        {
            LogAObject(ob);
        }
        logger.FlushData();
        logger.EndCSV();
        instanceNum++;
        UpdateText();
        UpdateLogger();


    }

    // Log a single object (Pos and Orientation)
    void LogAObject(GameObject ob)
    {
        if (ob != null)
        {
            Vector3 position = ob.transform.position;
            Quaternion rotation = ob.transform.rotation;
            

            // Create a new row of data to log
            List<string> rowData = logger.RowWithStartData();

            rowData.Add(ob.name); // Add the name of the game object

            rowData.Add(position.x.ToString()); // Add the x-coordinate of the position
            rowData.Add(position.y.ToString()); // Add the y-coordinate of the position
            rowData.Add(position.z.ToString()); // Add the z-coordinate of the position
            rowData.Add(rotation.x.ToString()); // Add the x-component of the rotation quaternion
            rowData.Add(rotation.y.ToString()); // Add the y-component of the rotation quaternion
            rowData.Add(rotation.z.ToString()); // Add the z-component of the rotation quaternion
            rowData.Add(rotation.w.ToString()); // Add the w-component of the rotation quaternion

            logger.AddRow(rowData); // Add the row of data to the CSV file
        }
       
    }


    // Update the task displayed on top of the mirror to tell the user what's the next task
    public void UpdateText()
    {
        TextMesh t = displayText.GetComponent<TextMesh>();
        switch (instanceNum)
        {
            case 1:
                t.text = "Task: Position";
                break;
            case 2:
                t.text = "Task: Orientation";
                break;
            case 3:
                t.text = "Task: Insertion";
                break;
            default:
                t.text = "Stop Recording and Go to the next target";
                break;

        }
        

    }

    // This function is called when target is changed
    public void UpdateTargetNumber()
    {
        targetNum = (targetNum + 1) % totalTargetNum;
        instanceNum = 1;
        UpdateText();
        UpdateLogger();
    }


    // This function is called when loging instance is demanded
    public void LogInstance()
    {
        // We only log the first three (pos, orientation, insertion)
        if(instanceNum >= 4)
        {
            UpdateText();
            Debug.Log("Instance logging Skipped for instance" + instanceNum);
        }
        else
        {
            Debug.Log("Instance logged target" + targetNum + "with Instance " + instanceNum);

            LogObjects();

        }

    }
}
