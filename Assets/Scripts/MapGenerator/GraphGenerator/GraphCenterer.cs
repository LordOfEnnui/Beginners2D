using System.Collections.Generic;
using UnityEngine;

public class GraphCenterer {

    public void CenterGraph(Graph graph) {
        int maxNodesLevel = FindLevelWithMaxNodes(graph);

        List<List<GraphNode>> levelNodes = graph.GetLevelNodes();

        int maxLevelHeight = levelNodes[maxNodesLevel].Count;

        for (int level = 0; level < levelNodes.Count; level++) {
            List<GraphNode> currentLevel = levelNodes[level];
            int currentLevelHeight = currentLevel.Count;

            float offset = ((float)(maxLevelHeight - currentLevelHeight)) / 2;

            for (int i = 0; i < currentLevel.Count; i++) {
                currentLevel[i].position = new Vector2(level, i + offset);
            }
        }
    }

    private int FindLevelWithMaxNodes(Graph graph) {
        List<List<GraphNode>> levelNodes = graph.GetLevelNodes();
        int maxNodes = 0;
        int maxNodesLevel = 0;

        for (int level = 0; level < levelNodes.Count; level++) {
            int nodesCount = levelNodes[level].Count;
            if (nodesCount > maxNodes) {
                maxNodes = nodesCount;
                maxNodesLevel = level;
            }
        }

        return maxNodesLevel;
    }
}
