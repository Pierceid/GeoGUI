using GeoGUI.Classes.Prototype;
using GeoGUI.Classes.Utils;
using System;

namespace GeoGUI.Classes {
    public class Nehnutelnost : Item {
        public int SupCislo { get; set; }
        public string Popis { get; set; }
        public GPS Pozicia { get; set; }

        public Nehnutelnost(int supCislo, string popis, GPS pozicia) {
            this.SupCislo = supCislo;
            this.Popis = popis;
            this.Pozicia = pozicia;
        }

        public override void PrintInfo() {
            Console.WriteLine($"Nehnutelnost: {this.SupCislo} - {this.Popis} - [{Util.FormatDoubleForExport(this.Pozicia.X)}°; {Util.FormatDoubleForExport(this.Pozicia.Y)}°]");
        }

        public override string GetInfo() {
            return $"Nehnutelnost,{this.Id},{this.SupCislo},{this.Popis}";
        }

        public override IPrototype Clone() {
            return new Nehnutelnost(this.SupCislo, this.Popis, this.Pozicia);
        }
    }
}
