using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecordActions : Singleton<RecordActions>
{
    private readonly string m_saveLocation = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\AiPokeClone";
    private string m_fileName;

    private string[] m_actions;

    protected override void InternalInit()
    {
        // Create the folder if it doesn't exist
        if (!Directory.Exists(m_saveLocation))
        {
            Directory.CreateDirectory(m_saveLocation);
        }
    }

    public void OnStartBattle()
    {
        DateTime now = DateTime.Now;
        m_fileName = $"battle_{now.Day}_{now.Month}_{now.Year}_{now.TimeOfDay.Hours}{now.TimeOfDay.Minutes}{now.TimeOfDay.Seconds}";

        // Create the file
        FileStream outStream = File.Create($"{m_saveLocation}\\{m_fileName}.csv");
    }

}
