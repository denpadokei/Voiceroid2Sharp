using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Voiceroid2Sharp.Core.Models;

namespace Voiceroid2Sharp.Core.Interfaces
{
    public interface IVoiceroid2 : IDisposable
    {
        event OnVoiceroid2ConnectedHadler OnConnected;
        event OnVoiceroid2DisconnectedHadler OnDisconnected;
        ReadOnlyDictionary<string, string> Voiceroids { get; }
        ConcurrentBag<Voiceroid2Entity> ActiveVoiceroids { get; }
        bool IsConnected { get; }
        bool IsPlaying { get; }
        DateTime LastPlay { get; }
        string CurrentVoiceroid { get; set; }
        void Connect(bool autoStart);
        Task ConnectAsync(bool autoStart);
        void Disconnect();
        void CreateActiveVoiceroidCollection();
        void Talk(IEnumerable<CommentEntity> messages);
        void Talk(params string[] messages);
        Task TalkAsync(IEnumerable<CommentEntity> messages);
        Task TalkAsync(params string[] messages);
    }
    public delegate void OnVoiceroid2ConnectedHadler(object sender, EventArgs e);
    public delegate void OnVoiceroid2DisconnectedHadler(object sender, EventArgs e);
}
