# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.10.1] - 2022-02-14
### Changed
- 优化当UIInChildrenField中有null时会打印错误日志并跳过当前生成

## [0.10.0] - 2022-02-12
### Changed
- 升级适配2021.2.7f1

## [0.9.2] - 2022-02-12
### Added
- Nuget.Editor程序集

### Changed
- 升级Utility到0.0.33

### Fixed
- 热更类型选用默认，热更运行时会报错
- 与Utility0.0.33冲突

## [0.9.1] - 2022-01-25
### Changed
- 支持可配置存档版本号

## [0.9.0] - 2021-12-31
### Added
- NuGet管理依赖
- 框架引导

### Change
- 替换Roslyn相关依赖
- WingjoyBuild -> 0.4.0

## [0.8.28] - 2021-12-30
### Changed
- 适配2021.2.7

## [0.8.27] - 2021-12-25
### Changed
- 新增MusicHelper、SoundHelpers get属性

## [0.8.26] - 2021-12-24
### Changed
- AudioSourceHelper IsPlaying状态判定调整为资源加载过程中也属于正在播放

## [0.8.25] - 2021-12-20
### Changed
- StandBy同步适配
- Launcher中如果网络状态为不可达，则跳过热更检测

## [0.8.24] - 2021-12-18
### Changed
- 已经打开的界面调用打开逻辑时，自动执行OnOpen回调

## [0.8.19] - 2021-12-15
### Added
- UI新增只Load不显示
- 新增设置UI界面外边距
- 新增下载完后自动根据情况重新读取本地化字典

## [0.8.18] - 2021-12-15
### Changed
- 将所有组件自身初始化内容放在组件自身内部
- OnLauncher改为异步

## [0.8.17] - 2021-12-15
### Added
- 新增播放音频时判定是否需要释放

## [0.8.16] - 2021-12-14
### Changed
- Enable YamlDotNet Auto Referenced

## [0.8.15] - 2021-12-14
### Added
- 新增释放所有句柄API
- 新增移除所有消息接受API

### Fixed
- 编辑器打包dll时路径可能不正确

## [0.8.14] - 2021-12-11
### Changed
- 播放背景音乐新增上一首淡出时长参数

## [0.8.13] - 2021-12-08
### Added
- 新增音频初始化配置
- 新增画质设置常量

## [0.8.12] - 2021-12-08
### Fixed
- 调整音效音量无效

## [0.8.11] - 2021-12-08
### Added
- 新增音量静音常量定义

## [0.8.10] - 2021-12-08
### Added
- 新增音量、是否静音接口

## [0.8.9] - 2021-12-07
### Added
- 异步保存

## [0.8.8] - 2021-11-26
### Changed
- 热更代码不再生成在SteamingAsset中
- 调整拷贝远端资源到Library代码

## [0.8.7] - 2021-11-24
### Changed
- OnAddressableInitializeSuccess 改为异步

## [0.8.6] - 2021-11-24
### Added
- Launcher 添加 OnAddressableInitializeSuccess
### Fixed
- Inspector 界面 Open AudioLibrary没有内容显示

## [0.8.5] - 2021-11-24
### Changed
- 将AudioLibrary加载方式改成AssetReference加载

## [0.8.4] - 2021-11-16
### Changed
- 调整OnDownloadProgressUpdate参数

## [0.8.3] - 2021-11-06
### Added
- 框架销毁事件 OnDestroy
### Fixed
- 热更代码没有携带宏定义编译

## [0.8.2] - 2021-11-06
### Added
- CopyRemoteBundleToStreamingAssets 拷贝RemoteBundle到StreamingAssets
### Changed
- BuildScriptPackedWithBundleListMode 将只生成清单，拷贝流程加到编译流程中

## [0.8.1] - 2021-11-06
### Added
- 添加Download状态变更函数

## [0.8.0] - 2021-11-05
### Added
- Addressable打包并将远程资源加入StreamingAsset中
- 基础启动顺序调整：Launcher.cs(先加载主工程框架-验证并下载最新资源-加载热更工程框架)

## [0.7.6] - 2021-10-29
### Fixed
- 添加音频时没有被设置为脏
- 快速多次加载会出现加载失败的报错

## [0.7.5] - 2021-10-28
### Fixed
- 播放音量时如果没有淡入时间应将音量设置成正常

## [0.7.4] - 2021-10-27
### Added
- 按钮播放音效组件
- 新增拷贝播放音频代码按钮

## [0.7.3] - 2021-10-27
### Fixed
- 复制粘贴音频名称错误

## [0.7.2] - 2021-10-27
### Added
- 事件触发音频

## [0.7.1] - 2021-10-22
### Added
- 音频淡出
- 停止播放
- 列表播放
  
## [0.7.0] - 2021-10-22
### Added
- 新增音频组件
    拖拽配置音频、自动生成音频ID脚本、复制音频ID字段名称

