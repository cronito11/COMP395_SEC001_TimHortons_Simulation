using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    List<GameObject> queue = new List<GameObject>();
    LinkedList<GameObject> q = new LinkedList<GameObject>();
    
    [Header("M/M/1 Queue Statistics")]
    [SerializeField] private int currentQueueLength = 0;
    [SerializeField] private int maxQueueLength = 0;
    [SerializeField] private float totalQueueLengthSum = 0f;
    [SerializeField] private int measurementCount = 0;
    [SerializeField] private float measurementInterval = 1f;

    private float timeSinceLastMeasurement = 0f;
    private int totalCustomersServed = 0;
    private float totalWaitTime = 0f;
    private Dictionary<GameObject, float> arrivalTimes = new Dictionary<GameObject, float>();

    public GameObject Last()
    {
        GameObject go = null;

        if (queue.Count > 0)
        {
            go= queue[queue.Count - 1];
        }
        return go;
    }

    public GameObject First()
    {
        GameObject go = null;

        if (queue.Count > 0)
        {
            go = queue[0];
        }
        return go;
    }

    public void Add(GameObject gameObject)
    {
        queue.Add(gameObject);
        arrivalTimes[gameObject] = Time.time;
        currentQueueLength = queue.Count;
        
        if (currentQueueLength > maxQueueLength)
        {
            maxQueueLength = currentQueueLength;
        }
        
#if DEBUG_QM
        print("**** QueueManager.Add:ID=" + gameObject.GetInstanceID() + ", Count="+queue.Count+" ****");
#endif
    }

    public GameObject PopFirst()
    {
        GameObject go = null;
        if (queue.Count > 0)
        {
            go = queue[0];
            
            if (arrivalTimes.ContainsKey(go))
            {
                float waitTime = Time.time - arrivalTimes[go];
                totalWaitTime += waitTime;
                totalCustomersServed++;
                arrivalTimes.Remove(go);
            }
            
            queue.RemoveAt(0);
            currentQueueLength = queue.Count;
        }
        return go;
    }

    public int Count()
    {   
        return queue.Count;
    }

    public void Update()
    {
        timeSinceLastMeasurement += Time.deltaTime;
        
        if (timeSinceLastMeasurement >= measurementInterval)
        {
            totalQueueLengthSum += currentQueueLength;
            measurementCount++;
            timeSinceLastMeasurement = 0f;
        }
        
#if DEBUG_QM
        print("*** QueueManager.Update: Count="+queue.Count+" ***");
#endif
    }

    public void Start()
    {
#if DEBUG_QM
        print("*** QueueManager.Start ***");    
#endif
    }

    public float GetAverageQueueLength()
    {
        return measurementCount > 0 ? totalQueueLengthSum / measurementCount : 0f;
    }

    public float GetAverageWaitTime()
    {
        return totalCustomersServed > 0 ? totalWaitTime / totalCustomersServed : 0f;
    }

    public int GetMaxQueueLength()
    {
        return maxQueueLength;
    }

    public int GetCurrentQueueLength()
    {
        return currentQueueLength;
    }

    public int GetTotalCustomersServed()
    {
        return totalCustomersServed;
    }

    public float GetUtilization(float lambda, float mu)
    {
        return mu > 0 ? lambda / mu : 0f;
    }

    public void RecordServiceCompletion(float avgServiceTime)
    {
    }

    public void ResetStatistics()
    {
        totalQueueLengthSum = 0f;
        measurementCount = 0;
        maxQueueLength = 0;
        totalCustomersServed = 0;
        totalWaitTime = 0f;
        arrivalTimes.Clear();
    }
}
