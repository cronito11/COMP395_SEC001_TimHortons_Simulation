using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrivalProcess : MonoBehaviour
{

    public GameObject carPrefab;
    public Transform carSpawnPlace;

    [Header("M/M/1 Queue Parameters")]
    [Tooltip("Arrival rate lambda (λ) - customers per hour")]
    public float arrivalRateAsCarsPerHour = 34.35f; // λ (lambda) from your Excel data: 3600/104.81 ≈ 34.35 car/hour
    public float interArrivalTimeInHours;
    private float interArrivalTimeInMinutes;
    private float interArrivalTimeInSeconds;

    public bool generateArrivals = true;

    [Header("Uniform Distribution")]
    public float minInterArrivalTimeInSeconds = 3; 
    public float maxInterArrivalTimeInSeconds = 60;

    [Header("Triangular Distribution")]
    public float a=3, b=7, c=5;
    
    [Header("Observed Distribution - Inter-Arrival Times")]
    [Tooltip("Inter-arrival times in SECONDS from your Excel data")]
    public float[] xs = { 9f, 50f, 100f, 150f, 200f, 250f, 283f };
    [Tooltip("Cumulative probabilities (must end with 1.0)")]
    public float[] ys = { 0f, 0.08f, 0.38f, 0.67f, 0.81f, 0.96f, 1f };

   public enum ArrivalIntervalTimeStrategy
    {
        ConstantIntervalTime,
        UniformIntervalTime,
        ExponentialIntervalTime,
        ObservedIntervalTime,
        TriangularDistribution,
    }

    public ArrivalIntervalTimeStrategy arrivalIntervalTimeStrategy=ArrivalIntervalTimeStrategy.ExponentialIntervalTime;

    QueueManager queueManager;

    [Header("Statistics")]
    [SerializeField] private int totalArrivals = 0;

    //UI debugging
#if DEBUG_AP
    public Text txtDebug;
#endif

    // Start is called before the first frame update
    void Start()
    {
        queueManager = GameObject.FindGameObjectWithTag("DriveThruWindow").GetComponent<QueueManager>();
        UpdateArrivalRate();
        StartCoroutine(GenerateArrivals());
#if DEBUG_AP
        print("proc#:" + System.Environment.ProcessorCount);
        txtDebug.text = "\nproc#:" + System.Environment.ProcessorCount;
#endif
    }

    public void UpdateArrivalRate()
    {
        interArrivalTimeInHours = 1.0f / arrivalRateAsCarsPerHour;
        interArrivalTimeInMinutes = interArrivalTimeInHours * 60;
        interArrivalTimeInSeconds = interArrivalTimeInMinutes * 60;
    }
   
    IEnumerator GenerateArrivals()
    {
        while (generateArrivals)
        {
            GameObject carGO=Instantiate(carPrefab, carSpawnPlace.position, Quaternion.identity);
            totalArrivals++;

            float timeToNextArrivalInSec = interArrivalTimeInSeconds;
            float U = Random.value;
            switch (arrivalIntervalTimeStrategy)
            {
                case ArrivalIntervalTimeStrategy.ConstantIntervalTime:
                    timeToNextArrivalInSec= interArrivalTimeInSeconds;
                    break;
                case ArrivalIntervalTimeStrategy.UniformIntervalTime:
                    timeToNextArrivalInSec = Random.Range(minInterArrivalTimeInSeconds, maxInterArrivalTimeInSeconds);
                    break;
                case ArrivalIntervalTimeStrategy.ExponentialIntervalTime:
                    float Lambda = arrivalRateAsCarsPerHour / 3600f;
                    timeToNextArrivalInSec = Utilities.GetExp(U, Lambda);
                    break;
                case ArrivalIntervalTimeStrategy.ObservedIntervalTime:
                    timeToNextArrivalInSec = Utilities.MultiInterpolate(ys, xs, U);
                    break;
                case ArrivalIntervalTimeStrategy.TriangularDistribution:
                    timeToNextArrivalInSec = Utilities.GetTriangularDistribution(U, a,b,c);
                    break;
                default:
                    print("No acceptable arrivalIntervalTimeStrategy:" + arrivalIntervalTimeStrategy);
                    break;

            }

            yield return new WaitForSeconds(timeToNextArrivalInSec);
        }
    }

    public void StopGeneratingArrivals()
    {
        generateArrivals = false;
    }

    public void ChangeArrivalStrategy(ArrivalIntervalTimeStrategy newStrategy)
    {
        arrivalIntervalTimeStrategy = newStrategy;
    }

    public int GetTotalArrivals()
    {
        return totalArrivals;
    }

    public void ResetStatistics()
    {
        totalArrivals = 0;
    }
}
