using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//New as of Feb.25rd

public class ServiceProcess : MonoBehaviour
{
    public GameObject carInService;
    public Transform carExitPlace;

    public float serviceRateAsCarsPerHour = 25; // car/hour
    public float interServiceTimeInHours; // = 1.0 / ServiceRateAsCarsPerHour;
    private float interServiceTimeInMinutes;
    private float interServiceTimeInSeconds;

    //public float ServiceRateAsCarsPerHour = 20; // car/hour
    public bool generateServices = false;

    //New as of Feb.23rd
    //Simple generation distribution - Uniform(min,max)
    //
    [Header("Uniform Distribution")]
    public float minInterServiceTimeInSeconds = 3;
    public float maxInterServiceTimeInSeconds = 60;
    
    [Header("Triangular Distribution")]
    public float a = 3, b = 7, c = 5; 

    //New as Feb.25th
    //CarController carController;
    QueueManager queueManager; //=new QueueManager();
    
    [Header("Observed Distribution - Service Times")]
    public float[] xs = { 0, 80, 160, 240, 320, 400 };  
    public float[] ys = { 0f, .3846f, .8077f, .9231f, .9615f, 1f }; 

    public enum ServiceIntervalTimeStrategy
    {
        ConstantIntervalTime,
        UniformIntervalTime,
        ExponentialIntervalTime,
        ObservedIntervalTime,
        TriangularDistribution
    }

    public ServiceIntervalTimeStrategy serviceIntervalTimeStrategy = ServiceIntervalTimeStrategy.UniformIntervalTime;

    // Start is called before the first frame update
    void Start()
    {
        queueManager = FindFirstObjectByType<QueueManager>();
        interServiceTimeInHours = 1.0f / serviceRateAsCarsPerHour;
        interServiceTimeInMinutes = interServiceTimeInHours * 60;
        interServiceTimeInSeconds = interServiceTimeInMinutes * 60;
        generateServices = true;  // Start service immediately
        StartCoroutine(GenerateServices());
    }

    IEnumerator GenerateServices()
    {
        while (generateServices)
        {
            // Wait until there's a car in the queue
            while (queueManager.Count() == 0)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Get the first car from queue to service
            carInService = queueManager.First();
            if (carInService == null)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            // Calculate service time
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
                    float Lambda = 1/ serviceRateAsCarsPerHour;
                    timeToNextServiceInSec = Utilities.GetExp(U, Lambda);
                    break;
                case ServiceIntervalTimeStrategy.ObservedIntervalTime:
                    timeToNextServiceInSec = Utilities.MultiInverseInterpolate(xs, ys, U);
                    break;
                case ServiceIntervalTimeStrategy.TriangularDistribution:
                    timeToNextServiceInSec = Utilities.GetTriangularDistribution(U, a, b, c);
                    break;
                default:
                    print("No acceptable ServiceIntervalTimeStrategy:" + serviceIntervalTimeStrategy);
                    break;
            }


            // Wait for service to complete
            yield return new WaitForSeconds(timeToNextServiceInSec);

            // Service complete - tell car to exit
            carInService.GetComponent<CarController>().ExitService(carExitPlace);
            carInService = null;
            
            // Loop continues to service next car
        }
    }

    public void ChangeServiceStrategy(ServiceIntervalTimeStrategy strategy)
    {
        serviceIntervalTimeStrategy = strategy;
    }
}
