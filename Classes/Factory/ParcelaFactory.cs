using GeoGUI.Classes.Crate;
using GeoGUI.Classes.Prototype;

namespace GeoGUI.Classes.Factory {
    public class ParcelaFactory : IItemFactory {
        private static ParcelaFactory instance = null;
        private static readonly object lockObj = new object();

        private ParcelaFactory() { }

        public static ParcelaFactory GetInstance() {
            if (instance == null) {
                lock (lockObj) {
                    if (instance == null) {
                        instance = new ParcelaFactory();
                    }
                }
            }

            return instance;
        }

        public Item CreateItem(int number, string description, GPS position) {
            return new Parcela(number, description, position);
        }
    }
}
