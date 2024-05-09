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
        // ����Ѵ��ڶ�ʱ������ֹͣ��������
        _timer?.Dispose();
        
        // ����һ���µĶ�ʱ��
        _timer = new Timer(_ =>
        {
            action();
            // ִ����Ϻ�ֹͣ��ʱ��
            _timer?.Dispose();
            _timer = null;
        }, null, _interval, TimeSpan.FromMilliseconds(-1)); // TimeSpan.FromMilliseconds(-1) ��ʾ��ʱ��ִֻ��һ��
    }
}
