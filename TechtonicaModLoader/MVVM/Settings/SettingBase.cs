using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.MVVM.Settings
{
    public abstract class SettingBase
    {
        // Members
        private string _name;
        private string _description;
        private string _category;

        // Properties

        [JsonIgnore] public string Name => _name;
        [JsonIgnore] public string Description => _description;
        [JsonIgnore] public string Category => _category;

        // Events

        public event Action SettingUpdated = delegate { }; // ToDo: test this

        // Constructors

        public SettingBase(string name, string description, string category) {
            _name = name;
            _description = description;
            _category = category;
        }

        // Public Functions

        public abstract void RestoreDefault();

        // Protected Functions

        protected void FireSettingUpdated() {
            SettingUpdated?.Invoke();
        }
    }
}