## [0.6.21] - 2021-09-06
### Fixed
- 打开界面顺序可能会导致不符合期望的效果

## [0.6.20] - 2021-09-01
### Fixed
- OnRelease无效

## [0.6.19] - 2021-09-01
### Added
- 新增AddDependency(IDisposable iDisposable)

## [0.6.18] - 2021-08-31
### Added
- 新增Get、GetAll、Has接口

## [0.6.17] - 2021-08-30
### Changed
- 移除UI框架内的异步部分

## [0.6.16] - 2021-08-28
### Fixed
- 重复打开同一界面，可能造成数据与期望不一致

## [0.6.15] - 2021-08-28
### Fixed
- CloseAll时忽略打开方式，避免有些界面因为Switch而被重新打开

## [0.6.14] - 2021-08-26
### Added
- 每个界面可以读取到自己的句柄信息

## [0.6.13] - 2021-08-26
### Changed
- 调整打开之前因切换被关闭的界面，由无法打开，改为可以打开，并且抛弃切换之后的界面打开踪迹

## [0.6.12] - 2021-08-21
### Fixed
- 修复忽略Lock失败

## [0.6.11] - 2021-08-21
### Added
- 忽略Lock关闭界面

## [0.6.10] - 2021-08-21
### Fixed
- 避免释放MASK

## [0.6.9] - 2021-08-21
### Added
- 释放所有界面时忽略锁定
- 导出全部Text新增扩展字符串配置

## [0.6.8] - 2021-07-30
### Fixed
- 修复无法自定义重写

## [0.6.7] - 2021-07-30
### Added
- 新增自定义主框架类
### Changed
- 如果UIForm没有挂载才去GraphicRaycaster

## [0.6.6] - 2021-07-21
### Fixed
- 修复释放方法错误

## [0.6.5] - 2021-07-16
### Changed
- 完善WingjoyMonoBehaviour释放流程

## [0.6.4] - 2021-07-14
### Fixed
- 修复资源释放可能导致的问题

## [0.6.3] - 2021-07-12
### Fixed
- 如果是预制件，并且要被创建的组件是当前预制件的根节点，则使用预制件的名称，防止每个引用该预制件的名称不一致导致生成代码有问题

## [0.6.2] - 2021-07-10
### Added
- 主框架添加SendUIMessage接口

## [0.6.1] - 2021-07-05
### Fixed
- 资源内文本替换使用Yaml库进行处理时文件路径错误

## [0.6.0] - 2021-07-05
### Fixed
- 资源内文本替换使用Yaml库进行处理后Unity格式不对的bug

## [0.5.9] - 2021-07-05
### Fixed
- 资源内文本替换使用Yaml库进行处理后Unity前缀丢失的bug

## [0.5.8] - 2021-07-05
### Changed
- 资源内文本替换使用Yaml库进行

## [0.5.7] - 2021-07-05
### Changed
- 添加GetOpeningFormCount获取正在打开的界面数量
### Fixed
- 通过TopSwitch被动打开的界面会被二次加入到正在打开的界面列表中

## [0.5.6] - 2021-07-03
### Changed
- BeforeOnLauncher 改为异步

## [0.5.5] - 2021-07-03
### Added
- WingjoyFramework 添加BeforeOnLauncher回调

## [0.5.4] - 2021-07-02
### Changed
- Launcher void -> UniTask

## [0.5.3] - 2021-07-02
### Fixed
- 命名空间与Unity2020冲突

## [0.5.2] - 2021-06-30
### Fixed
- 预制件下只有Prefab时，会导入多余的命名空间

## [0.5.1] - 2021-06-26
### Added
- UIGroupBase 新增OnFormOpen

## [0.5.0] - 2021-06-24
### Added
- UIGroup现在可以作为预制件脚本使用
- 新增WinjoyDebugger

## [0.4.2] - 2021-06-24
### Changed
- Utility 0.0.17 -> 0.0.18
- 移除编辑器部分关于DG的依赖

## [0.4.1] - 2021-06-23
### Added
- UIComponent add CloseAll
### Fixed
- UIBase m_Handles 没有初始化
- 没有LocalizationXml时打开游戏报错
- 导出所有Text是没有clear

## [0.4.0] - 2021-06-23
### Added
- 新增存档Component

## [0.3.10] - 2021-06-23
### Added
- 使用UIPrefabName生成的预制件，物体名称添加使用该特性的类名

## [0.3.9] - 2021-06-23
### Added
- 新增UIPrefabName特性，可以指定加载某个界面

## [0.3.8] - 2021-06-22
### Changed
- 搜索Asset中的中文改成使用YMAL遍历获得

## [0.3.7] - 2021-06-19
### Changed
- 优化switch逻辑

