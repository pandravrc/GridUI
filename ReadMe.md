# 機能
最大20種類を一覧表示から選択できるUIを表示し、結果を同期Intに格納します。

# インストール方法
gridUI.unitypackageをインポートして下さい。

# アンインストール方法
Assets/Pan/GridUIフォルダを削除して下さい。

# 何に使うもの？
エモートなど大量の選択肢から1つを選択したいときに使います。

# 使用法
Res/MENUの画像を実際のメニューに差し替えてください。
GridUIプレハブをアバタールートに入れてください。
配布時点では17番目(18個）のメニューまでが存在するサンプルになっています。変更するには、GridUIプレハブの「MA Parameters」の「Pan_GridUI_InLimit」の初期値を変更して下さい（最大19）。
結果に応じたアニメーションは次の2つの方法で設定できます。やりやすい方でどうぞ。
1. 「Pan_GridUI_n」というParameter(Ex:Int, FX:Float)に選択したアイコンの番号が入るので、それを使って自力でAnimatorを作成
2. 「Assets/Pan/GridUI/nReceiver/nReceiver.controller」に各選択に応じたステートがあるので、そこにAnimファイルを設定

# 注意
- デフォルトでは自分のみに見えます。他の人に見せたい場合は自動検出ではない4つのパラメータ全ての同期が必要です。(+25bit消費します)
- 毎F処理をしているので環境によって重くなる可能性があります。（ローカル動作なので問題は小さいと思います）、あまりテストしていません。

# Source Code
- https://github.com/pandravrc/gridUI

# 依存関係
- ModularAvatar
https://modular-avatar.nadena.dev/ja
- lilToon
https://lilxyzw.github.io/lilToon/

# 開発・動作環境
次の環境で開発し、動作を確認しています。
- VRChat SDK 3.3.0
- Av3 Emulator 3.3.1

# その他
- 配布物は5x4ですが他のサイズも用意可能です。ご相談下さい。

# ライセンス
MIT ライセンスに基づきます。
