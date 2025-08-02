using OpenCvSharp;

public class MatchStartDetector : IStateDetector
{
    private readonly Mat _template;
    public GameState State => GameState.MatchStart;

    public MatchStartDetector()
    {
        _template = Cv2.ImRead("Games/Shadowverse/Templates/start.png", ImreadModes.Grayscale);
    }

    public bool IsMatch(Mat screen, out double score, out OpenCvSharp.Point location)
    {
        score = 0;
        location = new OpenCvSharp.Point(0, 0);

        if (_template.Empty())
        {
            Console.WriteLine("[Error] テンプレートが読み込めていません。パスを確認してください。");
            return false;
        }

        // キャプチャ画像のサイズに基づきテンプレートを動的にリサイズ
        OpenCvSharp.Size screenSize = screen.Size();
        double scaleX = (double)screenSize.Width / _template.Width;
        double scaleY = (double)screenSize.Height / _template.Height;
        double scale = Math.Min(scaleX, scaleY) * 0.5; // 過度な拡大を避けるため0.5倍程度に抑制

        var newSize = new OpenCvSharp.Size((int)(_template.Width * scale), (int)(_template.Height * scale));

        if (newSize.Width <= 0 || newSize.Height <= 0 || screenSize.Width < newSize.Width || screenSize.Height < newSize.Height)
        {
            Console.WriteLine("[Error] 拡大後のテンプレートサイズが不正、または画面より大きいため比較不能。");
            return false;
        }

        using var resizedTemplate = _template.Resize(newSize);
        using var result = new Mat();
        Cv2.MatchTemplate(screen, resizedTemplate, result, TemplateMatchModes.CCoeffNormed);
        Cv2.MinMaxLoc(result, out _, out score, out _, out location);

        Console.WriteLine($"[{State}] 類似度: {score:F3} at {location} / Template: {newSize.Width}x{newSize.Height}");
        return score >= 0.85;
    }
}