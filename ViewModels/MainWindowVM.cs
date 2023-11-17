using Microsoft.Extensions.Logging;
using SuperSimpleTcp;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;

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
            SendCommand = new RelayCommand(SendData, checkserverinactivestatus);
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

        private Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

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

        #region Selected Client
        private string _selectedClient;

        public string SelectedClient
        {
            get { return _selectedClient; }
            set { 
                _selectedClient = value; 
                OnPropertyChanged(nameof(SelectedClient));
            }
        }

        #endregion Selected Client

        #region Button Commands
        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand SendCommand { get; set; }
        #endregion Button Commands

        #region Button Command Methods
        private void StopServer(object obj)
        {
            if (Clients.Count > 0)
            {
                foreach (var item in Clients)
                {
                    TCPServer.DisconnectClient(item);
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

        private void SendData(object obj)
        {
            if(SelectedClient != null)
            {
                TCPServer.Send(SelectedClient, Encoding.ASCII.GetBytes(DataToSend));
            }
            else
            {
                foreach (var item in Clients)
                {
                    TCPServer.Send(item, Encoding.ASCII.GetBytes(DataToSend));
                }
            }
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
            var clientid = e.IpPort;
            var data = Encoding.ASCII.GetString(e.Data);
            DataReceived += $"{clientid}: {data}{Environment.NewLine}";
        }

        private void ClientDisconnected(object? sender, ConnectionEventArgs e)
        {
            dispatcher.Invoke(new Action(() =>
            {
                Clients.Remove(e.IpPort);
            }));
            
        }

        private void ClientConnected(object? sender, ConnectionEventArgs e)
        {
            dispatcher.Invoke(new Action(() =>
            {
                Clients.Add(e.IpPort);
            }));
            
        }
        #endregion Event Methods

        #region Data Received
        private string _dataReceived;

        public string DataReceived
        {
            get { return _dataReceived; }
            set { 
                _dataReceived = value; 
                OnPropertyChanged(nameof(DataReceived));
            }
        }

        #endregion Data Received

        #region Data to be sent
        private string _dataToSend;

        public string DataToSend
        {
            get { return _dataToSend; }
            set { 
                _dataToSend = value; 
                OnPropertyChanged(nameof(DataToSend));
            }
        }

        #endregion Data to be sent

    }
}
