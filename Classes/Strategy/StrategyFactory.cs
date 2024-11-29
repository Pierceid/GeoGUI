using GeoGUI.Classes.Prototype;
using GeoGUI.Classes.Structure;

namespace GeoGUI.Classes.Strategy {
    public class StrategyFactory<T, U> : IStrategyFactory<T, U> where T : Item where U : IKey<U> {
        public IStrategy<T, U> CreateStrategy(double range) {
            if (range == 0) {
                return PointSearchStrategy<T, U>.GetInstance();
            } else {
                return RangeSearchStrategy<T, U>.GetInstance(range);
            }
        }
    }
}
