using System.Windows.Forms;

namespace GeoGUI.Classes {

    public class CounterLabelObserver : IObserver {
        private Label label;
        private SubjectList subject;
        private ComboBox comboBox;
        private KDTree<Parcela, GPS> parcelaTree;
        private KDTree<Nehnutelnost, GPS> nehnutelnostTree;
        private KDTree<Item, GPS> itemTree;

        public CounterLabelObserver(Label label, SubjectList subject, ComboBox comboBox, KDTree<Parcela, GPS> parcelaTree, KDTree<Nehnutelnost, GPS> nehnutelnostTree, KDTree<Item, GPS> itemTree) {
            this.label = label;
            this.subject = subject;
            this.comboBox = comboBox;
            this.parcelaTree = parcelaTree;
            this.nehnutelnostTree = nehnutelnostTree;
            this.itemTree = itemTree;
        }

        public void Update() {
            int count = this.subject.ResultList.Count;

            if (this.comboBox.SelectedIndex == 0) {
                this.label.Text = $"Counter: {count} / {this.parcelaTree.DataSize}";
            } else if (this.comboBox.SelectedIndex == 1) {
                this.label.Text = $"Counter: {count} / {this.nehnutelnostTree.DataSize}";
            } else {
                this.label.Text = $"Counter: {count} / {this.itemTree.DataSize}";
            }
        }
    }
}
