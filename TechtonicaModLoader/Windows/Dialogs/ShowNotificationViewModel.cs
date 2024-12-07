using CommunityToolkit.Mvvm.ComponentModel;
using TechtonicaModLoader.Resources;

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
        private string _buttonText;

        // Constructors

        public ShowNotificationViewModel(WarningLevel level, string title, string description, string buttonText) {
            _title = WarningLevelDisplay.Strings[level] + title;
            _description = description;
            _buttonText = string.IsNullOrEmpty(buttonText) ? StringResources.ButtonTextClose : buttonText;
        }
    }

    public enum WarningLevel {
        Info,
        Warning,
        Error
    }
}
