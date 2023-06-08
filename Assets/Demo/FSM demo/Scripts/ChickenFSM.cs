using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChickenFSM : MonoBehaviour {

    #region variables

    [SerializeField] private GameObject target;

    private StateMachineEngine chickenFSM;
    private MeshCollider visionCollider;
    private NavMeshAgent meshAgent;
    private NavMeshObstacle meshObstacle;
    private ArriveToDestination arriveToDestination;
    private Animator animator;

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        visionCollider = GetComponentInChildren<MeshCollider>();
        meshAgent = GetComponent<NavMeshAgent>();
        meshObstacle = GetComponent<NavMeshObstacle>();
        animator = GetComponent<Animator>();
        chickenFSM = new StateMachineEngine(false);

        CreateStateMachine();
    }

    private void CreateStateMachine()
    {
        // Perceptions
        WatchingPerception seePlayer = chickenFSM.CreatePerception<WatchingPerception>(new WatchingPerception(gameObject, target, visionCollider));
        arriveToDestination = chickenFSM.CreatePerception<ArriveToDestination>(new ArriveToDestination());
        Perception moveTimeout = chickenFSM.CreatePerception<TimerPerception>(5);
        Perception getDestinationOrTimeout = chickenFSM.CreateOrPerception<OrPerception>(moveTimeout, arriveToDestination);
        Perception timer = chickenFSM.CreatePerception<TimerPerception>(15);

        // States
        State idleState = chickenFSM.CreateEntryState("Idle", Idle);
        State chaseState = chickenFSM.CreateState("Chase", Chase);
        State moveState = chickenFSM.CreateState("Move around", MoveAround);

        // Transitios
        chickenFSM.CreateTransition("see the player from idle", idleState, seePlayer, chaseState);
        chickenFSM.CreateTransition("move randomly", idleState, timer, moveState);
        chickenFSM.CreateTransition("get to destination from chase", chaseState, arriveToDestination, idleState);
        //chickenFSM.CreateTransition("keep chasing", chaseState, seePlayer, chaseState);
        chickenFSM.CreateTransition("get to destination from move around", moveState, getDestinationOrTimeout, idleState);
        chickenFSM.CreateTransition("see the player from move around", moveState, seePlayer, chaseState);
    }

    // Update is called once per frame
    private void Update()
    {
        chickenFSM.Update();
        arriveToDestination.SetPosition(transform.position);
    }

    private void Idle()
    {
        meshAgent.enabled = false;
        meshObstacle.enabled = true;
        animator.SetBool("isMoving", false);
    }

    private void Chase()
    {
        meshObstacle.enabled = false;
        meshAgent.enabled = true;
        animator.SetBool("isMoving", true);

        meshAgent.destination = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        arriveToDestination.SetDestination(meshAgent.destination);
    }

    private void MoveAround()
    {
        meshObstacle.enabled = false;
        meshAgent.enabled = true;
        animator.SetBool("isMoving", true);

        Vector3 positionToRun = Random.insideUnitSphere * 13;
        meshAgent.destination = new Vector3(positionToRun.x, transform.position.y, positionToRun.z);
        arriveToDestination.SetDestination(meshAgent.destination);
    }
}