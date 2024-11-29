using System;

namespace GeoGUI.Classes.Prototype {
    public abstract class Item : IPrototype {
        private string id = Guid.NewGuid().ToString();

        public bool EqualsByID(Item other) => this.id == other.Id;

        public abstract void PrintInfo();

        public abstract string GetInfo();

        public abstract IPrototype Clone();

        public string Id { get => id; set => id = value; }
    }
}