## [0.3.6] - 2021-06-19
### Added
- UI模块新增Switch
### Removed
- UI模块移除HideOther

## [0.3.5] - 2021-06-18
### Changed
- 调整异步读取本地化xml

## [0.3.4] - 2021-06-18
### Added
- 依赖UniTask
### Changed
- 本地化流程中的TASK->UniTask

## [0.3.3] - 2021-06-17
### Changed
- 调整执行顺序，保证所有component onlauncher时，其他组件都初始化成功

## [0.3.2] - 2021-06-17
### Changed
- 分离主工程下的组件CoreMain CoreHotFix

## [0.3.1] - 2021-06-17
### Added
- 新增带有format的GS
### Changed
- Excel相关DLL只用于Editor
- 优化本地化编辑器按钮文本

## [0.3.0] - 2021-06-16
### Added
- 本地化相关组件

## [0.2.1] - 2021-06-11
### Added
- 新增通过泛型获取ComponentConfig
### Changed
- 升级WingjoyBuild -> 0.2.20
- 升级WingjoyUtility -> 0.0.16

## [0.2.0] - 2021-06-11
### Added
- 新增WngjoyFrameworkHotFixComponent
- 新增ComponentConfig
- 通过反射创建、并可以读取Mono配置
### Changed
- 所有热更类将被带有HotFix的命名空间包含

## [0.1.19] - 2021-06-10
### Changed
- System.Buffers.dll 调整为全平台可用

## [0.1.18] - 2021-06-10
### Added
- 添加释放所有界面接口
### Fixed
- 使用CloseOther并未关闭其他界面

## [0.1.17] - 2021-06-10
### Changed
- System.Runtime.CompilerServices.Unsafe.dll 调整为全平台可用

## [0.1.16] - 2021-06-09
### Changed
- System.Memory.dll 调整为全平台可用

## [0.1.15] - 2021-06-07
### Added
- Add Lock Field in UIFormBase
### Changed
- Use another method to achieve hideAll open mode

## [0.1.14] - 2021-06-07
### Added
- Add new OpenMode, HideOther:暂时隐藏其他界面，当当前界面关闭时，其他界面重新打开
### Changed
- upgrade wingjoubuild -> 0.2.15
### Fixed
- UIFormBase Init userData not used

## [0.1.13] - 2021-05-28
### Added
- 添加UIObject,辅组预制件UI脚本编写

## [0.1.12] - 2021-05-28
### Changed
- Group会使用带有父级的命名空间，以解决可能的类名冲突问题

## [0.1.11] - 2021-05-28
### Added
- WingjoyFrameworkComponent 支持Update
- UIFrom与UIGroup支持Update

## [0.1.10] - 2021-05-27
### Added
- 支持选择基类继承

## [0.1.9] - 2021-05-27
### Fixed
- using中包含UnityEditor

## [0.1.8] - 2021-05-27
### Added
- 允许自定义项目依赖内容
### Fixed
- 热更中使用<>导致报错

## [0.1.7] - 2021-05-25
### Fixed
- 子Group未调用OnInit

## [0.1.6] - 2021-05-25
### Fixed
- 修复读取CoreType方式错误

## [0.1.5] - 2021-05-25
### Changed
- 调整CoreType获取流程

## [0.1.4] - 2021-05-24
### Added
- 新增导入默认ILRuntime package

## [0.1.3] - 2021-05-24
### Added
- 支持自定义DLL编译工程选择

## [0.1.2] - 2021-05-24
### Fixed
- 修复父级绑定

## [0.1.1] - 2021-05-24
### Fixed
- 修复需要提前创建文件夹，未提前创建UIFormBase脚本导致后续流程无法正常进行

## [0.1.0] - 2021-05-22
### Added
- 支持热更

## [0.0.11] - 2021-05-20
### Added
- 新增addressable打包
### Changed
- 调整UIField寻找父级时保护自身物体

## [0.0.10] - 2021-05-20
### Added
- 完善UIGroup创建工作流

## [0.0.9] - 2021-05-19
### Added
- 支持配置预制件存放路径

## [0.0.8] - 2021-05-19
### Fixed
- 修正无法获取到外部类型

## [0.0.7] - 2021-05-19
### Changed
- 调整游戏入口

## [0.0.6] - 2021-05-19
### Changed
- 调整入口

## [0.0.5] - 2021-05-18
### Changed
- 修改类名

## [0.0.4] - 2021-05-18
### Added
- 更新UI框架

## [0.0.2] - 2021-05-17
### Changed
- 更新package

## [0.0.1] - 2021-03-25
### Added
- UI框架

[Unreleased]: http://gitealocal.wingjoy.cn/SVNBuildGroup/WinjoyFramework.git#upm
[0.0.1]: http://gitealocal.wingjoy.cn/SVNBuildGroup/WinjoyFramework.git#0.0.1