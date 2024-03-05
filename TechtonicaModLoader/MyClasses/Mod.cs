using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.Modes
{
    public class Mod : INotifyPropertyChanged
    {
        // Members
        private int _id;
        public int id {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged("id");
            }
        }

        private string _name;
        public string name {
            get => _name;
            set {
                _name = value;
                OnPropertyChanged("name");
            }
        }

        // Constructors

        public Mod() { }
        public Mod fromJson(string json) {
            return JsonConvert.DeserializeObject<Mod>(json);
        }

        // Public Functions

        public string toJson(bool indented = true) {
            return JsonConvert.SerializeObject(this, indented ? Formatting.Indented : Formatting.None);
        }

        // Private Functions

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
