using GeoGUI.Classes.Utility;
using System.Windows.Forms;

namespace GeoGUI.Classes {
    public class DataGridViewObserver : IObserver {
        private Subject subject;
        private DataGridView dataGridView;
        private ComboBox comboBox;

        public DataGridViewObserver(Subject subject, DataGridView dataGridView, ComboBox comboBox) {
            this.subject = subject;
            this.dataGridView = dataGridView;
            this.comboBox = comboBox;
        }

        public void Update() {
            this.dataGridView.Rows.Clear();

            if (this.dataGridView.Columns.Count == 0) {
                this.dataGridView.Columns.Add("Type", "Type");
                this.dataGridView.Columns.Add("Position", "Position");
                this.dataGridView.Columns.Add("Number", "Number");
                this.dataGridView.Columns.Add("Description", "Description");
            }

            foreach (Item item in this.subject.ResultList) {
                if (this.comboBox.SelectedIndex == 0 && item is Parcela p) {
                    this.dataGridView.Rows.Add("Parcela", $"{Util.FormatDoubleForExport(p.Pozicia.LatitudeValue)}° {p.Pozicia.LatitudeDirection}, {Util.FormatDoubleForExport(p.Pozicia.LongitudeValue)}° {p.Pozicia.LongitudeDirection}", p.CisParcely, p.Popis);
                } else if (this.comboBox.SelectedIndex == 1 && item is Nehnutelnost n) {
                    this.dataGridView.Rows.Add("Nehnutelnost", $"{Util.FormatDoubleForExport(n.Pozicia.LatitudeValue)}° {n.Pozicia.LatitudeDirection}, {Util.FormatDoubleForExport(n.Pozicia.LongitudeValue)}° {n.Pozicia.LongitudeDirection}", n.SupCislo, n.Popis);
                } else if (this.comboBox.SelectedIndex == 2) {
                    if (item is Parcela parcela) {
                        this.dataGridView.Rows.Add("Parcela", $"{Util.FormatDoubleForExport(parcela.Pozicia.LatitudeValue)}° {parcela.Pozicia.LatitudeDirection}, {Util.FormatDoubleForExport(parcela.Pozicia.LongitudeValue)}° {parcela.Pozicia.LongitudeDirection}", parcela.CisParcely, parcela.Popis);
                    } else if (item is Nehnutelnost nehnutelnost) {
                        this.dataGridView.Rows.Add("Nehnutelnost", $"{Util.FormatDoubleForExport(nehnutelnost.Pozicia.LatitudeValue)}° {nehnutelnost.Pozicia.LatitudeDirection}, {Util.FormatDoubleForExport(nehnutelnost.Pozicia.LongitudeValue)}° {nehnutelnost.Pozicia.LongitudeDirection}", nehnutelnost.SupCislo, nehnutelnost.Popis);
                    }
                }
            }
        }
    }
}
