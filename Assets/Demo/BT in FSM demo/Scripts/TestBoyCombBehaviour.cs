using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestBoyCombBehaviour : MonoBehaviour {

    #region variables

    [SerializeField] private GameObject badBoy;
    [SerializeField] private Transform housePoint;
    [SerializeField] private GameObject key;
    [SerializeField] private AudioClip doorOpenClip;
    [SerializeField] private AudioClip keyTakenClip;

    private StateMachineEngine stateMachine;
    private BehaviourTreeEngine behaviourTree;
    private NavMeshAgent meshAgent;
    private AudioSource audioSource;
    private bool doorOpened;
    private bool keyTaken;

    private ArriveToDestination arriveToHouse;
    private ArriveToDestination pointToRun;

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        meshAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        housePoint = GameObject.FindGameObjectWithTag("Door point").transform;
        key = GameObject.FindGameObjectWithTag("Key");
        doorOpened = false;
        keyTaken = false;

        CreateBehaviourTree();
        CreateFiniteStateMachine();
    }

    private void CreateFiniteStateMachine()
    {
        stateMachine = new StateMachineEngine(false);

        arriveToHouse = stateMachine.CreatePerception<ArriveToDestination>(new ArriveToDestination());
        arriveToHouse.SetDestination(housePoint.position);
        ValuePerception enemyNear = stateMachine.CreatePerception<ValuePerception>(() => Vector3.Distance(transform.position, badBoy.transform.position) < 8);
        BehaviourTreeStatusPerception enterTheHouse = stateMachine.CreatePerception<BehaviourTreeStatusPerception>(behaviourTree, ReturnValues.Succeed);
        pointToRun = stateMachine.CreatePerception<ArriveToDestination>(new ArriveToDestination());

        State goToHouse = stateMachine.CreateEntryState("Go to house", ToHouse);
        State subBehaviourTree = stateMachine.CreateSubStateMachine("Sub behaviour tree", behaviourTree);
        State runAway = stateMachine.CreateState("Run away", RunAway);
        State enterHouse = stateMachine.CreateState("Enter in house", EnterHouse);

        stateMachine.CreateTransition("To enter the house", goToHouse, arriveToHouse, subBehaviourTree);
        stateMachine.CreateTransition("To run away", goToHouse, enemyNear, runAway);
        behaviourTree.CreateExitTransition("Run away", subBehaviourTree, enemyNear, runAway);
        behaviourTree.CreateExitTransition("Enter the house", subBehaviourTree, enterTheHouse, enterHouse);
        stateMachine.CreateTransition("Stop running", runAway, pointToRun, goToHouse);
    }

    private void CreateBehaviourTree()
    {
        behaviourTree = new BehaviourTreeEngine(true);

        LeafNode toKey = behaviourTree.CreateLeafNode("To key", GoToKey, ArriveToKey);
        LeafNode getKey = behaviourTree.CreateLeafNode("Get key", GetKey, KeyTaken);
        LeafNode backToDoor = behaviourTree.CreateLeafNode("Back to door", ToDoor, ArriveToDoor);
        LeafNode openDoor = behaviourTree.CreateLeafNode("Open door", OpenDoor, DoorOpened);
        SequenceNode sequenceRoot = behaviourTree.CreateSequenceNode("Sequence root", false);
        sequenceRoot.AddChild(toKey);
        sequenceRoot.AddChild(getKey);
        sequenceRoot.AddChild(backToDoor);
        sequenceRoot.AddChild(openDoor);

        behaviourTree.SetRootNode(sequenceRoot);
    }

    // Update is called once per frame
    private void Update()
    {
        if(key == null) {
            key = GameObject.FindGameObjectWithTag("Key");
        }

        arriveToHouse.SetPosition(transform.position);
        pointToRun.SetPosition(transform.position);

        stateMachine.Update();
        behaviourTree.Update();
    }

    private void ToHouse()
    {
        meshAgent.speed = 3.5f;
        meshAgent.destination = housePoint.position;
    }

    private void RunAway()
    {
        meshAgent.speed = 8;
        Vector3 runPoint = Random.insideUnitSphere * 20;
        runPoint.y = transform.position.y;
        meshAgent.destination = runPoint;
        pointToRun.SetDestination(runPoint);
    }

    private void EnterHouse()
    {
        Destroy(this.gameObject);
    }

    private void GoToKey()
    {
        if(!keyTaken) {
            meshAgent.destination = key.transform.position;
        }
    }

    private ReturnValues ArriveToKey()
    {
        if(keyTaken) {
            return ReturnValues.Succeed;
        }
        else if(Vector3.Distance(transform.position, key.transform.position) < 0.3f) {
            keyTaken = true;
            return ReturnValues.Succeed;
        }
        else {
            return ReturnValues.Running;
        }
    }

    private void GetKey()
    {
        audioSource.clip = keyTakenClip;
        audioSource.Play();
        Destroy(key);
    }

    private ReturnValues KeyTaken()
    {
        if(GameObject.FindGameObjectWithTag("Key")) {
            return ReturnValues.Running;
        }
        else {
            return ReturnValues.Succeed;
        }
    }

    private void ToDoor()
    {
        meshAgent.destination = housePoint.position;
    }

    private ReturnValues ArriveToDoor()
    {
        if(Vector3.Distance(transform.position, housePoint.position) < 0.3f) {
            return ReturnValues.Succeed;
        }
        else {
            return ReturnValues.Running;
        }
    }

    private void OpenDoor()
    {
        audioSource.clip = doorOpenClip;
        audioSource.Play();
        doorOpened = true;
    }

    private ReturnValues DoorOpened()
    {
        if(doorOpened) {
            return ReturnValues.Succeed;
        }
        else {
            return ReturnValues.Failed;
        }
    }
}