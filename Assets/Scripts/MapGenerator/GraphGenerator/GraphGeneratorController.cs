using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GraphGeneratorController : MonoBehaviour {
    [SerializeField] private GraphPipelineConfig graphPipelineConfig;
    [SerializeField] private MapGenerationData mapGenerationData;
    [Inject] IDataRuntimeFactory dataRuntimeFactory;

    public GraphView graphView;
    [SerializeField] Button startButton;

    private void Start() {
        if (startButton != null)
            startButton.onClick.AddListener(GenerateGraph);
    }

    private void GenerateGraph() {
        if (graphPipelineConfig == null) {
            Debug.LogError("GraphPipelineConfig is not assigned!");
        }

        var pipeline = new GraphGenerationPipeline();
        var stages = graphPipelineConfig.stageConfigs;

        foreach (var stageConfig in stages) {
            if (stageConfig == null) continue;

            var stageInstance = dataRuntimeFactory.CreateInstanse(stageConfig) as IPipelineStage<GraphGenerationContext>;
            pipeline.AddStage(stageInstance);
        }

        var context = new GraphGenerationContext(mapGenerationData);

        pipeline.Execute(context);

        Debug.Log($"Generated graph {context.Graph}");

        graphView.DisplayGraph(context.Graph);
    }

    private void OnDestroy() {
        if (startButton != null)
        startButton.onClick.RemoveListener(GenerateGraph);
    }
}
