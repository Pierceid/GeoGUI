using GeoGUI.Classes.Prototype;
using GeoGUI.Classes.Structure;
using System.Collections.Generic;

namespace GeoGUI.Classes.Crate {
    public class Node<T, U> where T : Item where U : IKey<U> {
        public Node<T, U> Parent { get; set; }
        public Node<T, U> LeftSon { get; set; }
        public Node<T, U> RightSon { get; set; }
        public List<T> NodeData { get; set; }
        public U KeysData { get; set; }
        public int Level { get; set; }

        public Node(U keys) {
            this.Parent = null;
            this.LeftSon = null;
            this.RightSon = null;
            this.NodeData = new List<T>();
            this.KeysData = keys;
            this.Level = 0;
        }
    }
}
