using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LogMessageReceived : MonoBehaviour
{
    StringBuilder m_logStr = new StringBuilder();
    string m_logFileSavePath;

    private void Awake()
    {
        //var t = System.DateTime.Now.ToString("yyyyMMddhhmmss");
        m_logFileSavePath = string.Format($"{Application.persistentDataPath}/Player.log");
        Debug.Log(m_logFileSavePath);
        Application.logMessageReceived += OnLogCallBack;
    }

    private void OnLogCallBack(string condition, string stackTrace, LogType type)
    {
        m_logStr.Append(condition);
        m_logStr.Append("\n");
        m_logStr.Append(stackTrace);
        m_logStr.Append("\n");

        if (m_logStr.Length <= 0) return;
        if (!File.Exists(m_logFileSavePath))
        {
            var fs = File.Create(m_logFileSavePath);
            fs.Close();
        }
        using (var sw = File.AppendText(m_logFileSavePath))
        {
            sw.WriteLine(m_logStr.ToString());
        }
        m_logStr.Remove(0, m_logStr.Length);
    }
}
