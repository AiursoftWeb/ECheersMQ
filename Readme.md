
# Echeers Mq - 切尔思消息队列

[![Build Status](https://travis-ci.org/Echeers/Mq.svg?branch=master)](https://travis-ci.org/Echeers/Mq)

这是一个可以被独立部署运行的小型消息队列。

## 如何运行

**虽然这个项目是C#写的，它仍然具有非常出色的跨平台性。你可以在任何你喜欢的桌面和服务器平台下运行它！**

### 依赖项

* [.NET Core 2.0 SDK](https://www.microsoft.com/net)
* [npm](https://nodejs.org)

### 使用命令运行

请在下载项目的全部文件后，在项目根目录中运行：

    $ chmod +x ./resetdb.sh && ./resetdb.sh
    $ npm install ./wwwroot
    $ dotnet restore
    $ dotnet run

* 第一个命令将会创建数据库。如果你不修改任何配置，该程序会使用SQLite数据库，并将数据库创建在该目录下的app.db文件中。
* 即使这样可以启动这个项目并且使用它的全部功能，仍然不要在生产环境中这样部署。因为dotnet run命令仅仅用于调试使用。
* 第一个命令仅适用于Linux。针对Windows操作系统，请手动完成resetdb.sh中的工作。

### 在Visual Studio中运行

首先安装 Visual Studio 2017，并为其安装 .NET Core 开发工具包.

1. 双击 `Echeers.Mq.csproj`.
2. 猛击 `F5`.

## 如何部署于生产环境

请在项目本身的目录中执行下面命令:

    $ npm install
    $ dotnet restore
    $ dotnet publish

**该步骤不负责准备数据库。请提前将用于生产环境的数据库准备好，并调整appsettings.json和Startup.cs文件中的数据库连接配置信息**

如果你已经安装了IIS，直接在IIS中新建一个以下面地址为路径的站点:

    ./bin/Debug/netcoreapp2.0/publish/

如果你没有IIS，请在上面路径下，运行`dotnet ./Echeers.Mq.dll`

## 切尔思消息队列是什么

它本身作为应用平台，允许开发人员在该平台上管理自定义的应用信息，然后凭应用信息换取凭据信息。

它本身作为广播平台，允许开发人员在其中为自己的特定应用新建若干个频道。不同应用之间不能访问对方的频道。每个频道都会有连接秘钥。

它本身作为推送工具，客户端可以使用WebSocket协议连接到特定的频道。如果在客户端连接频道后，开发者对频道推送了任何信息，所有该频道的客户端都会收到该信息。

频道本身的生命是24小时。凭据本身的生命是20分钟。客户端错过的消息不负责重传。客户端一旦连接某个频道，则不会丢掉连接后的任何新推送的消息，除非网络中断。

它的应用信息、凭据信息、频道信息都存在它本身的数据库中。它的全部消息都存在它本身的内存中。每个消息一旦被所有客户端接收到则会被立即清除。因此它不负责存储任何消息。

它不负责检查客户端是真正阅读了消息，也不负责接受客户端对其发出的任何消息。它只是把所有消息都忠实的推给每一个客户端。

## 如何通过API访问它


### 换取AccessToken

请求地址：

    /api/AccessToken?appid={Your App Id}&appsecret={Your App Secret}

方法：  

    HTTP GET

接口说明：

    本接口能够将AppId和AppSecret换成AccessToken。请先登录本应用并创建一个App来获取AppId，和AppSecret，再利用本接口来换取AccessToken。

    任何情况下不应该将AppId和AppSecret发给其它服务器、发给客户端或传递给别人。

返回值示例：

    {
        "accessToken": "eaae66eae336cbd56bb48dbd810f1551",
        "deadTime": "2018-01-23T15:32:07.9299339+08:00",
        "code": 0,
        "message": "Successfully get access token."
    }

### 查询我的所有频道

请求地址：

    /Channel/ViewMyChannels?AccessToken=eaae66eae336cbd56bb48dbd810f1551

方法：  

    HTTP GET

接口说明：

    本接口能够针对一个App查询其所有频道。

返回值示例：

    {
        "appId": "6abfa8e1b43a99b36dc82acf57467787",
        "channel": [
            {
                "id": 1,
                "description": "Postman Channel",
                "connectKey": "70YYRYO4A18BV5UX6WNU",
                "createTime": "2018-01-23T15:13:41.5145618",
                "lifeTime": "23:59:59",
                "appId": "6abfa8e1b43a99b36dc82acf57467787"
            },
            {
                "id": 2,
                "description": "Postman Channel2",
                "connectKey": "R4J7OOVDAIDJPYYS6V35",
                "createTime": "2018-01-23T15:13:44.2688574",
                "lifeTime": "23:59:59",
                "appId": "6abfa8e1b43a99b36dc82acf57467787"
            }
        ],
        "code": 0,
        "message": "Successfully get your channels!"
    }

### 创建一个新的频道

请求地址：

    /Channel/CreateChannel

方法：  

    HTTP POST

表单：

    AccessToken={Your Access Token}&Description={Describe your channel}

表单编码：

    x-www-form-urlencoded

接口说明：

    本接口能够针对一个App创建一个新频道。其中，Description不是必需的参数。频道创建后会保留24小时。

返回值示例：

    {
        "channelId": 3,
        "connectKey": "SR943GQSQQ8ULRB6OHFJ",
        "code": 0,
        "message": "Successfully created your channel!"
    }

### 验证一个频道是否仍然存在

请求地址：

    /Channel/ValidateChannel?Id={Your ID}&Key={Connect Key}

方法：  

    HTTP GET

接口说明：

    本接口能够检查一个频道是否存在，以及是否能够使用特定Key连接。本接口不需要AccessToken。

返回值示例：

    // 频道存在且连接Key正确
    {
        "code": 0,
        "message": "Corrent Info."
    }

    // 频道存在但Key不正确
    {
        "code": -8,
        "message": "Wrong connection key!"
    }

    // 频道不存在
    {
        "code": -4,
        "message": "Can not find your channel!"
    }

### 在特定频道推送一条消息

请求地址：

    /Message/PushMessage

方法：  

    HTTP POST

表单：

    AccessToken={Your Access Token}&ChannelId={Target Channel Id}&MessageContent={Some Text Or Json Or XML}

表单编码：

    x-www-form-urlencoded

接口说明：

    本接口能够向特定频道推送特定内容的消息。内容可以是任意类型。注意，参数请进行URL编码。

返回值示例：

    {
        "code": 0,
        "message": "You have successfully created a message at channel:2!"
    }

### 收听一个频道

请求地址：

    ws://localhost:5000/Listen/Channel/{Your Channel Id}?Key={Connect Key}

请求方法：

    WebSocket

注意：
    如果你的站点已经支持了HTTPS，请将地址中的`ws`改为`wss`。

说明：

使用WebSocket连接后，所有该频道的消息都会通过WebSocket的onmessage事件被客户端触发。请参考[前端调用方法](./Views/Home/Test.cshtml)。

如果你想调试你的WebSocket接口而不想开发任何代码，可以使用你喜爱的浏览器访问`/Home/Test`。这里是一个非常简洁的WebSocket客户端。

服务器只会把最原生的事件内容推送过来，不会进行任何编码，也不会使用Json或XML。