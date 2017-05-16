using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger {

    private List<string> logEntries = new List<string>();

    
    public void Log(string str)
    {
        logEntries.Add(str);
    }

    public void Display()
    {
        foreach (string str in logEntries)
        {
            Debug.Log(str);
        }
        logEntries.Clear();
    }

    public void ConcatAndFlush(Logger logger)
    {
        foreach (string str in logger.logEntries)
        {
            Log(str);
        }
        logger.logEntries.Clear();
    }

}
