/*
 * Created by SharpDevelop.
 * User: bando
 * Date: 2026/03/31
 * Time: 10:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace timesync
{
	class TimeSync
	{
		[StructLayout(LayoutKind.Sequential)]
		//時刻設定構造体
		public struct SystemTime{
			public ushort wYear;
			public ushort wMonth;
			public ushort wDayOfWeek;
			public ushort wDay;
			public ushort wHour;
			public ushort wMinute;
			public ushort wSecond;
			public ushort wMiliseconds;
		}

		//時刻設定API
		[DllImport("kernel32.dll")]
		public static extern bool SetLocalTime(ref SystemTime sysTime);

		//uint 最大値+1
		static readonly double MAX_UINT=4294967296.0;
		static readonly string TIME_FORMAT="h:mm:ss.fff";
		//時刻合わせ
		public static int Main(string[] args)
		{
			DateTime t0=DateTime.Now;
			var serverDts=Get();
			DateTime t1=serverDts[0];
			DateTime t2=serverDts[1];
			DateTime t3=DateTime.Now;
			TimeSpan reply=(t1-t0).Add(t2-t3);
			//往復にかかった時間の半分を足す
			var theta=new TimeSpan(reply.Ticks/2);
			//server.Add(reply);
			if (args.Length==0){
				//差異表示&判定
				Console.WriteLine(String.Format("t0: {0}",t0.ToString(TIME_FORMAT)));
				Console.WriteLine(String.Format("t1: {0}",t1.ToString(TIME_FORMAT)));
				Console.WriteLine(String.Format("t2: {0}",t2.ToString(TIME_FORMAT)));
				Console.WriteLine(String.Format("t3: {0}",t3.ToString(TIME_FORMAT)));
				Console.WriteLine(String.Format("theta: {0}",theta.TotalMilliseconds.ToString()));
				if (Math.Abs(theta.TotalMilliseconds)<=1000.0d){
					return 0;
				}
				else{
					//1秒以上差があったら1を返す
					return 1;
				}
			}
			else{
				//時刻合わせ
				if (args[0]=="-sync"){
					SetNowDateTime(DateTime.Now.AddMilliseconds(theta.TotalMilliseconds));
				}
				return 0;
			}
		}
		//時刻取得（0: 受信時刻 1: 送信時刻）
		public static List<DateTime> Get()	{
			// DNS サーバーを通さない方が、より取得成功率が上がる
			//const string NTP_SERVER = "ntp.jst.mfeed.ad.jp";
			const string NTP_SERVER = "210.173.160.57";
			const int	 NTP_PORT	= 123;
			// NTPサーバーへの接続用 UDP 生成
			System.Net.Sockets.UdpClient objSck;
			System.Net.IPEndPoint		 ipAny = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);
			objSck = new System.Net.Sockets.UdpClient(ipAny);
			// リクエスト送信
			byte[] sdat = new byte[48];
			sdat[0]		= 0xB;
			objSck.Send(sdat, sdat.Length, NTP_SERVER, NTP_PORT);
			// 日時データ受信
			byte[] rdat = objSck.Receive(ref ipAny);
			var dts=new List<DateTime>();
			dts.Add(extractDateTimeStamp(rdat,32));//サーバー受信時刻
			dts.Add(extractDateTimeStamp(rdat,40));//サーバー送信時刻
			return dts;
		}
		//UDPダイアグラムからタイムスタンプを切り出し
		static DateTime extractDateTimeStamp(byte[] rdat, int start){
			// 1900/1/1 からの経過秒数
			uint from19000101 = (uint)(rdat[start] << 24 | rdat[start+1] << 16 | rdat[start+2] << 8 | rdat[start+3]);
			// 小数点以下
			uint fromFractionUint=((uint)(rdat[start +4] << 24 | rdat[start+5] << 16 | rdat[start+6] << 8 | rdat[start+7]));
			double fromFractionDouble=(double)fromFractionUint*1000.0/MAX_UINT;
			int fromFraction=(int)fromFractionDouble;
			// UTC Time
			DateTime dt = new DateTime(1900,1,1).AddSeconds(from19000101).AddMilliseconds(fromFraction);
			return dt.ToLocalTime();
			
		}
	    //システム日時に設定する日時を指定する
		public static bool SetNowDateTime(DateTime dt){
    		SystemTime sysTime = new SystemTime();
    		sysTime.wYear = (ushort) dt.Year;
    		sysTime.wMonth = (ushort) dt.Month;
    		sysTime.wDay = (ushort) dt.Day;
    		sysTime.wHour = (ushort) dt.Hour;
    		sysTime.wMinute = (ushort) dt.Minute;
    		sysTime.wSecond = (ushort) dt.Second;
    		sysTime.wMiliseconds = (ushort) dt.Millisecond;
    		//システム日時を設定する
    		return SetLocalTime(ref sysTime);
		}
	}
}
