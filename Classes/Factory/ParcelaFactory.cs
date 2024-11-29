using GeoGUI.Classes.Crate;
using GeoGUI.Classes.Prototype;

namespace GeoGUI.Classes.Factory {
    public class ParcelaFactory : IItemFactory {
        public Item CreateItem(int number, string description, GPS position) {
            return new Parcela(number, description, position);
        }
    }
}
