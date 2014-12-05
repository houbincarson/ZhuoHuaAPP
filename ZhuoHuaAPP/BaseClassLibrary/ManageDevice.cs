using Android.App;
using Android.Content;
using Android.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ZhuoHuaAPP
{
    class ManageDevice
    {
        //if no open gprs or wifi,then force open
		public bool openGPS(Context context)
        {
            bool open = false;
            Intent GPSIntent = new Intent();
            GPSIntent.SetClassName("com.android.settings",
                "com.android.settings.widget.SettingsAppWidgetProvider");
            GPSIntent.AddCategory("android.intent.category.ALTERNATIVE");
            GPSIntent.SetData(Android.Net.Uri.Parse("custom:3"));
            try
            {
                PendingIntent.GetBroadcast(context, 0, GPSIntent, 0).Send();
                open = true;
            }
            catch (Android.App.PendingIntent.CanceledException e)
            {
                e.PrintStackTrace();
                MessageBox.Show(context, "error", e.Message);
            }
            return open;
        }
        public static Boolean isConnectingToInternet(Context context)
        {
            ConnectivityManager connectivity = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            if (connectivity != null)
            {
                NetworkInfo[] info = connectivity.GetAllNetworkInfo();
                if (info != null)
                    for (int i = 0; i < info.Length; i++)
                        if (info[i].GetState() == NetworkInfo.State.Connected)
                        {
                            return true;
                        }
            }
            return false;
         
		}
		void DeleteCacheFileAll()
		{
				var cacheDirectory =Android.OS.Environment.ExternalStorageDirectory + "/" +
					Android.App.Application.Context.PackageName;
				DirectoryInfo directory = new DirectoryInfo(cacheDirectory);
				foreach (var fileInfo in directory.GetFiles("*", SearchOption.AllDirectories))
				{
					//size += fileInfo.Length;
					try
					{
						fileInfo.Delete();
					}
					catch
					{
					}
				}
				foreach (var directoryInfo in directory.GetDirectories("*", SearchOption.AllDirectories))
				{
					//size += fileInfo.Length;
					try
					{
						directoryInfo.Delete();
					}
					catch
					{
					}
				}
				directory.Delete ();
		}
    }
}
