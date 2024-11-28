using System.Collections.Generic;

namespace GeoGUI.Classes.Strategy {
    public class PointSearchStrategy<T, U> : IStrategy<T, U> where T : Item where U : IKey<U> {
        private static PointSearchStrategy<T, U> instance = null;
        private static readonly object lockObj = new object();

        private PointSearchStrategy() { }

        public static PointSearchStrategy<T, U> GetInstance() {
            if (instance == null) {
                lock (lockObj) {
                    if (instance == null) {
                        instance = new PointSearchStrategy<T, U>();
                    }
                }
            }

            return instance;
        }

        public List<T> Traverse(KDTree<T, U> tree, U keys) {
            return tree.FindNodes(keys);
        }
    }
}
