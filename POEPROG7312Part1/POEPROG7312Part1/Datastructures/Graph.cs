using System;
using System.Collections.Generic;

namespace POEPROG7312Part1.Datastructures
{
    // Represents an undirected graph for service requests
    // Used to visualize relationships between requests with the same category
    public class Graph
    {
        // Adjacency list stores each request and the list of connected requests
        private Dictionary<string, List<string>> adjacencyList = new();

        // Public getter to access the adjacency list
        public Dictionary<string, List<string>> AdjacencyList => adjacencyList;

        // Add a new node (request) to the graph if it doesn't exist
        public void AddNode(string requestId)
        {
            if (!adjacencyList.ContainsKey(requestId))
                adjacencyList[requestId] = new List<string>();
        }

        // Add an undirected edge between two request nodes
        // Represents a connection between two requests of the same category
        public void AddEdge(string requestA, string requestB)
        {
            if (!adjacencyList.ContainsKey(requestA)) AddNode(requestA);
            if (!adjacencyList.ContainsKey(requestB)) AddNode(requestB);

            adjacencyList[requestA].Add(requestB);
            adjacencyList[requestB].Add(requestA); // Ensure bidirectional connection
        }

        // Display all connections in the graph for debugging or visualization
        public void DisplayConnections()
        {
            foreach (var node in adjacencyList)
            {
                Console.WriteLine($"{node.Key} -> {string.Join(", ", node.Value)}");
            }
        }

        // Perform a Breadth-First Search (BFS) starting from a specific node
        // Visits all connected nodes level by level
        public void Traverse(string start)
        {
            var visited = new HashSet<string>(); // Tracks visited nodes
            var queue = new Queue<string>();     // Queue for BFS traversal

            queue.Enqueue(start); // Start from the given node
            visited.Add(start);

            Console.WriteLine("Graph Traversal:");

            while (queue.Count > 0)
            {
                string current = queue.Dequeue();
                Console.WriteLine(current);

                foreach (var neighbor in adjacencyList[current])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }
    }
}
// Bhaskar, S. & Kaarsgaard, R., 2021. Graph Traversals as Universal Constructions. Available at: https://arxiv.org/abs/2104.14877 (Accessed: 12 November 2025).