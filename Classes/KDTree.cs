using GeoGUI.Classes;
using System;
using System.Collections.Generic;

namespace GeoGUI {
    public class KDTree<T, U> where T : Item where U : IKey<U> {
        private Node<T, U> root;
        private int treeSize;
        private int dataSize;
        private int dimensions;

        public KDTree(int dimensions) {
            this.root = null;
            this.treeSize = 0;
            this.dataSize = 0;
            this.dimensions = dimensions;
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
            nodeToInsert.Level = level % this.dimensions;

            this.treeSize++;

            Console.WriteLine("InsertNode() >>> Node inserted!");
        }

        public List<T> FindNodes(U keys) {
            Node<T, U> nodeToFind = new Node<T, U>(keys);
            List<T> result = new List<T>();

            if (this.root == null) return result;

            Stack<Node<T, U>> nodesToVisit = new Stack<Node<T, U>>();
            nodesToVisit.Push(this.root);

            while (nodesToVisit.Count > 0) {
                Node<T, U> current = nodesToVisit.Pop();

                if (nodeToFind.KeysData.Equals(current.KeysData)) {
                    Console.WriteLine("FindNodes() >>> Node found!");
                    result.AddRange(current.NodeData);
                    break;
                }

                int comparison = nodeToFind.KeysData.Compare(current.KeysData, current.Level);

                if (comparison <= 0 && current.LeftSon != null) {
                    nodesToVisit.Push(current.LeftSon);
                } else if (current.RightSon != null) {
                    nodesToVisit.Push(current.RightSon);
                }
            }

            if (result.Count == 0) {
                Console.WriteLine("FindNodes() >>> Nodes not found!");
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
                Console.WriteLine("UpdateNode() >>> Data entry updated!");
                for (int i = 0; i < nodeToUpdate.NodeData.Count; i++) {
                    if (nodeToUpdate.NodeData[i].EqualsByID(oldData)) {
                        nodeToUpdate.NodeData[i] = newData;
                        return;
                    }
                }
            } else {
                // Case 2: Keys changed
                if (multipleDataEntries) {
                    Console.WriteLine("UpdateNode() >>> Data entry updated!");
                    nodeToUpdate.NodeData.Remove(oldData);
                    InsertNode(ref newData, newKeys);
                    this.dataSize--;
                } else {
                    Console.WriteLine("UpdateNode() >>> Node updated!");
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

            List<Node<T, U>> duplicateNodes = new List<Node<T, U>>();
            Stack<Node<T, U>> nodesToReinsert = new Stack<Node<T, U>>();
            List<Node<T, U>> visitedNodes = new List<Node<T, U>>();

            nodesToReinsert.Push(nodeToDelete);

            while (nodesToReinsert.Count > 0) {
                Node<T, U> current = nodesToReinsert.Pop();

                Stack<Node<T, U>> nodesToReplace = new Stack<Node<T, U>>();

                List<T> originalData = new List<T>();
                U originalKeys = keys;

                if (current != nodeToDelete) {
                    originalData = current.NodeData;
                    originalKeys = current.KeysData;
                }

                nodesToReplace.Push(current);
                visitedNodes.Clear();

                if (current.LeftSon != null || current.RightSon != null) visitedNodes.Add(current);

                while (nodesToReplace.Count > 0) {
                    Node<T, U> cur = nodesToReplace.Pop();

                    // If the current node is a leaf, remove it
                    if (cur.LeftSon == null && cur.RightSon == null) {
                        if (cur.Parent != null) {
                            if (cur.Parent.LeftSon == cur) {
                                cur.Parent.LeftSon = null;
                            } else {
                                cur.Parent.RightSon = null;
                            }
                            cur.Parent = null;
                            Console.WriteLine("DeleteNode() >>> Leaf node removed!");
                        } else {
                            this.root = null;
                            Console.WriteLine("DeleteNode() >>> Root node removed!");
                        }
                        this.treeSize--;
                        this.dataSize--;
                        continue;
                    }

                    // Find replacement for the current node
                    Node<T, U> replacement = null;

                    if (cur.LeftSon != null) {
                        replacement = FindMaxNode(cur);
                    } else if (cur.RightSon != null) {
                        replacement = FindMinNode(cur);
                    }

                    if (replacement != null) {
                        cur.KeysData = replacement.KeysData;
                        cur.NodeData = new List<T>(replacement.NodeData);

                        nodesToReplace.Push(replacement);
                        visitedNodes.Add(replacement);
                    }
                }

                if (visitedNodes.Count != 0) visitedNodes.RemoveAt(visitedNodes.Count - 1);

                // Reinsert duplicates for each visited node
                foreach (var visited in visitedNodes) {
                    duplicateNodes = FindDuplicateNodes(visited);
                    foreach (var duplicate in duplicateNodes) {
                        nodesToReinsert.Push(duplicate);
                    }
                }

                if (originalData.Count > 0) originalData.ForEach(x => InsertNode(ref x, originalKeys));
            }
        }

        private List<Node<T, U>> FindDuplicateNodes(Node<T, U> parent) {
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

        private Node<T, U> FindMinNode(Node<T, U> parent) {
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

        private Node<T, U> FindMaxNode(Node<T, U> parent) {
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

        private Node<T, U> FindNode(U keys, Node<T, U> startNode) {
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

        private Node<T, U> FindNodeReverse(U keys, Node<T, U> startNode) {
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

                if (comparison >= 0) {
                    if (current.RightSon != null) nodesToVisit.Push(current.RightSon);
                } else {
                    if (current.LeftSon != null) nodesToVisit.Push(current.LeftSon);
                }
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

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine();
            Console.WriteLine("Tree Size: " + a + "\nData Size: " + c + "\nDuplicates: " + b);
            Console.ResetColor();
            Console.WriteLine();
        }

        public List<Node<T, U>> GetAllNodes() {
            List<Node<T, U>> result = new List<Node<T, U>>();

            if (this.root == null) return result;

            Stack<Node<T, U>> nodesToVisit = new Stack<Node<T, U>>();
            Node<T, U> current = this.root;

            while (nodesToVisit.Count > 0 || current != null) {
                while (current != null) {
                    nodesToVisit.Push(current);
                    current = current.LeftSon;
                }

                current = nodesToVisit.Pop();

                result.Add(current);

                current = current.RightSon;
            }

            return result;
        }

        public Node<T, U> Root { get => root; set => root = value; }

        public int TreeSize { get => treeSize; set => treeSize = value; }

        public int DataSize { get => dataSize; set => dataSize = value; }
    }
}
