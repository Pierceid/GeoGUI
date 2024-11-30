using GeoGUI.Classes.Crate;
using System.Collections.Generic;

namespace GeoGUI.Classes.Flyweight {
    public class GPSFactory {
        private static GPSFactory instance = null;
        private static readonly object lockObj = new object();
        private readonly Dictionary<string, GPS> gpsPool = new Dictionary<string, GPS>();

        private GPSFactory() { }

        public static GPSFactory GetInstance() {
            if (instance == null) {
                lock (lockObj) {
                    if (instance == null) {
                        instance = new GPSFactory();
                    }
                }
            }

            return instance;
        }
        public GPS GetGPS(double latitudeValue, string latitudeDirection, double longitudeValue, string longitudeDirection) {
            string key = string.Join("_", latitudeValue, latitudeDirection, longitudeValue, longitudeDirection);

            lock (lockObj) {
                if (!this.gpsPool.TryGetValue(key, out GPS gps)) {
                    gps = new GPS(latitudeValue, latitudeDirection, longitudeValue, longitudeDirection);
                    this.gpsPool[key] = gps;
                }

                return gps;
            }
        }
    }
}
