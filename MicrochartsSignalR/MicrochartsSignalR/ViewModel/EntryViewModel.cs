using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MicrochartsSignalR.Annotations;
using Microsoft.AspNet.SignalR.Client;
using SkiaSharp;
using Xamarin.Forms;
using Entry = Microcharts.Entry;

namespace MicrochartsSignalR.ViewModel
{
    public class EntryViewModel : INotifyPropertyChanged
    {

        #region Properties
        protected IHubProxy _hub;
        protected HubConnection _connection;
        public event EventHandler<List<Entry>> ValueChanged;
        private List<Entry> _entries;
        private bool _reconnect = true;
        private readonly int _interval = 5000;
        private string _color;
        private const string url = "http://localhost:53681";
        private const string _nameHub = "MicrochartHub";
        #endregion

        #region Constructor
        public EntryViewModel()
        {
            NotificationView();
        }
        #endregion

        #region Objects
        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;
                OnPropertyChanged("Color");
            }
        }

        private string _label;

        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                OnPropertyChanged("Label");
            }
        }

        private string _valueLabel;

        public string ValueLabel
        {
            get { return _valueLabel; }
            set
            {
                _valueLabel = value;
                OnPropertyChanged("ValueLabel");
            }
        }

        private string _textColor;

        public string TextColor
        {
            get { return _textColor; }
            set
            {
                _textColor = value;
                OnPropertyChanged("TextColor");
            }
        }

        #endregion

        #region Methods
        private void NotificationView()
        {
            while (_reconnect)
            {

                if (!_reconnect)
                {
                    Thread.Sleep(_interval);
                }


                ConnectAsync();

                if (_connection != null)
                {
                    if (_connection.State == ConnectionState.Connected)
                    {
                        _hub.On<List<Entry>>("UpdateMicrochart",
                            signalValue => Device.BeginInvokeOnMainThread(() => UpdateSignalMicrocharts(signalValue)));
                    }
                    else
                    {
                        _reconnect = true;
                    }
                }
                else
                {
                    _reconnect = true;
                }


                // Montar temporario teste
                _entries = new List<Entry>();
                _entries.Add(new Entry(_interval)
                {
                    ValueLabel = "1000",
                    Label = "XXXXXXX",
                    Color = SKColor.Parse("#2c3e50"),
                });

            }
        }

        private void UpdateSignalMicrocharts(List<Entry> signalValue)
        {
            _entries = new List<Entry>();
            foreach (var item in signalValue)
            {
                _color = item.Color.ToString();
                _label = item.Label;
                _valueLabel = item.ValueLabel;

            }
        }

        private async void ConnectAsync()
        {
            Task.Run(() => { StartConnection(); });
        }

        private void StartConnection()
        {
            while (true)
            {
                if (!_reconnect)
                {
                    Thread.Sleep(_interval);
                    continue;
                }

                _connection = new HubConnection(url);
                _hub = _connection.CreateHubProxy(_nameHub);

                try
                {
                    _connection.Start().ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            _reconnect = true;
                        }
                        else
                        {
                            _reconnect = false;
                        }
                    });

                }
                catch (Exception ex)
                {
                    _reconnect = true;
                    Thread.Sleep(_interval);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
