using System.Windows.Forms;

namespace GeoGUI.Classes.Observer {
    public class TextBoxObserver : IObserver {
        private Subject subject;
        private TextBox[] textBoxes;

        public TextBoxObserver(Subject subject, TextBox[] textBoxes) {
            this.subject = subject;
            this.textBoxes = textBoxes;
        }

        public void Update() {
            var chosenItem = this.subject.ChosenItem;

            if (chosenItem is Parcela p) {
                SetFields(p.Pozicia, p.CisParcely, p.Popis);
            } else if (chosenItem is Nehnutelnost n) {
                SetFields(n.Pozicia, n.SupCislo, n.Popis);
            }
        }

        private void SetFields(GPS pozicia, int number, string popis) {
            this.textBoxes[0].Text = pozicia.LatitudeValue.ToString();
            this.textBoxes[1].Text = pozicia.LongitudeValue.ToString();
            this.textBoxes[2].Text = number.ToString();
            this.textBoxes[3].Text = popis.ToString();
        }
    }
}
