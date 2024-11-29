using GeoGUI.Classes.Crate;
using GeoGUI.Classes.Prototype;

namespace GeoGUI.Classes.Factory {
    public interface IItemFactory {
        Item CreateItem(int number, string description, GPS position);
    }
}
