using GeoGUI.Classes.Prototype;
using System;

namespace GeoGUI.Classes {
    public abstract class Item : IPrototype {
        private string id = Guid.NewGuid().ToString();

        public bool EqualsByID(Item other) => this.id == other.id;

        public abstract void PrintInfo();

        public abstract string GetInfo();

        public abstract Item Clone();

        IPrototype IPrototype.Clone() => this.Clone();

        public string Id { get => id; set => id = value; }
    }
}
