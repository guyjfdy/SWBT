using OpenCvSharp;

public class ResultDetector : IStateDetector
{
    private readonly Mat _template;
    public GameState State => GameState.Result;

    public ResultDetector()
    {
        _template = Cv2.ImRead("Games/Shadowverse/Templates/result.png", ImreadModes.Grayscale);
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

        // 相対領域: 画面の上部中央 (例: "RESULT" 文字領域)
        int x = (int)(screen.Width * 0.55);
        int y = (int)(screen.Height * 0.10);
        int w = (int)(screen.Width * 0.40);
        int h = (int)(screen.Height * 0.20);

        var roi = new Rect(x, y, w, h);

        if (roi.X < 0 || roi.Y < 0 || roi.X + roi.Width > screen.Width || roi.Y + roi.Height > screen.Height)
        {
            Console.WriteLine("[Error] ROIが画面外に出ています。");
            return false;
        }

        using var cropped = new Mat(screen, roi);

        // テンプレートをROIにスケール合わせ
        // テンプレートから対応するROI領域を切り出す（相対位置対応）
        int tx = (int)(_template.Width * 0.35);
        int ty = (int)(_template.Height * 0.01);
        int tw = (int)(_template.Width * 0.30);
        int th = (int)(_template.Height * 0.20);

        var templateRoi = new Rect(tx, ty, tw, th);
        if (templateRoi.X < 0 || templateRoi.Y < 0 || templateRoi.X + templateRoi.Width > _template.Width || templateRoi.Y + templateRoi.Height > _template.Height)
        {
            Console.WriteLine("[Error] テンプレートROIが不正です。");
            return false;
        }

        using var templateCrop = new Mat(_template, templateRoi);
        var resizedTemplate = templateCrop.Resize(new OpenCvSharp.Size(cropped.Width, cropped.Height));

        using var result = new Mat();
        Cv2.MatchTemplate(cropped, resizedTemplate, result, TemplateMatchModes.CCoeffNormed);
        Cv2.MinMaxLoc(result, out _, out score, out _, out location);
#if DEBUG
        Console.WriteLine("デバッグ時に実行");
        Cv2.ImWrite($"C:/Users/MW/Documents/Projects/SWBT/bin/Debug/net8.0-windows/out/debug_roi_{State}.png", cropped);
        Cv2.ImWrite($"C:/Users/MW/Documents/Projects/SWBT/bin/Debug/net8.0-windows/out/debug_template_{State}.png", resizedTemplate);
#endif
        location.X += roi.X;
        location.Y += roi.Y;

        Console.WriteLine($"[{State}] 類似度: {score:F3} at {location} / ROI: {roi.Width}x{roi.Height}");
        return score >= 0.85;
    }
}