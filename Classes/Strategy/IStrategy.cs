using System.Collections.Generic;

namespace GeoGUI.Classes.Strategy {
    public interface IStrategy<T, U> where T : Item where U : IKey<U> {
        List<T> Search(KDTree<T, U> tree, U position);
    }
}