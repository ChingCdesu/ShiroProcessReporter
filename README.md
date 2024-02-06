# ProcessReporterWin

[ProcessReporterWinpy](https://github.com/TNXG/ProcessReporterWinpy)的C#实现，魔改了部分模块。



### 主要修改部分

* 前台窗口监听使用Win32 Hook
* 正在播放的音乐信息使用WinRT的MediaSession监听（暂不支持网易云）



### TODO

- [ ] 网易云支持
- [ ] 自定义配置保存位置
- [ ] Log显示（文件 / UI内控制台）
- [x] 替换规则（Regex）
- [ ] 过滤规则（Regex）

### NEED FIX
- [ ] ProcessTraceService会被CLR中断
