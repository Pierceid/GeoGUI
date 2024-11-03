using GeoGUI.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GeoGUI {
    public partial class Form1 : Form {
        private KDTree<Parcela, GPS> parcelaTree = new KDTree<Parcela, GPS>();
        private KDTree<Nehnutelnost, GPS> nehnutelnostTree = new KDTree<Nehnutelnost, GPS>();
        private KDTree<Item, GPS> itemTree = new KDTree<Item, GPS>();
        private List<string> idList = new List<string>();
        private List<Item> resultList = new List<Item>();
        private Random random = new Random();
        private const string FILE_PATH = @"C:\Users\ipast\source\repos\GeoGUI\Files\exported.csv";
        private Item chosenItem = null;

        public Form1() {
            InitializeComponent();

            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 2;
        }

        private void ButtonClick(object sender, EventArgs e) {
            switch (sender) {
                case Button button when button == button1:
                    this.comboBox2.SelectedIndex = 0;
                    SearchItems(false);
                    break;

                case Button button when button == button2:
                    this.comboBox2.SelectedIndex = 1;
                    SearchItems(false);
                    break;

                case Button button when button == button3:
                    this.comboBox2.SelectedIndex = 2;
                    SearchItems(false);
                    break;

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
                    EditItem(true);
                    break;

                case Button button when button == button8:
                    RemoveItem();
                    break;

                case Button button when button == button9:
                    RemoveItem();
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

                case Button button when button.Name == "editButton":

                    break;

                case Button button when button.Name == "removeButton":

                    break;

                default:

                    break;
            }
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
            if (e.RowIndex >= 0) {
                if (e.ColumnIndex == 0) {
                    this.chosenItem = this.resultList[e.RowIndex];
                    EditItem(false);
                } else if (e.ColumnIndex == 1) {
                    this.chosenItem = this.resultList[e.RowIndex];
                    RemoveItem();
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
                }
            }
        }

        private void SearchItems(bool noMessageBox) {
            this.resultList.Clear();

            GPS gps1 = null, gps2 = null;

            bool validCoordinates1 = !string.IsNullOrEmpty(this.textBox1.Text) && !string.IsNullOrEmpty(this.textBox2.Text);
            bool validCoordinates2 = !string.IsNullOrEmpty(this.textBox3.Text) && !string.IsNullOrEmpty(this.textBox4.Text);

            if (validCoordinates1) {
                gps1 = ParseGPS(this.textBox1.Text, this.textBox2.Text);
            }

            if (validCoordinates2) {
                gps2 = ParseGPS(this.textBox3.Text, this.textBox4.Text);
            }

            try {
                if (this.comboBox2.SelectedIndex == 0) {
                    this.resultList.AddRange(this.parcelaTree.FindNodes(gps1));
                } else if (this.comboBox2.SelectedIndex == 1) {
                    this.resultList.AddRange(this.nehnutelnostTree.FindNodes(gps1));
                } else {
                    this.resultList.AddRange(this.itemTree.FindNodes(gps1));
                }
            } catch (NullReferenceException) {
                if (noMessageBox || !validCoordinates1) return;
                MessageBox.Show($"No matching nodes with keys: [{gps1.GetKeys()}].");
            } finally {
                UpdateResultsTableAndCounter();
            }

            try {
                if (this.comboBox2.SelectedIndex == 0) {
                    this.resultList.AddRange(this.parcelaTree.FindNodes(gps2));
                } else if (this.comboBox2.SelectedIndex == 1) {
                    this.resultList.AddRange(this.nehnutelnostTree.FindNodes(gps2));
                } else {
                    this.resultList.AddRange(this.itemTree.FindNodes(gps2));
                }
            } catch (NullReferenceException) {
                if (noMessageBox || !validCoordinates2) return;
                MessageBox.Show($"No matching nodes with keys: [{gps2.GetKeys()}].");
            } finally {
                UpdateResultsTableAndCounter();
            }
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
            position1 = ParseGPS(this.textBox1.Text, this.textBox2.Text);

            if (string.IsNullOrEmpty(this.textBox3.Text) || string.IsNullOrEmpty(this.textBox4.Text)) {
                ShowMessageBox("Insufficient data provided. Fill up the whole form.", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            position2 = ParseGPS(this.textBox3.Text, this.textBox4.Text);

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

            UpdateResultsTableAndCounter();
        }

        private void EditItem(bool confirm) {
            if (this.chosenItem == null) return;

            if (!confirm) {
                if (this.chosenItem is Parcela p) {
                    this.textBox1.Text = p.Pozicia.X.ToString();
                    this.textBox2.Text = p.Pozicia.Y.ToString();
                    this.comboBox1.SelectedIndex = 0;
                    this.textBox5.Text = p.CisParcely.ToString();
                    this.textBox6.Text = p.Popis;
                } else if (this.chosenItem is Nehnutelnost n) {
                    this.textBox1.Text = n.Pozicia.X.ToString();
                    this.textBox2.Text = n.Pozicia.Y.ToString();
                    this.comboBox1.SelectedIndex = 1;
                    this.textBox5.Text = n.SupCislo.ToString();
                    this.textBox6.Text = n.Popis;
                }

                this.textBox3.Text = "";
                this.textBox4.Text = "";
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
                position = ParseGPS(this.textBox1.Text, this.textBox2.Text);

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
                }

                this.textBox3.Enabled = true;
                this.textBox4.Enabled = true;
                this.comboBox1.Enabled = false;
            }
        }

        private void RemoveItem() {
            if (this.chosenItem == null) return;

            if (this.chosenItem is Parcela p) {
                this.textBox1.Text = p.Pozicia.X.ToString();
                this.textBox2.Text = p.Pozicia.Y.ToString();
                this.comboBox1.SelectedIndex = 0;
                this.textBox5.Text = p.CisParcely.ToString();
                this.textBox6.Text = p.Popis;
            } else if (this.chosenItem is Nehnutelnost n) {
                this.textBox1.Text = n.Pozicia.X.ToString();
                this.textBox2.Text = n.Pozicia.Y.ToString();
                this.comboBox1.SelectedIndex = 1;
                this.textBox5.Text = n.SupCislo.ToString();
                this.textBox6.Text = n.Popis;
            }

            this.textBox3.Text = "";
            this.textBox4.Text = "";
            this.textBox3.Enabled = false;
            this.textBox4.Enabled = false;
            this.comboBox1.Enabled = false;

            DialogResult result = MessageBox.Show("Are you sure you want to remove this item?", "Confirm Remove Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes) {
                if (this.chosenItem is Parcela parcela) {
                    this.parcelaTree.DeleteNode(ref parcela, parcela.Pozicia);
                    this.itemTree.DeleteNode(ref this.chosenItem, parcela.Pozicia);
                } else if (this.chosenItem is Nehnutelnost nehnutelnost) {
                    this.nehnutelnostTree.DeleteNode(ref nehnutelnost, nehnutelnost.Pozicia);
                    this.itemTree.DeleteNode(ref this.chosenItem, nehnutelnost.Pozicia);
                }

                UpdateResultsTableAndCounter();
            }

            this.textBox3.Enabled = true;
            this.textBox4.Enabled = true;
            this.comboBox1.Enabled = true;
        }

        private void ClearStructures() {
            this.parcelaTree.Clear();
            this.nehnutelnostTree.Clear();
            this.itemTree.Clear();

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
                        keysData = ParseGPS(coordinates[1].Replace('.', ','), coordinates[2].Replace('.', ','));
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

            UpdateResultsTableAndCounter();

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

            List<GPS> gpsList = new List<GPS>();

            int number;
            string description;
            double x, y;
            GPS position1, position2;

            for (int i = 0; i < parcelaCount; i++) {
                number = this.random.Next();
                description = GenerateRandomString(10);

                x = Math.Round(this.random.NextDouble() * 50, 0);
                y = Math.Round(this.random.NextDouble() * 50, 0);
                position1 = new GPS(x, y);

                x = Math.Round(this.random.NextDouble() * 50, 0);
                y = Math.Round(this.random.NextDouble() * 50, 0);
                position2 = new GPS(x, y);

                var parcela1 = new Parcela(number, description, position1);
                var parcela2 = new Parcela(number, description, position2);
                var item1 = parcela1 as Item;
                var item2 = parcela2 as Item;

                this.parcelaTree.InsertNode(ref parcela1, position1);
                this.parcelaTree.InsertNode(ref parcela2, position2);
                this.itemTree.InsertNode(ref item1, position1);
                this.itemTree.InsertNode(ref item2, position2);

                this.idList.Add(parcela1.Id);
                this.idList.Add(parcela2.Id);
                gpsList.Add(position1);
                gpsList.Add(position2);
            }

            for (int j = 0; j < nehnutelnostCount; j++) {
                number = this.random.Next();
                description = GenerateRandomString(10);

                x = Math.Round(this.random.NextDouble() * 50, 0);
                y = Math.Round(this.random.NextDouble() * 50, 0);
                position1 = new GPS(x, y);

                x = Math.Round(this.random.NextDouble() * 50, 0);
                y = Math.Round(this.random.NextDouble() * 50, 0);
                position2 = new GPS(x, y);

                if (this.random.NextDouble() < intersectionProb) {
                    position1 = gpsList[this.random.Next(gpsList.Count)];
                    List<GPS> filteredList = gpsList.Where(gps => gps != position1).ToList();
                    position2 = filteredList[this.random.Next(filteredList.Count)];
                }

                var nehnutelnost1 = new Nehnutelnost(number, description, position1);
                var nehnutelnost2 = new Nehnutelnost(number, description, position2);
                var item1 = nehnutelnost1 as Item;
                var item2 = nehnutelnost2 as Item;

                this.nehnutelnostTree.InsertNode(ref nehnutelnost1, position1);
                this.nehnutelnostTree.InsertNode(ref nehnutelnost2, position2);
                this.itemTree.InsertNode(ref item1, position1);
                this.itemTree.InsertNode(ref item2, position2);

                this.idList.Add(nehnutelnost1.Id);
                this.idList.Add(nehnutelnost2.Id);
            }

            UpdateResultsTableAndCounter();
        }

        private void UpdateResultsTableAndCounter() {
            this.dataGridView.Rows.Clear();

            if (this.dataGridView.Columns.Count == 0) {
                this.dataGridView.Columns.Add("Type", "Type");
                this.dataGridView.Columns.Add("Position", "Position");
                this.dataGridView.Columns.Add("Number", "Number");
                this.dataGridView.Columns.Add("Description", "Description");
                this.dataGridView.Columns.Add("Edit", "Edit");
                this.dataGridView.Columns.Add("Remove", "Remove");
            }

            int count = 0;

            foreach (Item item in this.resultList) {
                if (this.comboBox2.SelectedIndex == 0) {
                    if (item is Parcela p) {
                        this.dataGridView.Rows.Add("<e>", "<r>", "Parcela", $"{p.Pozicia.X}°, {p.Pozicia.Y}°", p.CisParcely, p.Popis);
                        count++;
                    }
                } else if (this.comboBox2.SelectedIndex == 1) {
                    if (item is Nehnutelnost n) {
                        this.dataGridView.Rows.Add("<e>", "<r>", "Nehnutelnost", $"{n.Pozicia.X}°, {n.Pozicia.Y}°", n.SupCislo, n.Popis);
                        count++;
                    }
                } else {
                    if (item is Parcela p) {
                        this.dataGridView.Rows.Add("<e>", "<r>", "Parcela", $"{p.Pozicia.X}°, {p.Pozicia.Y}°", p.CisParcely, p.Popis);
                        count++;
                    } else if (item is Nehnutelnost n) {
                        this.dataGridView.Rows.Add("<e>", "<r>", "Nehnutelnost", $"{n.Pozicia.X}°, {n.Pozicia.Y}°", n.SupCislo, n.Popis);
                        count++;
                    }
                }
            }

            if (this.comboBox2.SelectedIndex == 0) {
                this.label11.Text = $"Counter: {count} / {this.parcelaTree.DataSize}";
            } else if (this.comboBox2.SelectedIndex == 1) {
                this.label11.Text = $"Counter: {count} / {this.nehnutelnostTree.DataSize}";
            } else {
                this.label11.Text = $"Counter: {count} / {this.itemTree.DataSize}";
            }
        }

        private GPS ParseGPS(string latitude, string longitude) {
            double x = Double.MaxValue, y = Double.MaxValue;
            double.TryParse(latitude, out x);
            double.TryParse(longitude, out y);
            return new GPS(x, y);
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
