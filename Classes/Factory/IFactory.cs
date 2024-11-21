namespace GeoGUI.Classes.Factory {
    public interface IFactory {
        Item CreateItem(int number, string description, GPS position);
    }
}
