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
    [SerializeField] private TMP_Text timeScale;
    [SerializeField] private Scrollbar scrollbar;

    private ArrivalProcess arrivalProcess;
    private ServiceProcess serviceProcess;

    private const float MAX_TIME_SCALE = 100;

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

        scrollbar.onValueChanged.AddListener(OnScrollChanged);
        scrollbar.SetValueWithoutNotify(Time.timeScale * 0.01f);
        timeScale.SetText($"Time Scale: {Time.timeScale:F2}");
    }

    private void OnScrollChanged(float arg0)
    {
        arg0 = Mathf.Clamp(arg0, 0.01f, 1);
        Time.timeScale = arg0 * MAX_TIME_SCALE;
        timeScale.SetText($"Time Scale: {Time.timeScale:F2}");
    }

    private void OnDestroy()
    {
        dropdownArrival.onValueChanged.RemoveAllListeners();
        dropdownService.onValueChanged.RemoveAllListeners();
        scrollbar.onValueChanged.RemoveAllListeners();

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
