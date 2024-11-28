using System.Collections.Generic;

namespace GeoGUI.Classes.Flyweight {
    public class GPSFactory {
        public readonly Dictionary<string, GPS> gpsPool = new Dictionary<string, GPS>();
        private readonly object lockObj = new object();

        public GPS GetGPS(double latitudeValue, string latitudeDirection, double longitudeValue, string longitudeDirection) {
            string key = string.Join("_", latitudeValue, latitudeDirection, longitudeValue, longitudeDirection);

            lock (this.lockObj) {
                if (!this.gpsPool.TryGetValue(key, out GPS gps)) {
                    gps = new GPS(latitudeValue, latitudeDirection, longitudeValue, longitudeDirection);
                    this.gpsPool[key] = gps;
                }

                return gps;
            }
        }
    }
}
