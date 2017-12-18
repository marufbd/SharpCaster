using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sharpcaster;
using Sharpcaster.Core.Exceptions;
using Sharpcaster.Core.Interfaces;
using Sharpcaster.Core.Models;
using Sharpcaster.Core.Models.Media;

namespace CoreUi
{
    public sealed class Chromecast
    {
        private Chromecast()
        {
        }

        public static Chromecast Instance { get; } = new Chromecast();

        private DateTime _lastSearched=DateTime.MinValue;
        private ChromecastClient _client=new ChromecastClient();
        private string _appId;        
        private ChromecastReceiver _selectedDevice;
        private ICollection<ChromecastReceiver> _availableDevices;

        public async Task Discover(string appId)
        {
            _appId = appId;
            if (_availableDevices == null || _availableDevices?.Count == 0)
            {
                var devs = await AvailableDevices();
                if (devs != null && devs.Any())
                {
                    //_client?.Disconnect();
                    //_client = _client ?? new ChromecastClient();
                    //auto select device if only one exists
                    if (_availableDevices?.Count == 1)
                        _selectedDevice = _availableDevices.First();
                }
            }
            if (_selectedDevice != null)
            {
                //_client.Disconnect();
                //_client = new ChromecastClient();
                //var connStatus = await _client.ConnectChromecast(_selectedDevice);
            }
            //if(_client==null)
            //    _client=new ChromecastClient();
        }

        public async Task<IEnumerable<string>> AvailableDevices()
        {
            if(_availableDevices?.Count>0 && _lastSearched>DateTime.Now.AddMinutes(-5))
                return _availableDevices.Select(x => x.Name);

            IChromecastLocator locator = new Sharpcaster.Discovery.MdnsChromecastLocator();
            var devs = await locator.FindReceiversAsync();
            _availableDevices = devs.ToArray();
            if (!_availableDevices.Any())
                return null;

            _lastSearched=DateTime.Now;
            return _availableDevices.Select(x => x.Name);
        }
        

        public async Task LaunchReceiver(string deviceName, string appId)
        {
            await Discover(appId);
            if (_availableDevices != null && _availableDevices.Any(x => x.Name == deviceName))
            {
                _selectedDevice = _availableDevices.First(x => x.Name == deviceName);
                if(_client==null)
                    _client=new ChromecastClient();
                var connStatus = await _client.ConnectChromecast(_selectedDevice);
                var appLaunchStatus = await _client.LaunchApplicationAsync(appId);
            }                
        }

        public async Task StopReceiver()
        {
            if (IsConnected)
            {
                //await Connect();
                var sessionId = _client.GetChromecastStatus().Applications[0].SessionId;
                await _client.GetChannel<IReceiverChannel>().StopApplication(sessionId);
                _client.Disconnect();
                _client = null;
                //var sessionId = _client.GetChromecastStatus().Applications[0].SessionId;
                //if (_sessionId == sessionId)
                //{
                //    await _client.GetChannel<IReceiverChannel>().StopApplication(sessionId);
                //    await _client.DisconnectAsync();
                //}
            }
        }

        public async Task LoadMedia(string mediaUrl, string mimeType, string metaDataTitle, string metaDataSubtitle)
        {
            if (IsConnected)
            {
                var media = new Media
                {
                    ContentUrl = mediaUrl,
                    ContentType = mimeType,
                    StreamType = StreamType.Buffered,
                    Metadata = new MediaMetadata
                    {
                        Title = metaDataTitle,
                        SubTitle = metaDataSubtitle
                    }
                };

                var retryCount = 3;
                while (retryCount>0)
                {
                    try
                    {
                        await _client.GetChannel<IMediaChannel>().LoadAsync(media);
                        break;
                    }
                    catch (ReceiverDisconnectedException e)
                    {
                        retryCount--;
                    }
                }                              
            }
            else
            {
                throw new InvalidOperationException("Chromecast Not connected");
            }
        }

        public bool IsConnected
        {
            get
            {
                var status = _client?.GetChromecastStatus();
                return status?.Applications != null && status.Applications[0].AppId == _appId;
            }
        }
    }
}
