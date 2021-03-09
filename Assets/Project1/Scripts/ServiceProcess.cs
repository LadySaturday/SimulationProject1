using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//New as of Feb.25rd

public class ServiceProcess : MonoBehaviour
{
    public GameObject carInService;
    public Transform carExitPlace;

    public float serviceRateAsCarsPerHour = 30; // car/hour
    public float interServiceTimeInHours; // = 1.0 / ServiceRateAsCarsPerHour;
    private float interServiceTimeInMinutes;
    private float interServiceTimeInSeconds;

    //public float ServiceRateAsCarsPerHour = 20; // car/hour
    public bool generateServices = false;

    //New as of Feb.23rd
    //Simple generation distribution - Uniform(min,max)
    //
    public float minInterServiceTimeInSeconds = 3;
    public float maxInterServiceTimeInSeconds = 60;
    //

    //New as Feb.25th
    //CarController carController;
    QueueManager queueManager; //=new QueueManager();


    //new
    private float avgTimeInQ;//rho/(1-rho)*1/mu
    private float avgTimeInSystem;//1/(1-rho)*1/mu
    private float avgTimeAtWindow;
    private ArrivalMechanism arrivalMechanism;
    public enum ServiceIntervalTimeStrategy
    {
        ConstantIntervalTime,
        UniformIntervalTime,
        ExponentialIntervalTime,
        ObservedIntervalTime
    }

    public ServiceIntervalTimeStrategy serviceIntervalTimeStrategy = ServiceIntervalTimeStrategy.UniformIntervalTime;

    // Start is called before the first frame update
    void Start()
    {
        interServiceTimeInHours = 1.0f / serviceRateAsCarsPerHour;
        interServiceTimeInMinutes = interServiceTimeInHours * 60;
        interServiceTimeInSeconds = interServiceTimeInMinutes * 60;

        //new
        avgTimeInQ = arrivalMechanism.trafficIntensity / (arrivalMechanism.probSystemNotIdle) * (1 / arrivalMechanism.rateOfService);
        avgTimeInSystem = 1 / (arrivalMechanism.probSystemNotIdle) * (1 / arrivalMechanism.rateOfService);

        avgTimeAtWindow = avgTimeInSystem - avgTimeInQ;
    }
    private void OnTriggerEnter(Collider other)
    {
        print("ServiceProcess.OnTriggerEnter:other=" + other.gameObject.name);

        if (other.gameObject.tag == "Car")
        {
            carInService = other.gameObject;
            carInService.GetComponent<CarBehaviour>().stateMachine.TransitionTo("Service"); Debug.Log("In service");
            generateServices = true;
            StartCoroutine(GenerateServices());
        }
    }

    IEnumerator GenerateServices()
    {
        while (generateServices)
        {
            //Instantiate(carPrefab, carSpawnPlace.position, Quaternion.identity);
            float timeToNextServiceInSec = interServiceTimeInSeconds;
            switch (serviceIntervalTimeStrategy)
            {
                case ServiceIntervalTimeStrategy.ConstantIntervalTime:
                    timeToNextServiceInSec = interServiceTimeInSeconds;
                    break;
                case ServiceIntervalTimeStrategy.UniformIntervalTime:
                    timeToNextServiceInSec = Random.Range(minInterServiceTimeInSeconds, maxInterServiceTimeInSeconds);
                    break;
                case ServiceIntervalTimeStrategy.ExponentialIntervalTime:
                    float U = Random.value;
                    float Lambda = 1 / serviceRateAsCarsPerHour;
                    timeToNextServiceInSec = Utilities.GetExp(U, Lambda);
                    break;
                case ServiceIntervalTimeStrategy.ObservedIntervalTime:
                    timeToNextServiceInSec = interServiceTimeInSeconds;
                    break;
                default:
                    print("No acceptable ServiceIntervalTimeStrategy:" + serviceIntervalTimeStrategy);
                    break;

            }

            //New as of Feb.23rd
            //float timeToNextServiceInSec = Random.Range(minInterServiceTimeInSeconds,maxInterServiceTimeInSeconds);
            generateServices = false;
            yield return new WaitForSeconds(timeToNextServiceInSec);
            

        }
        carInService.GetComponent<CarBehaviour>().stateMachine.TransitionTo("Exit"); Debug.Log("Car exiting");

    }
    private void OnDrawGizmos()
    {
        //BoxCollidercarInService.GetComponent<BoxCollider>
        if (carInService)
        {
            Renderer r = carInService.GetComponent<Renderer>();
            r.material.color = Color.green;

        }


    }

}
