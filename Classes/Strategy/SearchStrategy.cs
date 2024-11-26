using System.Collections.Generic;
using System.Linq;

namespace GeoGUI.Classes.Strategy {
    public class SearchStrategy<T, U> : IStrategy<T, U> where T : Item where U : IKey<U> {
        public List<T> Search(KDTree<T, U> tree, U position) {
            return tree.FindNodes(position).Where(item => item is T).ToList();
        }
    }
}
