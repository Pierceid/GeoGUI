using GeoGUI.Classes.Prototype;
using System;

namespace GeoGUI.Classes.Factory {
    public class NehnutelnostFactory : IFactory {
        public IPrototype ClonePrototype(IPrototype item) {
            if (item is Nehnutelnost nehnutelnost) return nehnutelnost.Clone();

            throw new ArgumentException("Invalid prototype type for NehnutelnostFactory.");
        }

        public IPrototype CreatePrototype(int number, string description, GPS position) {
            return new Nehnutelnost(number, description, position);
        }
    }
}
