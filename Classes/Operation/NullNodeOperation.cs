using GeoGUI.Classes.Crate;
using GeoGUI.Classes.Prototype;
using GeoGUI.Classes.Structure;

namespace GeoGUI.Classes.Null {
    public class NullNodeOperation<T, U> : INodeOperation<T, U> where T : Item where U : IKey<U> {
        private static NullNodeOperation<T, U> instance = null;
        private static readonly object lockObj = new object();

        private NullNodeOperation() { }

        public static NullNodeOperation<T, U> GetInstance() {
            if (instance == null) {
                lock (lockObj) {
                    if (instance == null) {
                        instance = new NullNodeOperation<T, U>();
                    }
                }
            }
            return instance;
        }

        public void Execute(Node<T, U> node) { }
    }
}
