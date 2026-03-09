using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class CarController : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;

    NavMeshAgent navMeshAgent;
    public Transform targetWindow;
    public Transform targetCar=null;
    public Transform targetExit = null;

    public bool InService { get; set; }
    public GameObject driveThruWindow;
    public QueueManager queueManager;
    private Animator animator;

    public enum CarState
    {
        None=-1,
        Entered,  //going towards the DriveThruWindow (don't bump into fron cars)
        InService,
        Serviced
    }
    public CarState carState = CarState.None;
    // Start is called before the first frame update
    void Start()
    {
        driveThruWindow = GameObject.FindGameObjectWithTag("DriveThruWindow");
        targetWindow = driveThruWindow.transform;
        targetExit = GameObject.FindGameObjectWithTag("CarExit").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
#if DEBUG_CC
        print("Start: this.GO.ID=" + this.gameObject.GetInstanceID());
#endif

        //
        carState = CarState.Entered;
        prefabs[Random.Range(0, prefabs.Length)].SetActive(true);
        animator =  GetComponentInChildren<Animator>(false);
        FSMCar();

    }

    void FSMCar()
    {
#if DEBUG_CC
        print("CC.FSMCar:state="+carState+",ID="+this.gameObject.GetInstanceID());
#endif
        switch (carState)
        {
            case CarState.None: //do nothing - shouldn't happen
                break;
            case CarState.Entered:
                DoEntered();
                break;
            case CarState.InService:
                DoInService();
                break;
            case CarState.Serviced:
                DoServiced();
                break;
            default:
                print("carState unknown!:" + carState);
                break;

        }
    }
    void DoEntered()
    {
//        //queueManager = driveThruWindow.GetComponent<QueueManager>();
//        GameObject goLast = GameObject.FindGameObjectWithTag("DriveThruWindow").GetComponent<QueueManager>().Last();
//        if (goLast)
//        {
//#if DEBUG_CC
//            print("CC.DoEntered: goLast.ID=" + goLast.GetInstanceID());
//#endif
//            targetCar = goLast.transform;
//        }
//        else
//        {
//            targetCar = targetWindow;
//        }

        targetCar = targetWindow;

        queueManager = GameObject.FindGameObjectWithTag("DriveThruWindow").GetComponent<QueueManager>();
        queueManager.Add(this.gameObject);

        navMeshAgent.SetDestination(targetCar.position);
        navMeshAgent.isStopped = false;
    }
    void DoInService()
    {
        navMeshAgent.isStopped = true;
        //this.transform.position = targetWindow.position;
        //this.transform.rotation = Quaternion.identity;
    }
    void DoServiced()
    {
        navMeshAgent.SetDestination(targetExit.position);
        navMeshAgent.isStopped = false;
    }
    public void ChangeState(CarState newCarState)
    {
        this.carState = newCarState;
        animator.SetBool("IsWalking", newCarState != CarState.InService);
        FSMCar();
    }

    public void FixedUpdate()
    {

//        if (carState == CarState.Entered)
//        {
//            if (targetCar == null)
//            {
//#if DEBUG_CC
//            print("***** CarController.FixedUpdate:targetCar.pos=" + targetCar.position);
//#endif
//                targetCar = targetWindow;
//                //navMeshAgent.SetDestination(targetCar.position);
//                navMeshAgent.isStopped = false;
//            }
//        }

    }
    public void SetInService(bool value)
    {
        //Chaneg        InService = value;
        //if (InService)
        //{
        //    navMeshAgent.isStopped=true;
        //}
    }
    public void ExitService(Transform target)
    {
        //this.SetInService(false);
        
        queueManager.PopFirst();
        ChangeState(CarState.Serviced);
        //targetExit = target;

        //navMeshAgent.SetDestination(target.position);
        //navMeshAgent.isStopped = false;
    }

    private void OnTriggerEnter(Collider other)
    {
#if DEBUG_CC
        Debug.LogFormat("CarController(this={0}).OnTriggerEnter:other={1}",this.gameObject.GetInstanceID(), other.gameObject.tag);
#endif
        if (other.gameObject.tag == "Car")
        {
            //this.navMeshAgent.desiredVelocity.
            //if (targetCar == null)
            //{
                //targetCar = other.gameObject.transform;
                //navMeshAgent.SetDestination(targetCar.position);
            //}
        }
        else if (other.gameObject.tag == "DriveThruWindow")
        {
            ChangeState(CarState.InService);
            //SetInService(true);
        }
        else if (other.gameObject.tag == "CarExit")
        {
            Destroy(this.gameObject);
        }
    }


    private void OnDrawGizmos()
    {
#if DEBUG_CC
        print("InCC.OnDrawGizmos:targetCar.ID=" + targetCar.gameObject.GetInstanceID());
        print("InCC.OnDrawGizmos:targetCar.ID=" + targetExit.gameObject.GetInstanceID());

#endif
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, targetWindow.transform.position);
        if (targetCar)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(this.transform.position, targetCar.transform.position);

        }
        if (targetExit)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, targetExit.transform.position);

        }


    }

}
