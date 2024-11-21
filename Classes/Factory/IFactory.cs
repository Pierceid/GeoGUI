namespace GeoGUI.Classes.Factory {
    public interface IFactory<T> where T : Item {
        T CreateItem(int number, string description, GPS position);
    }
}
