using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.MVVM.ViewModels.Settings
{
    public abstract partial class SettingBase : ObservableObject
    {
        // Properties

        public string Name { get; }
        public string Description { get; }
        public string Category { get; }

        // Constructors

        public SettingBase(string name, string description, string category) {
            Name = name;
            Description = description;
            Category = category;

            if (!Description.EndsWith('.')) Description += '.';
        }
    }
}
