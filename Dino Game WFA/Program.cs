using Dino_Game_WFA.Resource;

namespace Dino_Game_WFA
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            ResourceHandler.LoadDefaultResources();

            ApplicationConfiguration.Initialize();
            Application.Run(new MainWindow());
        }
    }
}