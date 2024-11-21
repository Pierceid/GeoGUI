namespace GeoGUI.Classes.Factory {
    public class ParcelaFactory : IFactory {
        Item IFactory.CreateItem(int number, string description, GPS position) {
            return new Parcela(number, description, position);
        }
    }
}
