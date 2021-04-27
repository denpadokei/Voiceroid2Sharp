using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Voiceroid2Sharp.Standard.Models;

namespace Voiceroid2Sharp.Standard.Interfaces
{
    public interface IVoiceroid2 : IDisposable
    {
        ReadOnlyDictionary<string, string> Voiceroids { get; }
        ConcurrentBag<Voiceroid2Entity> ActiveVoiceroids { get; }
        bool IsConnected { get; }
        bool IsPlaying { get; }
        DateTime LastPlay { get; }
        string CurrentVoiceroid { get; set; }
        void Connect(bool autoStart);
        Task ConnectAsync(bool autoStart);
        void Disconnect();
        void Talk(IEnumerable<CommentEntity> messages);
        void Talk(params string[] messages);
        Task TalkAsync(IEnumerable<CommentEntity> messages);
        Task TalkAsync(params string[] messages);
    }
}
