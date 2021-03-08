using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrivalMechanism : MonoBehaviour
{
    //our arrival mechanism spawns a car on average 30 times per hour. 
    private float arrivalRate = 30;//30 per hour, Lambda
    private float interArrivalTime;
    private float rateOfService = 60;//mu
    private float serviceTime;
    private float trafficIntensity;//rho
    private float probSystemNotIdle;                          //Probability that system is idle
    private float probCarDoesntWait;                //Probability car doesn’t wait
    private float probCarWaits;
    private float n;//number of cars
    private float probNCarsInSystem;//rho^n*(1-rho)
    private float probGreaterNCarsInSystem;//Probability that >=n cars in sys
    private float avgNumCarsInSystem;//rho/(1-rho)
    private float avgNumCarsInQ_noQ;//rho^2/(1-rho)
    private float avgNumCarsInQ_WQ;//1/(1-rho)
    private float avgTimeInQ;//rho/(1-rho)*1/mu
    private float avgTimeInSystem;//1/(1-rho)*1/mu
    

    // Start is called before the first frame update
    void Start()
    {
        interArrivalTime = 1 / arrivalRate;//1/lambda
        serviceTime = 1 / rateOfService;//1/mu
        trafficIntensity = arrivalRate / rateOfService;//rho=lambda/mu
        probSystemNotIdle = 1 - trafficIntensity;
        probCarDoesntWait = 1 - trafficIntensity;
        probCarWaits = trafficIntensity;
        
        probNCarsInSystem = Mathf.Pow(trafficIntensity, n) * (probSystemNotIdle);
        probGreaterNCarsInSystem = Mathf.Pow(trafficIntensity, n);

        avgNumCarsInSystem = trafficIntensity / (probSystemNotIdle);
        avgNumCarsInQ_noQ = Mathf.Pow(trafficIntensity, 2) / (probSystemNotIdle);
        avgNumCarsInQ_WQ = 1 / (probSystemNotIdle);
        avgTimeInQ = trafficIntensity / (probSystemNotIdle) * (1 / rateOfService);
        avgTimeInSystem = 1 / (probSystemNotIdle) * (1 / rateOfService);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
