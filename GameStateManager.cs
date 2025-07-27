public class GameStateManager
{
    private GameState _currentState = GameState.Unknown;
    private readonly StateDetector _detector;

    public GameStateManager(StateDetector detector)
    {
        _detector = detector;
    }

    public void Update()
    {
        var capturedImage = ScreenCapturer.CaptureWindowByProcessName("ShadowverseWB"); // スクリーンショット取得
        var detectedState = _detector.DetectState(capturedImage);

        if (detectedState != _currentState)
        {
            Console.WriteLine($"状態変化: {_currentState} → {detectedState}");
            _currentState = detectedState;
        }
    }
}
