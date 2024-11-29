using GeoGUI.Classes.Crate;
using GeoGUI.Classes.Null;
using GeoGUI.Classes.Prototype;
using GeoGUI.Classes.Structure;
using System.IO;

namespace GeoGUI.Classes.Operation {
    public class WriteNodeOperation<T, U> : INodeOperation<T, U> where T : Item where U : IKey<U> {
        private static WriteNodeOperation<T, U> instance = null;
        private static readonly object lockObj = new object();
        private StreamWriter writer;

        private WriteNodeOperation(StreamWriter writer) {
            this.writer = writer;
        }

        public static WriteNodeOperation<T, U> GetInstance(StreamWriter writer) {
            if (instance == null) {
                lock (lockObj) {
                    if (instance == null) {
                        instance = new WriteNodeOperation<T, U>(writer);
                    }
                }
            }

            return instance;
        }

        public void Execute(Node<T, U> node) {
            string keysData = node.KeysData.GetKeys();
            string nodeData = string.Join(";", node.NodeData.ConvertAll(data => data.GetInfo()));
            this.writer.WriteLine($"'{keysData};'{nodeData}");
        }
    }
}
