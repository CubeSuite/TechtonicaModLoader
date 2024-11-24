namespace TechtonicaModLoader.Services
{
    public interface IDialogService
    {
        bool GetStringFromUser(out string output, string title, string defaultInput);
        bool GetUserConfirmation(string title, string description);
        void OpenSettingsDialog();
        void ShowErrorMessage(string title, string description, string buttonText = "Close");
        void ShowInfoMessage(string title, string description, string buttonText = "Close");
        void ShowWarningMessage(string title, string description, string buttonText = "Close");
    }
}