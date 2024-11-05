using System;

namespace GeoGUI.Classes {
    public class Nehnutelnost : Item {
        private int supCislo;
        private string popis;
        private GPS pozicia;

        public Nehnutelnost(int supCislo, string popis, GPS pozicia) {
            this.supCislo = supCislo;
            this.popis = popis;
            this.pozicia = pozicia;
        }

        public override void PrintInfo() {
            Console.WriteLine($"Nehnutelnost: {this.supCislo} - {this.popis} - [{this.pozicia.X}°; {this.pozicia.Y}°]");
        }

        public override string GetInfo() {
            return $"Nehnutelnost,{this.Id},{this.supCislo},{this.popis}";
        }

        public int SupCislo { get => supCislo; set => supCislo = value; }

        public string Popis { get => popis; set => popis = value; }

        public GPS Pozicia { get => pozicia; set => pozicia = value; }
    }
}
