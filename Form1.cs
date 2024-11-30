using GeoGUI.Classes.Crate;
using GeoGUI.Classes.Delegates;
using GeoGUI.Classes.Factory;
using GeoGUI.Classes.Observer;
using GeoGUI.Classes.Operation;
using GeoGUI.Classes.Prototype;
using GeoGUI.Classes.Strategy;
using GeoGUI.Classes.Structure;
using GeoGUI.Classes.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GeoGUI {
    public partial class Form1 : Form {
        private KDTree<Parcela, GPS> parcelaTree = new KDTree<Parcela, GPS>(4);
        private KDTree<Nehnutelnost, GPS> nehnutelnostTree = new KDTree<Nehnutelnost, GPS>(4);
        private KDTree<Item, GPS> itemTree = new KDTree<Item, GPS>(4);
        private List<string> idList = new List<string>();
        private List<Item> resultList = new List<Item>();
        private Random random = new Random();
        private Item chosenItem = null;

        private readonly string FILE_PATH = Path.GetFullPath(Path.Combine("..", "..", "Files", "data.txt"));
        private readonly Object[] blockableObjects = { };
        private readonly Subject subject = new Subject();
        private readonly IItemFactory parcelaFactory = ParcelaFactory.GetInstance();
        private readonly IItemFactory nehnutelnostFactory = NehnutelnostFactory.GetInstance();
        private readonly IStrategyFactory<Parcela, GPS> parcelaStrategyFactory = StrategyFactory<Parcela, GPS>.GetInstance();
        private readonly IStrategyFactory<Nehnutelnost, GPS> nehnutelnostStrategyFactory = StrategyFactory<Nehnutelnost, GPS>.GetInstance();
        private readonly IStrategyFactory<Item, GPS> itemStrategyFactory = StrategyFactory<Item, GPS>.GetInstance();
        private readonly TreeTraversal<Item, GPS> itemTreeTraversial = TreeTraversal<Item, GPS>.GetInstance();

        public Form1() {
            InitializeComponent();

            var formTextBoxes = new TextBox[] { this.textBox1, this.textBox2, this.textBox5, this.textBox6 };
            var formComboBoxes = new ComboBox[] { this.comboBox1, this.comboBox3, this.comboBox4 };
            var textBoxObserver = new TextBoxObserver(this.subject, formTextBoxes);
            var comboBoxObserver = new ComboBoxObserver(this.subject, formComboBoxes);
            var dataGridViewObserver = new DataGridViewObserver(this.subject, this.dataGridView, this.comboBox2);
            var counterLabelObserver = new LabelObserver(this.subject, this.label11, this.comboBox2, this.parcelaTree, this.nehnutelnostTree, this.itemTree);

            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 2;
            this.comboBox3.SelectedIndex = 0;
            this.comboBox4.SelectedIndex = 0;
            this.comboBox5.SelectedIndex = 0;
            this.comboBox6.SelectedIndex = 0;
            this.subject.Attach(textBoxObserver);
            this.subject.Attach(comboBoxObserver);
            this.subject.Attach(dataGridViewObserver);
            this.subject.Attach(counterLabelObserver);
            this.blockableObjects = new Object[] { this.textBox3, this.textBox4, this.comboBox5, this.comboBox6 };
        }

        private void FormClick(object sender, EventArgs e) {
            this.chosenItem = null;
            SetBlockableObjects(true);
        }

        private void ButtonClick(object sender, EventArgs e) {
            switch (sender) {
                case Button button when button == button1:
                    this.chosenItem = null;
                    break;

                case Button button when button == button2:
                    AddItem();
                    break;

                case Button button when button == button3:
                    EditItem();
                    break;

                case Button button when button == button4:
                    RemoveItem();
                    break;

                case Button button when button == button5:
                    DuplicateItem();
                    break;

                case Button button when button == button6:
                    ClearStructures();
                    break;

                case Button button when button == button7:
                    LoadFromFile();
                    break;

                case Button button when button == button8:
                    SaveToFile();
                    break;

                case Button button when button == button9:
                    GenerateNodes();
                    break;

                default:
                    break;
            }

            this.chosenItem = null;
            SetBlockableObjects(true);
            SearchItems();
            UpdateFormFields();
            UpdateTableAndCounter();
        }

        private void ComboBoxSelectionChanged(object sender, EventArgs e) {
            switch (sender) {
                case ComboBox comboBox when comboBox == comboBox1:
                    break;

                case ComboBox comboBox when comboBox == comboBox2:
                    SearchItems();
                    UpdateTableAndCounter();
                    break;

                default:
                    break;
            }
        }

        private void DataGridViewCellClick(object sender, DataGridViewCellEventArgs e) {
            if (this.resultList.Count == 0) return;

            if (e.RowIndex >= 0 && e.RowIndex < this.resultList.Count) {
                this.chosenItem = this.resultList[e.RowIndex];

                SetBlockableObjects(false);
                UpdateFormFields();
            }
        }

        private void SearchItems() {
            this.resultList.Clear();

            GPS gps1 = Util.ParseGPS(this.textBox1.Text, this.comboBox3.Text, this.textBox2.Text, this.comboBox4.Text);
            GPS gps2 = Util.ParseGPS(this.textBox3.Text, this.comboBox5.Text, this.textBox4.Text, this.comboBox6.Text);

            PerformSearch(gps1);

            if (gps1?.LatitudeValue == gps2?.LatitudeValue && gps1?.LongitudeValue == gps2?.LongitudeValue) return;

            PerformSearch(gps2);
        }

        private void AddItem() {
            GPS position1, position2;
            int number;
            string description;
            DialogResult result;

            if (string.IsNullOrEmpty(this.textBox1.Text) || string.IsNullOrEmpty(this.textBox2.Text)) {
                MessageBox.Show("Insufficient data provided. Fill up the whole form.", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            position1 = Util.ParseGPS(this.textBox1.Text, this.comboBox3.Text, this.textBox2.Text, this.comboBox4.Text);

            if (string.IsNullOrEmpty(this.textBox3.Text) || string.IsNullOrEmpty(this.textBox4.Text)) {
                MessageBox.Show("Insufficient data provided. Fill up the whole form.", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            position2 = Util.ParseGPS(this.textBox3.Text, this.comboBox5.Text, this.textBox4.Text, this.comboBox6.Text);

            if (string.IsNullOrEmpty(this.textBox5.Text)) {
                MessageBox.Show("Insufficient data provided. Fill up the whole form.", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int.TryParse(this.textBox5.Text, out number);

            if (string.IsNullOrEmpty(this.textBox6.Text)) {
                MessageBox.Show("Insufficient data provided. Fill up the whole form.", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            description = this.textBox6.Text;

            if (this.comboBox1.SelectedIndex == 0) {
                var item1 = this.parcelaFactory.CreateItem(number, description, position1);
                var item2 = this.parcelaFactory.CreateItem(number, description, position2);
                var parcela1 = item1 as Parcela;
                var parcela2 = item2 as Parcela;

                result = MessageBox.Show("Are you sure you want to add this item?", "Confirm Add Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes) {
                    this.parcelaTree.InsertNode(ref parcela1, position1);
                    this.parcelaTree.InsertNode(ref parcela2, position2);
                    this.itemTree.InsertNode(ref item1, position1);
                    this.itemTree.InsertNode(ref item2, position2);

                    this.idList.Add(parcela1.Id);
                    this.idList.Add(parcela2.Id);
                }
            } else {
                var item1 = this.nehnutelnostFactory.CreateItem(number, description, position1);
                var item2 = this.nehnutelnostFactory.CreateItem(number, description, position2);
                var nehnutelnost1 = item1 as Nehnutelnost;
                var nehnutelnost2 = item2 as Nehnutelnost;

                result = MessageBox.Show("Are you sure you want to add this item?", "Confirm Add Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes) {
                    this.nehnutelnostTree.InsertNode(ref nehnutelnost1, position1);
                    this.nehnutelnostTree.InsertNode(ref nehnutelnost2, position2);
                    this.itemTree.InsertNode(ref item1, position1);
                    this.itemTree.InsertNode(ref item2, position2);

                    this.idList.Add(nehnutelnost1.Id);
                    this.idList.Add(nehnutelnost2.Id);
                }
            }
        }

        private void EditItem() {
            if (this.chosenItem == null) return;

            GPS position;
            int number;
            string description;
            DialogResult result;

            if (string.IsNullOrEmpty(this.textBox1.Text) || string.IsNullOrEmpty(this.textBox2.Text)) {
                MessageBox.Show("Insufficient data provided. Fill up the whole form.", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            position = Util.ParseGPS(this.textBox1.Text, this.comboBox3.Text, this.textBox2.Text, this.comboBox4.Text);

            if (string.IsNullOrEmpty(this.textBox5.Text)) {
                MessageBox.Show("Insufficient data provided. Fill up the whole form.", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int.TryParse(this.textBox5.Text, out number);

            if (string.IsNullOrEmpty(this.textBox6.Text)) {
                MessageBox.Show("Insufficient data provided. Fill up the whole form.", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            description = this.textBox6.Text;

            result = MessageBox.Show("Are you sure you want to update this item?", "Confirm Update Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes) {
                try {
                    var oldItem = this.chosenItem;

                    if (oldItem is Parcela oldParcela) {
                        var newItem = this.parcelaFactory.CreateItem(number, description, position);
                        var newParcela = newItem as Parcela;

                        this.parcelaTree.UpdateNode(ref oldParcela, oldParcela.Pozicia, ref newParcela, newParcela.Pozicia);
                        this.itemTree.UpdateNode(ref oldItem, oldParcela.Pozicia, ref newItem, newParcela.Pozicia);
                    } else if (oldItem is Nehnutelnost oldNehnutelnost) {
                        var newItem = this.nehnutelnostFactory.CreateItem(number, description, position);
                        var newNehnutelnost = newItem as Nehnutelnost;

                        this.nehnutelnostTree.UpdateNode(ref oldNehnutelnost, oldNehnutelnost.Pozicia, ref newNehnutelnost, newNehnutelnost.Pozicia);
                        this.itemTree.UpdateNode(ref oldItem, oldNehnutelnost.Pozicia, ref newItem, newNehnutelnost.Pozicia);
                    }
                } catch (NullReferenceException) {
                    MessageBox.Show("Something went wrong when updating this item.", "Failed Update Item", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RemoveItem() {
            if (this.chosenItem == null) return;

            DialogResult result = MessageBox.Show("Are you sure you want to remove this item?", "Confirm Remove Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes) {
                try {
                    var item = this.chosenItem;

                    if (item is Parcela parcela) {
                        this.parcelaTree.DeleteNode(ref parcela, parcela.Pozicia);
                        this.itemTree.DeleteNode(ref item, parcela.Pozicia);
                    } else if (item is Nehnutelnost nehnutelnost) {
                        this.nehnutelnostTree.DeleteNode(ref nehnutelnost, nehnutelnost.Pozicia);
                        this.itemTree.DeleteNode(ref item, nehnutelnost.Pozicia);
                    }
                } catch (NullReferenceException) {
                    MessageBox.Show("Something went wrong when removing this item.", "Failed Remove Item", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DuplicateItem() {
            if (this.chosenItem == null) return;

            DialogResult result = MessageBox.Show("Are you sure you want to duplicate this item?", "Confirm Duplicate Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes) {
                try {
                    var item = this.chosenItem.Clone() as Item;

                    if (item is Parcela parcela) {
                        this.parcelaTree.InsertNode(ref parcela, parcela.Pozicia);
                        this.itemTree.InsertNode(ref item, parcela.Pozicia);
                    } else if (item is Nehnutelnost nehnutelnost) {
                        this.nehnutelnostTree.InsertNode(ref nehnutelnost, nehnutelnost.Pozicia);
                        this.itemTree.InsertNode(ref item, nehnutelnost.Pozicia);
                    }
                } catch (NullReferenceException) {
                    MessageBox.Show("Something went wrong when duplicating this item.", "Failed Duplicate Item", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearStructures() {
            this.parcelaTree.Clear();
            this.nehnutelnostTree.Clear();
            this.itemTree.Clear();
            this.idList.Clear();
            this.resultList.Clear();
            this.subject.ResultList.Clear();
            this.chosenItem = null;

            UpdateTableAndCounter();

            MessageBox.Show($"Data has been cleared.");
        }

        private void SaveToFile() {
            if (this.itemTree.Root == null) {
                MessageBox.Show("No data to save.");
                return;
            }

            if (File.Exists(FILE_PATH)) File.Delete(FILE_PATH);

            using (StreamWriter writer = new StreamWriter(FILE_PATH)) {
                writer.WriteLine("KeysData;NodeData >>>");
                this.itemTreeTraversial.InOrderTraversal(this.itemTree.Root, WriteNodeOperation<Item, GPS>.GetInstance(writer));
            }

            MessageBox.Show($"Data saved to: {FILE_PATH}");
        }

        private void LoadFromFile() {
            if (!File.Exists(FILE_PATH)) {
                MessageBox.Show("File does not exist.");
                return;
            }

            ClearStructures();

            using (StreamReader reader = new StreamReader(FILE_PATH)) {
                string header = reader.ReadLine();
                if (header == null || !header.StartsWith("KeysData;NodeData >>>")) {
                    MessageBox.Show("Invalid file format.");
                    return;
                }

                string line;
                while ((line = reader.ReadLine()) != null) {
                    string[] parts = line.Replace("'", "").Split(';');
                    if (parts.Length < 2) {
                        MessageBox.Show("Invalid data format in file.");
                        return;
                    }

                    string[] coordinates = parts[0].Split(',');
                    GPS keysData;
                    if (coordinates[0] == "GPS") {
                        keysData = Util.ParseGPS(Util.FormatDoubleForImport(coordinates[1]), coordinates[2], Util.FormatDoubleForImport(coordinates[3]), coordinates[4]);
                    } else {
                        return;
                    }

                    for (int i = 1; i < parts.Length; i++) {
                        List<string> dataEntry = parts[i].Split(',').ToList();
                        string type = dataEntry[0];
                        string id = dataEntry[1];
                        int number = -1;
                        int.TryParse(dataEntry[2], out number);
                        string description = dataEntry[3];

                        if (type == "Parcela") {
                            var parcela = new Parcela(number, description, keysData) { Id = id };
                            var item = parcela as Item;

                            this.parcelaTree.InsertNode(ref parcela, keysData);
                            this.itemTree.InsertNode(ref item, keysData);
                        } else if (type == "Nehnutelnost") {
                            var nehnutelnost = new Nehnutelnost(number, description, keysData) { Id = id };
                            var item = nehnutelnost as Item;

                            this.nehnutelnostTree.InsertNode(ref nehnutelnost, keysData);
                            this.itemTree.InsertNode(ref item, keysData);
                        }
                    }
                }
            }

            MessageBox.Show($"Data loaded from: {FILE_PATH}");
        }

        private void GenerateNodes() {
            int parcelaCount = 0, nehnutelnostCount = 0;
            double intersectionProb = 0.0;

            if (!string.IsNullOrEmpty(this.textBox7.Text)) {
                int.TryParse(this.textBox7.Text, out parcelaCount);
            }

            if (!string.IsNullOrEmpty(this.textBox8.Text)) {
                int.TryParse(this.textBox8.Text, out nehnutelnostCount);
            }

            if (!string.IsNullOrEmpty(this.textBox9.Text)) {
                double.TryParse(this.textBox9.Text, out intersectionProb);
            }

            if (parcelaCount == 0 && nehnutelnostCount == 0) {
                MessageBox.Show("Insufficient data provided. Both counts are at 0.", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            GenerateItems(parcelaCount, intersectionProb, this.parcelaTree, this.parcelaFactory);
            GenerateItems(nehnutelnostCount, intersectionProb, this.nehnutelnostTree, this.nehnutelnostFactory);
        }

        private void PerformSearch(GPS gps) {
            if (gps == null) return;

            try {
                double.TryParse(this.textBox14.Text, out double factor);

                switch (this.comboBox2.SelectedIndex) {
                    case 0:
                        AddResultsToList(this.parcelaStrategyFactory.CreateStrategy(factor), this.parcelaTree, gps);
                        break;

                    case 1:
                        AddResultsToList(this.nehnutelnostStrategyFactory.CreateStrategy(factor), this.nehnutelnostTree, gps);
                        break;

                    case 2:
                        AddResultsToList(this.itemStrategyFactory.CreateStrategy(factor), this.itemTree, gps);
                        break;

                    default:
                        break;
                }
            } catch (NullReferenceException) { }
        }

        private void GenerateItems<T>(int count, double intersectionProb, KDTree<T, GPS> tree, IItemFactory factory) where T : Item {
            List<GPS> gpsList = new List<GPS>();

            for (int i = 0; i < count / 2; i++) {
                int number = this.random.Next();
                string description = Util.GenerateRandomString(10);

                GPS position1 = Util.GenerateRandomGPS(50, 50);
                GPS position2 = Util.GenerateRandomGPS(50, 50);

                if (this.random.NextDouble() < intersectionProb && gpsList.Count > 1) {
                    position1 = gpsList[this.random.Next(gpsList.Count)];
                    List<GPS> filteredList = gpsList.Where(gps => gps != position1).ToList();
                    position2 = filteredList[this.random.Next(filteredList.Count)];
                }

                var item1 = factory.CreateItem(number, description, position1);
                var item2 = factory.CreateItem(number, description, position2);
                var item3 = item1 as T;
                var item4 = item2 as T;

                this.itemTree.InsertNode(ref item1, position1);
                this.itemTree.InsertNode(ref item2, position2);

                tree.InsertNode(ref item3, position1);
                tree.InsertNode(ref item4, position2);

                this.idList.Add(item1.Id);
                this.idList.Add(item2.Id);

                gpsList.Add(position1);
                gpsList.Add(position2);
            }
        }

        private void AddResultsToList<T, U>(IStrategy<T, U> strategy, KDTree<T, U> tree, U keys) where T : Item where U : IKey<U> {
            this.resultList.AddRange(strategy.Traverse(tree, keys).Where(x => !this.resultList.Contains(x)));
        }

        private void UpdateFormFields() {
            this.subject.SetChosenItem(this.chosenItem);
        }

        private void UpdateTableAndCounter() {
            this.subject.SetResultList(this.resultList);
        }

        private void SetBlockableObjects(bool isEnabled) {
            foreach (Object obj in this.blockableObjects) {
                if (obj is TextBox tb) {
                    tb.Enabled = isEnabled;
                } else if (obj is ComboBox cb) {
                    cb.Enabled = isEnabled;
                } else if (obj is Button b) {
                    b.Enabled = isEnabled;
                }
            }
        }
    }
}
