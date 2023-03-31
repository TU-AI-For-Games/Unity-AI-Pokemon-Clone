using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    // Sequence is a composite node that acts like an AND Logic gate.
    // Only succeeds if all children of the node succeed.
    public class Sequence : BTNode
    {
        public Sequence() : base() { }
        public Sequence(List<BTNode> children) : base(children) { }

        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;

            foreach (BTNode node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }
            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }
}