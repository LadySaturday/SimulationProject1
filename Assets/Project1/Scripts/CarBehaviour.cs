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
        refreshTargets += UpdateTarget;

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
                setDestination(targetTransform);
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
                //queueManager.Last()?.setDestination(window);

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
                }
                else
                {
                    setDestination(target.transform);
                }
            }
        }
        catch (Exception)
        {
            Debug.Log("bruh");
            target = null;
            targetTransform = window;
        }
    }

    private void OnDestroy()
    {
        refreshTargets -= UpdateTarget;
    }
}
