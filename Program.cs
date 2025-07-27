using System;
using System.Threading;

class Program
{
    static void Main()
    {
        var detector = new StateDetector();
        var manager = new GameStateManager(detector);

        Console.WriteLine("状態監視を開始します...");

        while (true)
        {
            manager.Update();
            Thread.Sleep(500); // 1秒おきに監視
        }
    }
}
