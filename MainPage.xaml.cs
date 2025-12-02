
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
                await DisplayAlertAsync("Info", msg, "OK");
            };
        }

        private void Entry_Completed(object? sender, EventArgs e)
        {
            if (BindingContext is MyMaui.ViewModel.MainViewModel vm)
            {
                // If using CommunityToolkit generated command:
                if (vm.AddCommand.CanExecute(null))
                    vm.AddCommand.Execute(null);

                // Or call the method directly if public:
                // vm.Add();

                ItemEntry.Focus();
            }
    }

    }
}
