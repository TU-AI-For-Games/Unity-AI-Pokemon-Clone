using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class Tree : MonoBehaviour
    {
        // Referece to root node
        private BTNode _root = null;

        protected void Start() // On start builds behaviour tree according to defined SetupTree.
        {
            _root = SetupTree();
        }

        private void Update() // If it has a tree, Evaluates continuously.
        {
            if (_root != null)
                _root.Evaluate();
        }

        protected abstract BTNode SetupTree();
    } 
}
