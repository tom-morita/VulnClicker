# VulnClicker

VulnClicker は、ゲーム・アプリ開発において作り込んでしまいがちな脆弱性を実際に体験し、セキュリティの重要性を学ぶための教材、いわゆる「やられアプリ」です。

C# / .NET で開発された、Windows向けのシンプルなクリッカーゲームとスコアサーバーで構成されています。一見すると普通のゲームですが、セキュリティ学習を目的として、意図的にさまざまな問題を含んでいます。

デコンパイラなどの解析ツールを活用して EXE や DLL をリバースエンジニアリングし、ハードコーディングされた機微な情報を探したり、プログラムの処理やデータの流れを調査したりしてみてください。また、実装上の問題点を見つけ出し、本来想定されていない挙動をどのように引き起こせるのか、その原因と対策について考えてみてください。

本プロジェクトは、ソフトウェアの解析や攻撃手法そのものを目的とするのではなく、**「どのような実装が脆弱性につながるのか」「安全なアプリケーションを作るためにはどうすればよいのか」**を、実際に手を動かしながら学習することを目的としています。

[!WARNING]
本プロジェクトはセキュリティ教育を目的としています。

本プロジェクトを利用した検証や解析は、自身が所有・管理する環境、または明示的に許可された環境でのみ行ってください。
本プロジェクトを通じて得た知識や技術を、許可なく第三者のソフトウェア、システム、サービス等の解析・攻撃に利用しないでください。



## 機能

## ゲームのルールについて

### 表：クリッカーゲームとして

- クリックすると点数が増えるボタンと点数が減るボタンがランダムな位置に出現するクリッカーゲーム
- 制限時間は30秒でハイスコアを目指す。
- ヒット：点数が増えるボタンをクリックする。点数は＋１
- ミス：点数が増えるボタンをクリックする。点数は-１
- コンボ：連続でヒットすれば連続回数Nに応じて点数が＋N。ミスすればコンボは０から再スタートする。
- 専用のスコアサーバーを起動させればランキングにスコア登録が可能。

### 裏：やられアプリとして

- やられアプリの脆弱性を見つけて、スコアを改ざんし、ハイスコアを目指す。


## プロジェクトの構成

### クライアントアプリ

- Windows Forms ベースのクリックゲーム本体
- ヒット、ミス、コンボを記録しスコアを算出
- ゲームの一時停止／再開
- ゲームデータのエクスポートとインポート
- ランキングサーバーへのスコア送信
- ランキングの表示
- HTTP／HTTPS 通信
- アクティベーション機能

### サーバーアプリ

