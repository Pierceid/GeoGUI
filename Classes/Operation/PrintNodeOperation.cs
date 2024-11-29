using GeoGUI.Classes.Crate;
using GeoGUI.Classes.Prototype;
using GeoGUI.Classes.Structure;

namespace GeoGUI.Classes.Null {
    public class PrintNodeOperation<T, U> : INodeOperation<T, U> where T : Item where U : IKey<U> {
        private static PrintNodeOperation<T, U> instance = null;
        private static readonly object lockObj = new object();

        private PrintNodeOperation() { }

        public static PrintNodeOperation<T, U> GetInstance() {
            if (instance == null) {
                lock (lockObj) {
                    if (instance == null) {
                        instance = new PrintNodeOperation<T, U>();
                    }
                }
            }
            return instance;
        }

        public void Execute(Node<T, U> node) {
            node.NodeData.ForEach(data => data.PrintInfo());
        }
    }
}
