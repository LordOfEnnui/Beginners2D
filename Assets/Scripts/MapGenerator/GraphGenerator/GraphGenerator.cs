using ModestTree;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphGenerator {
    private Graph graph;
    private MapGenerationData settings;

    public Graph GenerateGraph(MapGenerationData settings) {
        if (settings == null) {
            Debug.LogError("null map generation settings");
        }
        this.settings = settings;
        graph = new Graph();

        CreateInitialGraph();
        ModifyNodeCountWithDeviation();

        AddFirstNode();
        AddEndNode();
        AddEndNode();
        graph.UpdateNodesData();
        CreateMainPaths();

        return graph;
    }

    private void CreateInitialGraph() {
        for (int level = 0; level < settings.levelCount; level++) {
            List<GraphNode> currentLevelNodes = new List<GraphNode>();
            for (int i = 0; i < settings.initialNodesPerLevel; i++) {
                GraphNode newNode = new GraphNode(graph.GetNextNodeId(), new Vector2(level, i));
                currentLevelNodes.Add(newNode);
            }
            graph.AddLevel(currentLevelNodes);
        }
    }

    private void ModifyNodeCountWithDeviation() {
        List<List<GraphNode>> levelNodes = graph.GetLevelNodes();

        for (int level = 0; level < levelNodes.Count; level++) {
            List<GraphNode> currentLevel = levelNodes[level];
            int targetNodeCount = CalculateTargetNodeCount(level, levelNodes);

            AdjustLevelToTargetCount(currentLevel, targetNodeCount, level);
        }
    }

    private int CalculateTargetNodeCount(int level, List<List<GraphNode>> levelNodes) {
        if (level == 0) {
            return GetRandomNodeCountInRange(settings.minNodesPerLevel, settings.maxNodesPerLevel);
        }

        int prevLevelNodeCount = levelNodes[level - 1].Count;

        int minPossible = Math.Max(settings.minNodesPerLevel, prevLevelNodeCount - settings.maxNodeDeviation);
        int maxPossible = Math.Min(settings.maxNodesPerLevel, prevLevelNodeCount + settings.maxNodeDeviation);

        if (!settings.allowGradualIncrease) {
            maxPossible = Math.Min(maxPossible, prevLevelNodeCount);
        }
        if (!settings.allowGradualDecrease) {
            minPossible = Math.Max(minPossible, prevLevelNodeCount);
        }

        return GetRandomNodeCountInRange(minPossible, maxPossible);
    }

    private int GetRandomNodeCountInRange(int min, int max) {
        return UnityEngine.Random.Range(min, max + 1);
    }

    private void AdjustLevelToTargetCount(List<GraphNode> currentLevel, int targetCount, int level) {
        int currentCount = currentLevel.Count;

        if (currentCount > targetCount) {
            RemoveNodesFromLevel(currentLevel, currentCount - targetCount);
        } else if (currentCount < targetCount) {
            AddNodesToLevel(currentLevel, targetCount - currentCount, level);
        }
    }

    private void RemoveNodesFromLevel(List<GraphNode> currentLevel, int nodesToRemove) {
        if (nodesToRemove <= 0) return;

        List<int> indices = Enumerable.Range(0, currentLevel.Count).ToList();
        indices.Shuffle();

        for (int i = 0; i < nodesToRemove && currentLevel.Count > 0; i++) {
            int randomIndex = indices[i] % currentLevel.Count;
            currentLevel[randomIndex].ClearConnections();
            currentLevel.RemoveAt(randomIndex);

            for (int j = i + 1; j < indices.Count; j++) {
                if (indices[j] > randomIndex) {
                    indices[j]--;
                }
            }
        }
    }

    private void AddNodesToLevel(List<GraphNode> currentLevel, int nodesToAdd, int level) {
        for (int i = 0; i < nodesToAdd; i++) {
            GraphNode newNode = new GraphNode(graph.GetNextNodeId(),
                new Vector2(level, currentLevel.Count));
            newNode.level = level;
            currentLevel.Add(newNode);
        }
    }

    private void CreateMainPaths() {
        List<List<GraphNode>> levelNodes = graph.GetLevelNodes();

        for (int level = 0; level < levelNodes.Count; level++) {
            List<GraphNode> currentLevel = levelNodes[level];
            List<GraphNode> nextLevel = new();
            if (level < levelNodes.Count - 1) {
                nextLevel = levelNodes[level + 1];
            }

            foreach (var currentNode in currentLevel) {
                List<GraphNode> potentialNextConnections = GetNeardyNodes(currentNode, currentLevel, nextLevel);
                EnsureConnections(currentNode, potentialNextConnections);

                if (level >= 1) {
                    if (!currentNode.HasConnectionsToPrevLevel()) {
                        List<GraphNode> prevLevel = levelNodes[level - 1];
                        List<GraphNode> prevPottentialConnections = GetNeardyNodes(currentNode, currentLevel, prevLevel);
                        MinimalConnection(currentNode, prevPottentialConnections);
                    }
                }
            }
        }
    }

    private void MinimalConnection(GraphNode currentNode, List<GraphNode> prevPottentialConnections) {
        GraphNode connectedNode = ConnectOneRandomNode(currentNode, prevPottentialConnections);
        foreach (var nextNode in connectedNode.nextLevelConnections) {
            if (nextNode.prevLevelConnections.Count > 1) {
                GraphNode unnecessaryConnection = nextNode;
                connectedNode.UnConnect(unnecessaryConnection);
                break;
            }
        }
    }

    private void EnsureConnections(GraphNode currentNode, List<GraphNode> potentialConnections) {
        bool hasConnected = false;

        foreach (var targetNode in potentialConnections) {
            bool shouldConnect = UnityEngine.Random.value <= settings.randomConnectionChance;

            if (shouldConnect) {
                currentNode.ConnectTo(targetNode);
                hasConnected = true;
            }
        }

        if (!hasConnected && potentialConnections.Count > 0) {
            ConnectOneRandomNode(currentNode, potentialConnections);
        }
    }

    private GraphNode ConnectOneRandomNode(GraphNode currentNode, List<GraphNode> connections) {
        if (connections.TryGetRandomElement(out GraphNode targetNode)) {
            currentNode.ConnectTo(targetNode);
        }
        return targetNode;
    }

    private List<GraphNode> GetNeardyNodes(GraphNode currentNode, List<GraphNode> currentLevel, List<GraphNode> nextLevel) {
        List<GraphNode> connectTo = new List<GraphNode>();
        if (nextLevel.IsEmpty()) {
            return connectTo;
        }

        int currentIndex = currentLevel.IndexOf(currentNode);
        float relativePosition = currentLevel.Count > 1 ? (float)currentIndex / (currentLevel.Count - 1) : 0.5f;
        int targetIndex = Mathf.FloorToInt(relativePosition * (nextLevel.Count - 1));

        connectTo.Add(nextLevel[targetIndex]);

        if (targetIndex > 0) {
            connectTo.Add(nextLevel[targetIndex - 1]);
        }

        if (targetIndex < nextLevel.Count - 1) {
            connectTo.Add(nextLevel[targetIndex + 1]);
        }

        return connectTo;
    }

    private void AddFirstNode() {
        List<List<GraphNode>> levelNodes = graph.GetLevelNodes();

        List<GraphNode> enteranceLevel = new List<GraphNode>();
        GraphNode enteranceNode = new GraphNode(graph.GetNextNodeId(), new Vector2(levelNodes.Count, 0));
        enteranceLevel.Add(enteranceNode);

        List<GraphNode> firstLevel = levelNodes[0];
        graph.AddLevel(0, enteranceLevel);
        foreach (GraphNode node in firstLevel) {
            enteranceNode.ConnectToNext(node);
        }
    }

    private void AddEndNode() {
        List<List<GraphNode>> levelNodes = graph.GetLevelNodes();
        List<GraphNode> lastLevel = levelNodes[levelNodes.Count - 1];

        List<GraphNode> endLevel = new List<GraphNode>();
        GraphNode endNode = new GraphNode(graph.GetNextNodeId(), new Vector2(levelNodes.Count, 0));
        endLevel.Add(endNode);

        graph.AddLevel(endLevel);

        foreach (GraphNode node in lastLevel) {
            endNode.ConnectToPrev(node);
        }
    }
}
