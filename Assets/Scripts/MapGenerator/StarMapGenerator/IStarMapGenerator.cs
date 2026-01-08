using System;
using System.Collections.Generic;
using UnityEngine.Audio;

public interface IStarMapGenerator {
    StarMap GenerateNewMap(StarMapGenerationConfig _config);
}

public class StarMapGenerator : IStarMapGenerator, IDataLoader {
    private readonly IDataRuntimeFactory _dataFactory;
    private readonly StarNamerService _starNamer;
    private readonly GraphGenerator graphGenerator;
    
    private readonly IDataRepository<PlanetsData> planetsRepo;
    private PlanetsData planetsData;

    public StarMapGenerator(
        IDataRuntimeFactory dataFactory,
        StarNamerService starNamer) {
        _dataFactory = dataFactory;
        _starNamer = starNamer;

        graphGenerator = new GraphGenerator(_dataFactory);
    }

    public StarMap GenerateNewMap(StarMapGenerationConfig _config) {
        StarMap starMap = CreateFromGraph(_config);
        PopulatePlanetData(starMap, _config);
        return starMap;
    }

    public void Load() {
        planetsData = planetsRepo.Load();
    }

    private StarMap CreateFromGraph(StarMapGenerationConfig _config) {
        var graph = graphGenerator.GenerateGraph(_config.graphConfig);
        var map = new StarMap(graph.Seed);

        foreach (var layer in graph.Layers) {
            foreach (var node in layer) {

                string starName = _starNamer.GetUniqueName();

                var star = new Star(node.layer, node.layerIndex, starName);
                map.AddStar(star);

                foreach (var connection in node.GetAllConnections()) {
                    star.ConnectTo(new LayerCoord(connection.layer, connection.layerIndex));
                }
            }
        }

        return map;
    }

    private void PopulatePlanetData(StarMap starMap, StarMapGenerationConfig config) {
        int seed = starMap.Seed.GetHashCode();
        Random random = new Random(seed);

        PlanetsData planetsData = config.planetsData;
        List<BiomeData> biomes = planetsData.biomes;

        foreach (Star star in starMap.Stars.Values) {
            int biomeIndex = random.Next(0, biomes.Count);
            BiomeData biomeData = biomes[biomeIndex];

            PlanetConfig planetConfig = new PlanetConfig();
            planetConfig.seed = seed;

            planetConfig.BiomeLabel = biomeData.biomeLabel;
            planetConfig.planetSprite = biomeData.planetSprite;

            star.SetPlanetData(planetConfig);
        }
    }
}
