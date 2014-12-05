using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Java.Net;
using Java.IO;

using Android.Util;
using System.Text;
using System.IO;
using Android.Content.PM;
using System.Data;
using Android.Net;

namespace ZhuoHuaAPP
{
	class HttpDownloadFile
	{

		public static void InstallApkFile (Context context,string urlString, string PackName)
		{   string Err;
			ProgressDialog pd = ProgressDialog.Show(context, new Java.Lang.String("提示"), new Java.Lang.String("正在更新版本，请稍后……"), true);
            Java.Lang.Thread th = new Java.Lang.Thread(() =>
            {   
			
			  try 
				{
				downloadFile (context,urlString, PackName);
			     } 
				catch (Exception ex) 
				{
				pd.Dismiss();
				Err = ex.Message;
			//	MessageBox.Show(context,"提示",Err);
			    }
				finally 
			    {
			  	pd.Dismiss();
					try{
					openFile(context,setMkdir(context),PackName);
					}catch
					{
						Err="安装程序打开错误";
				//		MessageBox.Show(context,"提示",Err);

					}
			    }
			});
			th.Start();
		}
		public static void CheckPackVerson(Context context)
		{   
			DataTable VersonTable;
			try {
				VersonTable= WCFDataRequest.Instance.DataRequest_By_SimpDEs(
				context,
			    "CheckPackageVerson",
				new string[]{"PackageName"},
				new string[]{context.PackageName});

				if(VersonTable.Rows.Count==1 && 
				   Convert.ToDouble(VersonTable.Rows[0][3].ToString())
				   >context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode)
				{
					MessageBox.Confirm(context, "版本有更新", VersonTable.Rows[0][6].ToString(),"现在更新","稍后提醒",delegate{HttpDownloadFile.InstallApkFile(context,VersonTable.Rows[0][5].ToString(),context.PackageName+".apk");},
					                   new EventHandler<DialogClickEventArgs>(cancelHandler));  
				}

			} catch(Exception ex)
			{MessageBox.Show(context,"版本更新错误:",ex.Message);
			}
		}
		static void cancelHandler(object sender, DialogClickEventArgs e)
		{

		}
        private static Boolean checkSDCard()  

		{ 

        if(Android.OS.Environment.ExternalStorageState.Equals(Android.OS.Environment.MediaMounted)) 
         {  
             return true;  
         }else{  
             return false;  
         }  

		}
        private static void openFile(Context context,string FilePath,string FileName)
		{              Intent i = new Intent(Intent.ActionView);
				        i.SetDataAndType(Android.Net.Uri.Parse("file://"+FilePath+@"/"+FileName), "application/vnd.android.package-archive");
	                    context.StartActivity(i);
		}
        private static String setMkdir(Context context)
	{
		String filePath;
		if(checkSDCard())
		{
				filePath = Android.OS.Environment.GetExternalStoragePublicDirectory("").ToString()+@"/download";//File.Separator
		}else{
				filePath = context.CacheDir.AbsolutePath+@"/download";
		}
		 Java.IO.File file = new Java.IO.File(filePath);
		if(!file.Exists())
		{
			Boolean b = file.Mkdirs();

			Log.Error("file","目录不存在  创建文件    "+b);
		}else{
			Log.Error("file", "目录存在");
		}
		return filePath;
	}
        private static void downloadFile(Context context,string urlString,string FileName){
		URL url = new URL(urlString);
                    // 创建连接
			URLConnection conn=url.OpenConnection();

     //     Java.Net.HttpURLConnection conn = (HttpURLConnection) url.OpenConnection();
                    conn.Connect();
                    // 获取文件大小
			  int length = conn.ContentLength;

                    // 创建输入流
		//	FileInputStream getdataInputStream=conn.InputStream;
       //  InputStream getdataInputStream =conn.InputStream; 
			 System.IO.Stream getdataInputStream =  conn.InputStream;
		

                    Java.IO.File apkFile = new Java.IO.File(setMkdir(context),FileName);
		
	                FileOutputStream fos = new FileOutputStream(apkFile);
                    int count = 0;
                    // 缓存
                   byte[] buf = new byte[1024];
                    // 写入到文件中
                    do
                    {
				        UTF8Encoding enc=new UTF8Encoding();
                       int numread =  getdataInputStream.Read(buf,0,1024);
                //       
				count += numread;

                        // 计算进度条位置
                  //      progress = (int)(((float) count / length) * 100);
                        // 更新进度
                  //      mHandler.sendEmptyMessage(DOWNLOAD);
                        if (numread <= 0)

                        {
                            // 下载完成
                  //          mHandler.sendEmptyMessage(DOWNLOAD_FINISH);
                            break;
                        }
                        // 写入文件
                        fos.Write(buf, 0, numread);
                    } while (true);// 点击取消就停止下载.
                    fos.Close();
                    getdataInputStream.Close();
                }
	}
}


