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
using System.Json;

namespace ZhuoHuaAPP
{
	public  class WCFDataRequest
	{
        private string DEF_SVR_IP = "liuwusi.gnway.cc"; //"203.86.16.91";
		private string DEF_SVR_Port="13124";
        private string DEF_PROCE_DB = "CX_Android";
		private string DEF_METOD_NAME="DataRequest_By_SimpDEs";
		private string DEF_METOD_PARAM="_methodRequests";
		private WCFDataRequest(){
		}
		public static WCFDataRequest Instance {
			get {
				if(null==_Instance){
					_Instance=new WCFDataRequest ();
				}
				return _Instance;
			}
		}
		private static WCFDataRequest _Instance;
		public DataTable DataRequest_By_SimpDEs (Context context,string proceName, string[] paramKeys, string[] paramVals)
		{
			return DataRequest(context,DEF_SVR_IP,DEF_SVR_Port,DEF_METOD_NAME,DEF_METOD_PARAM,DEF_PROCE_DB,proceName,paramKeys,paramVals);
		}
		public DataTable DataRequest (Context context, string metodName, string metodParam, string proceDb, string proceName, string[] paramKeys, string[] paramVals)
		{
			return DataRequest(context,DEF_SVR_IP,DEF_SVR_Port,metodName,metodParam,proceDb,proceName,paramKeys,paramVals);
		}
		public  DataTable DataRequest (Context context,string svrIP,string svrPort,string metodName,string metodParam,string proceDb, string proceName, string[] paramKeys, string[] paramVals)
		{
			string _metodQst=CreateMetodRequestString(context,proceDb,proceName,paramKeys,paramVals);
			string _retString= SvrRequest(context,svrIP,svrPort,metodName,metodParam,_metodQst);
			return ConvertJSON2DataTable(_retString);
		}
		public  string SvrRequest (Context context,string proceName, string[] paramKeys, string[] paramVals)
		{string _metodQst=CreateMetodRequestString(context,DEF_PROCE_DB,proceName,paramKeys,paramVals);
			return SvrRequest(context,DEF_SVR_IP,DEF_SVR_Port,DEF_METOD_NAME,DEF_METOD_PARAM,_metodQst);
		}
		private  string SvrRequest(Context context,string svrIP,string svrPort, string metodName, string metodParam, string paramValue)
		{
			//加载设置的服务器IP和端口，没有则使用默认
			ISharedPreferences MyPrivate =  context.GetSharedPreferences("login",FileCreationMode.Private); //GetPreferences (FileCreationMode.WorldReadable);
			svrIP = MyPrivate.GetString ("ServerIP", svrIP);
			svrPort = MyPrivate.GetString ("ServerPort", svrPort);
		
			try
			{
				string _url =string.Format( @"http://{0}:{1}/SimpDbServer",svrIP,svrPort);
				string _sendMsg = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><{0} xmlns=\"http://tempuri.org/\"><{1}>{2}</{1}></{0}></s:Body></s:Envelope>";
				_sendMsg = string.Format(_sendMsg, metodName, metodParam, paramValue);
				HttpWebRequest _webRequest = System.Net.WebRequest.Create(_url) as HttpWebRequest;
				_webRequest.Method = "POST";
				_webRequest.ContentLength = _sendMsg.Length;
				_webRequest.ContentType = "text/xml; charset=utf-8";
				string _act = string.Format("http://tempuri.org/ISimpDbServer/{0}", metodName);
				_webRequest.Headers["SOAPAction"] = _act;
				_webRequest.Timeout = 1000 * 15;
				Stream _sm = null;
				try
				{
					_sm = _webRequest.GetRequestStream();
				}
				catch (WebException er)
				{
					if (WebExceptionStatus.Timeout== er.Status)
					{
						return "连接超时";
					}
				}
				StreamWriter _requestWriter = new StreamWriter(_sm);
				try
				{
					_requestWriter.Write(_sendMsg);
				}
				catch (Exception er)
				{
					return "连接超时"+er.Message;
				}
				finally
				{
					_requestWriter.Close();
					_requestWriter = null;
				}
				string _retString = WebResponseGet(_webRequest);
				_webRequest = null;
				string _headTag = string.Format("<{0}Result>", metodName);
				string _footTag = string.Format("</{0}Result>", metodName);
				int _startIndex = _retString.IndexOf(_headTag) + _headTag.Length;
				int _subLength = _retString.IndexOf(_footTag) - _startIndex;


				return _retString.Substring(_startIndex, _subLength);
			}
			catch(Exception )
			{
				return "连接超时";
			}
		}
		/// <summary> 
		/// Process the web response. 
		/// </summary> 
		/// <param name="webRequest">The request object.</param> 
		/// <returns>The response data.</returns> 
		public  string WebResponseGet(HttpWebRequest webRequest)
		{
			StreamReader responseReader = null;
			string responseData = "";
			try
			{
				responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
				responseData = responseReader.ReadToEnd();
			}
			catch(Exception )
			{
				return "连接错误";
			}
			finally
			{
				webRequest.GetResponse().GetResponseStream().Close();
				responseReader.Close();
				responseReader = null;
			}
			return responseData;
		}
		public  DataSet ConvertJSON2DataSet (string jsonString)
		{
			try {
				System.Json.JsonValue _retObj = System.Json.JsonValue.Load (new StringReader (jsonString));
				System.Json.JsonValue _retTabs=_retObj [0];
				DataSet Ds=new DataSet();
				if(_retTabs.JsonType==System.Json.JsonType.Array)
				{
					for (int _j = 0, _jCnt = _retTabs.Count; _j < _jCnt; _j++)
					{
						System.Json.JsonValue _retCols = _retTabs [_j] ["SimpDataArry"] [2];
						System.Json.JsonValue _retRows = _retTabs [_j] ["SimpDataArry"] [3];
						DataTable _dt = new DataTable ();
						if (_retCols.JsonType == System.Json.JsonType.Array) {
							for (int _i = 0, _iCnt = _retCols.Count; _i < _iCnt; _i++) {
								System.Json.JsonValue _col = _retCols [_i];
								if (_col.Count > 1) {
									string _colName = string.Format ("{0}", _col [0]);
									string _colTypeVal = string.Format ("{0}", _col [1]);
									int _colVal;
									if (int.TryParse (_colTypeVal, out _colVal)) {
										_dt.Columns.Add (_colName, ConvertColumnType (_colVal));
									}
								}
							}
						}
						if (_retRows.JsonType == System.Json.JsonType.Array) {
							for (int _i = 0, _iCnt = _retRows.Count; _i < _iCnt; _i++) {
								System.Json.JsonValue _row = _retRows [_i];

								object[] _objAry = CovertToObjectArray (_row);
								if (null != _objAry) {
									_dt.LoadDataRow (_objAry, false);

								}
							}
						}
						if (null != _dt)
						{Ds.Tables.Add(_dt);
						}
					}
				}
				if(null!=Ds)return Ds;
			}
			catch
			{
				return new DataSet();
			}
			return new DataSet();
		} 
		public  DataTable ConvertJSON2DataTable (string jsonString)
		{ 
			try {
				System.Json.JsonValue _retObj = System.Json.JsonValue.Load (new StringReader (jsonString));
				System.Json.JsonValue _retCols = _retObj [0] [0] ["SimpDataArry"] [2];
				System.Json.JsonValue _retRows = _retObj [0] [0] ["SimpDataArry"] [3];
				DataTable _dt = new DataTable ();
				if (_retCols.JsonType == System.Json.JsonType.Array) {
					for (int _i = 0, _iCnt = _retCols.Count; _i < _iCnt; _i++) {
						System.Json.JsonValue _col = _retCols [_i];
						if (_col.Count > 1) {
							string _colName = string.Format ("{0}", _col [0]);
							string _colTypeVal = string.Format ("{0}", _col [1]);
							int _colVal;
							if (int.TryParse (_colTypeVal, out _colVal)) {
								_dt.Columns.Add (_colName, ConvertColumnType (_colVal));
							}
						}
					}
				}
				if (_retRows.JsonType == System.Json.JsonType.Array) {
					for (int _i = 0, _iCnt = _retRows.Count; _i < _iCnt; _i++) {
						System.Json.JsonValue _row = _retRows [_i];

						object[] _objAry = CovertToObjectArray (_row);
						if (null != _objAry) {
							_dt.LoadDataRow (_objAry, false);
						}
					}
				}
				if (null != _dt)
				{return _dt;
				}
				return new DataTable();
			} 
			catch
			{
				return new DataTable();
			}
		}
		private  Type ConvertColumnType(int typeVal)
		{
			switch (typeVal)
			{
				//Byte = 0,
				case 0:
				return typeof(Byte);
				//Int64 = 1,
				case 1:
				return typeof(Int64);
				//Int32 = 2,
				case 2:
				return typeof(Int32);
				//String = 3,
				case 3:
				return typeof(String);
				//Boolean = 4,
				case 4:
				return typeof(Boolean);
				//DateTime = 5,
				case 5:
				return typeof(DateTime);
				//Decimal = 6,
				case 6:
				return typeof(Decimal);
				//Int16 = 7,
				case 7:
				return typeof(Int16);
			}
			return null;
		}
		private  string CreateMetodRequestString (Context context,string proceDb,string proceName, string[] paramKeys, string[] paramVals)
		{
			System.Json.JsonObject _jsonObj=new System.Json.JsonObject ();
			///使用设置中保存的数据库连接名
			ISharedPreferences MyPrivate =  context.GetSharedPreferences("login",FileCreationMode.Private); //GetPreferences (FileCreationMode.WorldReadable);
			_jsonObj.Add("ProceDb",MyPrivate.GetString ("ProceDb", proceDb));

			_jsonObj.Add("ProceName",proceName);
			_jsonObj.Add("ParamKeys",ConvertObjectAry2JsonValAry(paramKeys));
			_jsonObj.Add("ParamVals",ConvertObjectAry2JsonValAry(paramVals));
			return new JsonArray(_jsonObj).ToString();
		}
		private  JsonArray ConvertObjectAry2JsonValAry (string[] objAry)
		{
			JsonValue[] _jsonAry = new JsonValue[objAry.Length];
			for (int _i=0,_iCnt=objAry.Length; _i<_iCnt; _i++) {
				JsonValue _jsVlu=objAry[_i];
				_jsonAry[_i]=_jsVlu;
			}
			return new JsonArray (_jsonAry);
		}
		private  object[] CovertToObjectArray(System.Json.JsonValue row)
		{
			object[] _objAry = new object[row.Count];
			for (int _i = 0, _iCnt = row.Count; _i < _iCnt; _i++)
			{
				if(null !=row[_i])
				{
					System.Json.JsonValue _objRow=row[_i];
					switch (_objRow.JsonType)
					{
						case System.Json.JsonType.Boolean:                
						_objAry[_i] = Convert.ToBoolean(_objRow.ToString());
						break;
						case System.Json.JsonType.String:
						int length= _objRow.ToString().Length-2;
						_objAry[_i] = _objRow.ToString().Substring(1, length);
						break;
						case System.Json.JsonType.Number:
						_objAry[_i] = Convert.ToDecimal(_objRow.ToString());
						break;
						case System.Json.JsonType.Object:
						_objAry[_i] = "键:值";
						break;
						case System.Json.JsonType.Array:
						_objAry[_i] = "数组";
						break;
					}
				}
				else{_objAry[_i]=null;}
				//_objAry[_i] = string.Format("{0}", _objRow);
			}
			return _objAry;
		}
	}
}

