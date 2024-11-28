using System.Collections.Generic;

namespace GeoGUI.Classes.Flyweight {
    public class GPSFactory {
        private readonly Dictionary<string, GPS> gpsPool = new Dictionary<string, GPS>();
        private readonly object lockObj = new object();

        public GPS GetGPS(double x, string latitude, double y, string longitude) {
            string key = string.Join("_", x, latitude, y, longitude);

            lock (this.lockObj) {
                if (!this.gpsPool.TryGetValue(key, out GPS gps)) {
                    gps = new GPS(latitude, x, longitude, y);
                    this.gpsPool[key] = gps;
                }

                return gps;
            }
        }
    }
}
