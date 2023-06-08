using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestBoyFSM : MonoBehaviour {

    #region variables

    [SerializeField] private float minDistanceToChicken = 5;
    [SerializeField] private GameObject chicken;

    private StateMachineEngine testBoyFSM;
    private NavMeshAgent meshAgent;
    private ArriveToDestination arriveToDestination;
    private float distanceToChicken = 10;

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        meshAgent = GetComponent<NavMeshAgent>();
        testBoyFSM = new StateMachineEngine(false);

        CreateStateMachine();
    }

    private void CreateStateMachine()
    {
        // Perceptions
        Perception click = testBoyFSM.CreatePerception<PushPerception>();
        Perception clickOnMoving = testBoyFSM.CreatePerception<PushPerception>();
        arriveToDestination = testBoyFSM.CreatePerception<ArriveToDestination>(new ArriveToDestination());
        Perception timeToStopRunning = testBoyFSM.CreatePerception<TimerPerception>(7);
        Perception stopRunningAway = testBoyFSM.CreateOrPerception<OrPerception>(arriveToDestination, timeToStopRunning);
        Perception chickenNear = testBoyFSM.CreatePerception<ValuePerception>(() => distanceToChicken < minDistanceToChicken);

        // States
        State idleState = testBoyFSM.CreateEntryState("Idle");
        State movingState = testBoyFSM.CreateState("Moving", Move);
        State runAwayState = testBoyFSM.CreateState("Run away", RunAway);

        // Transitions
        testBoyFSM.CreateTransition("mouse clicked", idleState, click, movingState);
        testBoyFSM.CreateTransition("get to destination from moving", movingState, stopRunningAway, idleState);
        testBoyFSM.CreateTransition("chicken near from idle", idleState, chickenNear, runAwayState);
        testBoyFSM.CreateTransition("get to destination from run away", runAwayState, stopRunningAway, idleState);
        testBoyFSM.CreateTransition("chicken near from moving", movingState, chickenNear, runAwayState);
        testBoyFSM.CreateTransition("change destination", movingState, clickOnMoving, movingState);
    }

    // Update is called once per frame
    private void Update()
    {
        testBoyFSM.Update();
        arriveToDestination.SetPosition(transform.position);
        distanceToChicken = (chicken.transform.position - transform.position).magnitude;

        if(Input.GetMouseButtonDown(0)) {
            testBoyFSM.Fire("mouse clicked");
            testBoyFSM.Fire("change destination");
        }
    }

    private void Move()
    {
        meshAgent.speed = 3.5f;
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(cameraRay, out RaycastHit hit, 100f)) {
            meshAgent.destination = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            arriveToDestination.SetDestination(meshAgent.destination);
        }
    }

    private void RunAway()
    {
        meshAgent.speed = 7f;
        Vector3 positionToRun = Random.insideUnitSphere * 13;
        meshAgent.destination = new Vector3(positionToRun.x, transform.position.y, positionToRun.z);
        arriveToDestination.SetDestination(meshAgent.destination);
    }
}