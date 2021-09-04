namespace Test.ViewModels
{
    public class PopupViewModel : ViewModelBase
    {
        private string message;

        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
            }
        }
    }
}
