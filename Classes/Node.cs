using System.Collections.Generic;

namespace GeoGUI.Classes {
    internal class Node<T, U> where T : Item where U : IKey<U> {

        private Node<T, U> parent;
        private Node<T, U> leftSon;
        private Node<T, U> rightSon;
        private List<T> nodeData;
        private U keysData;
        private int level;

        public Node(U keys) {
            this.parent = null;
            this.leftSon = null;
            this.rightSon = null;
            this.nodeData = new List<T>();
            this.keysData = keys;
            this.level = 0;
        }

        public Node<T, U> Parent { get => parent; set => parent = value; }
        public Node<T, U> LeftSon { get => leftSon; set => leftSon = value; }
        public Node<T, U> RightSon { get => rightSon; set => rightSon = value; }
        public List<T> NodeData { get => nodeData; set => nodeData = value; }
        public U KeysData { get => keysData; set => keysData = value; }
        public int Level { get => level; set => level = value; }
    }
}
