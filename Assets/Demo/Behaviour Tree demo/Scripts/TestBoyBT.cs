using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestBoyBT : MonoBehaviour {

    #region variables

    public DoorStatus DoorState { get; set; }

    [SerializeField] private AudioClip doorOpenClip;
    [SerializeField] private AudioClip keyFoundClip;
    [SerializeField] private GameObject explosionFX;
    [SerializeField] private AudioClip explosionClip;

    private Transform doorPosition;
    private NavMeshAgent meshAgent;
    private AudioSource audioSource;
    private bool doorOpened;
    private bool keyObatined;
    private bool doorSmashed;

    private BehaviourTreeEngine behaviourTree;
    private SequenceNode rootSequence;
    private SelectorNode selectorDoor;
    private SequenceNode sequenceDoor;
    private LeafNode walkToDoor1;
    private LeafNode walkToDoor2;
    private LeafNode enterTheHouse;
    private LeafNode openDoor1;
    private LeafNode openDoor2;
    private LeafNode unlockDoor;
    private LeafNode smashDoor;

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        meshAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        doorPosition = GameObject.FindGameObjectWithTag("Door point").transform;

        doorOpened = false;
        keyObatined = false;
        doorSmashed = false;

        CreateBehaviourTree();
    }

    private void CreateBehaviourTree()
    {
        behaviourTree = new BehaviourTreeEngine(false);

        rootSequence = behaviourTree.CreateSequenceNode("Root", false);
        selectorDoor = behaviourTree.CreateSelectorNode("Selector door");
        sequenceDoor = behaviourTree.CreateSequenceNode("Sequence door", false);
        walkToDoor1 = behaviourTree.CreateLeafNode("Walk to door 1", WalkToDoor, ArriveToDoor);
        walkToDoor2 = behaviourTree.CreateLeafNode("Walk to door 2", WalkToDoor, ArriveToDoor);
        enterTheHouse = behaviourTree.CreateLeafNode("Enter the house", EnterTheHouse, HasEnteredTheHouse);
        openDoor1 = behaviourTree.CreateLeafNode("Open the door 1", OpenDoor, DoorOpened);
        openDoor2 = behaviourTree.CreateLeafNode("Open the door 2", OpenDoor, DoorOpened);
        unlockDoor = behaviourTree.CreateLeafNode("Find key", UnlockDoor, IsTheDoorUnlocked);
        smashDoor = behaviourTree.CreateLeafNode("Explode door", SmashDoor, DoorSmashed);

        rootSequence.AddChild(walkToDoor1);
        rootSequence.AddChild(selectorDoor);
        rootSequence.AddChild(enterTheHouse);

        selectorDoor.AddChild(openDoor1);
        selectorDoor.AddChild(sequenceDoor);
        selectorDoor.AddChild(smashDoor);

        sequenceDoor.AddChild(unlockDoor);
        sequenceDoor.AddChild(walkToDoor2);
        sequenceDoor.AddChild(openDoor2);

        behaviourTree.SetRootNode(rootSequence);
    }

    // Update is called once per frame
    private void Update()
    {
        behaviourTree.Update();

        //print("Nodo activo: " + behaviourTree.ActiveNode.StateNode.Name + " - Estado del nodo: " + behaviourTree.ActiveNode.ReturnValue);
    }

    private void WalkToDoor()
    {
        meshAgent.destination = new Vector3(doorPosition.position.x, transform.position.y, doorPosition.position.z);
    }

    private ReturnValues ArriveToDoor()
    {
        if(Mathf.Abs(transform.position.x - doorPosition.position.x) < 0.2 &&
            Mathf.Abs(transform.position.z - doorPosition.position.z) < 0.2) {
            return ReturnValues.Succeed;
        }
        else {
            return ReturnValues.Running;
        }
    }

    private void EnterTheHouse()
    {
        Destroy(this.gameObject, 2);
    }

    private ReturnValues HasEnteredTheHouse()
    {
        return ReturnValues.Succeed;
    }

    private void OpenDoor()
    {
        if(DoorState == DoorStatus.Opened) {
            audioSource.clip = doorOpenClip;
            audioSource.Play();
            doorOpened = true;
        }
        else {
            doorOpened = false;
        }
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

    private void UnlockDoor()
    {
        GameObject key = GameObject.FindGameObjectWithTag("Key");

        if(key != null) {
            meshAgent.destination = new Vector3(key.transform.position.x, transform.position.y, key.transform.position.z);
        }
    }

    private ReturnValues IsTheDoorUnlocked()
    {
        if(keyObatined) {
            return ReturnValues.Succeed;
        }
        else if(GameObject.FindGameObjectWithTag("Key") == null) {
            return ReturnValues.Failed;
        }
        else {
            return ReturnValues.Running;
        }
    }

    private void SmashDoor()
    {
        GameObject explosion = Instantiate(explosionFX, doorPosition);
        audioSource.clip = explosionClip;
        audioSource.Play();
        doorSmashed = true;

        Destroy(explosion, 3);
    }

    private ReturnValues DoorSmashed()
    {
        if(doorSmashed) {
            return ReturnValues.Succeed;
        }
        else {
            return ReturnValues.Running;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Key") {
            audioSource.clip = keyFoundClip;
            audioSource.Play();
            keyObatined = true;
            DoorState = DoorStatus.Opened;

            Destroy(other.gameObject);
        }
    }
}