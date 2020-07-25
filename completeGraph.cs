using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace traveling_salesman_console_ver
{
    public class completeGraph
    {// properties
        public long[,] distanceMatrix { get; set; } 
        public static long no_of_nodes { get; set; }
        public  static Node[] lastEdgeNodes= new Node[2];
        static public Node[] Nodes ; // use static to avoid duplication of nodes
        public class shortestpath
        {
            public Node source;
            public Node end;
            public long weight;
            public List<Undirected_Edge> pathEdges = new List<Undirected_Edge>();
            public shortestpath(Node s,Node e) 
            {
                source = s;
                end = e;
            }
        }       
        public Dictionary<long, long> shortestdistance = new Dictionary<long, long>();
        public Node keynode;
        public long keyvalue;

        public class Node
        {
            public long location;
            public int edges_found;
            public int possible_edges;
            public int index=1;
            public List<Node> adjacentNodes = new List<Node>();
            // dictionary to represent distance to different locations
            public Dictionary<long, long> distance;
            //node constructor
            public Node() 
            {}
            public Node(long local, Dictionary<long, long> dic)
            {
                this.location = local;
                this.distance = new Dictionary<long, long>(dic);
                this.edges_found = 0;
                this.possible_edges = 2;
            }
        }
        public Undirected_Edge lastEdge { get; set; }
        public class Hamilton_cycle
        {
            public Dictionary<string, Undirected_Edge> edges;
            public long totalweight() 
            {
                long total = edges.Skip(1).Sum(v => v.Value.weight);
                Console.WriteLine("total weight" +total);
                return total;
            }
            public Hamilton_cycle()
            {
                edges = new Dictionary<string, Undirected_Edge>();

            }
        }
        public Hamilton_cycle minimumHamiltonCycle = new Hamilton_cycle();
        public class Undirected_Edge
        {
            public Node Node1;
            public Node Node2;
            public long node1;
            public long node2;
            public long weight;
            public string id;
            public bool mark;
            long temp;
            public shortestpath arbitarypath;       
            public Undirected_Edge() { }
            public Undirected_Edge(Node n1,Node n2) 
            {
                this.Node1 = n1;
                this.Node2 = n2;
                this.node1 = n1.location;
                this.node2 = n2.location;
            }         
            public Undirected_Edge(long destination, long source)
            {
                this.node1 = destination;
                this.node2 = source;
                this.weight = Nodes[node2].distance[node1];
                this.Node1 = Nodes[node1];
                this.Node2 = Nodes[node2];
                if (node1 > node2)
                {
                    temp = this.node1;
                    this.node1 = node2;
                    this.node2 = temp;
                }
                // gives a uniquie id concatenation of smallest value node and highest value node
                this.id = string.Format("{0}:{1}", node1, node2);
            }
        }
        public completeGraph(long[,] data)
        {
            // check for no of zeros in each row
            try
            {
                if (isValidCompleteGraph(data))
                {
                    distanceMatrix = data;
                    no_of_nodes = data.GetLength(0);
                }
                else
                {
                    throw new notValidCompleteGraph("not complete graph");
                }
            }
            catch (notValidCompleteGraph e)
            {
                Console.WriteLine(e.Message);
            }
            // initialise Nodes array
            Nodes = new Node[no_of_nodes];
            Initialise_nodes();
            FindMinHamiltoncycle();
            lastedge();           
        }
            // if there ia row in matrix more than one zero in thematrix and if no of rows is not equal to no of collumns
        //exceptions
        public class notValidCompleteGraph : Exception 
        {
            public notValidCompleteGraph() { }
            public notValidCompleteGraph(string message)
            : base(message)
            {
            }
            public notValidCompleteGraph(string message, Exception inner)
                : base(message, inner)
            {
            }
        } 
        // methods
        // function to get the distance between two nodes 
        public long Getdistance(long from, long to)
        {
            return distanceMatrix[from, to];
        }   
        // function to check if given data is valid graph
        public bool isValidCompleteGraph(long[,] graph) 
        {            
            long no_of_rows = graph.GetLength(0);
            long no_of_collumns = graph.GetLength(1);
            long[] no_of_zeros = new long[no_of_rows];
            if (no_of_rows == no_of_collumns)
            {
                for (int i = 0; i < no_of_rows; i++) 
                {
                    for (int j = 0; j < no_of_collumns; j++)
                    {
                        if (graph[i, j] == 0 && i != j||i==j && graph[i,j]!=0) // morer than one zero or zero in position ij is not zero
                        {
                            // not valid graph
                            return false;
                        }
                    }
                }
                return true;
            }
            else 
            {
                return false; 
            }
        }

        // function to initialise nodes and store in Nodes array 
        public void Initialise_nodes()
        {
            Dictionary<long, long> dist = new Dictionary<long, long>();
            for (long i = 0; i < no_of_nodes; i++)
            { 
                for (long j = 0; j < no_of_nodes; j++)
                {
                    dist[j] = Getdistance(i, j);
                }
                Node temp = new Node(i, dist);
                Sort(temp);
                Nodes[i] = temp;
            }
        }
        // function to sort node distance data
        public void Sort(Node nm)
        {
            nm.distance = nm.distance.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
        //function to print the nodes
        public void printNodes() 
        {
            for (int i = 0; i < no_of_nodes; i++)
            {
                Console.WriteLine("for node " + i);

                foreach (KeyValuePair<long, long> item in Nodes[i].distance)
                {
                    Console.WriteLine("to: {0}, Value: {1}", item.Key, item.Value);
                }
            }
        }
        // function to finde edged till last max value edge for whic we can yse a shortest path for arbitary path
        public void FindMinHamiltoncycle() 
        {
            foreach (Node n in Nodes)
            {
                //Console.WriteLine("check node " + n.location);
                //Console.WriteLine("possible edges" + n.possible_edges);
                //Console.WriteLine("edges found " + n.edges_found);
                if (n.edges_found == 2)
                {
                    //if both edges are found it wont do anything
                }
                else
                {                   
                    while (n.index <= n.possible_edges) 
                    {
                        long check = n.distance.ElementAt(n.index).Key;

                        //nodes[check ]is the other node to check
                        for (int index2 = 1; index2 <= Nodes[check].possible_edges; index2++)// min range 2 only increase if necessary
                        {
                            long check2 = Nodes[check].distance.ElementAt(index2).Key;
                            //Console.WriteLine("node :" + n.location + " at check  : " + n.index + " goes to " + check + " node: " + check + " atcheck :" + index2 + " goes to " + check2);
                            bool notloop=true;
                            // prevents loop and keeps the cycle one edge missing
                            if (n.adjacentNodes.Count != 0 ) 
                            {
                                if (n.adjacentNodes.ElementAt(0).adjacentNodes != null) 
                                {
                                    //Console.WriteLine("adjacent node" + n.adjacentNodes.ElementAt(0).location);
                                    //Console.WriteLine("adjacent nof of " + n.adjacentNodes.ElementAt(0).adjacentNodes.ElementAt(0).location);
                                    foreach (Node m in n.adjacentNodes.ElementAt(0).adjacentNodes)
                                    {
                                        if (m.location == check)
                                        {
                                            notloop = false;

                                        }
                                    }
                                }   
                            }                          
                            if (n.location == Nodes[check].distance.ElementAt(index2).Key && Nodes[check].edges_found!=2&&notloop)
                            {
                                    //Console.WriteLine("node1:" + check + " node2:" + check2 + "match");
                                    n.index++;
                                    Undirected_Edge temp = new Undirected_Edge(check, check2);
                                    Undirected_Edge e;
                                    //Console.WriteLine("id:" + ed.id);
                                    // below code prevents duplicates
                                    if (minimumHamiltonCycle.edges.Count == no_of_nodes - 1) // get up to only -1 edges are left
                                    {
                                        return;
                                    }
                                    if (minimumHamiltonCycle.edges.TryGetValue(temp.id, out e))
                                    {}
                                    else
                                    {
                                        minimumHamiltonCycle.edges.Add(temp.id, temp);
                                        n.edges_found += 1;
                                        Nodes[check].edges_found += 1;
                                        //Console.WriteLine(n.edges_found + " edges found for node " + n.location + " and node " + Nodes[check].location);
                                        n.adjacentNodes.Add(Nodes[check]) ;
                                        Nodes[check].adjacentNodes.Add(n);
                                    }
                            }
                        }
                        n.index++;
                    }
                }
            }
            foreach (Node n in Nodes)
            {
                if (n.edges_found != 2)
                {
                    n.possible_edges += 1;
                }
            }
            if (minimumHamiltonCycle.edges.Count < no_of_nodes - 1)
            {
                FindMinHamiltoncycle();
            }
        }
        // function to print he hamiltonpath
        public void printMinHamiltonPath()
        {
            foreach (KeyValuePair<string, Undirected_Edge> item in minimumHamiltonCycle.edges)
            {
                Console.WriteLine("node: " + item.Key);
                Console.WriteLine(" weight:" + item.Value.weight);
            }
        }
        // function to find the lastnodes without edges
        public void lastedge()
        {
            foreach (Node n in Nodes)
            {
                if (n.edges_found == 1)
                {
                    if(lastEdgeNodes[0] == null)
                    {
                        lastEdgeNodes[0] = n;

                    }
                    else
                    {
                        lastEdgeNodes[1] = n;
                    }
                }
            }
            djikstra(distanceMatrix, lastEdgeNodes[0], lastEdgeNodes[1]);
        }
        // function gorr djikstras algo
        public void djikstra(long[,] graph, Node source, Node end)
        {
            lastEdge = new Undirected_Edge(source.location,end.location);
            bool[] visited = new bool[no_of_nodes];
            Node[] previousnode = new Node[no_of_nodes];
            foreach (Node n in Nodes)
            {
                // initialise
                shortestdistance.Add(n.location, graph[source.location, n.location]);
                visited[n.location] = false;
                previousnode[n.location] = source;
            }
            visited[source.location] = true;
            //order by value
            shortestdistance.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);  
            foreach (Node no in Nodes)
            {
                // get the minimum vaalue distance not visited
                for (int v = 0; v < no_of_nodes; ++v)
                {
                    if (visited[v] == false)
                    {
                        keynode = Nodes[shortestdistance.ElementAt(v).Key];
                        keyvalue = shortestdistance[keynode.location];
                    }
                }
                visited[keynode.location] = true;
                if (keynode.location == end.location)
                {
                    //Console.WriteLine("distance fronm " + source.location + " to " + end.location);
                    //Console.WriteLine(shortestdistance[keynode.location]);
                    shortestpath sm = new shortestpath(source,end);
                    sm.weight = shortestdistance[keynode.location];
                    lastEdge.weight = sm.weight;
                    Node i = end;
                    while (i.location != source.location)
                    {
                        Node currentnode = i;
                        i = previousnode[currentnode.location];
                        Undirected_Edge e = new Undirected_Edge(currentnode.location,i.location);
                        sm.pathEdges.Add(e);
                        //Console.WriteLine(currentnode.location + " to " + i.location);
                    }
                    lastEdge.arbitarypath = sm;
                    lastEdge.id += "arbitary";
                    minimumHamiltonCycle.edges.Add(lastEdge.id,lastEdge);
                }
                foreach (Node n in Nodes)
                {
                    long newdistance = keyvalue + graph[keynode.location, n.location];
                    long olddistance = shortestdistance[n.location];
                    if (newdistance < olddistance)
                    {
                        shortestdistance[n.location] = newdistance;
                        previousnode[n.location] = keynode;
                    }
                }
                shortestdistance.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
        }
        public void printArbitaryPath(Undirected_Edge ed) 
        {
            Console.WriteLine("Arbitary path for " + ed.id);
            foreach ( Undirected_Edge e in ed.arbitarypath.pathEdges) 
            {
                Console.WriteLine(e.id);
                Console.WriteLine(e.weight);
            }
        }
        public void printLastEdgeArbitaryPath() 
        {
            printArbitaryPath(lastEdge);       
        }
    }

}
