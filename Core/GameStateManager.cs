using System;
using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;

public class GameStateManager
{
    private GameState _currentState = GameState.Unknown;
    private readonly IGameStateDetectorSet _detectorSet;
    private readonly List<IStateDetector> _detectors;

    public GameStateManager(IGameStateDetectorSet detectorSet)
    {
        _detectorSet = detectorSet;
        _detectors = detectorSet.CreateDetectors();
    }

    public void Update(Bitmap bmp)
    {
        Mat screen = BitmapConverter.ToMat(bmp);
        Cv2.CvtColor(screen, screen, ColorConversionCodes.BGR2GRAY);

        foreach (var detector in _detectors)
        {
            if (detector.IsMatch(screen, out double score, out var loc))
            {
                if (_currentState != detector.State)
                {
                    Console.WriteLine($"[{_detectorSet.GameName}] 状態変化: {_currentState} → {detector.State} (score: {score:F3}, at: {loc})");
                    _currentState = detector.State;
                }
                return;
            }
        }

        Console.WriteLine($"[{_detectorSet.GameName}] 未検出。現在の状態: {_currentState}");
    }
}
