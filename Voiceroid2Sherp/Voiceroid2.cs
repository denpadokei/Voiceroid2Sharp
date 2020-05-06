using Codeer.Friendly;
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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Voiceroid2Sharp
{
    public class Voiceroid2 : BindableBase
    {
		//ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
		#region // プロパティ
		/// <summary>ボイロ一覧 を取得、設定</summary>
		private Dictionary<string, string> voiceroids_;
		/// <summary>ボイロ一覧 を取得、設定</summary>
		public Dictionary<string, string> VOICEROIDS
		{
			get => this.voiceroids_;

			set => this.SetProperty(ref this.voiceroids_, value);
		}

		/// <summary>アクティブなボイロ を取得、設定</summary>
		private ObservableSynchronizedCollection<Voiceroid2Entity> activeVoiceroids_;
		/// <summary>アクティブなボイロ を取得、設定</summary>
		public ObservableSynchronizedCollection<Voiceroid2Entity> ActiveVoiceroids
		{
			get => this.activeVoiceroids_;

			set => this.SetProperty(ref this.activeVoiceroids_, value);
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

		/// <summary><see cref="AppVar"> を取得、設定</summary>
		private AppVar statusLabel_;
		/// <summary><see cref="AppVar"> を取得、設定</summary>
		public AppVar StatusLabel
		{
			get => this.statusLabel_;

			private set => this.SetProperty(ref this.statusLabel_, value);
		}

		/// <summary>テキストビューコレクション を取得、設定</summary>
		private IWPFDependencyObjectCollection<DependencyObject> textViewCollection_;
		/// <summary>テキストビューコレクション を取得、設定</summary>
		public IWPFDependencyObjectCollection<DependencyObject> TextViewCollextion
		{
			get => this.textViewCollection_;

			private set => this.SetProperty(ref this.textViewCollection_, value);
		}

		/// <summary>話し中かどうか を取得、設定</summary>
		public bool IsTalking => !this.beginButton_.IsEnabled;
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
				this.Messages.Add(new CommentEntity(this.Message));
			}
		}
		#endregion
		//ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
		#region // パブリックメソッド
		/// <summary>
		/// VOICEROID2と連携します。
		/// </summary>
		/// <param name="autoStart">プロセスが見つからないときに自動で起動するかどうか。規定値はfalse。</param>
		public void ConnectV2(bool autoStart = false)
		{
			if (!File.Exists(VOICEROID2PATH)) {
				this.IsV2Connected = false;
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
					Thread.Sleep(3000);
					retrycount++;
				}
			}
			else if (autoStart) {
				var p = this.StartV2();
				while (retrycount < MAXRETRYCOUNT && !this.IsV2Connected) {
					this.AttachV2Editer(p);
					Thread.Sleep(3000);
					retrycount++;
					if (!this.IsV2Connected) {
						p.Refresh();
						p.Dispose();
						p = Process.GetProcessesByName("VoiceroidEditor").FirstOrDefault();
					}
				}
			}

			if (this.IsV2Connected) {
				this.Message = "VOICEROID2と接続しました。";
				this.FindVoiceroidProcess?.Invoke("VOICEROID2と接続しました。");
			}
			else {
				this.UnFindVoiceroidProcess?.Invoke("VOICEROID2との接続に失敗しました。");
			}
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
		/// VOICEROID2が終了したときに呼び出されます
		/// </summary>
		/// <param name="sender">呼び出し元の<see cref="Process"/></param>
		/// <param name="e">終了イベントを格納している<see cref="EventArgs"/></param>
		private void OnV2Exit(object sender, EventArgs e)
		{
			Debug.WriteLine("VOICEROID2終了を検出");
			this.voiceroidEditer_.Dispose();
			this.IsV2Connected = false;
			this.uiTreeTop_ = null;
			this.talkTextBox_ = null;
			this.playButton_ = null;
			this.beginButton_ = null;
			this.TextViewCollextion = null;
			this.OnExitVoiceroid2?.Invoke("VOICEROID2が終了しました。");
		}

		/// <summary>
		/// 文字列をVOICEROID2に送り再生ボタンをクリックします。
		/// 読み上げるキャラは<see cref="this.CharaName"/>を参照します。
		/// ただし、コマンドが入力されている場合はコマンドのキャラで読み上げます。
		/// </summary>
		private async void Talking(IList newItem)
		{
			await Task.Run(() =>
			{
				lock (this.lockObject_) {
					var talkCooldown = TimeSpan.FromSeconds(0.3);
					var now = DateTime.Now;
					if ((now - this.LastPlay) < talkCooldown) {
						Thread.Sleep(talkCooldown - (now - this.LastPlay));
					}
					while (this.IsTalking && this.SortedMessages.IndexOf(newItem.OfType<CommentEntity>().FirstOrDefault()) != 0) {
						Thread.Sleep(300);
					}
					var commentEntity = this.SortedMessages[0];
					var readingTarget = commentEntity.Message;
					foreach (var activeViceroid in this.ActiveVoiceroids.Where(x => !string.IsNullOrEmpty(x.Command))) {
						if (readingTarget.Contains(activeViceroid.Command)) {
							var replacedTarget = readingTarget.Replace(activeViceroid.Command, "");
							this.LastPlay = DateTime.Now;
							this.talkTextBox_.EmulateChangeText($"{activeViceroid.CharaName}＞{replacedTarget}");
							this.playButton_.EmulateClick();
							Thread.Sleep(300);
							while (this.IsTalking) {
								Thread.Sleep(500);
							}
							this.Messages.Remove(commentEntity);
							return;
						}
					}
					this.LastPlay = DateTime.Now;
					this.talkTextBox_.EmulateChangeText($"{this.CharaName}＞{readingTarget}");
					this.playButton_.EmulateClick();
					Thread.Sleep(300);
					while (this.IsTalking) {
						Thread.Sleep(500);
					}
					this.Messages.Remove(commentEntity);
				}
			});
		}

		/// <summary>
		/// <see cref="Messages.Count"/>の数が変化したときに呼び出されます。
		/// </summary>
		/// <param name="sender"><see cref="Messages"/></param>
		/// <param name="e">イベント<see cref="NotifyCollectionChangedEventArgs"/></param>
		private void OnMessageCollectionChenged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Remove || !this.IsV2Connected) {
				return;
			}
			this.Talking(e.NewItems);
		}

		/// <summary>
		/// VOICEROID2のプロセスからウインドウを取得します。
		/// </summary>
		/// <param name="vProcess">VOICEROID2の<see cref="Process"/></param>
		private void AttachV2Editer(Process vProcess)
		{
			this.IsV2Connected = false;
			this.voiceroidProcess_ = vProcess;
			this.voiceroidProcess_.Exited += this.OnV2Exit;

			try {
				this.voiceroidEditer_ = new WindowsAppFriend(this.voiceroidProcess_);
				this.uiTreeTop_ = WindowControl.FromZTop(this.voiceroidEditer_);

				this.TextViewCollextion = this.uiTreeTop_.GetFromTypeFullName(TALKEDITERVIEWNAME)[0].LogicalTree(TreeRunDirection.Descendants);

				if (this.TextViewCollextion.Count < 15) {
					return;
				}
#if DEBUG
				Debug.WriteLine("-----------------------------------------------");
				for (int i = 0; i < this.TextViewCollextion.Count; i++) {
					Debug.WriteLine($"アイテムID:{i}");
					Debug.WriteLine($"{this.TextViewCollextion[i]}");
				}
				Debug.WriteLine("-----------------------------------------------");
#endif

				this.talkTextBox_ = new WPFTextBox(this.textViewCollection_[4]);
				this.playButton_ = new WPFButtonBase(this.TextViewCollextion[6]);
				this.beginButton_ = new WPFButtonBase(this.TextViewCollextion[15]);
				this.LastPlay = DateTime.Now;
				this.talkTextBox_.EmulateChangeText("起動準備中、しばらくお待ちください。");
				this.playButton_.EmulateClick();

				this.IsV2Connected = true;
			}
			catch (Exception e) {
				Debug.WriteLine(e);
			}
		}

		/// <summary>
		/// VOICEROID2を起動します。
		/// </summary>
		/// <returns>起動したVOICEROID2の<see cref="Process"/></returns>
		private Process StartV2()
		{
			var p = new Process();
			p.StartInfo.FileName = VOICEROID2PATH;
			p.Start();
			p.WaitForInputIdle();
			Thread.Sleep(4000);
			return p;
		}
		#endregion
		//ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
		#region // メンバ変数
		private static string INSTALLFOLDERPATH = @"C:\Program Files (x86)\AHS\VOICEROID2";
		private static string VOICEROID2PATH = @"C:\Program Files (x86)\AHS\VOICEROID2\VoiceroidEditor.exe";
		private static string TALKEDITERVIEWNAME = "AI.Talk.Editor.TextEditView";
		private static string MAINWINDOWNAME = "AI.Talk.Editor.MainWindow";
		private static int MAXRETRYCOUNT = 5;
		private readonly object lockObject_ = new object();
		private WindowsAppFriend voiceroidEditer_;
		private Process voiceroidProcess_;
		private WindowControl uiTreeTop_;
		private WPFTextBox talkTextBox_;
		private WPFButtonBase playButton_;
		private WPFButtonBase beginButton_;
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
		}
		#endregion
	}
}
