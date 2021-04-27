using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using Prism.Commands;
using Prism.Mvvm;
using RM.Friendly.WPFStandardControls;
using StatefulModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Voiceroid2Sharp
{
	public class Voiceroid2 : BindableBase, IDisposable
	{
		//ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
		#region // プロパティ
		/// <summary>ボイロ一覧 を取得、設定</summary>
		private Dictionary<string, string> voiceroids_;
		/// <summary>ボイロ一覧 を取得、設定</summary>
		public Dictionary<string, string> VOICEROIDS
		{
			get => this.voiceroids_;

			private set => this.SetProperty(ref this.voiceroids_, value);
		}

		/// <summary>アクティブなボイロ を取得、設定</summary>
		private ObservableSynchronizedCollection<Voiceroid2Entity> activeVoiceroids_;
		/// <summary>アクティブなボイロ を取得、設定</summary>
		public ObservableSynchronizedCollection<Voiceroid2Entity> ActiveVoiceroids
		{
			get => this.activeVoiceroids_;

			private set => this.SetProperty(ref this.activeVoiceroids_, value);
		}
		/// <summary>選択中のキャラ名 を取得、設定</summary>
		private string charaName_;
		/// <summary>選択中のキャラ名 を取得、設定</summary>
		public string CharaName
		{
			get => this.charaName_;

			set => this.SetProperty(ref this.charaName_, value);
		}

		/// <summary>つながっているかどうか を取得、設定</summary>
		private bool isv2Connected_;
		/// <summary>つながっているかどうか を取得、設定</summary>
		public bool IsV2Connected
		{
			get => this.isv2Connected_;

			private set => this.SetProperty(ref this.isv2Connected_, value);
		}

		/// <summary>テキスト を取得、設定</summary>
		private string message_;
		/// <summary>テキスト を取得、設定</summary>
		public string Message
		{
			get => this.message_;

			set => this.SetProperty(ref this.message_, value);
		}

		/// <summary>読み上げ予定のメッセージ集 を取得、設定</summary>
		private ObservableSynchronizedCollection<CommentEntity> messages_;
		/// <summary>読み上げ予定のメッセージ集 を取得、設定</summary>
		public ObservableSynchronizedCollection<CommentEntity> Messages
		{
			get => this.messages_;

			private set => this.SetProperty(ref this.messages_, value);
		}

		/// <summary>コメントキュー を取得、設定</summary>
		private SortedObservableCollection<CommentEntity, DateTime> sortedMessages_;
		/// <summary>コメントキュー を取得、設定</summary>
		public SortedObservableCollection<CommentEntity, DateTime> SortedMessages
		{
			get => this.sortedMessages_;

			private set => this.SetProperty(ref this.sortedMessages_, value);
		}

		/// <summary>最後に喋った日時 を取得、設定</summary>
		private DateTime lastPlay_;
		/// <summary>最後に喋った日時 を取得、設定</summary>
		public DateTime LastPlay
		{
			get => this.lastPlay_;

			private set => this.SetProperty(ref this.lastPlay_, value);
		}

		/// <summary>テキストビューコレクション を取得、設定</summary>
		private IWPFDependencyObjectCollection<DependencyObject> textViewCollection_;
		/// <summary>テキストビューコレクション を取得、設定</summary>
		public IWPFDependencyObjectCollection<DependencyObject> TextEditerViewCollextion
		{
			get => this.textViewCollection_;

			private set => this.SetProperty(ref this.textViewCollection_, value);
		}

		/// <summary>開かれているかどうか を取得、設定</summary>
		public bool IsOpen => Process.GetProcessesByName(this.Voiceroid2Process.ProcessName)[0].MainWindowHandle != IntPtr.Zero;

		/// <summary> 再生中かどうか </summary>
		public bool IsPlaying => (bool)this.TextEditViewDataContext.IsPlaying;

		/// <summary>発話した文字やその他ログ を取得、設定</summary>
		private string log_;
		/// <summary>発話した文字やその他ログ を取得、設定</summary>
		public string Log
		{
			get => this.log_;

			private set => this.SetProperty(ref this.log_, value);
		}

		/// <summary>DataContextのdynamic型 を取得、設定</summary>
		private dynamic textEditViewDataContext_;
		/// <summary>DataContextのdynamic型 を取得、設定</summary>
		public dynamic TextEditViewDataContext
		{
			get => this.textEditViewDataContext_;

			set => this.SetProperty(ref this.textEditViewDataContext_, value);
		}

		public Process Voiceroid2Process => this.voiceroid2Process_;

		public WindowsAppFriend Editer => this.voiceroidEditer_;
		#endregion
		//ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
		#region // パブリックイベント
		public event Action<string> FindVoiceroidProcess;
		public event Action<string> UnFindVoiceroidProcess;
		public event Action<string> OnExitVoiceroid2;
		#endregion
		//ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
		#region // コマンド用メソッド
		#endregion
		//ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
		#region // オーバーライドメソッド
		protected override void OnPropertyChanged(PropertyChangedEventArgs args)
		{
			base.OnPropertyChanged(args);
			if (args.PropertyName == nameof(this.Message)) {
				if (!this.IsOpen && this.IsV2Connected) {
					this.DisConnectV2();
				}
				else {
					this.Messages.Add(new CommentEntity(this.Message));
				}
			}
		}
		#endregion
		//ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
		#region // パブリックメソッド
		/// <summary>
		/// VOICEROID2と連携します。
		/// </summary>
		/// <param name="autoStart">プロセスが見つからないときに自動で起動するかどうか。規定値はfalse。</param>
		public async Task ConnectV2(bool autoStart = false)
		{
			if (!File.Exists(VOICEROID2PATH)) {
				this.IsV2Connected = false;
				this.WriteLog("VOICEROID2がインストールされていません。");
				this.UnFindVoiceroidProcess?.Invoke("VOICEROID2がインストールされていません。");
				return;
			}

			if (!this.ActiveVoiceroids.Any()) {
				this.CreateActiveVoiceroidCollection();
			}

			var retrycount = 0;

			var voiceroidProcess = Process.GetProcessesByName("VoiceroidEditor");
			Debug.WriteLine("VoiceroidEditor Length:" + voiceroidProcess.Length.ToString());
			if (voiceroidProcess.Any()) {
				while (retrycount < MAXRETRYCOUNT && !this.IsV2Connected) {
					this.AttachV2Editer(voiceroidProcess[0]);
					retrycount++;
					if (!this.IsV2Connected) {
						await Task.Delay(3000);
					}
				}
			}
			else if (autoStart) {
				var p = await this.StartV2();
				while (retrycount < MAXRETRYCOUNT && !this.IsV2Connected) {
					this.AttachV2Editer(p);
					retrycount++;
					if (!this.IsV2Connected) {
						await Task.Delay(3000);
						p.Refresh();
						p.Dispose();
						p = Process.GetProcessesByName("VoiceroidEditor").FirstOrDefault();
					}
				}
			}

			if (this.IsV2Connected) {
				this.Messages.Clear();
				this.Message = "VOICEROID2と接続しました。";
				this.FindVoiceroidProcess?.Invoke("VOICEROID2と接続しました。");
			}
			else {
				this.UnFindVoiceroidProcess?.Invoke("VOICEROID2との接続に失敗しました。");
			}
		}

		/// <summary>
		/// VOICEROID2との接続を切ります。
		/// </summary>
		/// <param name="sender">呼び出し元の<see cref="Process"/></param>
		/// <param name="e">終了イベントを格納している<see cref="EventArgs"/></param>
		public void DisConnectV2()
		{
			Debug.WriteLine("VOICEROID2終了を検出");
			this.tokenSource_.Cancel();
			this.voiceroid2Process_?.Kill();
			this.voiceroidEditer_?.Dispose();
			this.IsV2Connected = false;
			this.mainWindow_ = null;
			this.talkTextBox_ = null;
			this.playButton_ = null;
			this.TextEditerViewCollextion = null;
			this.WriteLog("VOICEROIDと切断しました。");
			this.OnExitVoiceroid2?.Invoke("VOICEROID2と切断しました。");
		}

		/// <summary>
		/// インストールされているボイスロイドコレクションを作成します。
		/// </summary>
		public void CreateActiveVoiceroidCollection()
		{
			this.ActiveVoiceroids.Clear();
			var voicePath = Path.Combine(INSTALLFOLDERPATH, "Voice");
			if (Directory.Exists(voicePath)) {
				var directories = Directory.EnumerateDirectories(voicePath);
				if (!directories.Any()) {
					return;
				}
				foreach (var directory in directories) {
					var directoryInfo = new DirectoryInfo(directory);
					if (this.VOICEROIDS.ContainsKey(directoryInfo.Name)) {
						this.ActiveVoiceroids.Add(new Voiceroid2Entity() { CharaName = this.VOICEROIDS[directoryInfo.Name] });
					}
				}
			}
			if (this.ActiveVoiceroids.Any()) {
				this.CharaName = this.ActiveVoiceroids[0].CharaName;
			}
		}

		
		#endregion
		//ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
		#region // プライベートメソッド
		/// <summary>
		/// 文字列をVOICEROID2に送り再生ボタンをクリックします。
		/// 読み上げるキャラは<see cref="this.CharaName"/>を参照します。
		/// ただし、コマンドが入力されている場合はコマンドのキャラで読み上げます。
		/// </summary>
		private async Task TalkingAsync(IList newItems)
		{
			await this.semaphoreSlim_.WaitAsync();
			
			try {
				this.tokenSource_ = new CancellationTokenSource();
				foreach (var commentEntity in newItems.OfType<CommentEntity>()) {
					if (this.IsOpen != true) {
						this.Messages.Remove(commentEntity);
						return;
					}
					var readingTarget = commentEntity.Message;
					if (this.ActiveVoiceroids
						.Where(x => !string.IsNullOrEmpty(x.Command))
						.Any(x => Regex.IsMatch(readingTarget, $"^{x.Command.Replace(")", @"\)")}"))) {
						foreach (var activeViceroid in this.ActiveVoiceroids.Where(x => !string.IsNullOrEmpty(x.Command))) {
							var regex = new Regex($"^{activeViceroid.Command.Replace(")", @"\)")}");
							if (regex.IsMatch(readingTarget)) {
								var replacedTarget = regex.Replace(readingTarget, "");
								this.LastPlay = DateTime.Now;
								this.talkTextBox_.EmulateChangeText($"{activeViceroid.CharaName}＞{replacedTarget}");
								this.WriteLog($"{activeViceroid.CharaName}＞{replacedTarget}");
								this.playButton_.EmulateClick();
								await Task.Delay(300);
								this.RaisePropertyChanged(nameof(this.IsPlaying));
								while (this.IsPlaying) {
									if (this.IsOpen != true) {
										break;
									}
									await Task.Delay(500, this.tokenSource_.Token);
								}
								this.Messages.Remove(commentEntity);
								break;
							}
						}
					}
					else {
						this.LastPlay = DateTime.Now;
						this.talkTextBox_.EmulateChangeText($"{this.CharaName}＞{readingTarget}");
						this.WriteLog($"{this.CharaName}＞{readingTarget}");
						this.playButton_.EmulateClick();
						await Task.Delay(300);
						this.RaisePropertyChanged(nameof(this.IsPlaying));
						while (this.IsPlaying) {
							if (this.IsOpen != true) {
								break;
							}
							await Task.Delay(500, this.tokenSource_.Token);
						}
						this.Messages.Remove(commentEntity);
					}
				}
			}
			catch (Exception e) {
				Debug.WriteLine($"{e}");
			}
			finally {
				this.RaisePropertyChanged(nameof(this.IsPlaying));
				this.semaphoreSlim_.Release();
			}
		}

		/// <summary>
		/// <see cref="Messages.Count"/>の数が変化したときに呼び出されます。
		/// </summary>
		/// <param name="sender"><see cref="Messages"/></param>
		/// <param name="e">イベント<see cref="NotifyCollectionChangedEventArgs"/></param>
		private void OnMessageCollectionChenged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var _ = this.TalkingAsync(e.NewItems);
		}

		/// <summary>
		/// VOICEROID2のプロセスからウインドウを取得します。
		/// </summary>
		/// <param name="vProcess">VOICEROID2の<see cref="Process"/></param>
		private void AttachV2Editer(Process vProcess)
		{
			this.IsV2Connected = false;
			this.voiceroid2Process_ = vProcess;
			try {
				this.voiceroidEditer_ = new WindowsAppFriend(this.voiceroid2Process_);
				
				this.mainWindow_ = new WindowControl(this.voiceroidEditer_, this.Voiceroid2Process.MainWindowHandle);
				this.textEditerView_ = this.mainWindow_.GetFromTypeFullName(TALKEDITERVIEWNAME).Single();
				var textEditer = this.textEditerView_.Dynamic();
				this.TextEditViewDataContext = textEditer.DataContext;
				this.TextEditerViewCollextion = this.textEditerView_.LogicalTree(TreeRunDirection.Descendants);

				if (this.TextEditerViewCollextion.Count < 15) {
					return;
				}
#if DEBUG
				Debug.WriteLine("-----------------------------------------------");
				for (int i = 0; i < this.TextEditerViewCollextion.Count; i++) {
					Debug.WriteLine($"アイテムID:{i}");
					Debug.WriteLine($"{this.TextEditerViewCollextion[i]}");
				}
				Debug.WriteLine("-----------------------------------------------");
#endif
				this.talkTextBox_ = new WPFTextBox(this.textViewCollection_[4]);
				this.playButton_ = new WPFButtonBase(this.TextEditerViewCollextion[6]);
				this.IsV2Connected = true;
			}
			catch (Exception e) {
				this.WriteLog($"{e.Message}\n{e}");
				Debug.WriteLine(e);
			}
		}

		/// <summary>
		/// VOICEROID2を起動します。
		/// </summary>
		/// <returns>起動したVOICEROID2の<see cref="Process"/></returns>
		private async Task<Process> StartV2()
		{
			using (var p = new Process()) {
				p.StartInfo.FileName = VOICEROID2PATH;
				p.Start();
				p.WaitForInputIdle();
				this.WriteLog("VOICEROID2を起動中です。");
				await Task.Delay(4000);
				return p;
			}
		}
		private void WriteLog(string log)
		{
			var builder = new StringBuilder();
			builder.Append($"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}]:{log}");
			builder.Append("\n");
			builder.Append($@"{this.Log}");
			this.Log = $"{builder}";
		}
		#endregion
		//ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
		#region // メンバ変数
		private static readonly string INSTALLFOLDERPATH = @"C:\Program Files (x86)\AHS\VOICEROID2";
		private static readonly string VOICEROID2PATH = @"C:\Program Files (x86)\AHS\VOICEROID2\VoiceroidEditor.exe";
		private static readonly string TALKEDITERVIEWNAME = "AI.Talk.Editor.TextEditView";
		//private static readonly string MAINWINDOWNAME = "AI.Talk.Editor.MainWindow";
		//private static readonly string SPLASHWINDOWNAME = "AI.Framework.Wpf.SplashWindow";
		private static readonly int MAXRETRYCOUNT = 5;

		private SemaphoreSlim semaphoreSlim_;
		private WindowsAppFriend voiceroidEditer_;
		private Process voiceroid2Process_;
		private WindowControl mainWindow_;
		private AppVar textEditerView_;
		private WPFTextBox talkTextBox_;
		private WPFButtonBase playButton_;

		private CancellationTokenSource tokenSource_;
		#endregion
		//ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
		#region // 構築・破棄
		public Voiceroid2()
		{
			this.ActiveVoiceroids = new ObservableSynchronizedCollection<Voiceroid2Entity>();

			this.VOICEROIDS = new Dictionary<string, string>();

			this.VOICEROIDS.Add("yukari_44", "結月ゆかり(v1)");
			this.VOICEROIDS.Add("ai_44", "月読アイ(v1)");
			this.VOICEROIDS.Add("akane_west_44", "琴葉 茜(v1)");
			this.VOICEROIDS.Add("aoi_44", "琴葉 葵(v1)");
			this.VOICEROIDS.Add("kiritan_44", "東北きりたん(v1)");
			this.VOICEROIDS.Add("kou_44", "水奈瀬コウ(v1)");
			this.VOICEROIDS.Add("seika_44", "京町セイカ(v1)");
			this.VOICEROIDS.Add("shouta_44", "月読ショウタ(v1)");
			this.VOICEROIDS.Add("tamiyasu_44", "民安ともえ(v1)");
			this.VOICEROIDS.Add("yoshidakun_44", "鷹の爪 吉田くん(v1)");
			this.VOICEROIDS.Add("zunko_44", "東北ずん子(v1)");
			this.VOICEROIDS.Add("yukari_emo_44", "結月ゆかり");
			this.VOICEROIDS.Add("ai_4emo_4", "月読アイ");
			this.VOICEROIDS.Add("akane_west_emo_44", "琴葉 茜");
			this.VOICEROIDS.Add("aoi_emo_44", "琴葉 葵");
			this.VOICEROIDS.Add("kiritan_emo_44", "東北きりたん");
			this.VOICEROIDS.Add("kou_emo_44", "水奈瀬コウ");
			this.VOICEROIDS.Add("seika_emo_44", "京町セイカ");
			this.VOICEROIDS.Add("shouta_emo_44", "月読ショウタ");
			this.VOICEROIDS.Add("tamiyasu_emo_44", "民安ともえ");
			this.VOICEROIDS.Add("yoshidakun_emo_44", "鷹の爪 吉田くん");
			this.VOICEROIDS.Add("zunko_emo_44", "東北ずん子");
			this.VOICEROIDS.Add("akari_44", "紲星あかり");
			this.VOICEROIDS.Add("sora_44", "桜乃そら");


			this.Messages = new ObservableSynchronizedCollection<CommentEntity>();
			this.SortedMessages = this.Messages.ToSyncedSortedObservableCollection(key => key.SendDate);

			this.Messages.CollectionChanged += this.OnMessageCollectionChenged;

			this.semaphoreSlim_ = new SemaphoreSlim(1, 1);
		}

		#region IDisposable Support
		private bool disposedValue = false; // 重複する呼び出しを検出するには

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue) {
				if (disposing) {
					// TODO: マネージ状態を破棄します (マネージ オブジェクト)。
					this.semaphoreSlim_.Dispose();
					this.voiceroidEditer_.Dispose();
					this.voiceroid2Process_.Dispose();
				}

				// TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
				// TODO: 大きなフィールドを null に設定します。

				this.disposedValue = true;
			}
		}

		// TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
		// ~Voiceroid2()
		// {
		//   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
		//   Dispose(false);
		// }

		// このコードは、破棄可能なパターンを正しく実装できるように追加されました。
		public void Dispose()
		{
			// このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
			this.Dispose(true);
			// TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
			// GC.SuppressFinalize(this);
		}
		#endregion
		#endregion
	}
}
