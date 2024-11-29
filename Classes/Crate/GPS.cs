using GeoGUI.Classes.Structure;
using GeoGUI.Classes.Utility;
using System;

namespace GeoGUI.Classes.Crate {
    public class GPS : IKey<GPS> {
        public double LatitudeValue { get; }
        public string LatitudeDirection { get; }
        public double LongitudeValue { get; }
        public string LongitudeDirection { get; }

        public GPS(double latitudeValue, string latitudeDirection, double longitudeValue, string longitudeDirection) {
            this.LatitudeValue = latitudeValue;
            this.LatitudeDirection = latitudeDirection;
            this.LongitudeValue = longitudeValue;
            this.LongitudeDirection = longitudeDirection;
        }

        public int Compare(GPS other, int level) {
            if (level % 4 == 0) {
                return Util.CompareStrings(this.LatitudeDirection, other.LatitudeDirection);
            } else if (level % 4 == 1) {
                return Util.CompareDoubles(this.LatitudeValue, other.LatitudeValue);
            } else if (level % 4 == 2) {
                return Util.CompareStrings(this.LongitudeDirection, other.LongitudeDirection);
            } else {
                return Util.CompareDoubles(this.LongitudeValue, other.LongitudeValue);
            }
        }

        public bool Equals(GPS other) {
            return this.LatitudeValue == other.LatitudeValue && this.LatitudeDirection == other.LatitudeDirection &&
                this.LongitudeValue == other.LongitudeValue && this.LongitudeDirection == other.LongitudeDirection;
        }

        public bool CloseWithin(GPS other, double factor) {
            double deltaX = this.LatitudeValue - other.LatitudeValue;
            double deltaY = this.LongitudeValue - other.LongitudeValue;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY) <= factor;

        }

        public string GetKeys() {
            return $"GPS,{Util.FormatDoubleForExport(this.LatitudeValue)},{this.LatitudeDirection}," +
                $"{Util.FormatDoubleForExport(this.LongitudeValue)},{this.LongitudeDirection}";
        }
    }
}
