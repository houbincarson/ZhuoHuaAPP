using System.Threading;

using Android.App;
using Android.OS;
using Android.Content;

namespace ZhuoHuaAPP
{
    [Activity(Label = "ÕÆÉÏ×¿»ª", Theme = "@style/Theme.Splash", Icon = "@drawable/icon", NoHistory = true, MainLauncher = true)]			
	public class Splash : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate(bundle);
			Thread.Sleep(2500); 
			ISharedPreferences MyPrivate = GetSharedPreferences("login",FileCreationMode.Private); 
			string Role= MyPrivate.GetString ("Role", "");
			string Zone= MyPrivate.GetString ("Zone", "");
			bool AutoLogin=MyPrivate.GetBoolean ("Autologin", false);
			try
			{
				if(AutoLogin==true && Role=="admin")
				{
               //     StartActivity(typeof(NaviMenuHome));
                 //  StartActivity(typeof(login));
				}
				else if(AutoLogin==true && Role=="User")
				{
				//	StartActivity(typeof(ExhibitionActivity2));
				}
				else{
                    StartActivity(typeof(login));
				}
			}
			catch
			{this.Finish ();
			}
		}
	}
}

