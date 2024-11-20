using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.Windows;
using TechtonicaModLoader.Windows.Dialogs;
using TechtonicaModLoader.Windows.Settings;

namespace TechtonicaModLoader.Services
{
    public class DialogService
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
            return ShowDialog<GetConfirmationViewModel>([title, description], out _, 2, "Yes", "No");
        }

        public void ShowInfoMessage(string title, string description, string buttonText = "Close") {
            ShowDialog<ShowNotificationViewModel>([WarningLevel.Info, title, description, buttonText], out _, 1, buttonText);
        }

        public void ShowWarningMessage(string title, string description, string buttonText = "Close") {
            ShowDialog<ShowNotificationViewModel>([WarningLevel.Warning, title, description, buttonText], out _, 1, buttonText);
        }

        public void ShowErrorMessage(string title, string description, string buttonText = "Close") {
            ShowDialog<ShowNotificationViewModel>([WarningLevel.Error, title, description, buttonText], out _, 1, buttonText);
        }

        public void OpenSettingsDialog() {
            SettingsWindow window = new SettingsWindow();
            window.DataContext = new SettingsWindowViewModel(window, this);
            window.ShowDialog();
        }

        // Private Functions

        private bool ShowDialog<TViewModel>(object[] args, out object? result, int numButtons = 2, string leftButtonText = "Confirm", string rightButtonText = "Cancel") {
            if (!viewModelToViewMap.ContainsKey(typeof(TViewModel))) {
                Log.Error($"No dialog exists for view model: {typeof(TViewModel)}");
                result = null;
                return false;
            }

            GetUserInputWindow dialog = new GetUserInputWindow() {
                ButtonCount = numButtons,
                LeftButtonText = leftButtonText,
                RightButtonText = rightButtonText
            };

            Type? dialogType = viewModelToViewMap[typeof(TViewModel)];
            dialog.inputBorder.Content = Activator.CreateInstance(dialogType);
            object? viewModel = Activator.CreateInstance(typeof(TViewModel), args);
            dialog.DataContext = viewModel;
            
            bool? dialogResult = dialog.ShowDialog();
            if(dialogResult == true) {
                PropertyInfo? inputInfo = typeof(TViewModel).GetProperty("Input");
                if(viewModel is TViewModel vm && inputInfo != null) {
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
