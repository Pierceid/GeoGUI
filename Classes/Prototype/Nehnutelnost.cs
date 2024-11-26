using GeoGUI.Classes.Utils;
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
            Console.WriteLine($"Nehnutelnost: {this.supCislo} - {this.popis} - [{Util.FormatDoubleForExport(this.pozicia.X)}°; {Util.FormatDoubleForExport(this.pozicia.Y)}°]");
        }

        public override string GetInfo() {
            return $"Nehnutelnost,{this.Id},{this.supCislo},{this.popis}";
        }

        public override Item Clone() {
            return new Nehnutelnost(this.supCislo, this.popis, this.pozicia);
        }

        public int SupCislo { get => supCislo; set => supCislo = value; }

        public string Popis { get => popis; set => popis = value; }

        public GPS Pozicia { get => pozicia; set => pozicia = value; }
    }
}
