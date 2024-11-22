using GeoGUI.Classes.Prototype;
using System;

namespace GeoGUI.Classes.Factory {
    public class ParcelaFactory : IFactory {
        public IPrototype ClonePrototype(IPrototype item) {
            if (item is Parcela parcela) return parcela.Clone();

            throw new ArgumentException("Invalid prototype type for ParcelaFactory.");
        }

        public IPrototype CreatePrototype(int number, string description, GPS position) {
            return new Parcela(number, description, position);
        }
    }
}
