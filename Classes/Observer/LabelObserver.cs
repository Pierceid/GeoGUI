using GeoGUI.Classes.Crate;
using GeoGUI.Classes.Prototype;
using GeoGUI.Classes.Structure;
using System.Windows.Forms;

namespace GeoGUI.Classes.Observer {
    public class LabelObserver : IObserver {
        private Subject subject;
        private Label label;
        private ComboBox comboBox;
        private KDTree<Parcela, GPS> parcelaTree;
        private KDTree<Nehnutelnost, GPS> nehnutelnostTree;
        private KDTree<Item, GPS> itemTree;

        public LabelObserver(Subject subject, Label label, ComboBox comboBox, KDTree<Parcela, GPS> parcelaTree, KDTree<Nehnutelnost, GPS> nehnutelnostTree, KDTree<Item, GPS> itemTree) {
            this.subject = subject;
            this.label = label;
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
