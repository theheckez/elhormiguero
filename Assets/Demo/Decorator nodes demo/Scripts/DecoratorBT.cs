using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoratorBT : MonoBehaviour {

    #region variables

    [SerializeField] private GameObject fish;
    [SerializeField] private GameObject boot;
    [SerializeField] private Transform rodPoint;
    [SerializeField] private Transform toWaterPoint;
    [SerializeField] private Transform toBasketPoint;

    private Animator animator;
    private BehaviourTreeEngine behaviourTree;
    private bool fishCatched;
    private int thingsCathced;

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        thingsCathced = 0;

        CreateBehaviourTree();
    }

    private void CreateBehaviourTree()
    {
        behaviourTree = new BehaviourTreeEngine(false);

        LeafNode throwRod = behaviourTree.CreateLeafNode("Throw rod", ThrowRod, RodThrown);
        LeafNode catchSomething = behaviourTree.CreateLeafNode("Catch something", Catch, SomethingCatched);
        LeafNode returnToWater = behaviourTree.CreateLeafNode("Return to water", () => Invoke("ReturnToWater", 2), ReturnedToWater);
        LeafNode storeInBasket = behaviourTree.CreateLeafNode("Store in the basket", () => Invoke("StoreBasket", 2), StoredInBasket);
        TimerDecoratorNode timerNode = behaviourTree.CreateTimerNode("Timer node", catchSomething, 3);
        SelectorNode selectorNode = behaviourTree.CreateSelectorNode("Selector node");
        selectorNode.AddChild(returnToWater);
        selectorNode.AddChild(storeInBasket);
        SequenceNode sequenceNode = behaviourTree.CreateSequenceNode("Sequence node", false);
        sequenceNode.AddChild(throwRod);
        sequenceNode.AddChild(timerNode);
        sequenceNode.AddChild(selectorNode);
        LoopDecoratorNode rootNode = behaviourTree.CreateLoopNode("Root node", sequenceNode);

        behaviourTree.SetRootNode(rootNode);
    }

    // Update is called once per frame
    private void Update()
    {
        behaviourTree.Update();
    }

    private void ThrowRod()
    {
    }

    private ReturnValues RodThrown()
    {
        if(animator.GetBool("throw")) {
            //animator.SetBool("throw", false);
            return ReturnValues.Succeed;
        }
        else {
            return ReturnValues.Running;
        }
    }

    private void Catch()
    {
        int random = Random.Range(0, 2);

        animator.SetBool("throw", false);
        animator.SetBool("pickUp", true);
        GameObject gameObject;
        Vector3 newPosition = new Vector3(rodPoint.position.x, -1, rodPoint.position.z);
        if(random == 0) {
            Quaternion newRotation = Quaternion.Euler(-60, 25, rodPoint.rotation.z);
            gameObject = Instantiate(boot, newPosition, newRotation, rodPoint);
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            fishCatched = false;
        }
        else {
            Quaternion newRotation = Quaternion.Euler(-90, rodPoint.rotation.y, 90);
            gameObject = Instantiate(fish, newPosition, newRotation, rodPoint);
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            fishCatched = true;
        }

        thingsCathced++;
        Destroy(gameObject, 2);
    }

    private ReturnValues SomethingCatched()
    {
        if(animator.GetBool("pickUp")) {
            //animator.SetBool("pickUp", false);
            return ReturnValues.Succeed;
        }
        else {
            return ReturnValues.Running;
        }
    }

    private void ReturnToWater()
    {
        if(!fishCatched) {
            Quaternion newRotation = Quaternion.Euler(-60, 25, toWaterPoint.rotation.z);
            GameObject bootObject = Instantiate(boot, toWaterPoint.position, newRotation);

            animator.SetBool("pickUp", false);
            animator.SetBool("throw", true);
            Destroy(bootObject, 2);
        }
    }

    private ReturnValues ReturnedToWater()
    {
        if(fishCatched) {
            return ReturnValues.Failed;
        }
        else {
            return ReturnValues.Succeed;
        }
    }

    private void StoreBasket()
    {
        if(fishCatched) {
            Quaternion newRotation = Quaternion.Euler(-90, toBasketPoint.rotation.y, toBasketPoint.rotation.z);
            Instantiate(fish, toBasketPoint.position, newRotation);

            animator.SetBool("pickUp", false);
            animator.SetBool("throw", true);
        }
    }

    private ReturnValues StoredInBasket()
    {
        if(fishCatched) {
            return ReturnValues.Succeed;
        }
        else {
            return ReturnValues.Failed;
        }
    }
}