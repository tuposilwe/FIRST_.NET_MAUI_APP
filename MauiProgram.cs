using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyMaui.Data;
using MyMaui.Models;
using MyMaui.ViewModel;

namespace MyMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "items.db");

            // Use DbContextFactory so singletons/transients can create contexts safely.
            builder.Services.AddDbContextFactory<DataContext>(options =>
            {
                options.UseSqlite($"Data Source={dbPath}");
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);

            // Register pages & viewmodels as transient (or scoped) — avoid singleton for things that depend on DbContexts
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainViewModel>();

            builder.Services.AddTransient<DetailPage>();
            builder.Services.AddTransient<DetailViewModel>();

            var app = builder.Build();

            // Seed DB using the factory
            using (var scope = app.Services.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<DataContext>>();
                using var context = factory.CreateDbContext();
                context.Database.EnsureCreated();

                if (!context.Item.Any())
                {
                    context.Item.AddRange(
                        new Item { Name = "MIT" },
                        new Item { Name = "Stanford" },
                        new Item { Name = "Berkeley" }
                    );
                    context.SaveChanges();
                }
            }

            return app;
        }
    }
}
