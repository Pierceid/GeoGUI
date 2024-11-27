using System.Collections.Generic;
using System.Linq;

namespace GeoGUI.Classes.Strategy {
    public class RangeSearchStrategy<T, U> : IStrategy<T, U> where T : Item where U : IKey<U> {
        private readonly double range;

        public RangeSearchStrategy(double range) {
            this.range = range;
        }

        public List<T> Traverse(KDTree<T, U> tree, U keys) {
            return tree.GetAllNodes().Where(node => node.KeysData.CloseWithin(keys, range)).SelectMany(node => node.NodeData).ToList();
        }
    }
}
