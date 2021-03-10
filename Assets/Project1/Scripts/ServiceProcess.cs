using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//New as of Feb.25rd

public class ServiceProcess : MonoBehaviour
{
    public GameObject carInService;
    public Transform carExitPlace;

    public float serviceRateAsCarsPerHour = 30; // car/hour
    public float interServiceTime;

    //public float ServiceRateAsCarsPerHour = 20; // car/hour
    public bool generateServices = false;
    
    //Simple generation distribution - Uniform(min,max)

    public float minInterServiceTimeInSeconds = 3;
    public float maxInterServiceTimeInSeconds = 60;

    //M/1/1
    private float avgTimeInQ;//rho/(1-rho)*1/mu
    private float avgTimeInSystem;//1/(1-rho)*1/mu
    private const float timeScale= 60 * 60;
    
    public enum ServiceIntervalTimeStrategy
    {
        Constant,
        Uniform,
        Exponential,
        Observed,
        Interrupted
    }

    public ServiceIntervalTimeStrategy serviceIntervalTimeStrategy = ServiceIntervalTimeStrategy.Interrupted;

    // Start is called before the first frame update
    void Start()
    {
        calculations();      
    }

    public void calculations()
    {
        interServiceTime = (1.0f / serviceRateAsCarsPerHour)*60*60;

        //new
        avgTimeInQ = ArrivalMechanism.instance.trafficIntensity / (ArrivalMechanism.instance.probSystemNotIdle) * (1 / ArrivalMechanism.instance.rateOfService);
        avgTimeInSystem = 1 / (ArrivalMechanism.instance.probSystemNotIdle) * (1 / ArrivalMechanism.instance.rateOfService);

    }
    private void OnTriggerEnter(Collider other)
    {     
            if (other.gameObject.tag == "Car")
            {
                carInService = other.gameObject;
                CarBehaviour carBehaviour = carInService.GetComponent<CarBehaviour>();

                if (carBehaviour.stateMachine.CurrentState.Name != "service")
                {
                    carBehaviour.stateMachine.TransitionTo("Service");
                }
            generateServices = true;
            StartCoroutine(GenerateServices());
        }
        
       
    }

    
    

    IEnumerator GenerateServices()
    {
        while (generateServices)
        {
            //Instantiate(carPrefab, carSpawnPlace.position, Quaternion.identity);
            float timeToNextServiceInSec = interServiceTime;
            switch (serviceIntervalTimeStrategy)
            {
                case ServiceIntervalTimeStrategy.Constant:
                    timeToNextServiceInSec = interServiceTime;
                    break;
                case ServiceIntervalTimeStrategy.Uniform:
                    timeToNextServiceInSec = Random.Range(minInterServiceTimeInSeconds, maxInterServiceTimeInSeconds);
                    break;
                case ServiceIntervalTimeStrategy.Exponential:
                    float U = Random.value;
                    float Lambda = 1 / serviceRateAsCarsPerHour;
                    timeToNextServiceInSec = Utilities.GetExp(U, Lambda);
                    break;
                case ServiceIntervalTimeStrategy.Observed:
                    timeToNextServiceInSec = interServiceTime;
                    break;
                case ServiceIntervalTimeStrategy.Interrupted:
                    timeToNextServiceInSec = (avgTimeInSystem - avgTimeInQ)*timeScale; 
                    break;
                default:
                    print("No acceptable ServiceIntervalTimeStrategy:" + serviceIntervalTimeStrategy);
                    break;

            }

            //New as of Feb.23rd
            //float timeToNextServiceInSec = Random.Range(minInterServiceTimeInSeconds,maxInterServiceTimeInSeconds);
            generateServices = false;
           
            yield return new WaitForSeconds(timeToNextServiceInSec);
            if (carInService)
                carInService.GetComponent<CarBehaviour>().stateMachine.TransitionTo("Exit");

        }    

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
