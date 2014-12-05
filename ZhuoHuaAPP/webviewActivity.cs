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
using Android.Webkit;
using Android.Content.Res;
using Java.IO;
using Android.Graphics;

namespace ZhuoHuaAPP
{
    [Activity(Label = "卓华软件", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
              Theme = "@android:style/Theme.NoTitleBar")]
    public class webviewActivity : Activity
    {
        WebView webView = null;
        Handler handler = new Handler();
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.webviewlayout);
            webView = (WebView)FindViewById(Resource.Id.webView1);

            webView.HorizontalScrollBarEnabled = true;
            webView.ScrollBarStyle = ScrollbarStyles.InsideInset;
            WebSettings settings = webView.Settings; 
            // 设置字符集编码
            settings.DefaultTextEncodingName = ASCIIEncoding.UTF8.ToString();
            settings.PluginsEnabled = true;
            // 开启JavaScript支持
            settings.JavaScriptEnabled = true;
            settings.SetSupportZoom(true);
            settings.BuiltInZoomControls = true;
            webView.SetWebChromeClient(new MyWebChromeClient());
            webView.SetWebViewClient(new MyWebViewClient()); 
            JSinterface js = new JSinterface(this, handler, webView);
           
            webView.AddJavascriptInterface(js, "myObject");
            // 加载assets目录下的文件
            string url = "file:///android_asset/index.html";
            webView.LoadUrl(url);
            
        }
        public class MyWebChromeClient : WebChromeClient
        {
            public override bool OnJsAlert(WebView view, string url, string message, JsResult result)
            {
                Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
                System.Console.WriteLine("弹出了提示框");
                result.Confirm();
                return base.OnJsAlert(view, url, message, result);
            }
            public override bool OnJsConfirm(WebView view, string url, string message, JsResult result)
            {
                System.Console.WriteLine("弹出了确认框");
                result.Confirm();
                return base.OnJsConfirm(view, url, message, result);
            }
            public override bool OnJsPrompt(WebView view, string url, string message, string defaultValue, JsPromptResult result)
            {
                System.Console.WriteLine("弹出了输入框");
                result.Confirm();
                return base.OnJsPrompt(view, url, message, defaultValue, result);
            }
            public override bool OnJsBeforeUnload(WebView view, string url, string message, JsResult result)
            {
                System.Console.WriteLine("弹出了离开确认框");
                result.Confirm();
                return base.OnJsBeforeUnload(view, url, message, result);
            }
        }
        public class MyWebViewClient : WebViewClient
        {
            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                view.LoadUrl(url);
                return base.ShouldOverrideUrlLoading(view, url);
            }

            public override void OnPageStarted(WebView view, string url, Bitmap favicon)
            {
                base.OnPageStarted(view, url, favicon);
            }

            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
            }

            public override void OnLoadResource(WebView view, string url)
            {
                base.OnLoadResource(view, url);
            }
            public override void OnReceivedError(WebView view, ClientError errorCode, string description, string failingUrl)
            {
                base.OnReceivedError(view, errorCode, description, failingUrl);
            }
        }
    }
}