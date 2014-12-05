using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ZhuoHuaAPP
{
    [Activity(Label = "联系我们", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
              Theme = "@android:style/Theme.NoTitleBar")]
    public class AboutUs : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle); 
            SetContentView(Resource.Layout.aboutus);
        }
    }
}