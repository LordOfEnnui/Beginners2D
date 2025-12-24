
public class LevelProgressSystem : SaveableSystem<LevelProgress> {
    public override string SaveKey => "LevelProgress";
    private readonly ILevelProgressService _levelProgressService;
    public LevelProgressSystem(ILevelProgressService levelProgressService, ISaveLoadService saveLoadService) : base(saveLoadService) {
        _levelProgressService = levelProgressService;
    }
    public override LevelProgress CaptureState() {
        return _levelProgressService.GetProgress();
    }
    public override void ApplyState(LevelProgress state) {
        _levelProgressService.SetProgress(state);
    }
}