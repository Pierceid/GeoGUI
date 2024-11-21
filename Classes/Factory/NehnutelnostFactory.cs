namespace GeoGUI.Classes.Factory {
    public class NehnutelnostFactory : IFactory<Nehnutelnost> {
        public Nehnutelnost CreateItem(int number, string description, GPS position) {
            return new Nehnutelnost(number, description, position);
        }
    }
}
