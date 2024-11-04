using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGUI.Classes {
    internal class KDTree<T, U> where T : Item where U : IKey<U> {
        private Node<T, U> root;
        private int treeSize;
        private int dataSize;

        public KDTree() {
            this.root = null;
            this.treeSize = 0;
            this.dataSize = 0;
        }

        public void InsertNode(ref T data, U keys) {
            this.dataSize++;
            Node<T, U> current = this.root;
            Node<T, U> parent = null;
            int level = 0;

            Node<T, U> nodeToInsert = new Node<T, U>(keys);
            nodeToInsert.NodeData.Add(data);

            if (this.root == null) {
                this.treeSize++;
                this.root = nodeToInsert;
                return;
            }

            while (current != null) {
                parent = current;

                if (nodeToInsert.KeysData.Equals(current.KeysData)) {
                    current.NodeData.Add(data);
                    Console.WriteLine("InsertNode() >>> Data entry inserted!");
                    return;
                }

                int comparison = nodeToInsert.KeysData.Compare(current.KeysData, level);
                current = (comparison <= 0) ? current.LeftSon : current.RightSon;
                level++;
            }

            if (parent == null) return;

            int finalComparison = nodeToInsert.KeysData.Compare(parent.KeysData, level - 1);

            if (finalComparison <= 0) {
                parent.LeftSon = nodeToInsert;
            } else {
                parent.RightSon = nodeToInsert;
            }

            nodeToInsert.Parent = parent;
            nodeToInsert.Level = level;

            this.treeSize++;

            Console.WriteLine("InsertNode() >>> Node inserted!");
        }

        public List<T> FindNodes(U keys) {
            Node<T, U> nodeToFind = new Node<T, U>(keys);
            List<T> result = new List<T>();
            Node<T, U> current = this.root;
            int level = 0;

            while (current != null) {
                if (nodeToFind.KeysData.Equals(current.KeysData)) {
                    result.AddRange(current.NodeData);
                    Console.WriteLine("FindNodes() >>> Node found!");
                    break;
                }

                int comparison = nodeToFind.KeysData.Compare(current.KeysData, level);
                current = (comparison <= 0) ? current.LeftSon : current.RightSon;
                level++;
            }

            if (result.Count == 0) {
                Console.WriteLine("FindNodes() >>> Node not found!");
                throw new NullReferenceException();
            }

            return result;
        }

        public void UpdateNode(ref T oldData, U oldKeys, ref T newData, U newKeys) {
            if (this.root == null) return;

            Node<T, U> nodeToUpdate = this.FindNode(oldKeys, this.root);

            if (nodeToUpdate == null) return;

            bool multipleDataEntries = nodeToUpdate.NodeData.Count > 1;

            bool keysChanged = !oldKeys.Equals(newKeys);

            if (!keysChanged) {
                // Case 1: Keys unchanged
                Console.WriteLine("UpdateNode() >>> Data entry updated in-place!");
                for (int i = 0; i < nodeToUpdate.NodeData.Count; i++) {
                    if (nodeToUpdate.NodeData[i].EqualsByID(oldData)) {
                        nodeToUpdate.NodeData[i] = newData;
                        return;
                    }
                }
            } else {
                // Case 2: Keys changed
                if (multipleDataEntries) {
                    Console.WriteLine("UpdateNode() >>> Data entry removed and reinserted with new keys.");
                    nodeToUpdate.NodeData.Remove(oldData);
                    InsertNode(ref newData, newKeys);
                    this.dataSize--;
                } else {
                    Console.WriteLine("UpdateNode() >>> Node removed and reinserted with new keys.");
                    DeleteNode(ref oldData, oldKeys);
                    InsertNode(ref newData, newKeys);
                }
            }
        }

        public void DeleteNode(ref T data, U keys) {
            if (this.root == null) return;

            Node<T, U> nodeToDelete = this.FindNode(keys, this.root);

            if (nodeToDelete == null) return;

            if (nodeToDelete.NodeData.Count > 1) {
                this.dataSize--;
                Console.WriteLine("DeleteNode() >>> Data entry removed!");
                foreach (T nodeData in nodeToDelete.NodeData) {
                    if (nodeData.EqualsByID(data)) {
                        nodeToDelete.NodeData.Remove(data);
                        break;
                    }
                }
                return;
            }

            DeleteAndReplaceNode(nodeToDelete);
        }

        private void DeleteAndReplaceNode(Node<T, U> node) {
            if (node == null) return;

            HashSet<Node<T, U>> visitedNodes = new HashSet<Node<T, U>>();
            List<Node<T, U>> duplicateNodes = new List<Node<T, U>>();
            Stack<Node<T, U>> nodesToProcess = new Stack<Node<T, U>>();
            nodesToProcess.Push(node);

            while (nodesToProcess.Count > 0) {
                Node<T, U> current = nodesToProcess.Pop();

                if (visitedNodes.Contains(current)) continue;

                visitedNodes.Add(current);

                if (current.LeftSon == null && current.RightSon == null) {
                    if (current.Parent != null) {
                        if (current.Parent.LeftSon == current) {
                            current.Parent.LeftSon = null;
                        } else {
                            current.Parent.RightSon = null;
                        }
                    } else {
                        this.root = null;
                    }
                    continue;
                }

                Node<T, U> replacement = null;

                if (current.LeftSon != null) {
                    replacement = FindMaxNode(current, duplicateNodes);
                } else if (current.RightSon != null) {
                    replacement = FindMinNode(current, duplicateNodes);
                }

                if (replacement != null && !visitedNodes.Contains(replacement)) {
                    current.KeysData = replacement.KeysData;
                    current.NodeData = replacement.NodeData;

                    nodesToProcess.Push(replacement);
                    duplicateNodes.AddRange(FindDuplicateNodes(current));
                }
            }

            foreach (var duplicate in duplicateNodes) {
                this.treeSize--;
                this.dataSize -= duplicate.NodeData.Count;

                foreach (var nodeData in duplicate.NodeData) {
                    T data = nodeData;
                    DeleteNode(ref data, duplicate.KeysData);
                    InsertNode(ref data, duplicate.KeysData);
                }
            }

            this.treeSize--;
            this.dataSize--;

            Console.WriteLine("DeleteAndReplaceNode() >>> Node replaced and removed.");
        }

        private List<Node<T, U>> FindDuplicateNodes(Node<T, U> node) {
            List<Node<T, U>> duplicateNodes = new List<Node<T, U>>();

            if (node == null || node.RightSon == null) return duplicateNodes;

            Stack<Node<T, U>> nodesToVisit = new Stack<Node<T, U>>();
            nodesToVisit.Push(node.RightSon);

            while (nodesToVisit.Count > 0) {
                Node<T, U> current = nodesToVisit.Pop();

                int comparison = current.KeysData.Compare(node.KeysData, node.Level);

                if (comparison == 0) duplicateNodes.Add(current);

                if (current.Level != node.Level) {
                    if (current.LeftSon != null) nodesToVisit.Push(current.LeftSon);
                    if (current.RightSon != null) nodesToVisit.Push(current.RightSon);
                } else {
                    if (current.RightSon != null) nodesToVisit.Push(current.RightSon);
                }
            }

            return duplicateNodes;
        }

        private Node<T, U> FindMinNode(Node<T, U> node, List<Node<T, U>> illegalNodes) {
            if (node == null || node.RightSon == null) return null;

            Node<T, U> minNode = node.RightSon;
            Stack<Node<T, U>> nodesToVisit = new Stack<Node<T, U>>();
            nodesToVisit.Push(minNode);

            while (nodesToVisit.Count > 0) {
                Node<T, U> current = nodesToVisit.Pop();

                int comparison = current.KeysData.Compare(minNode.KeysData, node.Level);
                bool validNode = !illegalNodes.Any(x => x.KeysData.Equals(current.KeysData));

                if (comparison <= 0 && validNode) minNode = current;

                if (current.Level != node.Level) {
                    if (current.LeftSon != null) nodesToVisit.Push(current.LeftSon);
                    if (current.RightSon != null) nodesToVisit.Push(current.RightSon);
                } else {
                    if (current.LeftSon != null) nodesToVisit.Push(current.LeftSon);
                }
            }

            return minNode;
        }

        private Node<T, U> FindMaxNode(Node<T, U> node, List<Node<T, U>> illegalNodes) {
            if (node == null || node.LeftSon == null) return null;

            Node<T, U> maxNode = node.LeftSon;
            Stack<Node<T, U>> nodesToVisit = new Stack<Node<T, U>>();
            nodesToVisit.Push(maxNode);

            while (nodesToVisit.Count > 0) {
                Node<T, U> current = nodesToVisit.Pop();

                int comparison = current.KeysData.Compare(maxNode.KeysData, current.Level);
                bool validNode = !illegalNodes.Any(x => x.KeysData.Equals(current.KeysData));

                if (comparison >= 0 && validNode) maxNode = current;

                if (current.Level != node.Level) {
                    if (current.LeftSon != null) nodesToVisit.Push(current.LeftSon);
                    if (current.RightSon != null) nodesToVisit.Push(current.RightSon);
                } else {
                    if (current.LeftSon != null) nodesToVisit.Push(current.LeftSon);
                }
            }

            return maxNode;
        }    

        private Node<T, U> FindNode(U keys, Node<T, U> parent) {
            Node<T, U> nodeToFind = new Node<T, U>(keys);
            Node<T, U> current = parent;
            int level = 0;

            while (current != null) {
                if (nodeToFind.KeysData.Equals(current.KeysData)) {
                    Console.WriteLine("FindNode() >>> Node found!");
                    return current;
                }

                int comparison = nodeToFind.KeysData.Compare(current.KeysData, level);
                current = (comparison <= 0) ? current.LeftSon : current.RightSon;
                level++;
            }

            Console.WriteLine("FindNode() >>> Node not found!");
            throw new NullReferenceException();
        }

        public void Clear() {
            this.root = null;
            this.treeSize = 0;
            this.dataSize = 0;
        }

        public void PrintInOrder() {
            if (this.root == null) return;

            Stack<Node<T, U>> nodesToVisit = new Stack<Node<T, U>>();
            Node<T, U> current = this.root;

            int a = 0, b = 0, c = 0;

            while (nodesToVisit.Count > 0 || current != null) {
                while (current != null) {
                    nodesToVisit.Push(current);
                    current = current.LeftSon;
                }

                current = nodesToVisit.Pop();

                bool isFirst = true;
                current.NodeData.ForEach(x => {
                    if (isFirst) {
                        Console.ResetColor();
                        isFirst = false;
                        a++;
                    } else {
                        b++;
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    c++;
                    x.PrintInfo();
                });
                Console.ResetColor();

                current = current.RightSon;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nTree Size: " + this.treeSize + "\nData Size: " + this.DataSize);
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine("Tree Size: " + a + "\nData Size: " + c + "\nDuplicates: " + b);
            Console.ResetColor();
        }

        public Node<T, U> Root { get => root; set => root = value; }

        public int TreeSize { get => treeSize; set => treeSize = value; }

        public int DataSize { get => dataSize; set => dataSize = value; }
    }
}
