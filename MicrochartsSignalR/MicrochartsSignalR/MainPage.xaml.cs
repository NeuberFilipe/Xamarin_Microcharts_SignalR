using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Xamarin.Forms;
using XamarinSignalRExample;
using Entry = Microcharts.Entry;

namespace MicrochartsSignalR
{
    public partial class MainPage : ContentPage
    {
        #region Properties
        public SignalRClient SignalRClient = new SignalRClient();
        protected IHubProxy Hub;
        protected HubConnection connection;
        public event EventHandler<List<Entry>> ValueChanged;
        private List<Entry> entries;
        private bool _reconnect = true;
        private readonly int _interval = 5000;

        #endregion

        #region Methods
        private async Task ExecutaLoadCommand()
        {
            await SignalRClient.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                    DisplayAlert("Error", "An error occurred when trying to connect to SignalR: " + task.Exception.InnerExceptions[0].Message, "OK");
            });

            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                if (!SignalRClient.IsConnectedOrConnecting)
                    SignalRClient.Start();

                return true;
            });
        } 
        #endregion

        #region Constructor
        public MainPage()
        {
            InitializeComponent();
            //NotificationView();
            ExecutaLoadCommand();
        } 
        #endregion

        #region CodeTemp
        private void UpdateSignalMicrocharts(List<Entry> signalValue)
        {
            entries = new List<Entry>();
            foreach (var item in signalValue)
            {
                entries.Add(new Entry(entries.Count)
                {
                    Color = item.Color,
                    Label = item.Label,
                    ValueLabel = item.ValueLabel
                });
            }
        }
        //private void NotificationView()
        //{

        //        if (!_reconnect)
        //        {
        //            Thread.Sleep(5000);
        //        }
        //        ConnectAsync();
        //        if (_connection != null)
        //        {
        //            if (_connection.State == ConnectionState.Connected)
        //            {
        //                // Invoko o metododo do SignalR
        //                _hub.On<List<Entry>>("UpdateMicrochart",
        //                    signalValue => Device.BeginInvokeOnMainThread(() => UpdateSignalMicrocharts(signalValue)));
        //            }
        //            else
        //            {
        //                _reconnect = true;
        //            }
        //        }
        //        else
        //        {
        //            _reconnect = true;
        //        }


        //        entries = new List<Entry>();
        //        entries.Add(new Entry(1000)
        //        {
        //            ValueLabel = "1000",
        //            Label = "XXXXXXX",
        //            Color = SKColor.Parse("#2c3e50"),
        //        });

        //        Chart1.Chart = new RadialGaugeChart() { Entries = entries }; 

        //}

        //private async void ConnectAsync()
        //{
        //    Task.Run(() => { StartConnection(); });
        //}

        //private void StartConnection()
        //{
        //    while (true)
        //    {
        //        if (!_reconnect)
        //        {
        //            Thread.Sleep(_interval);
        //            continue;
        //        }

        //        _connection = new HubConnection("http://localhost:53681");
        //        _hub = _connection.CreateHubProxy("MicrochartHub");

        //        try
        //        {
        //            _connection.Start().ContinueWith(task =>
        //            {
        //                if (task.IsFaulted)
        //                {
        //                    _reconnect = true;
        //                }
        //                else
        //                {
        //                    _reconnect = false;
        //                }
        //            });

        //        }
        //        catch (Exception ex)
        //        {
        //            _reconnect = true;
        //            Thread.Sleep(_interval);
        //        }
        //    }
        //} 
        #endregion
    }
}
