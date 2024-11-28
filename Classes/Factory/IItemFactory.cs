namespace GeoGUI.Classes.Factory {
    public interface IItemFactory {
        Item CreateItem(int number, string description, GPS position);
    }
}
