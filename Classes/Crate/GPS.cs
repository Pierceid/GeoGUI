using GeoGUI.Classes.Utility;
using System;

namespace GeoGUI.Classes {
    public class GPS : IKey<GPS> {
        public double X { get; set; }
        public double Y { get; set; }
        public string Sirka { get; set; }
        public string Dlzka { get; set; }

        public GPS(string sirka, double x, string dlzka, double y) {
            this.Sirka = sirka;
            this.X = x;
            this.Dlzka = dlzka;
            this.Y = y;
        }

        public int Compare(GPS other, int level) {
            if (level % 4 == 0) {
                return this.CompareStrings(this.Sirka, other.Sirka);
            } else if (level % 4 == 1) {
                return this.ComparePositions(this.X, other.X);
            } else if (level % 4 == 2) {
                return this.CompareStrings(this.Dlzka, other.Dlzka);
            } else {
                return this.ComparePositions(this.Y, other.Y);
            }
        }

        public bool Equals(GPS other) {
            return this.X == other.X && this.Y == other.Y && this.Sirka == other.Sirka && this.Dlzka == other.Dlzka;
        }

        public bool CloseWithin(GPS other, double factor) {
            double deltaX = this.X - other.X;
            double deltaY = this.Y - other.Y;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY) <= factor;

        }

        public string GetKeys() {
            return $"GPS,{Util.FormatDoubleForExport(this.X)},{this.Sirka},{Util.FormatDoubleForExport(this.Y)},{this.Dlzka}";
        }

        private int ComparePositions(double value1, double value2) {
            if (value1 < value2) return -1;
            if (value1 > value2) return 1;
            return 0;
        }

        private int CompareStrings(string value1, string value2) {
            if (string.Compare(value1, value2) < 0) return -1;
            if (string.Compare(value1, value2) > 0) return 1;
            return 0;
        }
    }
}
