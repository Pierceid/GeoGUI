namespace GeoGUI.Classes.Factory {
    public class NehnutelnostFactory : IFactory {
        Item IFactory.CreateItem(int number, string description, GPS position) {
            return new Nehnutelnost(number, description, position);
        }
    }
}
