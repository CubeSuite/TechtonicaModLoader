using System.Reflection;
using TechtonicaModLoader.Windows;
using TechtonicaModLoader.Windows.Dialogs;
using TechtonicaModLoader.Windows.Settings;

namespace TechtonicaModLoader.Services
{
    public class DialogService : IDialogService
    {
        // Members

        // Public Functions

        public bool GetStringFromUser(out string output, string title, string defaultInput) {
            if (ShowDialog<GetStringViewModel, GetString>([title, defaultInput], out object? result)) {
                output = result?.ToString() ?? "";
                return true;
            }

            output = "";
            return false;
        }

        public bool GetUserConfirmation(string title, string description) {
            return ShowDialog<GetConfirmationViewModel, GetConfirmation>([title, description], out _, 2, "Yes", "No");
        }

        public void ShowInfoMessage(string title, string description, string buttonText = "Close") {
            ShowDialog<ShowNotificationViewModel, ShowNotification>([WarningLevel.Info, title, description, buttonText], out _, 1, buttonText);
        }

        public void ShowWarningMessage(string title, string description, string buttonText = "Close") {
            ShowDialog<ShowNotificationViewModel, ShowNotification>([WarningLevel.Warning, title, description, buttonText], out _, 1, buttonText);
        }

        public void ShowErrorMessage(string title, string description, string buttonText = "Close") {
            ShowDialog<ShowNotificationViewModel, ShowNotification>([WarningLevel.Error, title, description, buttonText], out _, 1, buttonText);
        }

        public void OpenSettingsDialog() {
            SettingsWindow window = new SettingsWindow();
            window.DataContext = new SettingsWindowViewModel(window, this);
            window.ShowDialog();
        }

        // Private Functions

        private bool ShowDialog<TViewModel, TView>(object[] args, out object? result, int numButtons = 2, string leftButtonText = "Confirm", string rightButtonText = "Cancel") {
            GetUserInputWindow dialog = new GetUserInputWindow() {
                ButtonCount = numButtons,
                LeftButtonText = leftButtonText,
                RightButtonText = rightButtonText
            };

            dialog.inputBorder.Content = Activator.CreateInstance(typeof(TView));
            object? viewModel = Activator.CreateInstance(typeof(TViewModel), args);
            dialog.DataContext = viewModel;

            bool? dialogResult = dialog.ShowDialog();
            if (dialogResult == true) {
                PropertyInfo? inputInfo = typeof(TViewModel).GetProperty("Input");
                if (viewModel is TViewModel vm && inputInfo != null) {
                    result = inputInfo.GetValue(vm);
                }
                else {
                    result = null;
                }

                return true;
            }

            result = null;
            return false;
        }
    }
}
