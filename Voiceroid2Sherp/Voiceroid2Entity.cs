using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Voiceroid2Sharp
{
    public class Voiceroid2Entity : BindableBase
    {
        /// <summary>キャラクター名 を取得、設定</summary>
        private string charaName_;
        /// <summary>キャラクター名 を取得、設定</summary>
        public string CharaName
        {
            get => this.charaName_;

            set => this.SetProperty(ref this.charaName_, value);
        }

        /// <summary>コマンド を取得、設定</summary>
        private string command_;
        /// <summary>コマンド を取得、設定</summary>
        public string Command
        {
            get => this.command_;

            set => this.SetProperty(ref this.command_, value);
        }
        public Voiceroid2Entity()
        {

        }
    }
}
