using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace MyMaui.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        IConnectivity connectivity;

        public MainViewModel(IConnectivity connectivity)
        {
            this.connectivity = connectivity;
        }

        public ObservableCollection<string> Items { get; } = new ObservableCollection<string>();

        [ObservableProperty]
        private string? text;

        // Automatically called whenever Text changes
        partial void OnTextChanged(string? oldValue, string? newValue)
        {
            AddCommand.NotifyCanExecuteChanged();
        }

        private bool CanAdd() => !string.IsNullOrWhiteSpace(Text);

        [RelayCommand(CanExecute = nameof(CanAdd))]
        private async Task Add()
        {
            //if (string.IsNullOrWhiteSpace(Text))
            //    return;

            if (connectivity.NetworkAccess != NetworkAccess.Internet) {
                await Shell.Current.DisplayAlertAsync("Uh oh!","No Internet","OK");
                return;
            }

            Items.Add(Text!.Trim());

            // Show toast after adding
            ShowToast?.Invoke($"{Text} Added");

            Text = string.Empty;
        }


        [RelayCommand]
        private void Delete(string s)
        {
            if (Items.Contains(s))
            {
                Items.Remove(s);
            }
        }



        [RelayCommand]
        private async Task Tap(string s)
        {
            await Shell.Current.GoToAsync($"{nameof(DetailPage)}?id={s}");
        }


        // Event to notify the view to show a toast
        public event Action<string>? ShowToast;
    }
}
