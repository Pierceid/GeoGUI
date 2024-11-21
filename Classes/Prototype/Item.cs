using System;

namespace GeoGUI.Classes {
    public abstract class Item {
        private string id = Guid.NewGuid().ToString();

        public bool EqualsByID(Item other) => this.id == other.id;

        public abstract void PrintInfo();

        public abstract string GetInfo();

        public abstract Item Clone();

        public string Id { get => id; set => id = value; }
    }
}
