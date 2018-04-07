using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Client;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;
using ConnectionState = Microsoft.AspNet.SignalR.Client.ConnectionState;
using Entry = Microcharts.Entry;

namespace XamarinSignalRExample
{
    public class SignalRClient
    {
        #region Properties
        protected IHubProxy _hub;
        protected HubConnection _connection;
        public event EventHandler<List<Entry>> ValueChanged;
        private List<Entry> _entries;
        public delegate void MessageReceived(string username, string message);
        private const string url = "http://localhost:53681";
        private const string _nameHub = "MicrochartHub";
        #endregion

        #region Constructor
        public SignalRClient()
        {
            Debug.WriteLine("SignalR Initialized...");
            InitializeSignalR().ContinueWith(task =>
            {
                Debug.WriteLine("SignalR Started...");
            });
        }
        #endregion

        #region Methods
        public async Task InitializeSignalR()
        {
            _connection = new HubConnection(url);
            await _connection.Start();
            _hub = _connection.CreateHubProxy(_nameHub);

            if (_connection.State == ConnectionState.Connected)
            {
                _hub.On<List<Entry>>("UpdateMicrochart",
                    signalValue => Device.BeginInvokeOnMainThread(() => UpdateSignalMicrocharts(signalValue)));
            }
        }

        private void UpdateSignalMicrocharts(List<Entry> signalValue)
        {
            _entries = new List<Entry>();
            foreach (var item in signalValue)
            {
                _entries.Add(new Entry(_entries.Count)
                {
                    Color = item.Color,
                    Label = item.Label,
                    ValueLabel = item.ValueLabel
                });
            }
        }

        public Task Start()
        {
            return _connection.Start();
        }

        public bool IsConnectedOrConnecting
        {
            get
            {
                return _connection.State != ConnectionState.Disconnected;
            }
        }

        public ConnectionState ConnectionState { get { return _connection.State; } } 
        #endregion

    }
}

