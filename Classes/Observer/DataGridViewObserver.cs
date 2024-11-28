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
                    this.dataGridView.Rows.Add("Parcela", $"{Util.FormatDoubleForExport(p.Pozicia.X)}° {p.Pozicia.Sirka}, {Util.FormatDoubleForExport(p.Pozicia.Y)}° {p.Pozicia.Dlzka}", p.CisParcely, p.Popis);
                } else if (this.comboBox.SelectedIndex == 1 && item is Nehnutelnost n) {
                    this.dataGridView.Rows.Add("Nehnutelnost", $"{Util.FormatDoubleForExport(n.Pozicia.X)}° {n.Pozicia.Sirka}, {Util.FormatDoubleForExport(n.Pozicia.Y)}° {n.Pozicia.Dlzka}", n.SupCislo, n.Popis);
                } else if (this.comboBox.SelectedIndex == 2) {
                    if (item is Parcela parcela) {
                        this.dataGridView.Rows.Add("Parcela", $"{Util.FormatDoubleForExport(parcela.Pozicia.X)}° {parcela.Pozicia.Sirka}, {Util.FormatDoubleForExport(parcela.Pozicia.Y)}° {parcela.Pozicia.Dlzka}", parcela.CisParcely, parcela.Popis);
                    } else if (item is Nehnutelnost nehnutelnost) {
                        this.dataGridView.Rows.Add("Nehnutelnost", $"{Util.FormatDoubleForExport(nehnutelnost.Pozicia.X)}° {nehnutelnost.Pozicia.Sirka}, {Util.FormatDoubleForExport(nehnutelnost.Pozicia.Y)}° {nehnutelnost.Pozicia.Dlzka}", nehnutelnost.SupCislo, nehnutelnost.Popis);
                    }
                }
            }
        }
    }
}
