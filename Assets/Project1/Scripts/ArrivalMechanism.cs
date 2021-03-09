using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrivalMechanism : MonoBehaviour
{
    //our arrival mechanism spawns a car on average 30 times per hour. 
    public static ArrivalMechanism instance { get; private set; }
   
    private float arrivalRate = 30;//30 per hour, Lambda
    private float interArrivalTime;
    public float rateOfService { get; private set; } = 60;//mu
    public float serviceTime { get; private set; } 
    public float trafficIntensity { get; private set; } //rho
    public float probSystemNotIdle { get; private set; }                           //Probability that system is idle
    private float probCarDoesntWait;                //Probability car doesn’t wait
    private float probCarWaits;
    private float n;//number of cars
    private float probNCarsInSystem;//rho^n*(1-rho)
    private float probGreaterNCarsInSystem;//Probability that >=n cars in sys
    private float avgNumCarsInSystem;//rho/(1-rho)
    private float avgNumCarsInQ_noQ;//rho^2/(1-rho)
    private float avgNumCarsInQ_WQ;//1/(1-rho)

    private bool canSpawn=true;
    //car prefab &spawn
    public GameObject carPrefab;

    public bool generateArrivals=true;

    private void Awake()
    {
        instance = this;

        //M/M/1 variables
        interArrivalTime = (1 / arrivalRate)*60*60;//1/lambda
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

    }

    private void Spawn()
    {
        canSpawn = false;
        Invoke("SetSpawnFlag", interArrivalTime);
        GameObject carClone = Instantiate(carPrefab, transform);
    }

    private void SetSpawnFlag()
    {
        canSpawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (generateArrivals && canSpawn) Spawn();
    }
}
