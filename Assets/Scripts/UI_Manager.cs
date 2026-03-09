using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ArrivalProcess;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdownArrival;
    [SerializeField] private TMP_Dropdown dropdownService;

    private ArrivalProcess arrivalProcess;
    private ServiceProcess serviceProcess;

    private void Start()
    {
        arrivalProcess = FindFirstObjectByType<ArrivalProcess>();
        serviceProcess = FindFirstObjectByType<ServiceProcess>();

        dropdownArrival.ClearOptions();
        dropdownArrival.AddOptions(Enum.GetNames(typeof(ArrivalIntervalTimeStrategy)).ToList());
        dropdownArrival.onValueChanged.AddListener(OnValueChanged);
        dropdownArrival.SetValueWithoutNotify((int) ArrivalIntervalTimeStrategy.ObservedIntervalTime);
        //Service time
        dropdownService.ClearOptions();
        dropdownService.AddOptions(Enum.GetNames(typeof(ServiceProcess.ServiceIntervalTimeStrategy)).ToList());
        dropdownService.onValueChanged.AddListener(OnServiceValueChanged);
        dropdownService.SetValueWithoutNotify((int)ServiceProcess.ServiceIntervalTimeStrategy.ObservedIntervalTime);
    }

    private void OnDestroy()
    {
        dropdownArrival.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(int arg0)
    {
        ArrivalIntervalTimeStrategy strategy = (ArrivalIntervalTimeStrategy)arg0;
        arrivalProcess.ChangeArrivalStrategy(strategy);
    }

    private void OnServiceValueChanged(int arg0)
    {
        ServiceProcess.ServiceIntervalTimeStrategy strategy = (ServiceProcess.ServiceIntervalTimeStrategy)arg0;
        serviceProcess.ChangeServiceStrategy(strategy);

    }
}
