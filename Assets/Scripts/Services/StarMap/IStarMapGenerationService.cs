public interface IStarMapGenerationService {
    StarMap GenerateNewMap(GraphGenerationConfig _config);
}

public class StarMapGenerationService : IStarMapGenerationService {
    private readonly IDataRuntimeFactory _dataFactory;
    private readonly StarNamerService _starNamer;

    public StarMapGenerationService(
        IDataRuntimeFactory dataFactory,
        StarNamerService starNamer) {
        _dataFactory = dataFactory;
        _starNamer = starNamer;
    }

    public StarMap GenerateNewMap(GraphGenerationConfig _config) {
        var generator = _dataFactory.CreateInstance<GraphGenerator>(_config);
        var graph = generator.GenerateGraph();
        var map = new StarMap(graph.Seed);

        foreach (var layer in graph.Layers) {
            foreach (var node in layer) {
                var star = new Star(node.layer, node.layerIndex, _starNamer.GetUniqueName());
                map.AddStar(star);

                foreach (var connection in node.GetAllConnections()) {
                    star.ConnectTo(new LayerCoord(connection.layer, connection.layerIndex));
                }
            }
        }

        return map;
    }
}
