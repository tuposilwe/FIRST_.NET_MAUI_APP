using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MyMaui.Data;
using MyMaui.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core; // for SnackbarOptions, SnackbarDuration


namespace MyMaui.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IConnectivity connectivity;
        private readonly IDbContextFactory<DataContext> dbFactory;

        public MainViewModel(
            IConnectivity connectivity,
            IDbContextFactory<DataContext> dbFactory)
        {
            this.connectivity = connectivity;
            this.dbFactory = dbFactory;
        }

        public ObservableCollection<Item> Items { get; set; } 
            = new ObservableCollection<Item>();

        // -----------------------------
        // LoadItems
        // -----------------------------
        [RelayCommand]
        public async Task LoadItems()
        {
            try
            {
                Items.Clear();

                using var context = dbFactory.CreateDbContext();

                var list = await context.Item.AsNoTracking().ToListAsync();

                foreach (var item in list)
                    Items.Add(item);
            }
            catch (Exception ex)
            {
                ShowToast?.Invoke("LoadItems ERROR: " + ex.Message);
            }
        }

        // -----------------------------
        // Add Item
        // -----------------------------
        [ObservableProperty]
        private string? text;

        partial void OnTextChanged(string? oldValue, string? newValue)
        {
            AddCommand.NotifyCanExecuteChanged();
        }

        private bool CanAdd() => !string.IsNullOrWhiteSpace(Text);

        [RelayCommand(CanExecute = nameof(CanAdd))]
        private async Task Add()
        {
            try
            {

                //if (string.IsNullOrWhiteSpace(Text))
                //    return;
                //if (connectivity.NetworkAccess != NetworkAccess.Internet)
                //{
                //    await Shell.Current.DisplayAlertAsync("Uh oh!", "No Internet", "OK");
                //    return;
                //}

                using var context = dbFactory.CreateDbContext();

                var newItem = new Item { Name = Text!.Trim() };
                context.Item.Add(newItem);
                await context.SaveChangesAsync();

                Items.Add(newItem);
                ShowToast?.Invoke($"{newItem.Name} Added");

                Text = string.Empty;
            }
            catch (Exception ex)
            {
                ShowToast?.Invoke("Add ERROR: " + ex.Message);
            }
        }

        // -----------------------------
        // Delete Item
        // -----------------------------
        [RelayCommand]
        private async Task Delete(Item item)
        {
            try
            {
                // Remove item from UI first (optimistic)
                Items.Remove(item);

                using var context = dbFactory.CreateDbContext();
                context.Item.Remove(item);
                await context.SaveChangesAsync();

                // Show Snackbar with Undo
                var snackbar = Snackbar.Make(
                    message: $"\"{item.Name}\" deleted",
                    action: async () =>
                    {
                        // Undo action: restore item
                        using var undoContext = dbFactory.CreateDbContext();
                        undoContext.Item.Add(item);
                        await undoContext.SaveChangesAsync();

                        //Items.Add(item);
                        int index = Items.Count; // or store previous index before removing
                        Items.Insert(index, item);
                        // restore in UI
                        ShowToast?.Invoke($"\"{item.Name}\" restored");
                    },
                    duration: TimeSpan.FromSeconds(5)
                );

                await snackbar.Show();
            }
            catch (Exception ex)
            {
                ShowToast?.Invoke("Delete ERROR: " + ex.Message);
            }
        }


        // -----------------------------
        // Edit / Update Item
        // -----------------------------
        [RelayCommand]
        private async Task Edit(Item item)
        {
            try
            {
                string newName = await App.Current.MainPage.DisplayPromptAsync(
                    "Edit Task",
                    "Enter new name:",
                    initialValue: item.Name
                );

                if (string.IsNullOrWhiteSpace(newName))
                    return;

                using var context = dbFactory.CreateDbContext();

                item.Name = newName.Trim();
                context.Item.Update(item);
                await context.SaveChangesAsync();

                // Refresh UI
                var index = Items.IndexOf(item);
                Items.RemoveAt(index);
                Items.Insert(index, item);

                ShowToast?.Invoke($"{item.Name} Updated");
            }
            catch (Exception ex)
            {
                ShowToast?.Invoke("Edit ERROR: " + ex.Message);
            }
        }

        // -----------------------------
        // Toast Event
        // -----------------------------
        public event Action<string>? ShowToast;
    }
}
