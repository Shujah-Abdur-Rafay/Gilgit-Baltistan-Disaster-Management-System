namespace GBDMS
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Add global exception handler to prevent debugger attachment
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            MainPage = new MainPage();
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                // Log the exception instead of triggering debugger
                System.Diagnostics.Debug.WriteLine($"Unhandled exception: {ex.Message}");
                Console.WriteLine($"Unhandled exception: {ex.Message}");
            }
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            // Log the exception and mark it as observed to prevent debugger attachment
            System.Diagnostics.Debug.WriteLine($"Unobserved task exception: {e.Exception.Message}");
            Console.WriteLine($"Unobserved task exception: {e.Exception.Message}");
            e.SetObserved();
        }
    }
}
