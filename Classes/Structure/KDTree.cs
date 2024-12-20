﻿using GeoGUI.Classes.Crate;
using GeoGUI.Classes.Delegates;
using GeoGUI.Classes.Null;
using GeoGUI.Classes.Prototype;
using System;
using System.Collections.Generic;

namespace GeoGUI.Classes.Structure {
    public class KDTree<T, U> where T : Item where U : IKey<U> {
        public Node<T, U> Root { get; set; }
        public int TreeSize { get; set; }
        public int DataSize { get; set; }
        public int Dimensions { get; set; }

        private readonly TreeTraversal<T, U> treeTraversal;

        public KDTree(int dimensions) {
            this.Root = null;
            this.TreeSize = 0;
            this.DataSize = 0;
            this.Dimensions = dimensions;
            this.treeTraversal = TreeTraversal<T, U>.GetInstance();
        }

        public void InsertNode(ref T data, U keys) {
            this.DataSize++;

            Node<T, U> current = this.Root;
            Node<T, U> parent = null;
            int level = 0;

            Node<T, U> nodeToInsert = new Node<T, U>(keys);
            nodeToInsert.NodeData.Add(data);

            if (this.Root == null) {
                this.TreeSize++;
                this.Root = nodeToInsert;
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
            nodeToInsert.Level = level % this.Dimensions;

            this.TreeSize++;

            Console.WriteLine("InsertNode() >>> Node inserted!");
        }

        public List<T> FindNodes(U keys) {
            Node<T, U> nodeToFind = new Node<T, U>(keys);
            List<T> result = new List<T>();

            if (this.Root == null) return result;

            Stack<Node<T, U>> nodesToVisit = new Stack<Node<T, U>>();
            nodesToVisit.Push(this.Root);

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
            if (this.Root == null) return;

            Node<T, U> nodeToUpdate = this.FindNode(oldKeys, this.Root);

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
                    this.DataSize--;
                } else {
                    Console.WriteLine("UpdateNode() >>> Node updated!");
                    DeleteNode(ref oldData, oldKeys);
                    InsertNode(ref newData, newKeys);
                }
            }
        }

        public void DeleteNode(ref T data, U keys) {
            if (this.Root == null) return;

            Node<T, U> nodeToDelete = this.FindNode(keys, this.Root);

            if (nodeToDelete == null) return;

            if (nodeToDelete.NodeData.Count > 1) {
                this.DataSize--;
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
                            this.Root = null;
                            Console.WriteLine("DeleteNode() >>> Root node removed!");
                        }
                        this.TreeSize--;
                        this.DataSize--;
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

        private Node<T, U> FindNode(U keys, Node<T, U> startNode) {
            return this.treeTraversal.FindNode(keys, startNode);
        }

        private List<Node<T, U>> FindDuplicateNodes(Node<T, U> parent) {
            return this.treeTraversal.FindDuplicateNodes(parent);
        }

        private Node<T, U> FindMinNode(Node<T, U> parent) {
            return this.treeTraversal.FindMinNode(parent);
        }

        private Node<T, U> FindMaxNode(Node<T, U> parent) {
            return this.treeTraversal.FindMaxNode(parent);
        }

        public void PrintInOrder() {
            if (this.Root == null) return;

            this.treeTraversal.InOrderTraversal(this.Root, PrintNodeOperation<T, U>.GetInstance());
        }

        public void Clear() {
            this.Root = null;
            this.TreeSize = 0;
            this.DataSize = 0;
        }
    }
}
