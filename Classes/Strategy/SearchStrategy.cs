using System.Collections.Generic;
using System.Linq;

namespace GeoGUI.Classes.Strategy {
    public class SearchStrategy<T> : IStrategy<T> where T : Item {
        public List<T> Search(KDTree<T, GPS> tree, GPS position) {
            return tree.FindNodes(position).Where(item => item.GetType() == typeof(T)).ToList();
        }
    }
}
