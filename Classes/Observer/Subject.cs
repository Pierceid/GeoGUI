using GeoGUI.Classes.Prototype;
using System.Collections.Generic;

namespace GeoGUI.Classes.Observer {
    public class Subject : ISubject {
        private List<IObserver> observers = new List<IObserver>();
        public Item ChosenItem { get; private set; } = null;
        public List<Item> ResultList { get; private set; } = new List<Item>();

        public void SetChosenItem(Item item) {
            this.ChosenItem = item;
            Notify();
        }

        public void SetResultList(List<Item> resultList) {
            this.ResultList = resultList;
            Notify();
        }

        public void Attach(IObserver observer) {
            this.observers.Add(observer);
        }

        public void Detach(IObserver observer) {
            this.observers.Remove(observer);
        }

        public void Notify() {
            foreach (var observer in this.observers) {
                observer.Update();
            }
        }
    }
}
