using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.Windows.Dialogs
{
    public partial class ShowNotificationViewModel : ObservableObject
    {
        // Properties

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private string _buttonText = "Close";

        // Constructors

        public ShowNotificationViewModel(WarningLevel level, string title, string description, string buttonText) {
            switch (level) {
                case WarningLevel.Info: _title = $"Info: "; break;
                case WarningLevel.Warning: _title = $"Warning: "; break;
                case WarningLevel.Error: _title = $"Error: "; break;
            }
            
            _title += title;
            _description = description;
            _buttonText = buttonText;
        }
    }

    public enum WarningLevel {
        Info,
        Warning,
        Error
    }
}
