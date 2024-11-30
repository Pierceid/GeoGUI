using GeoGUI.Classes.Prototype;
using GeoGUI.Classes.Structure;

namespace GeoGUI.Classes.Strategy {
    public class StrategyFactory<T, U> : IStrategyFactory<T, U> where T : Item where U : IKey<U> {
        private static StrategyFactory<T, U> instance = null;
        private static readonly object lockObj = new object();

        private StrategyFactory() { }

        public static StrategyFactory<T, U> GetInstance() {
            if (instance == null) {
                lock (lockObj) {
                    if (instance == null) {
                        instance = new StrategyFactory<T, U>();
                    }
                }
            }

            return instance;
        }
        
        public IStrategy<T, U> CreateStrategy(double range) {
            if (range == 0) {
                return new PointSearchStrategy<T, U>();
            } else {
                return new RangeSearchStrategy<T, U>(range);
            }
        }
    }
}
