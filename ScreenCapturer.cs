using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;

public static class ScreenCapturer
{
    // WinAPI定義
    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

    /// <summary>
    /// プロセス名からウィンドウをキャプチャして Bitmap を返す
    /// </summary>
    /// <param name="processName">拡張子なしのプロセス名（例: ShadowverseWB）</param>
    /// <returns>Bitmap または null</returns>
    public static Bitmap? CaptureWindowByProcessName(string processName)
    {
        Process[] processes = Process.GetProcessesByName(processName);

        if (processes.Length == 0)
        {
            Console.WriteLine($"プロセスが見つかりません: {processName}");
            return null;
        }

        IntPtr hWnd = processes[0].MainWindowHandle;

        if (hWnd == IntPtr.Zero)
        {
            Console.WriteLine("ウィンドウハンドルが取得できません");
            return null;
        }

        if (!GetWindowRect(hWnd, out RECT rect))
        {
            Console.WriteLine("ウィンドウの座標が取得できません");
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
