# Voiceroid2Sharp
VOICEROID2を外部から操作するライブラリ

# 使い方
## 初期化
V2Connectを走らせることでVOICEROID2Editerを探して接続します。
また、立ち上がっていない場合は自動で起動することもできます。
CreateVoiceroidActiveCollectionを実行すると読み上げ可能なVOICEROID2を検索してActiveVoiceroidに追加します。
## 読み上げ
MessagesにAddすると順番に読み上げます。
## その他
ActiveVoiceroidsの中のCommandにコマンドを登録してあげることで、Messagesを読み上げるキャラをメッセージごとに指定できます。


たまにバグります。
