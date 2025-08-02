using System;
using System.Threading;
using System.Drawing;

class Program
{
    static void Main()
    {
        IGameStateDetectorSet detectorSet = new ShadowverseDetectorSet(); // 他のゲームに差し替え可能
        var manager = new GameStateManager(detectorSet);

        Console.WriteLine($"[{detectorSet.GameName}] 状態検知を開始します...");

        while (true)
        {
            Bitmap? bmp = ScreenCapturer.CaptureWindowByProcessName(detectorSet.ProcessName);
            if (bmp != null)
            {
                manager.Update(bmp);
                bmp.Dispose();
            }
            else
            {
                Console.WriteLine($"[{detectorSet.GameName}] ウィンドウが見つかりません");
            }

            Thread.Sleep(1000);
        }
    }
}
