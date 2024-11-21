using System.Windows.Forms;

namespace GeoGUI.Classes {
    public class DataGridViewObserver : IObserver {
        private DataGridView dataGridView;
        private SubjectList subject;
        private ComboBox comboBox;

        public DataGridViewObserver(DataGridView dataGridView, SubjectList subject, ComboBox comboBox) {
            this.dataGridView = dataGridView;
            this.subject = subject;
            this.comboBox = comboBox;
        }

        public void Update() {
            this.dataGridView.Rows.Clear();

            if (this.dataGridView.Columns.Count == 0) {
                this.dataGridView.Columns.Add("Type", "Type");
                this.dataGridView.Columns.Add("Position", "Position");
                this.dataGridView.Columns.Add("Number", "Number");
                this.dataGridView.Columns.Add("Description", "Description");
                this.dataGridView.Columns.Add("Edit", "Edit");
                this.dataGridView.Columns.Add("Remove", "Remove");
            }

            foreach (Item item in this.subject.ResultList) {
                if (this.comboBox.SelectedIndex == 0 && item is Parcela p) {
                    this.dataGridView.Rows.Add("<e>", "<r>", "Parcela", $"{p.Pozicia.X}° {p.Pozicia.Sirka}, {p.Pozicia.Y}° {p.Pozicia.Dlzka}", p.CisParcely, p.Popis);
                } else if (this.comboBox.SelectedIndex == 1 && item is Nehnutelnost n) {
                    this.dataGridView.Rows.Add("<e>", "<r>", "Nehnutelnost", $"{n.Pozicia.X}° {n.Pozicia.Sirka}, {n.Pozicia.Y}° {n.Pozicia.Dlzka}", n.SupCislo, n.Popis);
                } else if (this.comboBox.SelectedIndex == 2) {
                    if (item is Parcela parcela) {
                        this.dataGridView.Rows.Add("<e>", "<r>", "Parcela", $"{parcela.Pozicia.X}° {parcela.Pozicia.Sirka}, {parcela.Pozicia.Y}° {parcela.Pozicia.Dlzka}", parcela.CisParcely, parcela.Popis);
                    } else if (item is Nehnutelnost nehnutelnost) {
                        this.dataGridView.Rows.Add("<e>", "<r>", "Nehnutelnost", $"{nehnutelnost.Pozicia.X}° {nehnutelnost.Pozicia.Sirka}, {nehnutelnost.Pozicia.Y}° {nehnutelnost.Pozicia.Dlzka}", nehnutelnost.SupCislo, nehnutelnost.Popis);
                    }
                }
            }
        }
    }
}
