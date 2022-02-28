# Changelog

## [0.0.33] - 2022-01-13
### Added
- 添加[OpenOrCreateButton]特性
- 添加ClassSelector非泛型应用

## [0.0.32] - 2021-12-22
### Changed
- 改善创建Asset体验

## [0.0.31] - 2021-12-22
### Fixed
- ToHex缺少位数

## [0.0.30] - 2021-12-07
### Added
- 支持管理收件人
- 添加异步压缩

## [0.0.29] - 2021-11-20
### Changed
- 添加一个新的手机号有效性正则表达式

## [0.0.28] - 2021-11-10
### Added
- add ClassSelector

## [0.0.27] - 2021-10-22
### Changed
- 恢复Singleton mInstance 字段 protected定义 

## [0.0.26] - 2021-10-22
### Changed
 - Text:StringBuilder s_CachedStringBuilder在广告播放回调里报空导致奖励无法正常发送的问题，保护一下（虽然不知道为什么）
 - Random:增加两个方法 - 1.获得一串不重复的随机数字 2.根据权重获得一个可能的ID

## [0.0.25] - 2021-10-22
### Fixed
- 编译报错

## [0.0.24] - 2021-10-22
### Added
- 新增部分编辑器工具函数

## [0.0.23] - 2021-10-22
### Changed
- 获取资产GUID的方式移动到Runtime中

## [0.0.22] - 2021-10-22
### Added
- 新增将文本转换为可用做文件名的安全文本
- 新增获取资产GUID的方式

## [0.0.21] - 2021-10-15
### Changed
- MD5:增加一个32位加密方法
- Text:增加身份证、手机号格式验证
- WaitFor:增加几个时间字段
### Added
- 增加一个自动隐藏物体工具脚本

## [0.0.1] - 2021-03-23
### Added
- Path工具类