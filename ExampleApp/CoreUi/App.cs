using DLToolkit.Forms.Controls;
using Xamarin.Forms;
using Xamvvm;

namespace CoreUi
{
    public class App:Application
    {
        public App()
        {
            FlowListView.Init();

            var factory = new XamvvmFormsFactory(this);
            //factory.RegisterNavigationPage<MainViewModel>(() => this.GetPageAsNewInstance<MainViewModel>());
            XamvvmCore.SetCurrentFactory(factory);

            //MainPage =new NavigationPage(this.GetPageFromCache<MainViewModel>() as Page);
            MainPage =new NavigationPage(this.GetPageFromCache<ChromecastTestViewModel>() as Page);
        }
    }
}
