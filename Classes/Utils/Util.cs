using System;
using System.Linq;

namespace GeoGUI.Classes.Utils {
    public static class Util {
        private static Random random = new Random();

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

        public static GPS GenerateRandomGPS(int latitude, int longitude) {
            double x = Math.Round(random.NextDouble() * latitude, 2);
            double y = Math.Round(random.NextDouble() * longitude, 2);
            string sirka = random.NextDouble() < 0.5 ? "W" : "E";
            string dlzka = random.NextDouble() < 0.5 ? "N" : "S";
            return new GPS(sirka, x, dlzka, y);
        }

        public static GPS ParseGPS(string latitude, string longitude, string sirka, string dlzka) {
            double x = Double.MaxValue, y = Double.MaxValue;
            double.TryParse(latitude, out x);
            double.TryParse(longitude, out y);
            return new GPS(sirka, x, dlzka, y);
        }
    }
}
