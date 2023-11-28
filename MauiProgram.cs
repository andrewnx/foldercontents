using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Hosting;
using CommunityToolkit.Maui;

namespace foldercontents
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                 .ConfigureFonts(fonts =>
                 {
                     fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                     fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                     // Add other fonts here as needed
                 });


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
