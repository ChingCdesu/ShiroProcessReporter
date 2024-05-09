using System;
using System.Threading;

namespace ShiroProcessReporter.Helper;

public class Debouncer
{
    private Timer _timer;
    private TimeSpan _interval;
    
    public Debouncer(TimeSpan interval)
    {
        _interval = interval;
    }

    public void Debounce(Action action)
    {
        // 如果已存在定时器，则停止并销毁它
        _timer?.Dispose();
        
        // 创建一个新的定时器
        _timer = new Timer(_ =>
        {
            action();
            // 执行完毕后，停止定时器
            _timer?.Dispose();
            _timer = null;
        }, null, _interval, TimeSpan.FromMilliseconds(-1)); // TimeSpan.FromMilliseconds(-1) 表示定时器只执行一次
    }
}
