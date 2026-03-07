using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceProcess : MonoBehaviour
{
    public GameObject carInService;
    public Transform carExitPlace;

    [Header("M/M/1 Queue Parameters")]
    [Tooltip("Service rate mu (μ) - customers per hour")]
    public float serviceRateAsCarsPerHour = 31.6f; // μ (mu) from your Excel data: 3600/113.88 ≈ 31.6 car/hour
    public float interServiceTimeInHours;
    private float interServiceTimeInMinutes;
    private float interServiceTimeInSeconds;

    public bool generateServices = false;

    [Header("Uniform Distribution")]
    public float minInterServiceTimeInSeconds = 3;
    public float maxInterServiceTimeInSeconds = 60;

    [Header("Observed Distribution - Service Times")]
    [Tooltip("Service times in SECONDS from your Excel data")]
    public float[] serviceTimesXs = { 5f, 50f, 100f, 150f, 200f, 250f, 300f, 399f };
    [Tooltip("Cumulative probabilities (must end with 1.0)")]
    public float[] serviceTimesYs = { 0f, 0.18f, 0.38f, 0.58f, 0.73f, 0.85f, 0.96f, 1f };

    QueueManager queueManager;

    [Header("Statistics")]
    [SerializeField] private int totalServiced = 0;
    [SerializeField] private float totalServiceTime = 0f;

    public enum ServiceIntervalTimeStrategy
    {
        ConstantIntervalTime,
        UniformIntervalTime,
        ExponentialIntervalTime,
        ObservedIntervalTime
    }

    public ServiceIntervalTimeStrategy serviceIntervalTimeStrategy = ServiceIntervalTimeStrategy.ExponentialIntervalTime;

    void Start()
    {
        queueManager = GameObject.FindGameObjectWithTag("DriveThruWindow").GetComponent<QueueManager>();
        UpdateServiceRate();
    }

    public void UpdateServiceRate()
    {
        interServiceTimeInHours = 1.0f / serviceRateAsCarsPerHour;
        interServiceTimeInMinutes = interServiceTimeInHours * 60;
        interServiceTimeInSeconds = interServiceTimeInMinutes * 60;
    }

    private void OnTriggerEnter(Collider other)
    {
#if DEBUG_SP
        print("ServiceProcess.OnTriggerEnter:otherID=" + other.gameObject.GetInstanceID());
#endif

        if (other.gameObject.tag == "Car")
        {
            carInService = other.gameObject;
            carInService.GetComponent<CarController>().SetInService(true);
            
            generateServices = true;
            StartCoroutine(GenerateServices());
        }
    }

    IEnumerator GenerateServices()
    {
        while (generateServices)
        {
            float timeToNextServiceInSec = interServiceTimeInSeconds;
            float U = Random.value;
            
            switch (serviceIntervalTimeStrategy)
            {
                case ServiceIntervalTimeStrategy.ConstantIntervalTime:
                    timeToNextServiceInSec = interServiceTimeInSeconds;
                    break;
                case ServiceIntervalTimeStrategy.UniformIntervalTime:
                    timeToNextServiceInSec = Random.Range(minInterServiceTimeInSeconds, maxInterServiceTimeInSeconds);
                    break;
                case ServiceIntervalTimeStrategy.ExponentialIntervalTime:
                    float Lambda = serviceRateAsCarsPerHour / 3600f;
                    timeToNextServiceInSec = Utilities.GetExp(U, Lambda);
                    break;
                case ServiceIntervalTimeStrategy.ObservedIntervalTime:
                    timeToNextServiceInSec = Utilities.MultiInterpolate(serviceTimesYs, serviceTimesXs, U);
                    break;
                default:
                    print("No acceptable ServiceIntervalTimeStrategy:" + serviceIntervalTimeStrategy);
                    break;
            }

            generateServices = false;
            totalServiceTime += timeToNextServiceInSec;
            yield return new WaitForSeconds(timeToNextServiceInSec);
        }
        
        totalServiced++;
        if (queueManager != null)
        {
            queueManager.RecordServiceCompletion(totalServiceTime / totalServiced);
        }
        carInService.GetComponent<CarController>().ExitService(carExitPlace);
    }

    private void OnDrawGizmos()
    {
        if (carInService)
        {
            Renderer r = carInService.GetComponent<Renderer>();
            r.material.color = Color.green;
        }
    }

    public int GetTotalServiced()
    {
        return totalServiced;
    }

    public float GetAverageServiceTime()
    {
        return totalServiced > 0 ? totalServiceTime / totalServiced : 0f;
    }

    public void ResetStatistics()
    {
        totalServiced = 0;
        totalServiceTime = 0f;
    }

    public void ChangeServiceStrategy(ServiceIntervalTimeStrategy newStrategy)
    {
        serviceIntervalTimeStrategy = newStrategy;
    }
}
