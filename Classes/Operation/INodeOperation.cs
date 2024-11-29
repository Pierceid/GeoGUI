using GeoGUI.Classes.Crate;
using GeoGUI.Classes.Prototype;
using GeoGUI.Classes.Structure;

namespace GeoGUI.Classes.Null {
    public interface INodeOperation<T, U> where T : Item where U : IKey<U> {
        void Execute(Node<T, U> node);
    }
}
