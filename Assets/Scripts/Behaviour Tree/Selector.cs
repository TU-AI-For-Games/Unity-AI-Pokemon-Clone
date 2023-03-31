using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    // Selector is a composite node that acts like an OR Logic gate.
    // Returns early if a child has succeeded or is running.
    public class Selector : BTNode
    {
        public Selector() : base() { }
        public Selector(List<BTNode> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (BTNode node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.SUCCESS;
                        return state;
                    default:
                        continue;
                }
            }
            state = NodeState.FAILURE;
            return state;
        }
    }
}
