//2024 CAB301 Assignment 3 
//TransportationNetwok.cs
//Assignment3B-TransportationNetwork

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

public partial class TransportationNetwork
{
    int INF = 9999;
    //int INF = int.MaxValue; 

    private string[]? intersections; //array storing the names of those intersections in this transportation network design
    private int[,]? distances; //adjecency matrix storing distances between each pair of intersections, if there is a road linking the two intersections

    public string[]? Intersections
    {
        get {return intersections;}
    }

    public int[,]? Distances
    {
        get { return distances; }
    }


    //Read information about a transportation network plan into the system
    //Preconditions: The given file exists at the given path and the file is not empty
    //Postconditions: Return true, if the information about the transportation network plan is read into the system, the intersections are stored in the class field, intersections, and the distances of the links between the intersections are stored in the class fields, distances;
    //                otherwise, return false and both intersections and distances are null.
    public bool ReadFromFile(string filePath)
    {

        //Pre Condition 
        if (!File.Exists(filePath) || string.IsNullOrEmpty(filePath))
        {
            return false;
        }

        try
        {
            // Read every line and store it in "lines" 
            string[] lines = File.ReadAllLines(filePath); 

            //Initialize the intersections array size.
            HashSet <string> intersectionSet = new HashSet<string> ();

            foreach (var line in lines)
            {
                string[] parts = line.Split(',');
                intersectionSet.Add(parts[0].Trim());
                intersectionSet.Add(parts[1].Trim());
            }
       

            // Convert the set to an array and store in the intersections field
            intersections = intersectionSet.ToArray();
            Array.Sort(intersections);


            // Initialize the distances matrix with INF values
            int n = intersections.Length;
            distances = new int[n, n];

            for(int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    distances[i,j] = INF;
                }
            }
           


            foreach (var line in lines )
            {

                string[] parts = line.Split(',');
                string from = parts[0].Trim();
                string to = parts[1].Trim();
                int distance = int.Parse(parts[2].Trim());

                // Find the indices of the intersections
                int fromIndex = Array.IndexOf(intersections, from);
                int toIndex = Array.IndexOf(intersections, to);

                // Store the distance in the matrix
                distances[fromIndex, toIndex] = distance;

            }
            return true;
        }
        catch
        {
            intersections = null;
            distances = null;
            return false;
        }
    }


    //Display the transportation network plan with intersections and distances between intersections
    //Preconditions: The given file exists at the given path and the file is not empty
    //Postconditions: The transportation netork is displayed in a matrix format
    public void DisplayTransportNetwork()
    {
        Console.Write("       ");
        for (int i = 0; i < intersections?.Length; i++)
        {
                    Console.Write(intersections[i].ToString().PadRight(5) + "  ");
        }
        Console.WriteLine();


        for (int i = 0; i < distances?.GetLength(0); i++)
        {
            Console.Write(intersections[i].ToString().PadRight(5) + "  ");
            for (int j = 0; j < distances?.GetLength(1); j++)
            {
                if (distances[i, j] == Int32.MaxValue)
                    Console.Write("INF  " + "  ");
                else
                    Console.Write(distances[i, j].ToString().PadRight(5)+"  ");
            }
            Console.WriteLine();
        }
    }


    //Check if this transportation network is strongly connected. A transportation network is strongly connected, if there is a path from any intersection to any other intersections in thihs transportation network. 
    //Precondition: Transportation network plan data have been read into the system.
    //Postconditions: return true, if this transpotation netork is strongly connected; otherwise, return false. This transportation network remains unchanged.
    public bool IsConnected()
    {
        //Check for nullable case
        if ((intersections == null) || (distances == null))
        {
            return false;
        }

        bool[] visited = new bool[intersections.Length];

        // Initialise all the vertices as not visited
        for (int i = 0; i < intersections.Length; i++)
        {
            visited[i] = false;
        }

        // Start the Depth First Search for the given graph from the first vertex
        DepthFirstSearchGivenGraph(0, visited);

        // Check that all vertices have been visited in given graph to verify that it is strongly connected
        for (int i = 0; i < intersections.Length; i++)
        {
            if (visited[i] == false)
            {
                return false;
            }
        }

        // Reset all the vertices as not visited for reverse graph
        for (int i = 0; i < intersections.Length; i++)
        {
            visited[i] = false;
        }

        // Start the Depth First Search for the reverse graph from the first vertex
        DepthFirstSearchReverseGraph(0, visited);

        // Check that all vertices have been visited for reverse graph to verify that it is strongly connected
        for (int i = 0; i < intersections.Length; i++)
        {
            if (visited[i] == false)
            {
                return false;
            }
        }

        return true;

    }

    //Mark each vertix (v) as visited if called.
    //Recursively call each adjacent vertix for the directed given graph
    private void DepthFirstSearchGivenGraph(int v, bool[] visited)
    {
        visited[v] = true;
        

        for (int i = 0; i < intersections!.Length; i++)
        {
            if (distances![v, i] != INF && !visited[i])
            {
                DepthFirstSearchGivenGraph(i, visited);
            }
        }
    }

    //Mark each vertix (v) as visited if called.
    //Recursively call each adjacent vertix for the reverse directed given graph
    private void DepthFirstSearchReverseGraph(int v, bool[] visited)
    {
        visited[v] = true;
        

        for (int i = 0; i < intersections!.Length; i++)
        {
            if (distances![i, v] != INF && !visited[i])
            {
                DepthFirstSearchReverseGraph(i, visited); 
            }
        }
    }

    //Find the shortest path between a pair of intersections
    //Precondition: transportation network plan data have been read into the system
    //Postcondition: return the shorest distance between two different intersections; return 0 if there is no path from startVerte to endVertex; returns -1 if startVertex or endVertex does not exists. This transportation network remains unchanged.
    public int FindShortestDistance(string startVertex, string endVertex)
    {
        //Check for nullable case
        if ((intersections == null) || (distances == null))
        {
            return -1;
        }

        //Find index of start and end vertices
        int startIndex = GetIntersectionIndexForVertex(startVertex);
        int endIndex = GetIntersectionIndexForVertex(endVertex);

        //Check that the startVertex and endVertex are valid strings
        if ((startIndex == INF) || (endIndex == INF))
        {
            return -1;
        }

        // for each vertex store the shortest distance from startVertex to each vertex
        int[] shortestDistances = new int[intersections.Length];

        // for each vertex store true or false if shortest distance from startVertex to each vertex has been finalised
        bool[] includedInShortestPathTreeSet = new bool[intersections.Length];


        // Initialize all shortestDistances as infinite and includedInShortestPathTreeSet as false
        for (int i = 0; i < intersections.Length; i++)
        {
            shortestDistances[i] = INF;
            includedInShortestPathTreeSet[i] = false;
        }

        // Distance of startVertex from itself is always 0
        shortestDistances[startIndex] = 0;

        for (int i = 0; i < intersections.Length - 1; i++)
        {
            // Minimum distance vertex from the set of vertices not yet processed
            // Note that in the first iteration the minDistanceVertex is always the same as the startIndex
            int minDistanceVertex = MinimumDistance(shortestDistances, includedInShortestPathTreeSet);

            // Mark the selected vertex as processed
            includedInShortestPathTreeSet[minDistanceVertex] = true;

            // Update the distance value of the adjacent vertices from the picked vertex
            for (int j = 0; j < intersections.Length; j++)
            {
                if ((!includedInShortestPathTreeSet[j]) 
                    && (distances[minDistanceVertex,j] != INF) 
                    && (shortestDistances[minDistanceVertex] != INF) 
                    && (shortestDistances[minDistanceVertex] + distances[minDistanceVertex,j] < shortestDistances[j])) 
                    {
                        shortestDistances[j] = shortestDistances[minDistanceVertex] + distances[minDistanceVertex, j];
                    }
            }
        }

        int result = shortestDistances[endIndex];

        if (result == INF)
        {
            return 0;
        } 
        return result;

    }
    
    //Get the index of a given string vertex stored in intersections
    private int GetIntersectionIndexForVertex(string vertex)
    {
        for (int i = 0; i < intersections!.Length; i++)
        {
            if (intersections[i].ToString().Equals(vertex, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }
        return INF;

    }

    //Find the vertex with the smallest distance value from the set of vertices that are not yet included in the includedInShortestPathTreeSet
    private int MinimumDistance(int[] shortestDistances, bool[] includedInShortestPathTreeSet)
    {
        // Initialize min value and index
        int min = INF;
        int min_index = -1;

        for (int i = 0; i < intersections!.Length; i++)
            if (includedInShortestPathTreeSet[i] == false && shortestDistances[i] <= min)
            {
                min = shortestDistances[i];
                min_index = i;
            }
        return min_index;
    }


    //Find the shortest path between all pairs of intersections
    //Precondition: transportation network plan data have been read into the system
    //Postcondition: return the shorest distances between between all pairs of intersections through a two-dimensional int array and this transportation network remains unchanged

    public int[,] FindAllShortestDistances()
    {
        //Check for nullable case
        if ((intersections == null) || (distances == null)) 
        {
            return new int[0,0];
        }

        int[,] shortestdistance = new int [intersections.Length, intersections.Length];


        /* Initialize the solution matrix same as input graph matrix. Or
           we can say the initial values of shortest distances are based
           on shortest paths considering no intermediate vertex. */
        for ( int i = 0; i < intersections.Length; i++)
        {
            for (int j = 0; j < intersections.Length; j++)
            {
                shortestdistance[i, j] = distances[i, j];
            }
        }         
                

        for (int k = 0; k < intersections.Length; k++)
        {
            for (int i = 0; i < intersections.Length; i++)
            {
                for (int j = 0; j < intersections.Length; j++)
                {
                    if (shortestdistance[i, k] + shortestdistance[k, j] < shortestdistance[i, j])
                        shortestdistance[i, j] = shortestdistance[i, k] + shortestdistance[k, j];
                   
                }
            }
        }
        return shortestdistance;
    }
}