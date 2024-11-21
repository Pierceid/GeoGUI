namespace GeoGUI.Classes {
    using System.Collections.Generic;

    public class SubjectList : ISubject {
        private List<IObserver> observers = new List<IObserver>();
        public List<Item> ResultList { get; private set; } = new List<Item>();

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
            foreach (var observer in observers) {
                observer.Update();
            }
        }
    }
}
