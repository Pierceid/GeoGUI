using GeoGUI.Classes.Utils;
using System;

namespace GeoGUI.Classes {
    public class Parcela : Item {
        private int cisParcely;
        private string popis;
        private GPS pozicia;

        public Parcela(int cisParcely, string popis, GPS pozicia) {
            this.cisParcely = cisParcely;
            this.popis = popis;
            this.pozicia = pozicia;
        }

        public override void PrintInfo() {
            Console.WriteLine($"Parcela: {this.cisParcely} - {this.popis} - [{Util.FormatDoubleForExport(this.pozicia.X)}°; {Util.FormatDoubleForExport(this.pozicia.Y)}°]");
        }

        public override string GetInfo() {
            return $"Parcela,{this.Id},{this.cisParcely},{this.popis}";
        }

        public override Item Clone() {
            return new Parcela(this.cisParcely, this.popis, this.pozicia);
        }

        public int CisParcely { get => cisParcely; set => cisParcely = value; }

        public string Popis { get => popis; set => popis = value; }

        public GPS Pozicia { get => pozicia; set => pozicia = value; }
    }
}
