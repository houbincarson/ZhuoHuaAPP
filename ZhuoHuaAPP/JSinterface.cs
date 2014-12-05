using System;
using System.Collections.Generic;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Webkit;
using Org.Json;
using Java.Lang;
using Android.Widget;
using Java.Interop;
namespace ZhuoHuaAPP
{
    class JSinterface : Java.Lang.Object
    {
        private Context mContext = null;
        private Handler mHandler = null;
        private WebView mView = null;

        private JSONArray jsonArray = new JSONArray();
        private Random random = new Random();

        public JSinterface(Context context, Handler handler, WebView webView)
        {
            mContext = context;
            mHandler = handler;
            mView = webView;
        }
        [Export]
        [JavascriptInterface]
        public void init()
        {
            mHandler.Post(() =>
            {
                mView.LoadUrl("javascript:setContactInfo('" + getJsonStr() + "')");
            });
        }
        public string getJsonStr()
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    JSONObject object1 = new JSONObject();
                    object1.Put("name",  i);
                    object1.Put("value", random.Next(30));
                    object1.Put("color", getRandColorCode());
                    jsonArray.Put(object1);
                }
                return jsonArray.ToString();
            }
            catch (JSONException e)
            {
                e.PrintStackTrace();
            }
            Console.WriteLine(jsonArray.ToString());
            return null;
        }

        private string getRandColorCode()
        {
            string r, g, b;
            Random random = new Random();
            r = Integer.ToHexString(random.Next(256)).ToUpper();
            g = Integer.ToHexString(random.Next(256)).ToUpper();
            b = Integer.ToHexString(random.Next(256)).ToUpper();

            r = r.Length == 1 ? "0" + r : r;
            g = g.Length == 1 ? "0" + g : g;
            b = b.Length == 1 ? "0" + b : b;

            return "#" + r + g + b;
        }
        [Export]
        [JavascriptInterface]
        public int getW()
        {
            return px2dip(mContext.Resources.DisplayMetrics.WidthPixels);
        }
        [Export]
        [JavascriptInterface]
        public int getH()
        {
            return px2dip(mContext.Resources.DisplayMetrics.HeightPixels);
        }

        public int px2dip(float pxValue)
        {
            float scale = mContext.Resources.DisplayMetrics.Density;
            return (int)(pxValue / scale + 0.5f);
        }
        [Export]
        [JavascriptInterface]
        public void setValue(string name, string value)
        {
            Toast.MakeText(mContext, name + " " + value + "%", ToastLength.Short).Show();

            Intent layOut = new Intent();
            layOut.SetClass(mContext, typeof(HomePage));
            Bundle homeData = new Bundle();
            layOut.PutExtras(homeData);
            mContext.StartActivity(layOut);
        }

    }
}
