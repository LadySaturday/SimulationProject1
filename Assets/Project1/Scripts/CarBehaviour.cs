using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class CarBehaviour : MonoBehaviour
{
    public static event System.Action refreshTargets;
    private NavMeshAgent agent;
    private static Transform window;
    private static Transform exit;
    public StateMachine stateMachine { get; private set; }
    private QueueManager queueManager;

    public bool hasBeenServiced = false;

    private CarBehaviour target;
    private Transform targetTransform;
    private int index;
    // Start is called before the first frame update
    void Start()
    {
        transform.parent = null;
       // refreshTargets += UpdateTarget;
        queueManager = GameObject.FindGameObjectWithTag("DriveThruWindow").GetComponent<QueueManager>();
        index=queueManager.Add(this);

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
                    target = queueManager.Next();
                    setDestination(target.transform);
                    targetTransform=target.transform;
                }
                else
                {
                    setDestination(window);
                    targetTransform = window;
                }
                   
            },
            OnStay = () =>
            {
                setDestination(targetTransform);
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
                    agent.isStopped = true;
                    
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
                refreshTargets?.Invoke();
            },
            OnStay = () =>
            {
                setDestination(exit);
            },
            OnExit = () =>
            {
                
            }

        });
        stateMachine.TransitionTo("Entered");
    }

    public void setDestination(Transform target)
    {
        if (target)
            agent.SetDestination(target.position);        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CarExit")
        {
            Destroy(this.gameObject);
        }

        else if (other.gameObject.tag == "Car"&&stateMachine.CurrentState.Name!="Exit")
            agent.isStopped = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Car")
            agent.isStopped = false;
        else if (other.gameObject.tag == "DriveThruWindow" && stateMachine.CurrentState.Name != "Exit")
            stateMachine.TransitionTo("exit");
    }


    private void Update()
    {
        stateMachine.Update();       
    }


    private void UpdateTarget()
    {
        
        try
        {
            if (stateMachine.CurrentState.Name != "exit")
            {
                if (!target || target.stateMachine.CurrentState.Name == "exit")
                {
                    target = null;
                    setDestination(window);
                    targetTransform = window;
                    agent.isStopped = false;
                }
                else
                {
                    
                }
            }
            else
            {
                target = null;
                setDestination(exit);
                targetTransform = exit;
                agent.isStopped = false;
            }
        }
        catch (Exception)
        {
            target = null;
            targetTransform = exit;
        }
    }

    private void OnDestroy()
    {
        refreshTargets -= UpdateTarget;
        
    }

  
    
}
