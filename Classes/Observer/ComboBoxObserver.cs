﻿using GeoGUI.Classes.Crate;
using GeoGUI.Classes.Prototype;
using System.Windows.Forms;

namespace GeoGUI.Classes.Observer {
    public class ComboBoxObserver : IObserver {
        private Subject subject;
        private ComboBox[] comboBoxes;

        public ComboBoxObserver(Subject subject, ComboBox[] comboBoxes) {
            this.subject = subject;
            this.comboBoxes = comboBoxes;
        }

        public void Update() {
            var chosenItem = this.subject.ChosenItem;

            if (chosenItem is Parcela p) {
                SetFields(p.Pozicia, 0);
            } else if (chosenItem is Nehnutelnost n) {
                SetFields(n.Pozicia, 1);
            }
        }

        private void SetFields(GPS position, int itemComboBoxIndex) {
            this.comboBoxes[0].SelectedIndex = itemComboBoxIndex;
            this.comboBoxes[1].SelectedIndex = position.LatitudeDirection == "N" ? 0 : 1;
            this.comboBoxes[2].SelectedIndex = position.LongitudeDirection == "E" ? 0 : 1;
        }
    }
}
