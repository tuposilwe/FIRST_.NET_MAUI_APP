using MyMaui.ViewModel;

namespace MyMaui
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;

            vm.ShowToast += async (msg) =>
            {
                // use MainThread to ensure UI call on UI thread
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlertAsync("Info", msg, "OK");
                });
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is MainViewModel vm)
            {
                if (vm.LoadItemsCommand.CanExecute(null))
                    vm.LoadItemsCommand.Execute(null);
            }
        }

        private void Entry_Completed(object? sender, EventArgs e)
        {
            if (BindingContext is MainViewModel vm)
            {
                if (vm.AddCommand.CanExecute(null))
                    vm.AddCommand.Execute(null);

                ItemEntry?.Focus();
            }
        }
    }
}
