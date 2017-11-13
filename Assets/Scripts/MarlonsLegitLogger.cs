using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MarlonsLegitLogger
{
#region singleton
    private static MarlonsLegitLogger instance;

    private MarlonsLegitLogger()
    {
        addTimestamp = false;
    }

    public static MarlonsLegitLogger Instance
    {
        get
        {
            if (instance == null)
            { instance = new MarlonsLegitLogger(); }
            return instance;
        }
    }
#endregion

    private string fileLocation;
    public string FileLocation
    {
        get { return fileLocation; }
        set { fileLocation = value; }
    }

    private string fileName;
    public string FileName
    {
        get { return fileName; }
        set { fileName = value; }
    }

    private bool addTimestamp;

    private string message;

    private StreamWriter stream;

    public void Log(string text)
    {
        WriteLog(text);
    }

    public void Heading(string text)
    {
        WriteLog("//=============================================\\");
        WriteLog("||=============================================||");
        WriteLog("       " + text);
        WriteLog("||=============================================||");
        WriteLog("\\=============================================//");
    }

    public void SubHeading(string text)
    {
        WriteLog("//=====================================\\");
        WriteLog("       " + text);
        WriteLog("\\=====================================//");
    }

    private void WriteLog(string text)
    {
        message = string.Empty;
        if (addTimestamp)
        { message = System.DateTime.Now.ToShortTimeString(); }
        message += text;
        if (stream == null)
        { CreateStream(); }
        if (stream != null)
        { stream.WriteLine(message); }
    }

    public void CreateStream()
    {
        if (stream != null)
        { stream.Close(); }
        if (fileLocation != null && fileName != null)
        { stream = new StreamWriter(fileLocation + fileName); }
    }

    public void CloseStream()
    {
        if (stream != null)
        { stream.Close(); }
    }
}
