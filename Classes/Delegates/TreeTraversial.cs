using System;
using System.Collections.Generic;

namespace GeoGUI.Classes.Delegates {
    public class TreeTraversal<T, U> where T : Item where U : IKey<U> {
        public void InOrderTraversal(Node<T, U> root, Action<Node<T, U>> action) {
            if (root == null || action == null) return;

            Node<T, U> current = root;
            Stack<Node<T, U>> stack = new Stack<Node<T, U>>();

            while (stack.Count > 0 || current != null) {
                while (current != null) {
                    stack.Push(current);
                    current = current.LeftSon;
                }

                current = stack.Pop();
                action?.Invoke(current);
                current = current.RightSon;
            }
        }

        public Node<T, U> FindNode(U keys, Node<T, U> startNode) {
            if (startNode == null) return null;

            Node<T, U> nodeToFind = new Node<T, U>(keys);

            Stack<Node<T, U>> nodesToVisit = new Stack<Node<T, U>>();
            nodesToVisit.Push(startNode);

            while (nodesToVisit.Count > 0) {
                Node<T, U> current = nodesToVisit.Pop();

                if (nodeToFind.KeysData.Equals(current.KeysData)) {
                    Console.WriteLine("FindNode() >>> Node found!");
                    return current;
                }

                int comparison = nodeToFind.KeysData.Compare(current.KeysData, current.Level);

                if (comparison <= 0 && current.LeftSon != null) {
                    nodesToVisit.Push(current.LeftSon);
                } else if (current.RightSon != null) {
                    nodesToVisit.Push(current.RightSon);
                }
            }

            Console.WriteLine("FindNode() >>> Node not found!");
            throw new NullReferenceException();
        }

        public List<Node<T, U>> FindDuplicateNodes(Node<T, U> parent) {
            List<Node<T, U>> duplicateNodes = new List<Node<T, U>>();

            if (parent == null || parent.RightSon == null) return duplicateNodes;

            Stack<Node<T, U>> nodesToVisit = new Stack<Node<T, U>>();
            nodesToVisit.Push(parent.RightSon);

            while (nodesToVisit.Count > 0) {
                Node<T, U> current = nodesToVisit.Pop();
                int comparison = current.KeysData.Compare(parent.KeysData, parent.Level);

                if (comparison == 0) duplicateNodes.Add(current);

                if (current.Level != parent.Level) {
                    if (current.LeftSon != null) nodesToVisit.Push(current.LeftSon);
                    if (current.RightSon != null) nodesToVisit.Push(current.RightSon);
                } else {
                    if (current.LeftSon != null) nodesToVisit.Push(current.LeftSon);
                }
            }

            return duplicateNodes;
        }

        public Node<T, U> FindMinNode(Node<T, U> parent) {
            return FindExtremumNode(parent, (comparison) => comparison <= 0, (node) => node.LeftSon, (node) => node.RightSon);
        }

        public Node<T, U> FindMaxNode(Node<T, U> parent) {
            return FindExtremumNode(parent, (comparison) => comparison >= 0, (node) => node.RightSon, (node) => node.LeftSon);
        }

        private Node<T, U> FindExtremumNode(Node<T, U> parent, Func<int, bool> comparisonFunction, Func<Node<T, U>, Node<T, U>> getSubtreeRootFunction, Func<Node<T, U>, Node<T, U>> getChildFunction) {
            if (parent == null) return null;

            Node<T, U> extremumNode = getSubtreeRootFunction(parent);

            if (extremumNode == null) return null;

            Stack<Node<T, U>> nodesToVisit = new Stack<Node<T, U>>();
            nodesToVisit.Push(extremumNode);

            while (nodesToVisit.Count > 0) {
                Node<T, U> current = nodesToVisit.Pop();

                int comparison = current.KeysData.Compare(extremumNode.KeysData, parent.Level);

                if (comparisonFunction(comparison)) extremumNode = current;

                if (current.Level != parent.Level) {
                    if (current.LeftSon != null) nodesToVisit.Push(current.LeftSon);

                    if (current.RightSon != null) nodesToVisit.Push(current.RightSon);
                } else {
                    Node<T, U> child = getChildFunction(current);

                    if (child != null) nodesToVisit.Push(child);
                }
            }

            return extremumNode;
        }
    }
}
