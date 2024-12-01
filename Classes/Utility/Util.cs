using GeoGUI.Classes.Crate;
using GeoGUI.Classes.Flyweight;
using System;
using System.Linq;

namespace GeoGUI.Classes.Utility {
    public static class Util {
        private static Random random = new Random();
        private static GPSFactory gpsFactory = GPSFactory.GetInstance();

        public static int CompareDoubles(double value1, double value2) {
            if (value1 < value2) return -1;
            if (value1 > value2) return 1;
            return 0;
        }

        public static int CompareStrings(string value1, string value2) {
            if (string.Compare(value1, value2) < 0) return -1;
            if (string.Compare(value1, value2) > 0) return 1;
            return 0;
        }

        public static string FormatDoubleForExport(double number) {
            return number.ToString().Replace(',', '.');
        }

        public static string FormatDoubleForImport(string number) {
            return number.Replace('.', ',');
        }

        public static string GenerateRandomString(int length) {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static GPS GenerateRandomGPS(int maxLatitude, int maxLongitude) {
            double latitudeValue = Math.Round(random.NextDouble() * maxLatitude, 2);
            double longitudeValue = Math.Round(random.NextDouble() * maxLongitude, 2);
            string latitudeDirection = random.NextDouble() < 0.5 ? "N" : "S";
            string longitudeDirection = random.NextDouble() < 0.5 ? "E" : "W";
            return gpsFactory.GetGPS(latitudeValue, latitudeDirection, longitudeValue, longitudeDirection);
        }

        public static GPS ParseGPS(string latitudeValue, string latitudeDirection, string longitudeValue, string longitudeDirection) {
            double latitudeValue = double.TryParse(latitudeString, out double latParsed) ? latParsed : double.MaxValue;
            double longitudeValue = double.TryParse(longitudeString, out double lonParsed) ? lonParsed : double.MaxValue;
            return gpsFactory.GetGPS(latitudeValue, latitudeDirection, longitudeValue, longitudeDirection);
        }
    }
}
