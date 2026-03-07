using UnityEngine;
using TMPro;

public class MM1StatisticsDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private QueueManager queueManager;
    [SerializeField] private ArrivalProcess arrivalProcess;
    [SerializeField] private ServiceProcess serviceProcess;

    [Header("UI Text Elements")]
    [SerializeField] private TextMeshProUGUI txtLambda;
    [SerializeField] private TextMeshProUGUI txtMu;
    [SerializeField] private TextMeshProUGUI txtRho;
    [SerializeField] private TextMeshProUGUI txtCurrentQueueLength;
    [SerializeField] private TextMeshProUGUI txtAverageQueueLength;
    [SerializeField] private TextMeshProUGUI txtMaxQueueLength;
    [SerializeField] private TextMeshProUGUI txtAverageWaitTime;
    [SerializeField] private TextMeshProUGUI txtTotalArrivals;
    [SerializeField] private TextMeshProUGUI txtTotalServiced;
    [SerializeField] private TextMeshProUGUI txtSimulationTime;
    [SerializeField] private TextMeshProUGUI txtTheoreticalAvgQueue;
    [SerializeField] private TextMeshProUGUI txtTheoreticalAvgWait;

    [Header("Settings")]
    [SerializeField] private float updateInterval = 0.5f;

    private float timeSinceLastUpdate = 0f;
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
    }

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            UpdateDisplay();
            timeSinceLastUpdate = 0f;
        }
    }

    void UpdateDisplay()
    {
        if (queueManager == null || arrivalProcess == null || serviceProcess == null)
            return;

        float lambda = arrivalProcess.arrivalRateAsCarsPerHour;
        float mu = serviceProcess.serviceRateAsCarsPerHour;
        float rho = mu > 0 ? lambda / mu : 0f;

        float theoreticalAvgQueueLength = rho < 1 ? (rho * rho) / (1 - rho) : float.PositiveInfinity;
        float theoreticalAvgWaitTime = rho < 1 ? (rho / (mu * (1 - rho))) * 3600f : float.PositiveInfinity;

        if (txtLambda != null)
            txtLambda.text = $"? (Arrival Rate): {lambda:F2} cars/hour";

        if (txtMu != null)
            txtMu.text = $"? (Service Rate): {mu:F2} cars/hour";

        if (txtRho != null)
        {
            txtRho.text = $"? (Utilization): {rho:F3}";
            if (rho >= 1f)
                txtRho.text += " ?? UNSTABLE!";
        }

        if (txtCurrentQueueLength != null)
            txtCurrentQueueLength.text = $"Current Queue: {queueManager.GetCurrentQueueLength()}";

        if (txtAverageQueueLength != null)
            txtAverageQueueLength.text = $"Avg Queue Length: {queueManager.GetAverageQueueLength():F2}";

        if (txtMaxQueueLength != null)
            txtMaxQueueLength.text = $"Max Queue Length: {queueManager.GetMaxQueueLength()}";

        if (txtAverageWaitTime != null)
            txtAverageWaitTime.text = $"Avg Wait Time: {queueManager.GetAverageWaitTime():F2} sec";

        if (txtTotalArrivals != null)
            txtTotalArrivals.text = $"Total Arrivals: {arrivalProcess.GetTotalArrivals()}";

        if (txtTotalServiced != null)
            txtTotalServiced.text = $"Total Serviced: {serviceProcess.GetTotalServiced()}";

        if (txtSimulationTime != null)
        {
            float simTime = Time.time - simulationStartTime;
            txtSimulationTime.text = $"Simulation Time: {simTime:F1} sec ({(simTime / 60f):F1} min)";
        }

        if (txtTheoreticalAvgQueue != null)
        {
            if (float.IsPositiveInfinity(theoreticalAvgQueueLength))
                txtTheoreticalAvgQueue.text = "Theoretical Lq: ? (Unstable)";
            else
                txtTheoreticalAvgQueue.text = $"Theoretical Lq: {theoreticalAvgQueueLength:F2}";
        }

        if (txtTheoreticalAvgWait != null)
        {
            if (float.IsPositiveInfinity(theoreticalAvgWaitTime))
                txtTheoreticalAvgWait.text = "Theoretical Wq: ? (Unstable)";
            else
                txtTheoreticalAvgWait.text = $"Theoretical Wq: {theoreticalAvgWaitTime:F2} sec";
        }
    }

    public void ResetAllStatistics()
    {
        if (queueManager != null)
            queueManager.ResetStatistics();
        if (arrivalProcess != null)
            arrivalProcess.ResetStatistics();
        if (serviceProcess != null)
            serviceProcess.ResetStatistics();

        simulationStartTime = Time.time;
    }
}
