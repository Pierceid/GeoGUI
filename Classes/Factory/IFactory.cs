using GeoGUI.Classes.Prototype;

namespace GeoGUI.Classes.Factory {
    public interface IFactory {
        IPrototype CreatePrototype(int number, string description, GPS position);
        IPrototype ClonePrototype(IPrototype item);
    }
}
