using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrivalProcess : MonoBehaviour
{

    public GameObject carPrefab;
    public Transform carSpawnPlace;

    [Header("Constant, or Exponential Distribution" )]
    public float arrivalRateAsCarsPerHour = 20; // car/hour
    public float interArrivalTimeInHours; // = 1.0 / arrivalRateAsCarsPerHour;
    private float interArrivalTimeInMinutes;
    private float interArrivalTimeInSeconds;


    //public float arrivalRateAsCarsPerHour = 20; // car/hour
    public bool generateArrivals = true;

    //New as of Feb.23rd
    //Simple generation distribution - Uniform(min,max)
    //
    [Header("Uniform Distribution")]
    public float minInterArrivalTimeInSeconds = 3; 
    public float maxInterArrivalTimeInSeconds = 60;

    [Header("Triangular Distribution")]
    //Ref: https://en.wikipedia.org/wiki/Triangular_distribution
    public float a=3, b=7, c=5; // You should have c in (a,b)   a<c<b
    //
    [Header("Observed Distribution")]
    [Tooltip("These are Cumulative Distribution data ys")]
    public float[] xs = { 0, 1, 2, 10 };
    [Tooltip("These are Cumulative Distribution data xs")]
    public float[] ys = { 0, .75f, .97f, 1f };
   public enum ArrivalIntervalTimeStrategy
    {
        ConstantIntervalTime,
        UniformIntervalTime,
        ExponentialIntervalTime,
        ObservedIntervalTime,
        TriangularDistribution
    }

    public ArrivalIntervalTimeStrategy arrivalIntervalTimeStrategy=ArrivalIntervalTimeStrategy.UniformIntervalTime;

    //New as of Feb.25th
    QueueManager queueManager;

    //UI debugging
#if DEBUG_AP
    public Text txtDebug;
#endif

    // Start is called before the first frame update
    void Start()
    {
        queueManager = GameObject.FindGameObjectWithTag("DriveThruWindow").GetComponent<QueueManager>();
        interArrivalTimeInHours = 1.0f / arrivalRateAsCarsPerHour;
        interArrivalTimeInMinutes = interArrivalTimeInHours * 60;
        interArrivalTimeInSeconds = interArrivalTimeInMinutes * 60;
        StartCoroutine(GenerateArrivals());
#if DEBUG_AP
        print("proc#:" + System.Environment.ProcessorCount);
        txtDebug.text = "\nproc#:" + System.Environment.ProcessorCount;
#endif
    }
   
    IEnumerator GenerateArrivals()
    {
        while (generateArrivals)
        {
            GameObject carGO=Instantiate(carPrefab, carSpawnPlace.position, Quaternion.identity);
            //if (queueManager.Count() > 0)
            //{
            //    queueManager.Add(carGO);
            //} //The first car as added in the queue when in DriveThruWindow

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
                    
                    float Lambda = 1 / arrivalRateAsCarsPerHour;
                    timeToNextArrivalInSec = Utilities.GetExp(U,Lambda);
                    break;
                case ArrivalIntervalTimeStrategy.ObservedIntervalTime:
                    timeToNextArrivalInSec = Utilities.MultiInterpolate(ys,xs,U) * 60; //we get it in min, so *60 => in sec
                    break;
                case ArrivalIntervalTimeStrategy.TriangularDistribution:
                    timeToNextArrivalInSec = Utilities.GetTriangularDistribution(U, a,b,c);
                    break;
                default:
                    print("No acceptable arrivalIntervalTimeStrategy:" + arrivalIntervalTimeStrategy);
                    break;

            }

            //New as of Feb.23rd
            //float timeToNextArrivalInSec = Random.Range(minInterArrivalTimeInSeconds,maxInterArrivalTimeInSeconds);
            yield return new WaitForSeconds(timeToNextArrivalInSec);

            //yield return new WaitForSeconds(interArrivalTimeInSeconds);

        }

    }
}
