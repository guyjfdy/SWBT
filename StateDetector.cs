using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.Extensions;

public class StateDetector
{
    private Mat _templateStart;
    private Mat _templateBattle;
    private Mat _templateResult;

    public StateDetector()
    {
        _templateStart = Cv2.ImRead("Templates/start.png", ImreadModes.Grayscale);
        _templateBattle = Cv2.ImRead("Templates/battle.png", ImreadModes.Grayscale);
        _templateResult = Cv2.ImRead("Templates/result.png", ImreadModes.Grayscale);
    }

    public GameState DetectState(Bitmap screenBmp)
    {
        Mat screenMat = BitmapConverter.ToMat(screenBmp);
        Cv2.CvtColor(screenMat, screenMat, ColorConversionCodes.BGR2GRAY);

        if (MatchTemplate(screenMat, _templateStart, "試合開始")) return GameState.MatchStart;
        if (MatchTemplate(screenMat, _templateBattle, "試合中")) return GameState.InBattle;
        if (MatchTemplate(screenMat, _templateResult, "結果")) return GameState.Result;

        Console.WriteLine("[検知失敗] どの状態とも一致しません。");
        return GameState.Unknown;
    }


    private bool MatchTemplate(Mat screen, Mat template, string label, double threshold = 0.4)
    {
        using var result = new Mat();
        Cv2.MatchTemplate(screen, template, result, TemplateMatchModes.CCoeffNormed);
        Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point maxLoc);

        Console.WriteLine($"[{label}] 類似度: {maxVal:F3} 座標: ({maxLoc.X}, {maxLoc.Y})");

        return maxVal >= threshold;
    }

}
