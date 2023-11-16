using Microsoft.Extensions.Logging;
using SuperSimpleTcp;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace tcpserver
{
    public class MainWindowVM : BaseVM
    {
        #region Injected Fields
        private readonly ILogger<MainWindowVM> _logger;
        #endregion Injected Fields

        #region Constructor
        public MainWindowVM(ILogger<MainWindowVM> logger)
        {
            _logger = logger;

            StartCommand = new RelayCommand(StartServer, checkserveractivestatus);
            StopCommand = new RelayCommand(StopServer, checkserverinactivestatus);
        }
        #endregion Constructor

        #region TCPServer IP & Port
        private string _serverIpPort;

        public string ServerIpPort
        {
            get {
                if(_serverIpPort == null)
                {
                    _serverIpPort = "172.0.0.1:9090";
                }
                return _serverIpPort; 
            }
            set { 
                _serverIpPort = value; 
                OnPropertyChanged(nameof(ServerIpPort));
            }
        }

        #endregion TCPServer IP & Port

        #region TCP Server
        private SimpleTcpServer _tcpServer;

        public SimpleTcpServer TCPServer
        {
            get { return _tcpServer; }
            set { 
                _tcpServer = value;
                OnPropertyChanged(nameof(TCPServer));
            }
        }
        #endregion TCP Server

        #region Client List
        private ObservableCollection<string> _clients;

        public ObservableCollection<string> Clients
        {
            get { 
                if(_clients == null)
                {
                    _clients = new ObservableCollection<string>();
                }
                return _clients; 
            }
            set { 
                _clients = value; 
                OnPropertyChanged(nameof(Clients));
            }
        }

        #endregion Client List

        #region Button Commands
        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }
        #endregion Button Commands

        #region Button Command Methods
        private void StopServer(object obj)
        {
            if (Clients.Count > 0)
            {
                foreach (var item in Clients)
                {
                    TCPServer.DisconnectClient(item);
                    Clients.Remove(item);
                }
            }

            TCPServer.Stop();
        }

        private void StartServer(object obj)
        {
            SimpleTcpServer simpleTcpServer = new SimpleTcpServer(ServerIpPort);
            simpleTcpServer.Events.DataReceived += DataReceivedFromClient;
            simpleTcpServer.Events.ClientDisconnected += ClientDisconnected;
            simpleTcpServer.Events.ClientConnected += ClientConnected;
            simpleTcpServer.Start();
            TCPServer = simpleTcpServer;
        }
        #endregion Button Command Methods

        #region RelayCommand Functions
        private bool checkserverinactivestatus(object arg)
        {
            if (TCPServer == null)
            {
                return false;
            }
            else if (!TCPServer.IsListening)
            {
                return false;
            }
            return true;
        }

        private bool checkserveractivestatus(object arg)
        {
            if (TCPServer == null || !TCPServer.IsListening)
            {
                return true;
            }
            return false;
        }
        #endregion RelayCommand Functions

        #region Event Methods
        private void DataReceivedFromClient(object? sender, DataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ClientDisconnected(object? sender, ConnectionEventArgs e)
        {
            Clients.Remove(e.IpPort);
        }

        private void ClientConnected(object? sender, ConnectionEventArgs e)
        {
            Clients.Add(e.IpPort);
        }
        #endregion Event Methods
    }
}
