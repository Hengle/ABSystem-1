# ABSystem
自制 unity AssetBundle 更新系统

### 使用方式:

#### 1. 将ABSystem的预制体添加到场景中.

#### 2. 设置其中的参数, 各参数的含有和格式有:

##### Remote Setting: 
> 远程服务器设置相关, 都是一些特定的uri.
* Remote Version URI:
> 获取远程版本URI, 将会以HTTP的GET的方法去请求该URI, 要求返回Json信息, 其格式为{"Version": "1.0.0"}这样的形式.
* Remote Asset Bundle List URI:
> 获得远程版本的AB包列表的信息, 将使用HTTP的GET方法请求该URI, 要求返回Json信息, 其是一个Json数组, 数组中的每一项表示一个AB包的信息, 例如:[{"NAME1": "HASH1"}, {"NAME2": "HASH2"}]. 每个Json对象的键为AB包的名称, 值为AB包的哈希值.
* Remote Asset Bundle Download Entry:
> 当下载AB包时, 使用的下载入口, 其请求的格式如下:
>   Entry?Name=xxx&Version=xxx
>一般情况下, 你可以根据其请求参数将其重定向的真正的下载URI中, 但是要注意, 下载的URI必须要同时支持HEAD方法和GET方法, 因为要使用HEAD方法
来获取要下载的包的大小.
##### Local Setting:
> 本地应用相关.
* Asset Bundle Path:
> 用于储存下载的AB包的目录名称, 其将存放于Application.persistentDataPath下, 而所有下载的AB包就存放于这个指定的目录下
* Default Version:
> 当在上面设置的目录下找不到Version.json文件来读取版本信息时, 将使用的默认版本.
##### AutoUpdate:
> 如果勾选上了, 其将会在Start函数中自动开始进行检查更新.
