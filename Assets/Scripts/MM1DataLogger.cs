using System;
using System.IO;
using System.Text;
using UnityEngine;

public class MM1DataLogger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private QueueManager queueManager;
    [SerializeField] private ArrivalProcess arrivalProcess;
    [SerializeField] private ServiceProcess serviceProcess;

    [Header("Logging Settings")]
    [SerializeField] private float loggingInterval = 5f;
    [SerializeField] private bool enableLogging = true;
    [SerializeField] private string logFileName = "MM1_Simulation_Data";

    private float timeSinceLastLog = 0f;
    private StringBuilder logData;
    private float simulationStartTime;

    void Start()
    {
        if (queueManager == null)
            queueManager = GameObject.FindGameObjectWithTag("DriveThruWindow").GetComponent<QueueManager>();
        if (arrivalProcess == null)
            arrivalProcess = FindObjectOfType<ArrivalProcess>();
        if (serviceProcess == null)
            serviceProcess = FindObjectOfType<ServiceProcess>();

        simulationStartTime = Time.time;
        InitializeLog();
    }

    void Update()
    {
        if (!enableLogging)
            return;

        timeSinceLastLog += Time.deltaTime;

        if (timeSinceLastLog >= loggingInterval)
        {
            LogCurrentState();
            timeSinceLastLog = 0f;
        }
    }

    private void InitializeLog()
    {
        logData = new StringBuilder();
        logData.AppendLine("M/M/1 Queue Simulation Data Log");
        logData.AppendLine("Generated: " + DateTime.Now.ToString());
        logData.AppendLine();
        logData.AppendLine("Time(sec),Lambda(?),Mu(?),Rho(?),CurrentQueue,AvgQueue,MaxQueue,AvgWaitTime(sec),TotalArrivals,TotalServiced,TheoreticalLq,TheoreticalWq");
    }

    private void LogCurrentState()
    {
        if (queueManager == null || arrivalProcess == null || serviceProcess == null)
            return;

        float elapsedTime = Time.time - simulationStartTime;
        float lambda = arrivalProcess.arrivalRateAsCarsPerHour;
        float mu = serviceProcess.serviceRateAsCarsPerHour;
        float rho = mu > 0 ? lambda / mu : 0f;

        int currentQueue = queueManager.GetCurrentQueueLength();
        float avgQueue = queueManager.GetAverageQueueLength();
        int maxQueue = queueManager.GetMaxQueueLength();
        float avgWaitTime = queueManager.GetAverageWaitTime();
        int totalArrivals = arrivalProcess.GetTotalArrivals();
        int totalServiced = serviceProcess.GetTotalServiced();

        float theoreticalLq = rho < 1 ? (rho * rho) / (1 - rho) : float.PositiveInfinity;
        float theoreticalWq = rho < 1 ? (rho / (mu * (1 - rho))) * 3600f : float.PositiveInfinity;

        logData.AppendLine($"{elapsedTime:F2},{lambda:F2},{mu:F2},{rho:F4},{currentQueue},{avgQueue:F2},{maxQueue},{avgWaitTime:F2},{totalArrivals},{totalServiced},{theoreticalLq:F2},{theoreticalWq:F2}");
    }

    public void ExportLog()
    {
        if (logData == null || logData.Length == 0)
        {
            Debug.LogWarning("No data to export!");
            return;
        }

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"{logFileName}_{timestamp}.csv";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            File.WriteAllText(filePath, logData.ToString());
            Debug.Log($"Data exported successfully to: {filePath}");
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.RevealInFinder(filePath);
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to export data: {e.Message}");
        }
    }

    public void StartLogging()
    {
        enableLogging = true;
    }

    public void StopLogging()
    {
        enableLogging = false;
    }

    public void ClearLog()
    {
        InitializeLog();
        simulationStartTime = Time.time;
    }

    private void OnDestroy()
    {
        if (enableLogging)
        {
            ExportLog();
        }
    }
}
