namespace GeoGUI.Classes {
    internal class GPS : IKey<GPS> {
        private double x;
        private double y;

        public GPS(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public int Compare(GPS other, int level) {
            if (level % 2 == 0) {
                return this.ComparePositions(this.x, other.X);
            } else {
                return this.ComparePositions(this.y, other.Y);
            }
        }

        public bool Equals(GPS other) {
            return this.x == other.X && this.y == other.Y;
        }

        private int ComparePositions(double value1, double value2) {
            if (value1 < value2) return -1;
            if (value1 > value2) return 1;
            return 0;
        }

        public string GetKeys() {
            return $"{this.x.ToString().Replace(',', '.')},{this.y.ToString().Replace(',', '.')}";
        }

        public double X { get => x; set => x = value; }

        public double Y { get => y; set => y = value; }
    }
}
