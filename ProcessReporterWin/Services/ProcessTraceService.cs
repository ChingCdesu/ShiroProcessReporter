using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Accessibility;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using Windows.Win32.System.Threading;

namespace ProcessReporterWin.Services;

public class ProcessTraceService
{
    private UnhookWinEventSafeHandle? _hookHandle = null;

    private readonly ILogger<ProcessTraceService> _logger;

    public delegate void FrontWindowChangeHandler(string windowTitle, string processName);

    public event FrontWindowChangeHandler OnFrontWindowChanged;

    public ProcessTraceService(ILogger<ProcessTraceService> logger)
    {
        _logger = logger;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        var callback = new WINEVENTPROC(OnFrontWindowChange);
        _hookHandle = PInvoke.SetWinEventHook(0x0003 /* EVENT_SYSTEM_FOREGROUND */, 0x0003 /* EVENT_SYSTEM_FOREGROUND */, handle, callback, 0, 0, 0);
    }

    ~ProcessTraceService()
    {
        if (_hookHandle != null)
        {
            _hookHandle.Dispose();
            _hookHandle = null;
        }
    }

    private unsafe void OnFrontWindowChange(HWINEVENTHOOK eventHook, uint @event, HWND hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
    {
        if (hwnd == IntPtr.Zero)
        {
            _logger.LogError("Front window handle is null");
            return;
        }

        var length = PInvoke.GetWindowTextLength(hwnd);
        if (length <= 0)
        {
            _logger.LogError("Failed to get window title length");
            return;
        }

        var windowTitlePtr = Marshal.AllocHGlobal(255);
        if (PInvoke.GetWindowText(hwnd, (char*)windowTitlePtr.ToPointer(), 255) <= 0)
        {
            _logger.LogError("Failed to get window title");
            Marshal.FreeHGlobal(windowTitlePtr);
            return;
        }

        var windowTitle = Marshal.PtrToStringAuto(windowTitlePtr);
        if (windowTitle is null)
        {
            _logger.LogError("WindowTitle is null");
            Marshal.FreeHGlobal(windowTitlePtr);
            return;
        }
        _logger.LogInformation("Front window changed to {WindowTitle}", windowTitle);
        Marshal.FreeHGlobal(windowTitlePtr);

        var processIdPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(uint)));
        if (PInvoke.GetWindowThreadProcessId(hwnd, (uint*)processIdPtr.ToPointer()) == 0)
        {
            _logger.LogError("Failed to get process ID");
            Marshal.FreeHGlobal(processIdPtr);
            return;
        }
        var processId = (uint)Marshal.ReadInt32(processIdPtr);
        Marshal.FreeHGlobal(processIdPtr);

        var processHandle = PInvoke.OpenProcess_SafeHandle(PROCESS_ACCESS_RIGHTS.PROCESS_QUERY_INFORMATION | PROCESS_ACCESS_RIGHTS.PROCESS_VM_READ, false, processId);
        if (processHandle.IsInvalid)
        {
            _logger.LogError("Failed to get process handle");
            processHandle.Close();
            return;
        }
        var processNamePtr = Marshal.AllocHGlobal(1024);
        PInvoke.GetModuleBaseName(processHandle, null, (char*)processNamePtr.ToPointer(), 1024);
        processHandle.Close();

        var processName = Marshal.PtrToStringAuto(processNamePtr);
        if (processName is null)
        {
            _logger.LogError("ProcessName is null");
            Marshal.FreeHGlobal(processNamePtr);
            return;
        }

        _logger.LogInformation("ProcessName is {ProcessName}", processName);
        Marshal.FreeHGlobal(processNamePtr);
        if (OnFrontWindowChanged is not null)
        {
            OnFrontWindowChanged(windowTitle, processName);
        }
    }
}
