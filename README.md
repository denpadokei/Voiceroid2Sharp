# Voiceroid2Sharp
VOICEROID2を外部から操作するライブラリ

# 使い方
## 初期化
V2Connectを走らせることでVOICEROID2Editerを探して接続します。
また、立ち上がっていない場合は自動で起動することもできます。
CreateVoiceroidActiveCollectionを実行すると読み上げ可能なVOICEROID2を検索してActiveVoiceroidに追加します。
## 読み上げ
Messageに文字列を代入するか、MessagesにAddすると順番に読み上げます。
## その他
ActiveVoiceroidsの中のCommandにコマンドを登録してあげることで、Messagesを読み上げるキャラをメッセージごとに指定できます。

また、VOICEROID2が32bitビルドのためこちらも32bitでビルドしないとうまく動きません。


たまにバグります。

## サンプル
コメントを100個連続で読み上げます。
        
        public void ReadOneHandredCommment()
        {
                var voiceroid2 = new Voiceroid2();
                voiceroid2.V2Connect(true);
                
                while (!this.voiceroid2Sharp_.IsV2Connected) {
                        Thread.Sleep(1000);
                }
                for (int i = 0; i < 100; i++) {
                this.voiceroid2Sharp_.Message = $"コメントその{i}";
                }
                while (this.voiceroid2Sharp_.Messages.Any() || this.voiceroid2Sharp_.IsTalking) {
                Thread.Sleep(500);
                }
                Console.WriteLine(this.voiceroid2Sharp_.Log);
        }
