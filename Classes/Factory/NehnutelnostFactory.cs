using GeoGUI.Classes.Crate;
using GeoGUI.Classes.Prototype;

namespace GeoGUI.Classes.Factory {
    public class NehnutelnostFactory : IItemFactory {
        private static NehnutelnostFactory instance = null;
        private static readonly object lockObj = new object();

        private NehnutelnostFactory() { }

        public static NehnutelnostFactory GetInstance() {
            if (instance == null) {
                lock (lockObj) {
                    if (instance == null) {
                        instance = new NehnutelnostFactory();
                    }
                }
            }

            return instance;
        }

        public Item CreateItem(int number, string description, GPS position) {
            return new Nehnutelnost(number, description, position);
        }
    }
}
