﻿using GeoGUI.Classes.Crate;
using GeoGUI.Classes.Prototype;
using GeoGUI.Classes.Structure;
using System.Collections.Generic;

namespace GeoGUI.Classes.Strategy {
    public class RangeSearchStrategy<T, U> : IStrategy<T, U> where T : Item where U : IKey<U> {
        private double range;

        public RangeSearchStrategy(double range) {
            this.range = range;
        }

        public List<T> Traverse(KDTree<T, U> tree, U keys) {
            List<T> result = new List<T>();

            if (tree.Root == null) return result;

            Stack<Node<T, U>> nodesToVisit = new Stack<Node<T, U>>();
            nodesToVisit.Push(tree.Root);

            while (nodesToVisit.Count > 0) {
                Node<T, U> currentNode = nodesToVisit.Pop();

                if (currentNode.KeysData.CloseWithin(keys, this.range)) result.AddRange(currentNode.NodeData);

                if (currentNode.LeftSon != null) nodesToVisit.Push(currentNode.LeftSon);

                if (currentNode.RightSon != null) nodesToVisit.Push(currentNode.RightSon);
            }

            return result;
        }
    }
}
