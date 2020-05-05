using Codeer.Friendly;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using Prism.Commands;
using Prism.Mvvm;
using RM.Friendly.WPFStandardControls;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Voiceroid2Sharp
{
    public class Voiceroid2Sharp : BindableBase
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

			set => this.SetProperty(ref this.isv2Connected_, value);
		}

		/// <summary>読み上げ予定のテキスト集 を取得、設定</summary>
		private ObservableSynchronizedCollection<string> messages_;
		/// <summary>読み上げ予定のテキスト集 を取得、設定</summary>
		public ObservableSynchronizedCollection<string> Messages
		{
			get => this.messages_;

			set => this.SetProperty(ref this.messages_, value);
		}

		/// <summary><see cref="AppVar"> を取得、設定</summary>
		private AppVar statusLabel_;
		/// <summary><see cref="AppVar"> を取得、設定</summary>
		public AppVar StatusLabel
		{
			get => this.statusLabel_;

			set => this.SetProperty(ref this.statusLabel_, value);
		}

		/// <summary>話し中かどうか を取得、設定</summary>
		public bool IsTalking => !this.StatusLabel.ToString().Contains("テキストの読み上げは完了しました。");
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
					Thread.Sleep(3000);
					this.AttachV2Editer(voiceroidProcess[0]);
					retrycount++;
				}
			}
			else if (autoStart) {
				var p = this.StartV2();
				while (retrycount < MAXRETRYCOUNT && !this.IsV2Connected) {
					Thread.Sleep(3000);
					this.AttachV2Editer(p);
					retrycount++;
				}
			}

			if (this.IsV2Connected) {
				this.Messages.Add("VOICEROID2と接続しました。");
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
			this.OnExitVoiceroid2?.Invoke("VOICEROID2が終了しました。");
		}

		/// <summary>
		/// 文字列をVOICEROID2に送り再生ボタンをクリックします。
		/// 読み上げるキャラは<see cref="this.CharaName"/>を参照します。
		/// ただし、コマンドが入力されている場合はコマンドのキャラで読み上げます。
		/// </summary>
		private async void Talking()
		{
			await Task.Run(() =>
			{
				var lastPlay = DateTime.Now;
				var talkCooldown = TimeSpan.FromSeconds(0.3);
				while (this.IsTalking) {
					Thread.Sleep(50); // spin wait
				}
				var now = DateTime.Now;
				if ((now - lastPlay) < talkCooldown) {
					Thread.Sleep(talkCooldown - (now - lastPlay));
				}
				lastPlay = DateTime.Now;
				var readingTarget = this.Messages[0];
				foreach (var activeViceroid in this.ActiveVoiceroids.Where(x => !string.IsNullOrEmpty(x.Command))) {
					if (readingTarget.Contains(activeViceroid.Command)) {
						var replacedTarget = readingTarget.Replace(activeViceroid.Command, "");
						this.talkTextBox_.EmulateChangeText($"{activeViceroid.CharaName}＞{replacedTarget}");
						this.playButton_.EmulateClick();
						this.Messages.Remove(readingTarget);
						return;
					}
				}
				this.talkTextBox_.EmulateChangeText($"{this.CharaName}＞{readingTarget}");
				this.playButton_.EmulateClick();
				this.Messages.Remove(readingTarget);
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
			this.Talking();

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

				var textViewCollection = this.uiTreeTop_.GetFromTypeFullName(TALKEDITERVIEWNAME)[0].LogicalTree(TreeRunDirection.Descendants);

				if (textViewCollection.Count < 15) {
					return;
				}

				if (string.IsNullOrEmpty(textViewCollection[4].ToString())) {
					return;
				}

				this.talkTextBox_ = new WPFTextBox(textViewCollection[4]);
				this.playButton_ = new WPFButtonBase(textViewCollection[6]);
				this.talkTextBox_.EmulateChangeText("起動準備中、しばらくお待ちください。");
				this.playButton_.EmulateClick();

				var mainViewCollection = this.uiTreeTop_.GetFromTypeFullName(MAINWINDOWNAME)[0].LogicalTree(TreeRunDirection.Descendants);
				Debug.WriteLine("-------------------------------------------------------");

				for (int i = 0; i < mainViewCollection.Count; i++) {
					try {
						if (mainViewCollection[i].ToString().Contains("読み上げ")) {
							Debug.WriteLine($"アイテムID{i}");
							Debug.WriteLine($"{mainViewCollection[i]}");
							this.StatusLabel = mainViewCollection[i];
							break;
						}
					}
					catch (Exception e) {
						Debug.WriteLine(e);
						break;
					}
				}
				Debug.WriteLine("-------------------------------------------------------");
				this.IsV2Connected = true;
				return;
			}
			catch (Exception e) {
				Debug.WriteLine(e);
				return;
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
		private static int MAXRETRYCOUNT = 3;
		private WindowsAppFriend voiceroidEditer_;
		private Process voiceroidProcess_;
		private WindowControl uiTreeTop_;
		private WPFTextBox talkTextBox_;
		private WPFButtonBase playButton_;
		#endregion
		//ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
		#region // 構築・破棄
		public Voiceroid2Sharp()
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


			this.Messages = new ObservableSynchronizedCollection<string>();
			this.Messages.CollectionChanged += this.OnMessageCollectionChenged;
		}
		#endregion

	}
}
