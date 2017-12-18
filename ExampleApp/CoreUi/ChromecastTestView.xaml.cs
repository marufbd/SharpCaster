using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamvvm;

namespace CoreUi
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChromecastTestView : ContentPage, IBasePage<ChromecastTestViewModel>
	{
		public ChromecastTestView ()
		{
			InitializeComponent ();
		}
	}
}