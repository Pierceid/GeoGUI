namespace GeoGUI.Classes {
    internal interface IKey<U> {
        int Compare(U other, int level);
        bool Equals(U other);
        string GetKeys();
    }
}
