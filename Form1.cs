using GeoGUI.Classes;
using GeoGUI.Classes.Factory;
using GeoGUI.Classes.Strategy;
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

        private IFactory parcelaFactory = new ParcelaFactory();
        private IFactory nehnutelnostFactory = new NehnutelnostFactory();
        private SubjectList subjectList = new SubjectList();

        private const string FILE_PATH = @"C:\Users\ipast\source\repos\GeoGUI\Files\exported.txt";

        public Form1() {
            InitializeComponent();

            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 2;
            this.comboBox3.SelectedIndex = 0;
            this.comboBox4.SelectedIndex = 0;
            this.comboBox5.SelectedIndex = 0;
            this.comboBox6.SelectedIndex = 0;

            var dataGridViewObserver = new DataGridViewObserver(this.dataGridView, this.subjectList, this.comboBox2);
            var counterLabelObserver = new CounterLabelObserver(this.label11, this.subjectList, this.comboBox2, this.parcelaTree, this.nehnutelnostTree, this.itemTree);

            this.subjectList.Attach(dataGridViewObserver);
            this.subjectList.Attach(counterLabelObserver);
        }

        private void ButtonClick(object sender, EventArgs e) {
            switch (sender) {
                case Button button when button == button1:
                    this.comboBox2.SelectedIndex = 0;
                    SearchItems(false);
                    UpdateResultsTableAndCounter();
                    return;

                case Button button when button == button2:
                    this.comboBox2.SelectedIndex = 1;
                    SearchItems(false);
                    UpdateResultsTableAndCounter();
                    return;

                case Button button when button == button3:
                    this.comboBox2.SelectedIndex = 2;
                    SearchItems(false);
                    UpdateResultsTableAndCounter();
                    return;

                case Button button when button == button4:
                    this.comboBox1.SelectedIndex = 0;
                    AddItem();
                    break;

                case Button button when button == button5:
                    this.comboBox1.SelectedIndex = 1;
                    AddItem();
                    break;

                case Button button when button == button6:
                    EditItem(true);
                    break;

                case Button button when button == button7:
                    RemoveItem();
                    break;

                case Button button when button == button8:

                    break;

                case Button button when button == button9:
                    DuplicateItem();
                    break;

                case Button button when button == button10:
                    ClearStructures();
                    break;

                case Button button when button == button11:
                    LoadFromFile();
                    break;

                case Button button when button == button12:
                    SaveToFile();
                    break;

                case Button button when button == button13:
                    GenerateNodes();
                    break;

                default:

                    break;
            }

            SearchItems(true);
            UpdateResultsTableAndCounter();
        }

        private void DuplicateItem() {
            if (this.chosenItem == null) return;

            if (this.chosenItem is Parcela p) {
                this.textBox1.Text = p.Pozicia.X.ToString();
                this.textBox2.Text = p.Pozicia.Y.ToString();
                this.comboBox1.SelectedIndex = 0;
                this.comboBox3.SelectedIndex = p.Pozicia.Sirka == "W" ? 0 : 1;
                this.comboBox4.SelectedIndex = p.Pozicia.Dlzka == "N" ? 0 : 1;
                this.textBox5.Text = p.CisParcely.ToString();
                this.textBox6.Text = p.Popis;
            } else if (this.chosenItem is Nehnutelnost n) {
                this.textBox1.Text = n.Pozicia.X.ToString();
                this.textBox2.Text = n.Pozicia.Y.ToString();
                this.comboBox1.SelectedIndex = 1;
                this.comboBox3.SelectedIndex = n.Pozicia.Sirka == "W" ? 0 : 1;
                this.comboBox4.SelectedIndex = n.Pozicia.Dlzka == "N" ? 0 : 1;
                this.textBox5.Text = n.SupCislo.ToString();
                this.textBox6.Text = n.Popis;
            }

            this.textBox3.Enabled = false;
            this.textBox4.Enabled = false;
            this.comboBox1.Enabled = false;

            DialogResult result = MessageBox.Show("Are you sure you want to duplicate this item?", "Confirm Duplicate Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes) {
                try {
                    var itemClone = this.chosenItem.Clone();

                    if (itemClone is Parcela parcelaClone) {
                        this.parcelaTree.InsertNode(ref parcelaClone, parcelaClone.Pozicia);
                        this.itemTree.InsertNode(ref itemClone, parcelaClone.Pozicia);
                    } else if (itemClone is Nehnutelnost nehnutelnostClone) {
                        this.nehnutelnostTree.InsertNode(ref nehnutelnostClone, nehnutelnostClone.Pozicia);
                        this.itemTree.InsertNode(ref itemClone, nehnutelnostClone.Pozicia);
                    }
                } catch (NullReferenceException) {
                    MessageBox.Show("Something went wrong when duplicating this item.", "Failed Duplicate Item", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            SearchItems(true);
            UpdateResultsTableAndCounter();

            this.textBox3.Enabled = true;
            this.textBox4.Enabled = true;
            this.comboBox1.Enabled = true;
        }

        private void ComboBoxSelectionChanged(object sender, EventArgs e) {
            switch (sender) {
                case ComboBox comboBox when comboBox == comboBox1:
                    break;

                case ComboBox comboBox when comboBox == comboBox2:
                    SearchItems(true);
                    UpdateResultsTableAndCounter();
                    break;

                default:
                    break;
            }
        }

        private void DataGridViewCellClick(object sender, DataGridViewCellEventArgs e) {
            if (this.resultList.Count == 0) return;

            if (e.RowIndex >= 0) {
                if (e.ColumnIndex == 0) {
                    this.chosenItem = this.resultList[e.RowIndex];
                    DuplicateItem();
                } else if (e.ColumnIndex == 1) {
                    this.chosenItem = this.resultList[e.RowIndex];
                    EditItem(false);
                } else if (e.ColumnIndex == 2) {
                    this.chosenItem = this.resultList[e.RowIndex];
                    RemoveItem();
                } else {
                    this.textBox3.Enabled = true;
                    this.textBox4.Enabled = true;
                    this.comboBox1.Enabled = true;
                }
            }
        }

        private void DataGridViewCellContentClick(object sender, DataGridViewCellEventArgs e) {
            if (this.resultList.Count == 0) return;

            if (e.RowIndex >= 0 && e.RowIndex < this.resultList.Count) {
                if (e.ColumnIndex == 0) {
                    this.chosenItem = this.resultList[e.RowIndex];
                    EditItem(false);
                } else if (e.ColumnIndex == 1) {
                    this.chosenItem = this.resultList[e.RowIndex];
                    RemoveItem();
                } else {
                    this.textBox3.Enabled = true;
                    this.textBox4.Enabled = true;
                    this.comboBox1.Enabled = true;
                }
            }
        }

        private void SearchItems(bool noMessageBox) {
            this.resultList.Clear();

            GPS gps1 = ParseGPS(this.textBox1.Text, this.textBox2.Text, this.comboBox3.Text, this.comboBox4.Text);
            GPS gps2 = ParseGPS(this.textBox3.Text, this.textBox4.Text, this.comboBox5.Text, this.comboBox6.Text);

            PerformSearch(gps1, gps1 != null, noMessageBox);

            if (gps1?.X == gps2?.X && gps1?.Y == gps2?.Y) return;

            PerformSearch(gps2, gps2 != null, noMessageBox);
        }

        private void PerformSearch(GPS gps, bool gpsValid, bool noMessageBox) {
            if (gps == null) return;

            try {
                switch (this.comboBox2.SelectedIndex) {
                    case 0:
                        AddResultsToList(new SearchStrategy<Parcela>(), this.parcelaTree, gps);
                        break;

                    case 1:
                        AddResultsToList(new SearchStrategy<Nehnutelnost>(), this.nehnutelnostTree, gps);
                        break;

                    default:
                        try {
                            AddResultsToList(new SearchStrategy<Parcela>(), this.parcelaTree, gps);
                        } catch (NullReferenceException) {
                            if (!noMessageBox && gpsValid) {
                                MessageBox.Show($"No matching nodes with keys: [{gps.GetKeys()}].");
                            }
                        }
                        AddResultsToList(new SearchStrategy<Nehnutelnost>(), this.nehnutelnostTree, gps);
                        break;
                }
            } catch (NullReferenceException) {
                if (!noMessageBox && gpsValid) {
                    MessageBox.Show($"No matching nodes with keys: [{gps.GetKeys()}].");
                }
            }
        }

        private void AddResultsToList<T>(IStrategy<T> strategy, KDTree<T, GPS> tree, GPS gps) where T : Item {
            this.resultList.AddRange(strategy.Search(tree, gps));
        }

        private void AddItem() {
            GPS position1, position2;
            int number;
            string description;
            DialogResult result;

            if (string.IsNullOrEmpty(this.textBox1.Text) || string.IsNullOrEmpty(this.textBox2.Text)) {
                ShowMessageBox("Insufficient data provided. Fill up the whole form.", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            position1 = ParseGPS(this.textBox1.Text, this.textBox2.Text, this.comboBox3.Text, this.comboBox4.Text);

            if (string.IsNullOrEmpty(this.textBox3.Text) || string.IsNullOrEmpty(this.textBox4.Text)) {
                ShowMessageBox("Insufficient data provided. Fill up the whole form.", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            position2 = ParseGPS(this.textBox3.Text, this.textBox4.Text, this.comboBox5.Text, this.comboBox6.Text);

            if (string.IsNullOrEmpty(this.textBox5.Text)) {
                ShowMessageBox("Insufficient data provided. Fill up the whole form.", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int.TryParse(this.textBox5.Text, out number);

            if (string.IsNullOrEmpty(this.textBox6.Text)) {
                ShowMessageBox("Insufficient data provided. Fill up the whole form.", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            description = this.textBox6.Text;

            if (this.comboBox1.SelectedIndex == 0) {
                var parcela1 = new Parcela(number, description, position1);
                var parcela2 = new Parcela(number, description, position2);
                var item1 = parcela1 as Item;
                var item2 = parcela2 as Item;

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
                var nehnutelnost1 = new Nehnutelnost(number, description, position1);
                var nehnutelnost2 = new Nehnutelnost(number, description, position2);
                var item1 = nehnutelnost1 as Item;
                var item2 = nehnutelnost2 as Item;

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

        private void EditItem(bool confirm) {
            if (this.chosenItem == null) return;

            if (!confirm) {
                if (this.chosenItem is Parcela p) {
                    this.textBox1.Text = p.Pozicia.X.ToString();
                    this.textBox2.Text = p.Pozicia.Y.ToString();
                    this.comboBox1.SelectedIndex = 0;
                    this.comboBox3.SelectedIndex = p.Pozicia.Sirka == "W" ? 0 : 1;
                    this.comboBox4.SelectedIndex = p.Pozicia.Dlzka == "N" ? 0 : 1;
                    this.textBox5.Text = p.CisParcely.ToString();
                    this.textBox6.Text = p.Popis;
                } else if (this.chosenItem is Nehnutelnost n) {
                    this.textBox1.Text = n.Pozicia.X.ToString();
                    this.textBox2.Text = n.Pozicia.Y.ToString();
                    this.comboBox1.SelectedIndex = 1;
                    this.comboBox3.SelectedIndex = n.Pozicia.Sirka == "W" ? 0 : 1;
                    this.comboBox4.SelectedIndex = n.Pozicia.Dlzka == "N" ? 0 : 1;
                    this.textBox5.Text = n.SupCislo.ToString();
                    this.textBox6.Text = n.Popis;
                }

                this.textBox3.Enabled = false;
                this.textBox4.Enabled = false;
                this.comboBox1.Enabled = false;
            } else {
                GPS position;
                int number;
                string description;
                DialogResult result;

                if (string.IsNullOrEmpty(this.textBox1.Text) || string.IsNullOrEmpty(this.textBox2.Text)) {
                    ShowMessageBox("Insufficient data provided. Fill up the whole form.", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                position = ParseGPS(this.textBox1.Text, this.textBox2.Text, this.comboBox3.Text, this.comboBox4.Text);

                if (string.IsNullOrEmpty(this.textBox5.Text)) {
                    ShowMessageBox("Insufficient data provided. Fill up the whole form.", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int.TryParse(this.textBox5.Text, out number);

                if (string.IsNullOrEmpty(this.textBox6.Text)) {
                    ShowMessageBox("Insufficient data provided. Fill up the whole form.", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                description = this.textBox6.Text;

                result = MessageBox.Show("Are you sure you want to update this item?", "Confirm Update Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes) {
                    try {
                        if (this.chosenItem is Parcela p) {
                            var parcela = new Parcela(number, description, position);
                            var item = parcela as Item;

                            this.parcelaTree.UpdateNode(ref p, p.Pozicia, ref parcela, parcela.Pozicia);
                            this.itemTree.UpdateNode(ref this.chosenItem, p.Pozicia, ref item, parcela.Pozicia);
                        } else if (this.chosenItem is Nehnutelnost n) {
                            var nehnutelnost = new Nehnutelnost(number, description, position);
                            var item = nehnutelnost as Item;

                            this.nehnutelnostTree.UpdateNode(ref n, n.Pozicia, ref nehnutelnost, nehnutelnost.Pozicia);
                            this.itemTree.UpdateNode(ref this.chosenItem, n.Pozicia, ref item, nehnutelnost.Pozicia);
                        }
                    } catch (NullReferenceException) {
                        MessageBox.Show("Something went wrong when updating this item.", "Failed Update Item", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                this.textBox3.Enabled = true;
                this.textBox4.Enabled = true;
                this.comboBox1.Enabled = true;
            }
        }

        private void RemoveItem() {
            if (this.chosenItem == null) return;

            if (this.chosenItem is Parcela p) {
                this.textBox1.Text = p.Pozicia.X.ToString();
                this.textBox2.Text = p.Pozicia.Y.ToString();
                this.comboBox1.SelectedIndex = 0;
                this.comboBox3.SelectedIndex = p.Pozicia.Sirka == "W" ? 0 : 1;
                this.comboBox4.SelectedIndex = p.Pozicia.Dlzka == "N" ? 0 : 1;
                this.textBox5.Text = p.CisParcely.ToString();
                this.textBox6.Text = p.Popis;
            } else if (this.chosenItem is Nehnutelnost n) {
                this.textBox1.Text = n.Pozicia.X.ToString();
                this.textBox2.Text = n.Pozicia.Y.ToString();
                this.comboBox1.SelectedIndex = 1;
                this.comboBox3.SelectedIndex = n.Pozicia.Sirka == "W" ? 0 : 1;
                this.comboBox4.SelectedIndex = n.Pozicia.Dlzka == "N" ? 0 : 1;
                this.textBox5.Text = n.SupCislo.ToString();
                this.textBox6.Text = n.Popis;
            }

            this.textBox3.Enabled = false;
            this.textBox4.Enabled = false;
            this.comboBox1.Enabled = false;

            DialogResult result = MessageBox.Show("Are you sure you want to remove this item?", "Confirm Remove Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes) {
                try {
                    if (this.chosenItem is Parcela parcela) {
                        this.parcelaTree.DeleteNode(ref parcela, parcela.Pozicia);
                        this.itemTree.DeleteNode(ref this.chosenItem, parcela.Pozicia);
                    } else if (this.chosenItem is Nehnutelnost nehnutelnost) {
                        this.nehnutelnostTree.DeleteNode(ref nehnutelnost, nehnutelnost.Pozicia);
                        this.itemTree.DeleteNode(ref this.chosenItem, nehnutelnost.Pozicia);
                    }
                } catch (NullReferenceException) {
                    MessageBox.Show("Something went wrong when removing this item.", "Failed Remove Item", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            SearchItems(true);
            UpdateResultsTableAndCounter();

            this.textBox3.Enabled = true;
            this.textBox4.Enabled = true;
            this.comboBox1.Enabled = true;
        }

        private void ClearStructures() {
            this.parcelaTree.Clear();
            this.nehnutelnostTree.Clear();
            this.itemTree.Clear();
            this.idList.Clear();
            this.resultList.Clear();
            this.subjectList.ResultList.Clear();
            this.chosenItem = null;

            UpdateResultsTableAndCounter();
        }

        private void SaveToFile() {
            if (this.itemTree.Root == null) {
                MessageBox.Show("K-D tree is empty.");
                return;
            }

            if (File.Exists(FILE_PATH)) File.Delete(FILE_PATH);

            using (StreamWriter writer = new StreamWriter(FILE_PATH)) {
                writer.WriteLine("KeysData;NodeData >>>");

                Queue<Node<Item, GPS>> queue = new Queue<Node<Item, GPS>>();
                queue.Enqueue(this.itemTree.Root);

                while (queue.Count > 0) {
                    Node<Item, GPS> current = queue.Dequeue();

                    string keysData = current.KeysData.GetKeys();
                    string nodeData = string.Join(";", current.NodeData.ConvertAll(data => data.GetInfo()));

                    writer.WriteLine($"'{keysData};'{nodeData}");

                    if (current.LeftSon != null) queue.Enqueue(current.LeftSon);

                    if (current.RightSon != null) queue.Enqueue(current.RightSon);
                }
            }

            MessageBox.Show($"K-D tree saved to: {FILE_PATH}");
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
                        keysData = ParseGPS(coordinates[1].Replace('.', ','), coordinates[3].Replace('.', ','), coordinates[2], coordinates[4]);
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
                            var parcela = new Parcela(number, description, keysData);
                            parcela.Id = id;
                            var item = parcela as Item;

                            this.parcelaTree.InsertNode(ref parcela, keysData);
                            this.itemTree.InsertNode(ref item, keysData);
                        } else if (type == "Nehnutelnost") {
                            var nehnutelnost = new Nehnutelnost(number, description, keysData);
                            nehnutelnost.Id = id;
                            var item = nehnutelnost as Item;

                            this.nehnutelnostTree.InsertNode(ref nehnutelnost, keysData);
                            this.itemTree.InsertNode(ref item, keysData);
                        }
                    }
                }
            }

            MessageBox.Show($"K-D tree loaded from: {FILE_PATH}");
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
                ShowMessageBox("Insufficient data provided. Both counts are at 0.", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            GenerateItems<Parcela>(parcelaCount, intersectionProb, this.parcelaTree, this.parcelaFactory);
            GenerateItems<Nehnutelnost>(nehnutelnostCount, intersectionProb, this.nehnutelnostTree, this.nehnutelnostFactory);
        }

        private void GenerateItems<T>(int count, double intersectionProb, KDTree<T, GPS> tree, IFactory factory) where T : Item {
            List<GPS> gpsList = new List<GPS>();

            for (int i = 0; i < count / 2; i++) {
                int number = this.random.Next();
                string description = GenerateRandomString(10);

                GPS position1 = GenerateRandomGPS();
                GPS position2 = GenerateRandomGPS();

                if (this.random.NextDouble() < intersectionProb && gpsList.Count > 1) {
                    position1 = gpsList[this.random.Next(gpsList.Count)];
                    List<GPS> filteredList = gpsList.Where(gps => gps != position1).ToList();
                    position2 = filteredList[this.random.Next(filteredList.Count)];
                }

                var item1 = factory.CreatePrototype(number, description, position1) as Item;
                var item2 = factory.CreatePrototype(number, description, position2) as Item;
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

        private GPS GenerateRandomGPS() {
            double x = Math.Round(this.random.NextDouble() * 50, 0);
            double y = Math.Round(this.random.NextDouble() * 50, 0);
            string sirka = this.random.NextDouble() < 0.5 ? "W" : "E";
            string dlzka = this.random.NextDouble() < 0.5 ? "N" : "S";
            return new GPS(sirka, x, dlzka, y);
        }

        private void UpdateResultsTableAndCounter() {
            this.subjectList.SetResultList(this.resultList);
        }

        private GPS ParseGPS(string latitude, string longitude, string sirka, string dlzka) {
            double x = Double.MaxValue, y = Double.MaxValue;
            double.TryParse(latitude, out x);
            double.TryParse(longitude, out y);
            return new GPS(sirka, x, dlzka, y);
        }

        private string GenerateRandomString(int length) {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[this.random.Next(s.Length)]).ToArray());
        }

        private void ShowMessageBox(string title, string message, MessageBoxButtons buttons, MessageBoxIcon icon) {
            MessageBox.Show(title, message, buttons, icon);
        }
    }
}
