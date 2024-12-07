using System.Reflection;
using System.Windows;
using TechtonicaModLoader.Resources;
using TechtonicaModLoader.Windows;
using TechtonicaModLoader.Windows.Dialogs;

namespace TechtonicaModLoader.Services
{
    public interface IDialogService
    {
        public bool GetStringFromUser(out string output, string title, string defaultInput);
        public bool GetUserConfirmation(string title, string description);
        public void ShowInfoMessage(string title, string description, string buttonText = "");
        public void ShowWarningMessage(string title, string description, string buttonText = "");
        public void ShowErrorMessage(string title, string description, string buttonText = "");

        public void OpenSettingsDialog(IServiceProvider serviceProvider);
    }

    public class DialogService : IDialogService
    {
        // Members

        private static Dictionary<Type, Type> viewModelToViewMap = new Dictionary<Type, Type>() {
            {typeof(GetStringViewModel), typeof(GetString) },
            {typeof(GetConfirmationViewModel), typeof(GetConfirmation) },
            {typeof(ShowNotificationViewModel), typeof(ShowNotification) }
        };

        // Public Functions

        public bool GetStringFromUser(out string output, string title, string defaultInput) {
            if (ShowDialog<GetStringViewModel>([title, defaultInput], out object? result)) {
                output = result?.ToString() ?? "";
                return true;
            }

            output = "";
            return false;
        }

        public bool GetUserConfirmation(string title, string description) {
            return ShowDialog<GetConfirmationViewModel>([title, description], out _, 2, StringResources.ButtonTextYes, StringResources.ButtonTextNo);
        }

        public void ShowInfoMessage(string title, string description, string buttonText = "") {
            if (string.IsNullOrEmpty(buttonText)) buttonText = StringResources.ButtonTextClose;
            ShowDialog<ShowNotificationViewModel>([WarningLevel.Info, title, description, buttonText], out _, 1, buttonText);
        }

        public void ShowWarningMessage(string title, string description, string buttonText = "") {
            if (string.IsNullOrEmpty(buttonText)) buttonText = StringResources.ButtonTextClose;
            ShowDialog<ShowNotificationViewModel>([WarningLevel.Warning, title, description, buttonText], out _, 1, buttonText);
        }

        public void ShowErrorMessage(string title, string description, string buttonText = "") {
            if (string.IsNullOrEmpty(buttonText)) buttonText = StringResources.ButtonTextClose;
            ShowDialog<ShowNotificationViewModel>([WarningLevel.Error, title, description, buttonText], out _, 1, buttonText);
        }

        public void OpenSettingsDialog(IServiceProvider serviceProvider) {
            SettingsWindow window = new SettingsWindow(serviceProvider);
            window.ShowDialog();
        }

        // Private Functions

        private bool ShowDialog<TViewModel>(object[] args, out object? result, int numButtons = 2, string leftButtonText = "", string rightButtonText = "") {
            if (string.IsNullOrEmpty(leftButtonText)) leftButtonText = StringResources.ButtonTextConfirm;
            if (string.IsNullOrEmpty(rightButtonText)) rightButtonText = StringResources.ButtonTextCancel;
            if (!viewModelToViewMap.ContainsKey(typeof(TViewModel))) {
                throw new Exception($"No dialog exists for view model: {typeof(TViewModel)}");
            }

            object? viewModel = null;
            bool? dialogResult = null;

            Application.Current.Dispatcher.Invoke(delegate () {
                GetUserInputWindow dialog = new() {
                     ButtonCount = numButtons,
                     LeftButtonText = leftButtonText,
                     RightButtonText = rightButtonText
                };

                Type? dialogType = viewModelToViewMap[typeof(TViewModel)];
                dialog.inputBorder.Content = Activator.CreateInstance(dialogType);
                viewModel = Activator.CreateInstance(typeof(TViewModel), args);
                dialog.DataContext = viewModel;
                bool? dialogResult = dialog.ShowDialog();
            });
            
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
