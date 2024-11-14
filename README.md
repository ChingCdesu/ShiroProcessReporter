# ProcessReporterWin

[ProcessReporterWinpy](https://github.com/TNXG/ProcessReporterWinpy)的C#实现，魔改了部分模块。



### 主要修改部分

* 前台窗口监听使用Win32 Hook
* 正在播放的音乐信息使用WinRT的MediaSession监听（暂不支持网易云）



### TODO

- [ ] 网易云支持
- [ ] 自定义配置保存位置
- [x] Log显示（文件 / UI内控制台）
- [x] 替换规则（Regex）
- [x] 过滤规则（Regex）

### 安装

- 下载安装程序 [Releases](https://github.com/ChingCdesu/ShiroProcessReporter/releases)
- 双击安装应用
  - 如果提示证书问题，下载这个证书文件：[Certificate.cer](https://github.com/ChingCdesu/ShiroProcessReporter/releases/tag/v2.0.0-beta.1)
  - 双击证书文件，选择安装证书
  - 储存位置选 `本地计算机` 下一步
  - 证书储存选择 `将所有的证书都放入下列存储`
  - 点浏览，选择 `受信任的根证书颁发机构`
  - 下一步知道完成安装
  - 重新双击安装应用
