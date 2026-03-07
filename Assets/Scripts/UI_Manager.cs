using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ArrivalProcess;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private ArrivalProcess arrivalProcess;
    [SerializeField] private TextMeshProUGUI arrivalTitle;


    private void Start()
    {
        arrivalProcess = FindFirstObjectByType<ArrivalProcess>();
        dropdown.ClearOptions();
        dropdown.AddOptions(Enum.GetNames(typeof(ArrivalIntervalTimeStrategy)).ToList());
        dropdown.onValueChanged.AddListener(OnValueChanged);
        OnValueChanged(0);
    }

    private void OnDestroy()
    {
        dropdown.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(int arg0)
    {
        ArrivalIntervalTimeStrategy strategy = (ArrivalIntervalTimeStrategy)arg0;
        arrivalProcess.ChangeArrivalStrategy(strategy);

        arrivalTitle.text = $"Arrival Process: {strategy}";
    }
}
