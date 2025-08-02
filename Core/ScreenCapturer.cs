using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

public static class ScreenCapturer
{
    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

    public static Bitmap? CaptureWindowByProcessName(string processName)
    {
        var processes = Process.GetProcessesByName(processName);
        if (processes.Length == 0)
        {
            Console.WriteLine($"プロセスが見つかりません: {processName}");
            return null;
        }

        IntPtr hWnd = processes[0].MainWindowHandle;
        if (hWnd == IntPtr.Zero || !GetWindowRect(hWnd, out RECT rect))
        {
            Console.WriteLine("ウィンドウが取得できません");
            return null;
        }

        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;

        Bitmap bmp = new Bitmap(width, height);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            IntPtr hdc = g.GetHdc();
            PrintWindow(hWnd, hdc, 0);
            g.ReleaseHdc(hdc);
        }

        return bmp;
    }
}