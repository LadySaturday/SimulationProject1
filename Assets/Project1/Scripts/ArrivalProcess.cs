using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrivalProcess : MonoBehaviour
{

    public GameObject carPrefab;
    public Transform carSpawnPlace;

    public float arrivalRateAsCarsPerHour = 30;// car/hour
    public float interArrivalTime; // = 1.0 / arrivalRateAsCarsPerHour;
    private float interArrivalTimeInMinutes;
    private float interArrivalTimeInSeconds;

    //public float arrivalRateAsCarsPerHour = 20; // car/hour
    public bool generateArrivals = true;

    //New as of Feb.23rd
    //Simple generation distribution - Uniform(min,max)
    //
    public float minInterArrivalTimeInSeconds = 3; 
    public float maxInterArrivalTimeInSeconds = 60;
    //
    public enum ArrivalIntervalTimeStrategy
    {
        ConstantIntervalTime,
        UniformIntervalTime,
        ExponentialIntervalTime,
        ObservedIntervalTime
    }

    public ArrivalIntervalTimeStrategy arrivalIntervalTimeStrategy=ArrivalIntervalTimeStrategy.UniformIntervalTime;

    //New as of Feb.25th
    QueueManager queueManager;


    // Start is called before the first frame update
    void Start()
    {
        queueManager = GameObject.FindGameObjectWithTag("DriveThruWindow").GetComponent<QueueManager>();
        interArrivalTime = 1.0f / arrivalRateAsCarsPerHour;
        interArrivalTimeInMinutes = interArrivalTime * 60;
        interArrivalTimeInSeconds = interArrivalTimeInMinutes * 60;
        StartCoroutine(GenerateArrivals());

    }
   
    IEnumerator GenerateArrivals()
    {
        while (generateArrivals)
        {
            GameObject carGO=Instantiate(carPrefab, carSpawnPlace.position, Quaternion.identity);
         
            float timeToNextArrivalInSec = interArrivalTimeInSeconds;
            switch (arrivalIntervalTimeStrategy)
            {
                case ArrivalIntervalTimeStrategy.ConstantIntervalTime:
                    timeToNextArrivalInSec= interArrivalTimeInSeconds;
                    break;
                case ArrivalIntervalTimeStrategy.UniformIntervalTime:
                    timeToNextArrivalInSec = Random.Range(minInterArrivalTimeInSeconds, maxInterArrivalTimeInSeconds);
                    break;
                case ArrivalIntervalTimeStrategy.ExponentialIntervalTime:
                    float U = Random.value;
                    float Lambda = 1 / arrivalRateAsCarsPerHour;
                    timeToNextArrivalInSec = Utilities.GetExp(U,Lambda);
                    break;
                case ArrivalIntervalTimeStrategy.ObservedIntervalTime:
                    timeToNextArrivalInSec = interArrivalTimeInSeconds;
                    break;
                default:
                    print("No acceptable arrivalIntervalTimeStrategy:" + arrivalIntervalTimeStrategy);
                    break;

            }

            //New as of Feb.23rd
            //float timeToNextArrivalInSec = Random.Range(minInterArrivalTimeInSeconds,maxInterArrivalTimeInSeconds);
            yield return new WaitForSeconds(timeToNextArrivalInSec);

            //yield return new WaitForSeconds(interArrivalTimeInSeconds);

        }

    }
}
