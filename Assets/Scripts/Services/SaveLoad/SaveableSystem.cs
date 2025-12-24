using Zenject;

public abstract class SaveableSystem<T> : ISaveable<T> {
    public abstract string SaveKey { get; }
    public abstract T CaptureState();
    public abstract void ApplyState(T state);

    // Self-registration
    private ISaveLoadService _saveLoadService;

    protected SaveableSystem(ISaveLoadService saveLoadService) {
        _saveLoadService = saveLoadService;
        _saveLoadService.Register(this);
    }
}
