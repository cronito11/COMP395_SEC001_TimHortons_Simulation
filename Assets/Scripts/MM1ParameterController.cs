using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MM1ParameterController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ArrivalProcess arrivalProcess;
    [SerializeField] private ServiceProcess serviceProcess;
    [SerializeField] private QueueManager queueManager;

    [Header("Arrival UI Controls")]
    [SerializeField] private TMP_Dropdown arrivalStrategyDropdown;
    [SerializeField] private Slider arrivalRateSlider;
    [SerializeField] private TMP_InputField arrivalRateInput;
    [SerializeField] private TextMeshProUGUI arrivalRateLabel;

    [Header("Service UI Controls")]
    [SerializeField] private TMP_Dropdown serviceStrategyDropdown;
    [SerializeField] private Slider serviceRateSlider;
    [SerializeField] private TMP_InputField serviceRateInput;
    [SerializeField] private TextMeshProUGUI serviceRateLabel;

    [Header("Control Buttons")]
    [SerializeField] private Button resetButton;
    [SerializeField] private Button startStopButton;
    [SerializeField] private TextMeshProUGUI startStopButtonText;

    [Header("Slider Ranges")]
    [SerializeField] private float minArrivalRate = 5f;
    [SerializeField] private float maxArrivalRate = 60f;
    [SerializeField] private float minServiceRate = 5f;
    [SerializeField] private float maxServiceRate = 60f;

    private bool isSimulationRunning = true;

    private void Start()
    {
        if (arrivalProcess == null)
            arrivalProcess = FindObjectOfType<ArrivalProcess>();
        if (serviceProcess == null)
            serviceProcess = FindObjectOfType<ServiceProcess>();
        if (queueManager == null)
            queueManager = GameObject.FindGameObjectWithTag("DriveThruWindow").GetComponent<QueueManager>();

        InitializeArrivalControls();
        InitializeServiceControls();
        InitializeButtons();
    }

    private void InitializeArrivalControls()
    {
        if (arrivalStrategyDropdown != null)
        {
            arrivalStrategyDropdown.ClearOptions();
            arrivalStrategyDropdown.AddOptions(Enum.GetNames(typeof(ArrivalProcess.ArrivalIntervalTimeStrategy)).ToList());
            arrivalStrategyDropdown.value = (int)arrivalProcess.arrivalIntervalTimeStrategy;
            arrivalStrategyDropdown.onValueChanged.AddListener(OnArrivalStrategyChanged);
        }

        if (arrivalRateSlider != null)
        {
            arrivalRateSlider.minValue = minArrivalRate;
            arrivalRateSlider.maxValue = maxArrivalRate;
            arrivalRateSlider.value = arrivalProcess.arrivalRateAsCarsPerHour;
            arrivalRateSlider.onValueChanged.AddListener(OnArrivalRateSliderChanged);
        }

        if (arrivalRateInput != null)
        {
            arrivalRateInput.text = arrivalProcess.arrivalRateAsCarsPerHour.ToString("F2");
            arrivalRateInput.onEndEdit.AddListener(OnArrivalRateInputChanged);
        }

        UpdateArrivalRateLabel();
    }

    private void InitializeServiceControls()
    {
        if (serviceStrategyDropdown != null)
        {
            serviceStrategyDropdown.ClearOptions();
            serviceStrategyDropdown.AddOptions(Enum.GetNames(typeof(ServiceProcess.ServiceIntervalTimeStrategy)).ToList());
            serviceStrategyDropdown.value = (int)serviceProcess.serviceIntervalTimeStrategy;
            serviceStrategyDropdown.onValueChanged.AddListener(OnServiceStrategyChanged);
        }

        if (serviceRateSlider != null)
        {
            serviceRateSlider.minValue = minServiceRate;
            serviceRateSlider.maxValue = maxServiceRate;
            serviceRateSlider.value = serviceProcess.serviceRateAsCarsPerHour;
            serviceRateSlider.onValueChanged.AddListener(OnServiceRateSliderChanged);
        }

        if (serviceRateInput != null)
        {
            serviceRateInput.text = serviceProcess.serviceRateAsCarsPerHour.ToString("F2");
            serviceRateInput.onEndEdit.AddListener(OnServiceRateInputChanged);
        }

        UpdateServiceRateLabel();
    }

    private void InitializeButtons()
    {
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(OnResetClicked);
        }

        if (startStopButton != null)
        {
            startStopButton.onClick.AddListener(OnStartStopClicked);
            UpdateStartStopButton();
        }
    }

    private void OnArrivalStrategyChanged(int value)
    {
        arrivalProcess.ChangeArrivalStrategy((ArrivalProcess.ArrivalIntervalTimeStrategy)value);
    }

    private void OnServiceStrategyChanged(int value)
    {
        serviceProcess.ChangeServiceStrategy((ServiceProcess.ServiceIntervalTimeStrategy)value);
    }

    private void OnArrivalRateSliderChanged(float value)
    {
        arrivalProcess.arrivalRateAsCarsPerHour = value;
        arrivalProcess.UpdateArrivalRate();
        
        if (arrivalRateInput != null)
            arrivalRateInput.text = value.ToString("F2");
        
        UpdateArrivalRateLabel();
    }

    private void OnArrivalRateInputChanged(string value)
    {
        if (float.TryParse(value, out float rate))
        {
            rate = Mathf.Clamp(rate, minArrivalRate, maxArrivalRate);
            arrivalProcess.arrivalRateAsCarsPerHour = rate;
            arrivalProcess.UpdateArrivalRate();
            
            if (arrivalRateSlider != null)
                arrivalRateSlider.value = rate;
            
            UpdateArrivalRateLabel();
        }
    }

    private void OnServiceRateSliderChanged(float value)
    {
        serviceProcess.serviceRateAsCarsPerHour = value;
        serviceProcess.UpdateServiceRate();
        
        if (serviceRateInput != null)
            serviceRateInput.text = value.ToString("F2");
        
        UpdateServiceRateLabel();
    }

    private void OnServiceRateInputChanged(string value)
    {
        if (float.TryParse(value, out float rate))
        {
            rate = Mathf.Clamp(rate, minServiceRate, maxServiceRate);
            serviceProcess.serviceRateAsCarsPerHour = rate;
            serviceProcess.UpdateServiceRate();
            
            if (serviceRateSlider != null)
                serviceRateSlider.value = rate;
            
            UpdateServiceRateLabel();
        }
    }

    private void UpdateArrivalRateLabel()
    {
        if (arrivalRateLabel != null)
        {
            float avgTime = 3600f / arrivalProcess.arrivalRateAsCarsPerHour;
            arrivalRateLabel.text = $"? = {arrivalProcess.arrivalRateAsCarsPerHour:F2} cars/hr (avg: {avgTime:F1}s)";
        }
    }

    private void UpdateServiceRateLabel()
    {
        if (serviceRateLabel != null)
        {
            float avgTime = 3600f / serviceProcess.serviceRateAsCarsPerHour;
            serviceRateLabel.text = $"? = {serviceProcess.serviceRateAsCarsPerHour:F2} cars/hr (avg: {avgTime:F1}s)";
        }
    }

    private void OnResetClicked()
    {
        if (queueManager != null)
            queueManager.ResetStatistics();
        if (arrivalProcess != null)
            arrivalProcess.ResetStatistics();
        if (serviceProcess != null)
            serviceProcess.ResetStatistics();

        MM1StatisticsDisplay statsDisplay = FindObjectOfType<MM1StatisticsDisplay>();
        if (statsDisplay != null)
            statsDisplay.ResetAllStatistics();
    }

    private void OnStartStopClicked()
    {
        isSimulationRunning = !isSimulationRunning;
        
        if (isSimulationRunning)
        {
            arrivalProcess.generateArrivals = true;
            if (!arrivalProcess.gameObject.activeInHierarchy)
                arrivalProcess.gameObject.SetActive(true);
        }
        else
        {
            arrivalProcess.StopGeneratingArrivals();
        }

        UpdateStartStopButton();
    }

    private void UpdateStartStopButton()
    {
        if (startStopButtonText != null)
        {
            startStopButtonText.text = isSimulationRunning ? "Stop" : "Start";
        }
    }

    private void OnDestroy()
    {
        if (arrivalStrategyDropdown != null)
            arrivalStrategyDropdown.onValueChanged.RemoveListener(OnArrivalStrategyChanged);
        if (serviceStrategyDropdown != null)
            serviceStrategyDropdown.onValueChanged.RemoveListener(OnServiceStrategyChanged);
        if (arrivalRateSlider != null)
            arrivalRateSlider.onValueChanged.RemoveListener(OnArrivalRateSliderChanged);
        if (serviceRateSlider != null)
            serviceRateSlider.onValueChanged.RemoveListener(OnServiceRateSliderChanged);
        if (arrivalRateInput != null)
            arrivalRateInput.onEndEdit.RemoveListener(OnArrivalRateInputChanged);
        if (serviceRateInput != null)
            serviceRateInput.onEndEdit.RemoveListener(OnServiceRateInputChanged);
        if (resetButton != null)
            resetButton.onClick.RemoveListener(OnResetClicked);
        if (startStopButton != null)
            startStopButton.onClick.RemoveListener(OnStartStopClicked);
    }
}
