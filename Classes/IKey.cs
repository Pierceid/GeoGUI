namespace GeoGUI.Classes {
    public interface IKey<U> {
        int Compare(U other, int level);
        bool Equals(U other);
        bool CloseWithin(U other, double factor);
        string GetKeys();
    }
}
