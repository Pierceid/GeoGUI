using GeoGUI.Classes.Prototype;
using GeoGUI.Classes.Utility;
using System;

namespace GeoGUI.Classes {
    public class Parcela : Item {
        public int CisParcely { get; set; }
        public string Popis { get; set; }
        public GPS Pozicia { get; set; }

        public Parcela(int cisParcely, string popis, GPS pozicia) {
            this.CisParcely = cisParcely;
            this.Popis = popis;
            this.Pozicia = pozicia;
        }

        public override void PrintInfo() {
            Console.WriteLine($"Parcela: {this.CisParcely} - {this.Popis} - [{Util.FormatDoubleForExport(this.Pozicia.LatitudeValue)}°; {Util.FormatDoubleForExport(this.Pozicia.LongitudeValue)}°]");
        }

        public override string GetInfo() {
            return $"Parcela,{this.Id},{this.CisParcely},{this.Popis}";
        }

        public override IPrototype Clone() {
            return new Parcela(this.CisParcely, this.Popis, this.Pozicia);
        }
    }
}
