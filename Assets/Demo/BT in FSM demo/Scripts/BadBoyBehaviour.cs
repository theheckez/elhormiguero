using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BadBoyBehaviour : MonoBehaviour {

    #region variables

    [SerializeField] private Transform[] routePoints;

    private StateMachineBehaviour stateMachine;
    private BehaviourTreeEngine behaviourTree;
    private NavMeshAgent meshAgent;

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        meshAgent = GetComponent<NavMeshAgent>();

        CreateBehaviourTree();
    }

    private void CreateBehaviourTree()
    {
        behaviourTree = new BehaviourTreeEngine(false);

        LeafNode toPoint1 = behaviourTree.CreateLeafNode("To point 1", () => ToPoint(routePoints[0]), () => ArrivedToPoint(routePoints[0]));
        LeafNode toPoint2 = behaviourTree.CreateLeafNode("To point 2", () => ToPoint(routePoints[1]), () => ArrivedToPoint(routePoints[1]));
        LeafNode toPoint3 = behaviourTree.CreateLeafNode("To point 3", () => ToPoint(routePoints[2]), () => ArrivedToPoint(routePoints[2]));
        LeafNode toPoint4 = behaviourTree.CreateLeafNode("To point 4", () => ToPoint(routePoints[3]), () => ArrivedToPoint(routePoints[3]));
        LeafNode toPoint5 = behaviourTree.CreateLeafNode("To point 5", () => ToPoint(routePoints[4]), () => ArrivedToPoint(routePoints[4]));
        LeafNode toPoint6 = behaviourTree.CreateLeafNode("To point 6", () => ToPoint(routePoints[5]), () => ArrivedToPoint(routePoints[5]));
        SequenceNode sequenceRoute = behaviourTree.CreateSequenceNode("Route", false);
        sequenceRoute.AddChild(toPoint1);
        sequenceRoute.AddChild(toPoint2);
        sequenceRoute.AddChild(toPoint3);
        sequenceRoute.AddChild(toPoint4);
        sequenceRoute.AddChild(toPoint5);
        sequenceRoute.AddChild(toPoint6);
        LoopDecoratorNode loopNode = behaviourTree.CreateLoopNode("Loop root", sequenceRoute);

        behaviourTree.SetRootNode(loopNode);
    }

    // Update is called once per frame
    private void Update()
    {
        behaviourTree.Update();
    }

    private void ToPoint(Transform destinationPoint)
    {
        meshAgent.destination = destinationPoint.position;
    }

    private ReturnValues ArrivedToPoint(Transform destinationPoint)
    {
        if(Mathf.Abs((transform.position - destinationPoint.position).magnitude) <= 0.5f) {
            return ReturnValues.Succeed;
        }
        else {
            return ReturnValues.Running;
        }
    }
}