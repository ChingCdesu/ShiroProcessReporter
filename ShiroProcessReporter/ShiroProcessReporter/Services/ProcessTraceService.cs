using System.Buffers;
using System.Diagnostics;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Accessibility;
using Microsoft.Extensions.Logging;
using static Windows.Win32.PInvoke;
using ShiroProcessReporter;
using Microsoft.Extensions.DependencyInjection;
using System;
using ShiroProcessReporter.Helper;

namespace ShiroProcessReporter.Services;

// Reference: https://github.com/walterlv/Walterlv.ForegroundWindowMonitor

public class ProcessTraceService
{
    public delegate void FrontWindowChangeHandler(string windowTitle, string processName);

    private readonly ILogger<ProcessTraceService> _logger;
    private readonly HWINEVENTHOOK _hookHandle;

    public ProcessTraceService()
    {
        _logger = AppLogger.Factory.CreateLogger<ProcessTraceService>();

        _hookHandle = SetWinEventHook(
            EVENT_SYSTEM_FOREGROUND,
            EVENT_SYSTEM_FOREGROUND,
            HMODULE.Null,
            OnFrontWindowChange,
            0, 0,
            WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);

        // 开启消息循环，以便 WinEventProc 能够被调用。
        if (GetMessage(out var lpMsg, default, default, default))
        {
            TranslateMessage(in lpMsg);
            DispatchMessage(in lpMsg);
        }
    }

    public event FrontWindowChangeHandler OnFrontWindowChanged;

    ~ProcessTraceService()
    {
        UnhookWinEvent(_hookHandle);
    }

    private static void OnFrontWindowChange(HWINEVENTHOOK eventHook, uint @event, HWND hwnd, int idObject, int idChild,
        uint idEventThread, uint dwmsEventTime)
    {
        var currentWindow = GetForegroundWindow();

        var processId = GetProcessIdCore(currentWindow);

        var processName = Process.GetProcessById((int)processId).ProcessName;

        var windowTitle = CallWin32ToGetPWSTR(512, (p, l) => GetWindowText(currentWindow, p, l));

        var service = App.ServiceProvider.GetService<ProcessTraceService>();

        if (service?.OnFrontWindowChanged is not null) service.OnFrontWindowChanged(windowTitle, processName);
    }

    private static unsafe uint GetProcessIdCore(HWND hWnd)
    {
        uint pid = 0;
        GetWindowThreadProcessId(hWnd, &pid);
        return pid;
    }

    private static unsafe string CallWin32ToGetPWSTR(int bufferLength, Func<PWSTR, int, int> getter)
    {
        var buffer = ArrayPool<char>.Shared.Rent(bufferLength);
        try
        {
            fixed (char* ptr = buffer)
            {
                getter(ptr, bufferLength);
                return new string(buffer, 0, Array.IndexOf(buffer, '\0'));
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }
}