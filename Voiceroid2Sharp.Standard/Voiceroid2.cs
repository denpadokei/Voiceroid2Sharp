using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using RM.Friendly.WPFStandardControls;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Voiceroid2Sharp.Standard.Interfaces;
using Voiceroid2Sharp.Standard.Models;

namespace Voiceroid2Sharp.Standard
{
    public class Voiceroid2 : IVoiceroid2
    {
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // プロパティ
        public ReadOnlyDictionary<string, string> Voiceroids { get; private set; }

        public ConcurrentBag<Voiceroid2Entity> ActiveVoiceroids { get; } = new ConcurrentBag<Voiceroid2Entity>();

        public bool IsConnected { get; private set; }

        public bool IsPlaying => (bool)this._textEditViewDataContext.IsPlaying;
        public DateTime LastPlay { get; private set; }
        public string CurrentVoiceroid { get; set; }
        private bool IsOpen => Process.GetProcessesByName(this._voiceroid2Process.ProcessName)[0].MainWindowHandle != IntPtr.Zero;
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // パブリックイベント
        public event OnVoiceroid2ConnectedHadler OnConnected;
        public event OnVoiceroid2DisconnectedHadler OnDisconnected;
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // ハンドラー
        public delegate void OnVoiceroid2ConnectedHadler(object sender, EventArgs e);
        public delegate void OnVoiceroid2DisconnectedHadler(object sender, EventArgs e);
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // オーバーライドメソッド
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // パブリックメソッド
        public void Connect(bool autoStart)
        {
            if (!File.Exists(VOICEROID2PATH)) {
                this.IsConnected = false;
                Debug.WriteLine("VOICEROID2がインストールされていません。");
                return;
            }

            if (!this.ActiveVoiceroids.Any()) {
                this.CreateActiveVoiceroidCollection();
            }

            var retrycount = 0;

            var voiceroidProcess = Process.GetProcessesByName("VoiceroidEditor");
            Debug.WriteLine("VoiceroidEditor Length:" + voiceroidProcess.Length.ToString());
            if (voiceroidProcess.Any()) {
                while (retrycount < MAXRETRYCOUNT && !this.IsConnected) {
                    this.AttachV2Editer(voiceroidProcess[0]);
                    retrycount++;
                    if (!this.IsConnected) {
                        Thread.Sleep(3000);
                    }
                }
            }
            else if (autoStart) {
                var p = this.LaunchVoiceroid2();
                while (retrycount < MAXRETRYCOUNT && !this.IsConnected) {
                    this.AttachV2Editer(p);
                    retrycount++;
                    if (!this.IsConnected) {
                        Thread.Sleep(3000);
                        p.Refresh();
                        p.Dispose();
                        p = Process.GetProcessesByName("VoiceroidEditor").FirstOrDefault();
                    }
                }
            }

            if (this.IsConnected) {
                this.Talk("VOICEROID2と接続しました。");
                this.OnConnected?.Invoke(this, EventArgs.Empty);
            }
        }
        public Task ConnectAsync(bool autoStart) => Task.Run(() => this.Connect(autoStart));
        public void Disconnect()
        {
            Debug.WriteLine("VOICEROID2終了を検出");
            this._tokenSource?.Cancel();
            this._voiceroid2Process?.Kill();
            this._voiceroid2Process?.Dispose();
            this._voiceroidEditer?.Dispose();
            this._textEditerView?.Dispose();
            this._mainWindow = null;
            this._talkTextBox = null;
            this._playButton = null;
            this._textViewCollection = null;
            Debug.WriteLine("VOICEROIDと切断しました。");
            this.IsConnected = false;
            this.OnDisconnected?.Invoke(this, EventArgs.Empty);
        }
        public void Talk(IEnumerable<CommentEntity> messages)
        {
            lock (_lockObject) {
                try {
                    if (!this.IsConnected) {
                        return;
                    }
                    if (!this.IsOpen && this.IsConnected) {
                        this.Disconnect();
                        return;
                    }
                    this._tokenSource = new CancellationTokenSource();
                    foreach (var commentEntity in messages) {
                        if (!this.IsOpen) {
                            return;
                        }
                        var readingTarget = commentEntity.Message;

                        var anyCommand = this.ActiveVoiceroids
                            .Where(x => !string.IsNullOrEmpty(x.Command))
                            .Any(x => Regex.IsMatch(readingTarget, $"^{x.Command.Replace(")", @"\)")}"));
                        if (anyCommand) {
                            foreach (var activeViceroid in this.ActiveVoiceroids.Where(x => !string.IsNullOrEmpty(x.Command))) {
                                var regex = new Regex($"^{activeViceroid.Command.Replace(")", @"\)")}");
                                if (regex.IsMatch(readingTarget)) {
                                    var replacedTarget = regex.Replace(readingTarget, "");
                                    this.LastPlay = DateTime.Now;
                                    this._talkTextBox.EmulateChangeText($"{activeViceroid.VoiceroidName}＞{replacedTarget}");
                                    //this.WriteLog($"{activeViceroid.CharaName}＞{replacedTarget}");
                                    this._playButton.EmulateClick();
                                    Thread.Sleep(300);
                                    while (this.IsPlaying) {
                                        if (!this.IsOpen || this._tokenSource.IsCancellationRequested) {
                                            break;
                                        }
                                        Thread.Sleep(500);
                                    }
                                    break;
                                }
                            }
                        }
                        else {
                            this.LastPlay = DateTime.Now;
                            this._talkTextBox.EmulateChangeText($"{this.CurrentVoiceroid}＞{readingTarget}");
                            //this.WriteLog($"{this.CharaName}＞{readingTarget}");
                            this._playButton.EmulateClick();
                            Thread.Sleep(300);
                            while (this.IsPlaying) {
                                if (!this.IsOpen || this._tokenSource.IsCancellationRequested) {
                                    break;
                                }
                                Thread.Sleep(500);
                            }
                        }
                    }
                }
                catch (Exception e) {
                    Debug.WriteLine($"{e}");
                }
            }
        }
        public void Talk(params string[] messages) => this.Talk(messages.Select(x => new CommentEntity(x)));
        public Task TalkAsync(IEnumerable<CommentEntity> messages) => Task.Run(() => this.Talk(messages));
        public Task TalkAsync(params string[] messages) => Task.Run(() => this.Talk(messages));
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // プライベートメソッド
        /// <summary>
		/// VOICEROID2のプロセスからウインドウを取得します。
		/// </summary>
		/// <param name="vProcess">VOICEROID2の<see cref="Process"/></param>
		private void AttachV2Editer(Process vProcess)
        {
            this.IsConnected = false;
            this._voiceroid2Process = vProcess;
            try {
                this._voiceroidEditer = new WindowsAppFriend(this._voiceroid2Process);

                this._mainWindow = new WindowControl(this._voiceroidEditer, this._voiceroid2Process.MainWindowHandle);
                this._textEditerView = this._mainWindow.GetFromTypeFullName(TALKEDITERVIEWNAME).Single();
                var textEditer = this._textEditerView.Dynamic();
                this._textEditViewDataContext = textEditer.DataContext;
                this._textViewCollection = this._textEditerView.LogicalTree(TreeRunDirection.Descendants);

                if (this._textViewCollection.Count < 15) {
                    return;
                }
#if DEBUG
				Debug.WriteLine("-----------------------------------------------");
				for (int i = 0; i < this._textViewCollection.Count; i++) {
					Debug.WriteLine($"ItemID:{i}");
					Debug.WriteLine($"{this._textViewCollection[i]}");
				}
				Debug.WriteLine("-----------------------------------------------");
#endif
                this._talkTextBox = new WPFTextBox(this._textViewCollection[4]);
                this._playButton = new WPFButtonBase(this._textViewCollection[6]);
                this.IsConnected = true;
            }
            catch (Exception e) {
                Debug.WriteLine(e);
            }
        }

        /// <summary>
		/// インストールされているボイスロイドコレクションを作成します。
		/// </summary>
		private void CreateActiveVoiceroidCollection()
        {
            while (this.ActiveVoiceroids.TryTake(out _)) {

            }
            var voicePath = Path.Combine(INSTALLFOLDERPATH, "Voice");
            if (Directory.Exists(voicePath)) {
                var directories = Directory.EnumerateDirectories(voicePath);
                if (!directories.Any()) {
                    return;
                }
                foreach (var directory in directories) {
                    var directoryInfo = new DirectoryInfo(directory);
                    if (this.Voiceroids.TryGetValue(directoryInfo.Name, out var value)) {
                        this.ActiveVoiceroids.Add(new Voiceroid2Entity(value, ""));
                    }
                }
            }
            if (this.ActiveVoiceroids.Any()) {
                this.CurrentVoiceroid = this.ActiveVoiceroids.First().VoiceroidName;
            }
        }

        /// <summary>
		/// VOICEROID2を起動します。
		/// </summary>
		/// <returns>起動したVOICEROID2の<see cref="Process"/></returns>
		private Process LaunchVoiceroid2()
        {
            using (var p = new Process()) {
                p.StartInfo.FileName = VOICEROID2PATH;
                p.Start();
                p.WaitForInputIdle();
                Debug.WriteLine("VOICEROID2を起動中です。");
                Thread.Sleep(4000);
                return p;
            }
        }
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // メンバ変数
        private static readonly string INSTALLFOLDERPATH = @"C:\Program Files (x86)\AHS\VOICEROID2";
        private static readonly string VOICEROID2PATH = @"C:\Program Files (x86)\AHS\VOICEROID2\VoiceroidEditor.exe";
        private static readonly string TALKEDITERVIEWNAME = "AI.Talk.Editor.TextEditView";
        private static readonly int MAXRETRYCOUNT = 5;

        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private WindowsAppFriend _voiceroidEditer;
        private Process _voiceroid2Process;
        private WindowControl _mainWindow;
        private AppVar _textEditerView;
        private WPFTextBox _talkTextBox;
        private WPFButtonBase _playButton;
        private CancellationTokenSource _tokenSource;
        private bool disposedValue;
        private dynamic _textEditViewDataContext;
        private IWPFDependencyObjectCollection<DependencyObject> _textViewCollection;
        private static object _lockObject = new object();
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // 構築・破棄
        public Voiceroid2()
        {
            var voiceroidsDic = new Dictionary<string, string>();

            voiceroidsDic.Add("yukari_44", "結月ゆかり(v1)");
            voiceroidsDic.Add("ai_44", "月読アイ(v1)");
            voiceroidsDic.Add("akane_west_44", "琴葉 茜(v1)");
            voiceroidsDic.Add("aoi_44", "琴葉 葵(v1)");
            voiceroidsDic.Add("kiritan_44", "東北きりたん(v1)");
            voiceroidsDic.Add("kou_44", "水奈瀬コウ(v1)");
            voiceroidsDic.Add("seika_44", "京町セイカ(v1)");
            voiceroidsDic.Add("shouta_44", "月読ショウタ(v1)");
            voiceroidsDic.Add("tamiyasu_44", "民安ともえ(v1)");
            voiceroidsDic.Add("yoshidakun_44", "鷹の爪 吉田くん(v1)");
            voiceroidsDic.Add("zunko_44", "東北ずん子(v1)");
            voiceroidsDic.Add("yukari_emo_44", "結月ゆかり");
            voiceroidsDic.Add("ai_4emo_4", "月読アイ");
            voiceroidsDic.Add("akane_west_emo_44", "琴葉 茜");
            voiceroidsDic.Add("aoi_emo_44", "琴葉 葵");
            voiceroidsDic.Add("kiritan_emo_44", "東北きりたん");
            voiceroidsDic.Add("kou_emo_44", "水奈瀬コウ");
            voiceroidsDic.Add("seika_emo_44", "京町セイカ");
            voiceroidsDic.Add("shouta_emo_44", "月読ショウタ");
            voiceroidsDic.Add("tamiyasu_emo_44", "民安ともえ");
            voiceroidsDic.Add("yoshidakun_emo_44", "鷹の爪 吉田くん");
            voiceroidsDic.Add("zunko_emo_44", "東北ずん子");
            voiceroidsDic.Add("akari_44", "紲星あかり");
            voiceroidsDic.Add("sora_44", "桜乃そら");

            this.Voiceroids = new ReadOnlyDictionary<string, string>(voiceroidsDic);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                    this._semaphoreSlim?.Dispose();
                    this._voiceroidEditer?.Dispose();
                    this._voiceroid2Process?.Dispose();
                    this._textEditerView?.Dispose();
                    this._tokenSource?.Dispose();
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~Voiceroid2Sharp()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
