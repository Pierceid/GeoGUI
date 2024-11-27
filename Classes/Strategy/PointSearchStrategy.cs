using System.Collections.Generic;

namespace GeoGUI.Classes.Strategy {
    public class PointSearchStrategy<T, U> : IStrategy<T, U> where T : Item where U : IKey<U> {
        public List<T> Traverse(KDTree<T, U> tree, U keys) {
            return tree.FindNodes(keys);
        }
    }
}
