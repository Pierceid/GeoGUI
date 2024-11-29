using GeoGUI.Classes.Crate;
using GeoGUI.Classes.Prototype;

namespace GeoGUI.Classes.Factory {
    public class NehnutelnostFactory : IItemFactory {
        public Item CreateItem(int number, string description, GPS position) {
            return new Nehnutelnost(number, description, position);
        }
    }
}
