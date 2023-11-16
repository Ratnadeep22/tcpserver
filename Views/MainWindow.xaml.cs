using Microsoft.Extensions.Logging;
using System.Windows;

namespace tcpserver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILogger<MainWindow> _logger;
        private readonly MainWindowVM _mainWindowVM;
        public MainWindow(ILogger<MainWindow> logger, MainWindowVM mainWindowVM)
        {
            _logger = logger;
            _mainWindowVM = mainWindowVM;

            InitializeComponent();

            this.DataContext = _mainWindowVM;
        }
    }
}
