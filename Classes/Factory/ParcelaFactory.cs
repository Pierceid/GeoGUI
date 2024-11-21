namespace GeoGUI.Classes.Factory {
    public class ParcelaFactory : IFactory<Parcela> {
        public Parcela CreateItem(int number, string description, GPS position) {
            return new Parcela(number, description, position);
        }
    }
}