- ASP.NET Core ミニマル API
- HTTP および HTTPS エンドポイント
- スコアの登録
- トップ 10 ランキング
- スコア／コンボランキング
- スコアの検証
- 設定可能なデフォルトランキング
- JSON ベースのスコア保存
- 難読化ツールの一つ[Obfuscar](https://docs.lextudio.com/obfuscar/) 対応のリリースビルド

### pythonベースの解析支援ツール
- bese64_calculator.py :
   アプリのスコア改ざん検知用ハッシュ計算やBase64で変換された文字列のエンコード/デコード支援ツール
- httpreq2curl.py :
   HTTPリクエストをコマンドラインで再送信できるようcURLやInvoke-WebRequestの形式に変換するツール


## プロジェクト外で利用するツール

### .NETデコンパイルツール
- [dnSpy](https://github.com/dnSpy/dnSpy/releases)：推奨
  - 使い方：[凄すぎて大草原不可避な.NET デコンパイラdnSpyを使ってみる](https://qiita.com/Tokeiya/items/54fbf30cb21c77c05c41)
- [ILSpy](https://github.com/icsharpcode/ILSpy/releases)：動作未検証
  - 使い方：[ILSpyで.NETのアセンブリを逆コンパイルしてソースコードを参照する C#](https://johobase.com/ilspy/)
    
### パケットキャプチャ
- [WireShark](https://www.wireshark.org/)

### コマンドラインツール
- PowerShell
- コマンドプロンプト（cmd.exe）
- レジストリ エディター (regedit.exe) 


## 要件
- Windows
- .NET 10 SDK
- Git：任意

インストールされている .NET のバージョンを確認してください：

```powershell
dotnet --version
```

## リポジトリ構造

```text
/VulnClicker
│
├─── README.md
│
├─VulnClicker
│  │  Program.cs
│  │  VulnClicker.csproj
│  │
│  ├─Forms
│  │      Activation.cs
│  │      Activation.Designer.cs
│  │      Clicker.cs
│  │      Clicker.Designer.cs
│  │      RegisterResultForm.cs
│  │      RegisterResultForm.Designer.cs
│  │      TitleForm.cs
│  │      TitleForm.Designer.cs
│  │
│  ├─Models
│  │      GameSaveData.cs
│  │      GameState.cs
│  │      RankingEntry.cs
│  │      ScoreEntry.cs
│  │      StartOptions.cs
│  │      StartUpInfo.cs
│  │      StartUpInfoManager.cs
│  │      TitleThema.cs
│  │
│  └─Services
│          ExportData.cs
│          ImportData.cs
│          ScoreClient.cs
│
└─VulnClickerScoreServer
│  │  appsettings.json
│  │  obfuscar.xml
│  │  obfuscation.json
│  │  Program.cs
│  │  protections.json
│  │  VulnClickerScoreServer.csproj
│  │
│  ├─Data
│  │      scores.json
│  │
│  ├─Models
│  │      ScoreEntry.cs
│  │
│  ├─Properties
│  │      launchSettings.json
│  │
│  └─Services
│          ScoreRepository.cs
└─tools
       bese64_calculator.py
       httpreq2curl.py
       request.txt

```

## ゲームアプリのビルド

リポジトリをクローンし、.NET CLI を使用してプロジェクトをビルドします。
VulnClickerはアプリ解析の教材としてデバッグビルドします。

```powershell
git clone <リポジトリURL>
cd VulnClicker

dotnet build
```

## サーバー処理の難読化

スコアサーバーはリリースビルド後に難読化処理を施しています。
難読化ツールのインストール

```powershell
dotnet tool install --global Obfuscar.GlobalTool
dotnet tool install --global obfuscar.console
obfuscar.console --help
```

難読か設定はobfuscar.xmlに記述している。

```powershell
cd VulnClickerScoreServer
dotnet clean -c Release
dotnet publish -c Release
obfuscar.console .\obfuscar.xml
```

難読化が成功すれば、生成された難読化済みdllを既存のdllと差し替える。

```powershell
cd VulnClickerScoreServer
mkdir .\bin\Release\net10.0\publish_obfus
Remove-Item `
    ".\bin\Release\net10.0\publish_obfus" `
    -Recurse -Force `
    -ErrorAction SilentlyContinue

Copy-Item `
    ".\bin\Release\net10.0\publish" `
    ".\bin\Release\net10.0\publish_obfus" `
    -Recurse

Copy-Item `
    ".\bin\Release\net10.0\obfuscated\VulnClickerScoreServer.dll" `
    ".\bin\Release\net10.0\publish_obfus\VulnClickerScoreServer.dll" `
    -Force
cd .\bin\Release\net10.0\publish_obfus
.\VulnClickerScoreServer.exe
```


### サーバーの実行

実行後に
```powershell
cd VulnClickerScoreServer
dotnet run
```

### クライアントの実行

別のターミナルを開きます：

```powershell
cd VulnClicker
dotnet run
```

オンラインランキング機能を使用する前に、スコアサーバーを起動してください。


## API

クライアントであるゲームアプリからスコアサーバーへさまざまな通信を行います。
API仕様は非公開です。
 - スコア登録
 - ランキング取得（スコア順）
 - ランキング取得（コンボ数順）
 - その他、隠しAPI

スコア登録は、HTTPリクエストでjson形式でPOST送信を行います。
WireSharkで通信を観測することができればAPI仕様を把握する手掛かりとなります。
スコアサーバーはローカルホスト上で起動しているため、WireSharkでは以下の設定を選択してください。
 - インターフェース: "adapter for loopback traffic capture"
 - ディスプレイフィルター: "http||tls"


## サーバーの設定

基本的には設定変更不要ですが、サーバーの動作は `appsettings.json` を通じて設定できます。

設定オプションには、次のような機能があります：

- HTTPS リダイレクト
- スコアの検証
- バージョンの検証
- サーバーのバージョン
- ランキングの上限
