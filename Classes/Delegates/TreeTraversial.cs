using System;
using System.Collections.Generic;

namespace GeoGUI.Classes.Delegates {
    public class TreeTraversal<T, U> where T : Item where U : IKey<U> {
        public List<Node<T, U>> InOrderTraversal(Node<T, U> root) {
            List<Node<T, U>> result = new List<Node<T, U>>();

            if (root == null) return result;

            Node<T, U> current = root;
            Stack<Node<T, U>> stack = new Stack<Node<T, U>>();

            while (stack.Count > 0 || current != null) {
                while (current != null) {
                    stack.Push(current);
                    current = current.LeftSon;
                }

                current = stack.Pop();
                result.Add(current);
                current = current.RightSon;
            }

            return result;
        }

        public void InOrderActionTraversal(Node<T, U> root, Action<Node<T, U>> action) {
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

        public Node<T, U> FindMinNode(Node<T, U> parent) {
            if (parent == null || parent.RightSon == null) return null;

            Node<T, U> minNode = parent.RightSon;
            Stack<Node<T, U>> nodesToVisit = new Stack<Node<T, U>>();
            nodesToVisit.Push(minNode);

            while (nodesToVisit.Count > 0) {
                Node<T, U> current = nodesToVisit.Pop();

                int comparison = current.KeysData.Compare(minNode.KeysData, parent.Level);

                if (comparison <= 0) minNode = current;

                if (current.Level != parent.Level) {
                    if (current.LeftSon != null) nodesToVisit.Push(current.LeftSon);
                    if (current.RightSon != null) nodesToVisit.Push(current.RightSon);
                } else {
                    if (current.LeftSon != null) nodesToVisit.Push(current.LeftSon);
                }
            }

            return minNode;
        }

        public Node<T, U> FindMaxNode(Node<T, U> parent) {
            if (parent == null || parent.LeftSon == null) return null;

            Node<T, U> maxNode = parent.LeftSon;
            Stack<Node<T, U>> nodesToVisit = new Stack<Node<T, U>>();
            nodesToVisit.Push(maxNode);

            while (nodesToVisit.Count > 0) {
                Node<T, U> current = nodesToVisit.Pop();

                int comparison = current.KeysData.Compare(maxNode.KeysData, parent.Level);

                if (comparison >= 0) maxNode = current;

                if (current.Level != parent.Level) {
                    if (current.LeftSon != null) nodesToVisit.Push(current.LeftSon);
                    if (current.RightSon != null) nodesToVisit.Push(current.RightSon);
                } else {
                    if (current.RightSon != null) nodesToVisit.Push(current.RightSon);
                }
            }

            return maxNode;
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
    }
}
