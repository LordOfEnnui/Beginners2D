using Zenject;

public interface IStateFactory {
    IState CreateState<T>() where T : class, IState;
}

public class StateFactory : IStateFactory {
    private readonly DiContainer _container;

    public StateFactory(DiContainer container) {
        _container = container;
    }

    IState IStateFactory.CreateState<T>() {
        return _container.Resolve<T>();
    }
}
