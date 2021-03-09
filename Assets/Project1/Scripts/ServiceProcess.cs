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

    


    //new
    private float avgTimeInQ;//rho/(1-rho)*1/mu
    private float avgTimeInSystem;//1/(1-rho)*1/mu
    private const float timeScale= 60 * 60;

    private bool serving = false;
    public enum ServiceIntervalTimeStrategy
    {
        Constant,
        Uniform,
        Exponential,
        ObservedIntervalTime,
        Interrupted
    }

    public ServiceIntervalTimeStrategy serviceIntervalTimeStrategy = ServiceIntervalTimeStrategy.Uniform;

    // Start is called before the first frame update
    void Start()
    {
        interServiceTimeInHours = 1.0f / serviceRateAsCarsPerHour;
        interServiceTimeInMinutes = interServiceTimeInHours * 60;
        interServiceTimeInSeconds = interServiceTimeInMinutes * 60;

        //new
        avgTimeInQ = ArrivalMechanism.instance.trafficIntensity / (ArrivalMechanism.instance.probSystemNotIdle) * (1 / ArrivalMechanism.instance.rateOfService);
        avgTimeInSystem = 1 / (ArrivalMechanism.instance.probSystemNotIdle) * (1 / ArrivalMechanism.instance.rateOfService);

       
    }
    private void OnTriggerStay(Collider other)
    {
        print("ServiceProcess.OnTriggerEnter:other=" + other.gameObject.name);

        
            if (other.gameObject.tag == "Car"&&!serving)
            {
            serving = true;
            carInService = other.gameObject;
            CarBehaviour carBehaviour = carInService.GetComponent<CarBehaviour>();

                if (carBehaviour.stateMachine.CurrentState.Name != "service")
                {
                
                carBehaviour.stateMachine.TransitionTo("Service"); //Debug.Log("In service");
                }
                else
                {
                    //Debug.Log("We in service bro");
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
            float timeToNextServiceInSec = interServiceTimeInSeconds;
            switch (serviceIntervalTimeStrategy)
            {
                case ServiceIntervalTimeStrategy.Constant:
                    timeToNextServiceInSec = interServiceTimeInSeconds;
                    break;
                case ServiceIntervalTimeStrategy.Uniform:
                    timeToNextServiceInSec = Random.Range(minInterServiceTimeInSeconds, maxInterServiceTimeInSeconds);
                    break;
                case ServiceIntervalTimeStrategy.Exponential:
                    float U = Random.value;
                    float Lambda = 1 / serviceRateAsCarsPerHour;
                    timeToNextServiceInSec = Utilities.GetExp(U, Lambda);
                    break;
                case ServiceIntervalTimeStrategy.ObservedIntervalTime:
                    timeToNextServiceInSec = interServiceTimeInSeconds;
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
            

        }
        carInService.GetComponent<CarBehaviour>().stateMachine.TransitionTo("Exit"); Debug.Log("Car exiting");
        serving = false;

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
