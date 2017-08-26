# ABSystem
自制 unity AssetBundle 更新系统

## 特点

1. 异步下载, 不会阻塞主线程.
2. 支持下载进度查看.
3. 附带打包用的编辑器拓展, 方便打包时附带额外信息.

* * *

## 使用方式:

### 1. 将ABSystem导入到你的项目中, 你可以放置在Assets目录或其子目录中.

### 2. 将ABSystem的预制体添加到场景中.

### 3. 设置其中的参数, 各参数的含义和格式如下:

#### Remote Setting: 
> 远程服务器设置相关, 都是一些特定的uri.

* Remote Version URI:

> 获取远程版本URI, 将会以HTTP的GET的方法去请求该URI, 要求返回Json信息, 其格式为 **{"Version": "1.0.0"}** 这样的形式.

* Remote Asset Bundle List URI:

> 获得远程版本的AB包列表的信息, 将使用HTTP的GET方法请求该URI, 其在请求时会带有Version的请求参数, 例如: **ABList?Version=1.0.0**. 要求返回一个Json数组, 数组中的每一项表示一个AB包的信息, 例如: **[{"NAME1": "HASH1"}, {"NAME2": "HASH2"}]** . 每个Json对象的键为AB包的名称, 值为AB包的哈希值.

* Remote Asset Bundle Download Entry:

> 当下载AB包时, 使用的下载入口, 其请求的格式如下:  
  **Entry?Name=xxx&Version=xxx**  
一般情况下, 你可以根据其请求参数将其重定向的真正的下载URI中, 但是要注意, 下载的URI必须要同时支持HEAD方法和GET方法, 因为要使用HEAD方法
来获取要下载的包的大小. 另外, **只支持301和302重定向, 且重定向的次数只有1次**.

#### Local Setting:
> 本地应用相关.

* Asset Bundle Path:
> 用于储存下载的AB包的目录名称, 其将存放于Application.persistentDataPath下, 而所有下载的AB包就存放于这个指定的目录下

* Default Version:
> 当在上面设置的目录下找不到Version.json文件来读取版本信息时, 将使用的默认版本号.

#### AutoUpdate:
> 如果勾选上了, 其将会在Start函数中自动开始进行检查更新.

### 4. 运行场景
> 当然, 当打包成应用后, 其到了ABSystem所在的场景时, 就会自动运行.

## 附带的打包工具
> ABSystem提供了编辑器扩展来进行打包, 当你导入ABSystem到你的项目后, 你可以在菜单栏中找到 *ABSystem->Create AssetBundles* , 其里面有几个设置可以按需调节.

* Create Version Info: 
> 是否创建版本信息, 即是否生成Version.json文件. 一般情况下, 都推荐勾选它. 因为版本间的差异都是通过这个文件中记载的json信息进行比对的.

* Version:
> 打包的版本是多少. 当勾选上面的生成版本信息时, 会出现此输入框. 不能为空.

* Create Resource List:
> 是否生成ResourceList.json文件. 一般情况下, 也推荐勾选它. 因为在计算要更新的包的时候, 是根据这个文件中的信息进行比对的.

* Create:
> 开始打包, 其会在项目文件夹中生成一个名为AssetBundles的目录, 里面包含了打包的AB包和其他信息.  
有三点需要注意的地方:  
1\. 是在项目文件夹中生成, 其和Assets目录是同级的.  
2\. 打包的目标平台是当前Build Setting中的当前平台, 请注意切换.
3\. 打包后AssetBundles中的目录结构, 就是更新到 *Asset Bundle Path* 中的目录结构.

* Clear:
> 删除生成的目录及里面的全部内容.

## 总结:
> 所以, 一般的开发流程是:
1. 按照unity的标准方式进行AB包的标记, 并切换到目标平台.
2. 使用附带的打包工具, 设置好版本号后, 点击Create进行打包. 若途中需要对AB包的标记进行修改, 则在修改后再次点击Create.
3. 将打包好的AB包放到服务器中, 并为Remote Setting中使用的URI提供接口.
4. 在场景中放入ABSystem的预制体, 做好相应设置.
5. 测试发布.

* * *

## API
> 如果你需要进度条, 或者在更新前提示用户需要下载内容的大小, 并给出一个更新按钮的功能, 你可能需要自己写, 但是ABSystem也提供了一下API可供使用. 这些API暂时都由ABSystem命名空间下的ABManager.Updater提供, 其中, ABManager作为Unity中的单例模式存在, 所以在使用这些API时, 你依然需要将ABSystem的预制体放入场景中, 并做好设置, 不同的是, 现在不需要启动Auto Update的功能. 

#### ABManger.Updater.*
* Check()
> 检查, 进行各种属性的设置, 在第一次访问其他属性前, 必须先调用这个方法. 否则将抛出ABUnCheckException. 注意, 下面访问各属性时, 可能得到的是旧数据, 为了确保得到的新数据, 你需要先调用Check()方法, Check()方法会对所有的属性进行更新.

* HasNewVersion
> 返回bool值, 表示当前是否有新版本.

* CurrentVersion
> 返回string, 表示当前的版本. 

* DownloadSize
> 返回long, 表示需要下载的字节数.

* Progress
> 返回int, 是0-100直接的数字, 表示下载进度.

* CurrentDownloadItem
> 返回ABDownloadItem对象, 表示当前下载的对象, 其可以的字段可在ABData文件中查看, 当下载完成后返回null. 注意, 不要修改此对象中的字段值, 否则后果自负.

* UpdateToNew()
> 开始下载更新. 注意, 在调用此函数之前, 请先调用Check函数. 就像Start函数中展现的那样.

