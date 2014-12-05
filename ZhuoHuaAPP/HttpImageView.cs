using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using Environment = Android.OS.Environment;

namespace Widget
{
	public class HttpImageView : ImageView
	{
		/// <summary>
		/// 图片地址
		/// </summary>
		public string HttpImageUrl { private set; get; }

		/// <summary>
		/// 使用缓存
		/// </summary>
		public bool UseCache { set; get; }

		/// <summary>
		/// 比如，1表示原图大小，2表示1/2原图，4表示1/4原图，以此类推
		/// </summary>
		public int SampleSize { set; get; }

		private string CacheDirectory;

		public HttpImageView(Activity context, string httpImageUrl, bool useCache = false, int sampleSize = 1)
			: base(context)
		{
			HttpImageUrl = httpImageUrl;
			UseCache = useCache;
			SampleSize = sampleSize;
			Initialize();
		}

		public HttpImageView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			HttpImageUrl = attrs.GetAttributeValue(null, "httpmageurl");
			var attr = attrs.GetAttributeValue(null, "usecache");
			var val = false;
			bool.TryParse(attr, out val);
			UseCache = val;
			var samplesize = attrs.GetAttributeValue(null, "samplesize");
			var s = 1;
			int.TryParse(samplesize, out s);
			if (s == 0)
				s = 1;
			SampleSize = s;
			Initialize();
		}
		public HttpImageView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			HttpImageUrl = attrs.GetAttributeValue(null, "httpmageurl");
			var attr = attrs.GetAttributeValue(null, "usecache");
			var val = false;
			bool.TryParse(attr, out val);
			UseCache = val;
			var samplesize = attrs.GetAttributeValue(null, "samplesize");
			var s = 1;
			int.TryParse(samplesize, out s);
			if (s == 0)
				s = 1;
			SampleSize = s;
			Initialize();
		}

		private void Initialize()
		{
			CacheDirectory = GetCacheDirectory();
			if (!string.IsNullOrEmpty(HttpImageUrl))
				LoadImage();
		}

		public void SetHttpImageUrl(string httpImageUlr)
		{
			HttpImageUrl = httpImageUlr;
			if (!string.IsNullOrEmpty (HttpImageUrl)) {
				LoadImage ();
			}
			else 
			{
				(Context as Activity).RunOnUiThread(
					() => { 
					this.SetImageResource(Android.Resource.Drawable.IcMenuReportImage);//IcMenuGallery);
				});
				return;
			}
		}

		private void LoadImage()
		{
			if (UseCache)
			{
				var localPath = GetLocalFilePath();
				if (File.Exists(localPath))
				{
					var bitmap = GetBitmap(localPath);
					if (bitmap != null)
					{
						(Context as Activity).RunOnUiThread(() => { 
							this.SetImageBitmap(bitmap);
							if(bitmap != null){bitmap.Dispose();bitmap=null;}
						});
					}
				}
				else
				{
					using (var webClient = new WebClient())
					{
						webClient.DownloadFileAsync(new Uri(HttpImageUrl), localPath);
						webClient.DownloadFileCompleted += (o, e) =>
						{
							if (e.Error != null || e.Cancelled)
							{
								if (File.Exists(localPath))
								{
									File.Delete(localPath);
								}
								(Context as Activity).RunOnUiThread(
									() => { 
									this.SetImageResource(Android.Resource.Drawable.IcMenuReportImage);//IcMenuGallery);
								});
								return ;
							}
							else
							{
								var bitmap = GetBitmap(localPath);
								(Context as Activity).RunOnUiThread(
									() => { 
									this.SetImageBitmap(bitmap);
									if(bitmap != null){bitmap.Dispose();bitmap=null;}
								});
							}
						};
						webClient.Dispose ();
					}
				}
			}
			else
			{
				using (var webClient = new WebClient())
				{
					webClient.DownloadDataAsync(new Uri(HttpImageUrl));
					webClient.DownloadDataCompleted += (o, e) =>
					{
						if (e.Result != null)
						{
							var bitmap = BitmapFactory.DecodeStream(new MemoryStream(e.Result), null,
							                                        new BitmapFactory.Options()
							                                        {
								InSampleSize = SampleSize,
							});
							(Context as Activity).RunOnUiThread(() => {
								this.SetImageBitmap(bitmap); 
								if(bitmap != null){bitmap.Dispose();bitmap=null;}
							});
						}
					};
					webClient.Dispose ();
				}
			}
		}
		/// <summary>
		/// 生成位图对象
		/// </summary>
		/// <param name="localUrl"></param>
		/// <returns></returns>
		private Bitmap GetBitmap(string localUrl)
		{
			try
			{
				using (var stream = File.Open(localUrl, FileMode.Open))
				{
					if (stream.Length > 0)
					{
						var bitmap = BitmapFactory.DecodeStream(stream, null,
						                                        new BitmapFactory.Options() { InSampleSize = SampleSize, });
						stream.Dispose();
						return bitmap;
					}
					stream.Close();
					return null;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}
		/// <summary>
		/// 取得图片的本地完整路径
		/// </summary>
		/// <returns></returns>
		private string GetLocalFilePath()
		{
			return MakeDir() + "/" + GetLocalFileName();
		}
		/// <summary>
		/// 创建图片的本地目录并返回目录路径
		/// </summary>
		/// <returns></returns>
		private string MakeDir()
		{
			var dir = CacheDirectory + "/" + Math.Abs(HttpImageUrl.GetHashCode() % 50);

			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			return dir;
		}
		/// <summary>
		/// 生成本地文件名
		/// </summary>
		/// <returns></returns>
		private string GetLocalFileName()
		{
			string httpUrl = HttpImageUrl;
			httpUrl = httpUrl.ToLower();
			Match match = Regex.Match(httpUrl, @".+(.jpg|.jpeg|.png|.bmp|.gif)", RegexOptions.IgnoreCase);
			string ext = ".png";
			if (match.Success)
				ext = string.IsNullOrEmpty(match.Groups[1].Value) ? ".png" : match.Groups[1].Value;
			//显示存储
			//return GetMd5 (httpUrl)+ ext;
			//隐示存储
			return GetMd5 (httpUrl);
		}

		private string GetMd5(String str)
		{
			byte[] result = Encoding.Default.GetBytes(str);
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] output = md5.ComputeHash(result);
			return BitConverter.ToString(output).Replace("-", "");
		}

		/// <summary>
		/// 缓存目录文件总字节数
		/// </summary>
		/// <returns></returns>
		public static long GetTotalCacheFileSize()
		{
			var cacheDirectory = GetCacheDirectory();
			DirectoryInfo directory = new DirectoryInfo(cacheDirectory);
			long size = 0;
			foreach (var fileInfo in directory.GetFiles("*", SearchOption.AllDirectories))
			{
				size += fileInfo.Length;
			}

			return size;
		}

		public static string GetCacheDirectory()
		{
			var cacheDirectory = "";
			if (Environment.ExternalStorageState == "mounted")
			{
				// cacheDirectory = Environment.ExternalStorageDirectory + "/" +
				//		Android.App.Application.Context.PackageName + "/ImageViewCache";
				cacheDirectory = Environment.ExternalStorageDirectory + "/Batar/ImageViewCache";
			}
			else
			{
				cacheDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) +
					"/ImageViewCache";
			}
			return cacheDirectory;
		}

		public static void DeleteCacheFileAll()
		{
			var cacheDirectory = GetCacheDirectory();
			DirectoryInfo directory = new DirectoryInfo(cacheDirectory);
			var deleteCount = 0;
			foreach (var fileInfo in directory.GetFiles("*", SearchOption.AllDirectories))
			{
				//size += fileInfo.Length;
				try
				{
					fileInfo.Delete();
					deleteCount++;
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
					deleteCount++;
				}
				catch
				{
				}
			}
			directory.Delete ();
		}
		public static Task DeleteCacheFileAllAsync()
		{
			Task task = new Task(DeleteCacheFileAll);
			task.Start(TaskScheduler.FromCurrentSynchronizationContext());
			return task;
		}
		/// <summary>
		/// 删除字节数大于给定值的图片缓存文件
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static int DeleteCacheFileBySizeLargerThan(int size)
		{
			var cacheDirectory = GetCacheDirectory();
			DirectoryInfo directory = new DirectoryInfo(cacheDirectory);
			var deleteCount = 0;
			foreach (var fileInfo in directory.GetFiles("*", SearchOption.AllDirectories))
			{
				//size += fileInfo.Length;
				try
				{
					if (fileInfo.Length >= size)
					{
						fileInfo.Delete();
						deleteCount++;
					}
				}
				catch
				{
				}
			}
			return deleteCount;
		}

		/// <summary>
		/// 删除字节数小于给定值的图片缓存文件
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static int DeleteCacheFileBySizeSmallerThan(int size)
		{
			var cacheDirectory = GetCacheDirectory();
			DirectoryInfo directory = new DirectoryInfo(cacheDirectory);
			var deleteCount = 0;
			foreach (var fileInfo in directory.GetFiles("*", SearchOption.AllDirectories))
			{
				try
				{
					if (fileInfo.Length <= size)
					{
						fileInfo.Delete();
						deleteCount++;
					}
				}
				catch
				{
				}
			}
			return deleteCount;
		}

		/// <summary>
		/// 删除所有图片总字节数大于给定值的缓存文件
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static int DeleteCacheFileByTotalSizeLargerThan(int size)
		{
			var cacheDirectory = GetCacheDirectory();
			DirectoryInfo directory = new DirectoryInfo(cacheDirectory);
			var deleteCount = 0;
			long Totalsize =0;
			foreach (var fileInfo in directory.GetFiles("*", SearchOption.AllDirectories))
			{
				Totalsize += fileInfo.Length;
			}
			if(Totalsize>size)
			{
				foreach (var fileInfo in directory.GetFiles("*", SearchOption.AllDirectories))
				{
					try
					{
						fileInfo.Delete();
						deleteCount++;
					}
					catch
					{
					}	
				}
			}
			return deleteCount;
		}
		/// <summary>
		/// 删除最后修改时间早于指定时间的图片缓存文件
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public static int DeleteCacheFileByLastWriteTimeBefore(DateTime time)
		{
			var cacheDirectory = GetCacheDirectory();
			DirectoryInfo directory = new DirectoryInfo(cacheDirectory);
			var deleteCount = 0;
			foreach (var fileInfo in directory.GetFiles("*", SearchOption.AllDirectories))
			{
				try
				{
					if (fileInfo.LastWriteTime <= time)
					{
						fileInfo.Delete();
						deleteCount++;
					}
				}
				catch
				{
				}
			}
			return deleteCount;
		}

		/// <summary>
		/// 删除最后修改时间晚于指定时间的图片缓存文件
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public static int DeleteCacheFileByLastWriteTimeAfter(DateTime time)
		{
			var cacheDirectory = GetCacheDirectory();
			DirectoryInfo directory = new DirectoryInfo(cacheDirectory);
			var deleteCount = 0;
			foreach (var fileInfo in directory.GetFiles("*", SearchOption.AllDirectories))
			{
				try
				{
					if (fileInfo.LastWriteTime >= time)
					{
						fileInfo.Delete();
						deleteCount++;
					}
				}
				catch
				{
				}
			}
			return deleteCount;
		}

		/// <summary>
		/// 异步删除
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static Task<int> DeleteCacheFileBySizeLargerThanAsync(int size)
		{
			Task<int> task = new Task<int>(() => { return DeleteCacheFileBySizeLargerThan(size); });
			task.Start(TaskScheduler.FromCurrentSynchronizationContext());
			return task;
		}

		/// <summary>
		/// 异步删除
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static Task<int> DeleteCacheFileByTotalSizeLargerThanAsync(int size)
		{
			Task<int> task = new Task<int>(() => { return DeleteCacheFileByTotalSizeLargerThan(size); });
			task.Start(TaskScheduler.FromCurrentSynchronizationContext());
			return task;
		}

		/// <summary>
		/// 异步删除
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static Task<int> DeleteCacheFileBySizeSmallerThanAsync(int size)
		{
			Task<int> task = new Task<int>(() => { return DeleteCacheFileBySizeSmallerThan(size); });
			task.Start(TaskScheduler.FromCurrentSynchronizationContext());
			return task;
		}

		/// <summary>
		/// 异步删除
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static Task<int> DeleteCacheFileByLastWriteTimeBeforeAsync(DateTime time)
		{
			Task<int> task = new Task<int>(() => { return DeleteCacheFileByLastWriteTimeBefore(time); });
			task.Start(TaskScheduler.FromCurrentSynchronizationContext());
			return task;
		}

		/// <summary>
		/// 异步删除
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static Task<int> DeleteCacheFileByLastWriteTimeAfterAsync(DateTime time)
		{
			Task<int> task = new Task<int>(() => { return DeleteCacheFileByLastWriteTimeAfter(time); });
			task.Start(TaskScheduler.FromCurrentSynchronizationContext());
			return task;
		}
	}
}