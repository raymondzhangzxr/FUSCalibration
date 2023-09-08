// May 2023
// Augmented Mirror: CSV logging utility file
// Ray Zhang xzhan227@jh.ede
// Modified from https://gist.github.com/julenka/20ece5141e821cb6d0a9cb530d3dc96d

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if WINDOWS_UWP
using Windows.Storage;
#endif

namespace MyCSVUtils
{
    /// <summary>
    /// Component that Logs data to a CSV.
    /// Assumes header is fixed.
    /// Copy and paste this logger to create your own CSV logger.
    /// CSV Logger breaks data up into settions (starts when application starts) which are folders
    /// and instances which are files
    /// A session starts when the application starts, it ends when the session ends.
    /// 
    /// In Editor, writes to MyDocuments/SessionFolderRoot folder
    /// On Device, saves data in the Pictures/SessionFolderRoot
    /// 
    /// How to use:
    /// Find the csvlogger
    /// if it has not started a CSV, create one.
    /// every frame, log stuff
    /// Flush data regularly
    /// 
    /// **Important: Requires the PicturesLibrary capability!**
    /// </summary>
    public class CSVLogger : MonoBehaviour

    {
        
        #region Constants to modify
        private const string SessionFolderRoot = "CSVLogger";
        
        #endregion

        #region private members
        private string m_sessionPath;
        private string m_filePath;
        private string m_recordingId;
        private string m_sessionId;

        private StringBuilder m_csvData;
        #endregion
        #region public members
        public string RecordingInstance => m_recordingId;
        public string DataSuffix = "data";
        public string CSVHeader = "Timestamp,SessionID,RecordingID," +
                                        "ObjectName,TransX,TransY,TransZ,RotX,RotY,RotZ,RotW";
        public bool isCSV = false; // True if a new csv file is created
        // For DeBugging 
        //public GameObject textOb;

        // private static System.IO.TextWriter Synchronized(System.IO.TextWriter csvWriter);
        #endregion

        // Use this for initialization
        async void Start()
        {
            await MakeNewSession();
        }

        async Task MakeNewSession()
        {
            m_sessionId = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string rootPath = "";
#if WINDOWS_UWP
            
            rootPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, SessionFolderRoot);
            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
#else
            rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SessionFolderRoot);
            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
#endif
            m_sessionPath = Path.Combine(rootPath, m_sessionId);
            Directory.CreateDirectory(m_sessionPath);
            Debug.Log("CSVLogger logging data to " + m_sessionPath);
            
        }

        public void StartNewCSV()
        {
            m_recordingId = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            var filename = m_recordingId + "-" + DataSuffix + ".csv";
            m_filePath = Path.Combine(m_sessionPath, filename);
            if (m_csvData != null)
            {
                EndCSV();
            }

            m_csvData = new StringBuilder();
            m_csvData.AppendLine(CSVHeader);
            isCSV = true;

            // Debuging output
            //TextMesh t = textOb.GetComponent<TextMesh>();
            //t.text = "New CSV Made " + m_filePath;
        }


        public void EndCSV()
        {
            if (m_csvData == null)
            {
                return;
            }
            using (var csvWriter = new StreamWriter(m_filePath, true))
            {
                csvWriter.Write(m_csvData.ToString());
            }
            m_recordingId = null;
            m_csvData = null;
            isCSV = false;
        }

        public void OnDestroy()
        {
            EndCSV();
        }

        public void AddRow(List<String> rowData)
        {
            AddRow(string.Join(",", rowData.ToArray()));
        }

        public void AddRow(string row)
        {
            m_csvData.AppendLine(row);
        }

        /// <summary>
        /// Writes all current data to current file
        /// </summary>
        public void FlushData()
        {
            using (var csvWriter = new StreamWriter(m_filePath, true))
            {

                csvWriter.Write(m_csvData.ToString());
            }

            m_csvData.Clear();
        }

        /// <summary>
        /// Returns a row populated with common start data like
        /// timestamp
        /// </summary>
        /// <returns></returns>
        public List<String> RowWithStartData()
        {
            List<String> rowData = new List<String>();
            rowData.Add(Time.timeSinceLevelLoad.ToString("##.000"));
            return rowData;
        }

    }
}