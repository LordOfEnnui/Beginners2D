public interface ILevelProgressService {
    string GetCurrentLevelName();
    LevelProgress GetProgress();
    void SetProgress(LevelProgress progress);
}

