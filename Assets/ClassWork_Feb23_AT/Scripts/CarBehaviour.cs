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
    // Start is called before the first frame update
    void Start()
    {
        

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
                agent.isStopped = true;
            },
            OnStay = () =>
            {

            },
            OnExit = () =>
            {
                agent.isStopped = false;
            }

        });
        stateMachine.AddState(new StateMachine.State()
        {
            Name = "Exit",
            OnEnter = () =>
            {
                setDestination(exit);
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

    private void Update()
    {
        stateMachine.Update();
    }
}
