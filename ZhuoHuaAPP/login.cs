using System;
using System.Net;
using System.IO;
using System.Xml;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Net;
using System.Data;
using System.Threading;

namespace ZhuoHuaAPP
{
	[Activity(Label = "卓华软件",ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
	          Theme = "@android:style/Theme.NoTitleBar")]
    public class login : Activity
    {
		string firstDayInMon=null;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.login);
	
			Button loginButton=FindViewById<Button>(Resource.Id.login_btn_login_2);
			loginButton.Click+=new EventHandler(loginButton_Click);
            ImageButton ibtn = FindViewById<ImageButton>(Resource.Id.imageButton1);
            ibtn.Click += new EventHandler(ibtn_Click);
		}

        void ibtn_Click(object sender, EventArgs e)
        {
            Intent layOut = new Intent();
            layOut.SetClass(this, typeof(ServerSetting));
            Bundle homeData = new Bundle();
            layOut.PutExtras(homeData);
            StartActivity(layOut);
        }
        void loginButton_Click(object sender, EventArgs e)
        {
            Intent layOut = new Intent();
            layOut.SetClass(this, typeof(HomePage));
            Bundle homeData = new Bundle();
            layOut.PutExtras(homeData);
            StartActivity(layOut);
        }
        //void loginButton_Click(object sender, EventArgs e)
        //{
        //    if (ManageDevice.isConnectingToInternet(this) == false)
        //    {
        //        MessageBox.Show(this, "无可用网络", "请检查网络");
        //        return;
        //    }   

        //    EditText login_edit_account_2=FindViewById<EditText>(Resource.Id.login_edit_account_2);
        //    EditText login_edit_pwd_2=FindViewById<EditText>(Resource.Id.login_edit_pwd_2);
        //    DateTime firstOfmonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01);
        //    firstDayInMon=firstOfmonth.ToString("yyyy-MM-dd");
        //    string loginString="";
        //    string Err1="";
        //    DataTable loginDatatable=new DataTable();
        //    ProgressDialog pd = ProgressDialog.Show(this, new Java.Lang.String("提示"), new Java.Lang.String("正在登录，请稍后……"), true);
        //    Java.Lang.Thread th = new Java.Lang.Thread();
        //    th = new Java.Lang.Thread(() =>
        //    {   

        //        //验证登录
        //        try
        //        {
        //        loginString= WCFDataRequest.Instance.SvrRequest(
        //        this,
        //        "ZhuoHualogin",
        //        new string[]{"loginAcount","loginPassword"},
        //        new string[]{login_edit_account_2.Text,login_edit_pwd_2.Text,});
        //        }
        //        catch(Exception ex)
        //        {
        //            pd.Dismiss();
        //            Err1=ex.Message;
        //            RunOnUiThread(() => 
        //                          {
        //            MessageBox.Show(this,"连接服务器错误",Err1);
        //            });
        //            return;
        //        }

        //        if(loginString=="连接超时")
        //        {
        //            pd.Dismiss();
        //            RunOnUiThread(() => 
        //                          {
        //            MessageBox.Show(this,"连接超时","检查网络或者联系服务商");
        //            });
        //            return;
        //        }else
        //        {
        //            loginDatatable=WCFDataRequest.Instance.ConvertJSON2DataTable(loginString);
        //        }
        //        if(loginDatatable.Rows.Count==1 && loginDatatable.Rows[0][1].ToString()=="admin")
        //        {
        //                SaveLoginInfo(loginDatatable.Rows[0][1].ToString(),"");		    
        //                Intent layOut = new Intent();
        //    //			layOut.SetClass(this, typeof(NaviMenuHome));
        //                Bundle homeData = new Bundle();
        //                layOut.PutExtras(homeData);
        //                StartActivity(layOut);
        //                pd.Dismiss();
        //                this.Finish();
        //        }
        //        //else if(loginDatatable.Rows.Count==1 && loginDatatable.Rows[0][1].ToString()=="ZTManager")
        //        //{
        //        //        SaveLoginInfo(loginDatatable.Rows[0][1].ToString(),loginDatatable.Rows[0][3].ToString());
        //        //        pd.Dismiss();
        //        //        Intent layOut = new Intent();
        //        //        layOut.SetClass(this, typeof(ExhibitionActivity2));
        //        //        Bundle homeData = new Bundle();
        //        //        layOut.PutExtras(homeData);
        //        //        StartActivity(layOut);
        //        //        pd.Dismiss();
        //        //    this.Finish();
        //        //}
        //        else
        //        {
        //            pd.Dismiss();
        //            RunOnUiThread(() =>
        //            {
        //                MessageBox.Show(this,"提示","帐号密码错误");
        //            });
        //        }
        //    });
        //    th.Start(); 
        //}
		void SaveLoginInfo (string Role,string Zone)
		{

			EditText login_edit_account_2 = FindViewById<EditText> (Resource.Id.login_edit_account_2);
			EditText login_edit_pwd_2 = FindViewById<EditText> (Resource.Id.login_edit_pwd_2);
			CheckBox login_cb_savepwd_2 = FindViewById<CheckBox> (Resource.Id.login_cb_savepwd_2);
			CheckBox login_Auto = FindViewById<CheckBox> (Resource.Id.login_Auto);
			ISharedPreferences MyPrivate =  GetSharedPreferences("login",FileCreationMode.Private);//GetPreferences (FileCreationMode.Private);
			ISharedPreferencesEditor e = MyPrivate.Edit ();
			e.PutString ("Account", login_edit_account_2.Text);
			e.PutString ("Role", Role);
			e.PutString ("Zone", Zone);
			if (login_cb_savepwd_2.Checked == true) 
			{
				e.PutString ("pwd", login_edit_pwd_2.Text);
			}
			else 
			{
				e.PutString ("pwd","");
			}
			e.PutBoolean("savepwd",login_cb_savepwd_2.Checked);
			e.PutBoolean ("Autologin", login_Auto.Checked);
			e.Commit();
		}
	    void Initialize ()
		{
			EditText login_edit_account_2 = FindViewById<EditText> (Resource.Id.login_edit_account_2);
			EditText login_edit_pwd_2 = FindViewById<EditText> (Resource.Id.login_edit_pwd_2);
			CheckBox login_cb_savepwd_2 = FindViewById<CheckBox> (Resource.Id.login_cb_savepwd_2);
			CheckBox login_Auto = FindViewById<CheckBox> (Resource.Id.login_Auto);
			ISharedPreferences MyPrivate =  GetSharedPreferences("login",FileCreationMode.Private); //GetPreferences (FileCreationMode.WorldReadable);
			login_edit_account_2.Text = MyPrivate.GetString ("Account", "");
			login_edit_pwd_2.Text = MyPrivate.GetString ("pwd", "");
			login_cb_savepwd_2.Checked = MyPrivate.GetBoolean ("savepwd", false);
			login_Auto.Checked = MyPrivate.GetBoolean ("Autologin", false);
		}
	}
}
