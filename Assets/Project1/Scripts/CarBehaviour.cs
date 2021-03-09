using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class CarBehaviour : MonoBehaviour
{
    private NavMeshAgent agent;
    private static Transform window;
    private static Transform exit;
    public StateMachine stateMachine { get; private set; }
    private QueueManager queueManager;

    private bool hasBeenServiced = false;
    // Start is called before the first frame update
    void Start()
    {

        queueManager = GameObject.FindGameObjectWithTag("DriveThruWindow").GetComponent<QueueManager>();
        queueManager.Add(this);

        agent = GetComponent<NavMeshAgent>();
        if (!window)
            window = GameObject.FindGameObjectWithTag("DriveThruWindow").transform;
        if (!exit)
            exit = GameObject.FindGameObjectWithTag("CarExit").transform;

        stateMachine = new StateMachine();
        stateMachine.AddState(new StateMachine.State()
        {
            Name = "Entered",
            OnEnter = () =>
            {
                if (queueManager.Count() > 1)
                {
                    setDestination((queueManager.Next()).transform);
                }
                   
                else
                    setDestination(window);
            },
            OnStay = () =>
            {

            },
            OnExit = () =>
            {
                
            }

        });
        stateMachine.AddState(new StateMachine.State()
        {
            Name = "Service",
            OnEnter = () =>
            {

                if (hasBeenServiced)
                {
                    stateMachine.TransitionTo("Exit");
                    
                }
                    
                else
                {
                    agent.isStopped = true;
                    hasBeenServiced = true;
                }
            },
            OnStay = () =>
            {

            },
            OnExit = () =>
            {
              
            }

        });
        stateMachine.AddState(new StateMachine.State()
        {
            Name = "Exit",
            OnEnter = () =>
            {
                agent.isStopped = false;
                setDestination(exit);
                queueManager.PopFirst();
                queueManager.Last()?.setDestination(window);
            },
            OnStay = () =>
            {

            },
            OnExit = () =>
            {
                
            }

        });

        stateMachine.TransitionTo("Entered");
    }

    public void setDestination(Transform target)
    {
        agent.SetDestination(target.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CarExit")
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        stateMachine.Update();
        
    }
}
