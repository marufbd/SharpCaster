using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Xamvvm;

namespace CoreUi
{
    public class ChromecastTestViewModel : BasePageModel
    {

        private readonly string _appId = "1FB91ED9"; // BDCast custom streaming player
        //private readonly string _appId = "CC1AD845"; //default android receiver app
        //private readonly string _appId = "B3419EF5"; //sharpcaster demo
        

        public void Init()
        {
            Status = Chromecast.Instance.IsConnected ? "Connected" : "Disconnected";
        }

        public string Status
        {
            get => GetField<string>();
            set => SetField(value);
        }

        private async Task StartCast()
        {
            Status = "Searching...";
            var devs = await Chromecast.Instance.AvailableDevices();
            if (devs != null)
            {
                Status = "Found";
                var selectedDeviceName =
                    await UserDialogs.Instance.ActionSheetAsync("Cast To", "Cancel", null, null, devs.ToArray());
                if (selectedDeviceName != "Cancel")
                {
                    //UserDialogs.Instance.ShowLoading("launching player...");                    
                    Status = "Launching app...";
                    try
                    {
                        await Chromecast.Instance.LaunchReceiver(selectedDeviceName, _appId);
                        Status = $"Connected to: {selectedDeviceName}";
                    }
                    catch (Exception e)
                    {
                        Status = $"Error: {e.Message}";
                        UserDialogs.Instance.Alert($"Error Launching: {e.GetType().FullName}, Message: {e.Message}, Trace: {e.StackTrace}");
                    }

                    //Status = "Stopping..";
                    //await Chromecast.Instance.StopReceiver();
                    //Status = "Stopped";
                }
                //UserDialogs.Instance.HideLoading();
            }
            else
            {
                Status = "No devices found";
            }
        }

        private async Task LoadMedia()
        {
            if (!Chromecast.Instance.IsConnected)
            {
                UserDialogs.Instance.Alert("Not connected, use Cast button first");
                return;
            }                

            try
            {
                Status = "Loading media onto chromecast...";
                await Chromecast.Instance.LoadMedia(
                    "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4",
                    "video/mp4",
                    "Designing For Google Cast",
                    "ondemand"
                );
                Status = "Playing media";
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert($"Error Loading Media: {e.GetType().FullName}, Message: {e.Message}, Trace: {e.StackTrace}");
            }
        }

        public ICommand CastTo => BaseCommand.FromTask(_ => StartCast());        

        public ICommand LoadMediaCommand => BaseCommand.FromTask(_ => LoadMedia());

        public ICommand Stop => new BaseCommand(async (arg) =>
        {
            Status = "Stopping...";
            try
            {
                await Chromecast.Instance.StopReceiver();
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert($"Error Stopping: {e.GetType().FullName}, Message: {e.Message}, Trace: {e.StackTrace}");
            }

            Init();
        });

        public ICommand CastAndLoad => new BaseCommand(async (arg) =>
        {
            await StartCast();
            if(Chromecast.Instance.IsConnected)
                await LoadMedia();
        });

        public ICommand Discover=>new BaseCommand(async(arg) =>
        {
            Status = "Connecting (will search if required)....";
            try
            {                
                await Chromecast.Instance.Discover(_appId);
                Init();
            }
            catch (Exception e)
            {
                Status = e.Message;
                UserDialogs.Instance.Alert($"Error Discovering: {e.GetType().FullName}, Message: {e.Message}, Trace: {e.StackTrace}");                   
            }                    
        });
    }
}
