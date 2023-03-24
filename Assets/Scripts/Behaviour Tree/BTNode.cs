using System.Collections;
using System.Collections.Generic;

// To make this more reusable, it has been made a namespace
namespace BehaviourTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class BTNode
    {
        protected NodeState state;
        
        public BTNode parent;
        protected List<BTNode> children = new List<BTNode>();

        // Shared data
        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        public BTNode()
        {
            parent = null;
        }

        public BTNode(List<BTNode> children)
        {
            foreach (BTNode child in children)
                _Attach(child);
        }

        private void _Attach(BTNode btNode)
        {
            btNode.parent = this;
            children.Add(btNode);
        }

        // Evaluate NodeState prototype
        public virtual NodeState Evaluate() => NodeState.FAILURE;

        // Set sharedd data
        public void SetData(string key, object value)
        {
            _dataContext[key] = value;
        }

        // Recursive Get data function that goes up the tree until desired key is found or root is reached
        public object GetData(string key)
        {
            object value = null;
            if (_dataContext.TryGetValue(key, out value))
                return value;

            BTNode btNode = parent;
            while (btNode != null)
            {
                value = btNode.GetData(key);
                if (value != null)
                    return value;
                btNode = btNode.parent;
            }
            return null;
        }

        // Recursively search for desired key and remove if found, else if root is reached the ignore request
        public bool ClearData(string key)
        {
            if (_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }

            BTNode btNode = parent;
            while (btNode != null)
            {
                bool cleared = btNode.ClearData(key);
                if (cleared)
                    return true;
                btNode = btNode.parent;
            }
            return false;
        }
    }
}
