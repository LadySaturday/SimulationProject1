using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    private Dropdown dropDown;
    private Slider slider;
    private ServiceProcess serviceProcess;
    private ArrivalMechanism arrivalMechanism;

    private InputField serviceRate;
    private InputField arrivalRate;
    // Start is called before the first frame update
    void Start()
    {
        serviceProcess = GameObject.FindGameObjectWithTag("DriveThruWindow").GetComponent<ServiceProcess>();
        arrivalMechanism = GameObject.FindGameObjectWithTag("CarSpawn").GetComponent<ArrivalMechanism>();
        dropDown = GameObject.FindGameObjectWithTag("Dropdown").GetComponent<Dropdown>();
        slider = GameObject.FindGameObjectWithTag("Slider").GetComponent<Slider>();
        serviceRate = GameObject.FindGameObjectWithTag("ServiceRate").GetComponent<InputField>();
        arrivalRate = GameObject.FindGameObjectWithTag("ArrivalRate").GetComponent<InputField>();
    }
    
    public void onArrivalChange()
    {
        if (!string.IsNullOrWhiteSpace(arrivalRate.text))
            arrivalMechanism.arrivalRate = float.Parse(arrivalRate.text);
        else
            arrivalMechanism.arrivalRate = 0;
        arrivalMechanism.calculations();//redraw all variables dependant on this change
        serviceProcess.calculations();
    }

    public void onServiceRateChange()
    {
        if (!string.IsNullOrWhiteSpace(arrivalRate.text))
            serviceProcess.serviceRateAsCarsPerHour = float.Parse(serviceRate.text);
        else
            serviceProcess.serviceRateAsCarsPerHour = 0;
        serviceProcess.calculations();//redraw all variables dependant on this change
        arrivalMechanism.calculations();
    }

    public void onTimeScaleChange()
    {
        Time.timeScale = slider.value;
    }
    public void onStrategyChange()
    {
        switch (dropDown.options[dropDown.value].text)//will need to change to add arrival mechanism
        {
            case "Constant":
                serviceProcess.serviceIntervalTimeStrategy = ServiceProcess.ServiceIntervalTimeStrategy.Constant;
                break;
            case "Exponential":
                serviceProcess.serviceIntervalTimeStrategy = ServiceProcess.ServiceIntervalTimeStrategy.Exponential;
                break;
            case "Uniform":
                serviceProcess.serviceIntervalTimeStrategy = ServiceProcess.ServiceIntervalTimeStrategy.Uniform;
                break;
            case "Observed":
                serviceProcess.serviceIntervalTimeStrategy = ServiceProcess.ServiceIntervalTimeStrategy.Observed;
                break;
            case "Interrupted":
                serviceProcess.serviceIntervalTimeStrategy = ServiceProcess.ServiceIntervalTimeStrategy.Interrupted;
                break;
        }      
    }
}
