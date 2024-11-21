using System.Collections.Generic;

namespace GeoGUI.Classes.Strategy {
    public interface IStrategy<T> where T : Item {
        List<T> Search(KDTree<T, GPS> tree, GPS position);
    }
}
