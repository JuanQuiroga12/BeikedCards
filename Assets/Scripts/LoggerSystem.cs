using UnityEngine;
using System;
using System.IO;

public class LoggerSystem : MonoBehaviour
{
    private string logFilePath;
    private StreamWriter logWriter;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        // Crear carpeta logs si no existe
        string directory = Path.Combine(Application.persistentDataPath, "Logs");
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        // Crear archivo log con fecha y hora
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        logFilePath = Path.Combine(directory, $"log_{timestamp}.txt");

        // Iniciar el logger
        logWriter = new StreamWriter(logFilePath, true);
        logWriter.AutoFlush = true;

        // Suscribirse a eventos de log
        Application.logMessageReceived += HandleLog;

        // Log inicial
        WriteToLog("=== INICIO DE SESIÓN ===");
        WriteToLog($"Versión: {Application.version}, Plataforma: {SystemInfo.deviceModel}");
    }

    private void HandleLog(string message, string stackTrace, LogType type)
    {
        string prefix = type == LogType.Error || type == LogType.Exception ? "ERROR: " :
                      type == LogType.Warning ? "WARN: " : "INFO: ";

        WriteToLog($"{prefix}{message}");

        if (type == LogType.Error || type == LogType.Exception)
            WriteToLog(stackTrace);
    }

    private void WriteToLog(string message)
    {
        string timeStamp = DateTime.Now.ToString("HH:mm:ss.fff");
        logWriter.WriteLine($"[{timeStamp}] {message}");
    }

    private void OnDestroy()
    {
        WriteToLog("=== FIN DE SESIÓN ===");

        // Limpiar recursos
        Application.logMessageReceived -= HandleLog;
        logWriter.Close();
        logWriter.Dispose();
    }

    // Puedes añadir este método para acceder al archivo de logs
    public string GetLogFilePath()
    {
        return logFilePath;
    }
}
