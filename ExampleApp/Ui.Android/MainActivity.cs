using Acr.UserDialogs;
using Android.App;
using Android.OS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Ui.Android
{
    [Activity(Label = "ChromecastDemo", Theme = "@style/AppTheme", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            //SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            //Button button = FindViewById<Button>(Resource.Id.myButton);

            UserDialogs.Init(this);

            //button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };            
            Forms.Init(this, savedInstanceState);
            
            LoadApplication(new CoreUi.App());
        }
    }
}

