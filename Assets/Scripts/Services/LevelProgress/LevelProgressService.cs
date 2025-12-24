using System;

public class LevelProgressService : ILevelProgressService {
    private LevelProgress _levelProgress = new LevelProgress { CurrentLevel = 1, HighestLevelUnlocked = 1 };

    private const string LevelNamePrefix = "Level_";
    public string GetCurrentLevelName() {
        return LevelNamePrefix + _levelProgress.CurrentLevel;
    }

    public LevelProgress GetProgress() {
        return _levelProgress;
    }
    public void SetProgress(LevelProgress progress) {
        _levelProgress = progress;
    }
}


[Serializable]
public class LevelProgress {
    public int CurrentLevel;
    public int HighestLevelUnlocked;
}

