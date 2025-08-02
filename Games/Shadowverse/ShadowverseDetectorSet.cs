using System.Collections.Generic;

public class ShadowverseDetectorSet : IGameStateDetectorSet
{
    public string GameName => "Shadowverse";
    public string ProcessName => "ShadowverseWB";

    public List<IStateDetector> CreateDetectors()
    {
        return new List<IStateDetector>
        {
            new MatchStartDetector(),
            new BattleDetector(),
            new ResultDetector()
        };
    }
}