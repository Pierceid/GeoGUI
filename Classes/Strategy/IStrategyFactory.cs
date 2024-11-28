﻿namespace GeoGUI.Classes.Strategy {
    public interface IStrategyFactory<T, U> where T : Item where U : IKey<U> {
        IStrategy<T, U> CreateStrategy(double range);
    }
}
