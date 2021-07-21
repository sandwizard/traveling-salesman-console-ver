using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;
namespace traveling_salesman_console_ver
{
    public class completeGraph
    {// properties
        public long[,] distanceMatrix { get; set; }
        public static long no_of_nodes { get; set; }
        public static Node[] lastEdgeNodes = new Node[2];
        static public Node[] Nodes; // use static to avoid duplication of nodes
        // the constructor below is run when the class is created
        public completeGraph(long[,] data)
        {
            // check for no of zeros in each row
            distanceMatrix = data;
            no_of_nodes = data.GetLength(0);
            //try
            //{
            //    if (isValidCompleteGraph(data))
            //    {
            //        distanceMatrix = data;
            //        no_of_nodes = data.GetLength(0);
            //    }
            //    else
            //    {
            //        throw new notValidCompleteGraph("not complete graph");
            //    }
            //}
            //catch (notValidCompleteGraph e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            // initialise Nodes array
            Nodes = new Node[no_of_nodes];
            Initialise_nodes();
            FindMinHamiltoncycle();
            //based_on_priority();
            lastedge();
        }
        public class shortestpath
        {
            public Node source;
            public Node end;
            public long weight;
            public List<Undirected_Edge> pathEdges = new List<Undirected_Edge>();
            public shortestpath(Node s, Node e)
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
            public int index = 1;
            public Node adj_1;
            public Node adj_2;
            public int current_min_probable_edge_index = 1;
            public  long sum_of_distances;
            
            // dictionary to represent distance to different locations
            public Dictionary<long, long> distance;
            //node constructor
            public Node()
            { }
            public Node(long local, Dictionary<long, long> dic)
            {
                this.location = local;
                this.distance = new Dictionary<long, long>(dic);
                this.edges_found = 0;
                this.possible_edges = 2;
                this.sum_of_distances = distance.Skip(0).Sum(v => v.Value); 
                //Console.WriteLine("sum for node :" + this.location + " =  " + sum_of_distances);
            }
        }
        public Undirected_Edge lastEdge { get; set; }
        public class Hamilton_cycle
        {
            public Dictionary<string, Undirected_Edge> edges;
            public long totalweight()
            {
                long total = edges.Skip(0).Sum(v => v.Value.weight);
                Console.WriteLine("total weight" + total);
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
            public Undirected_Edge(Node n1, Node n2)
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
                        if (graph[i, j] == 0 && i != j || i == j && graph[i, j] != 0) // morer than one zero or zero in position ij is not zero
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
                    //Console.WriteLine("distance "+j +" = " + dist[j]);
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
        // find a wway to make thee checking of minn vaklue edge on nodes ascnchronous

        public void add_adjacent_node(Node n1, Node n2)
        {
            if (n1.adj_1 == null)
            {
                n1.adj_1 = n2;


            }
            else
            {
                n1.adj_2 = n2;

            }

            if (n2.adj_1 == null)
            {
                n2.adj_1 = n1;

            }
            else
            {
                n2.adj_2 = n1;
            }




        }
        public void check_for_loop(Node n1,Node n2 ) 
        {

        
        
        }
        public void FindMiHamiltoncycle()
        {   while (minimumHamiltonCycle.edges.Count != no_of_nodes - 1) 
            {
                foreach (Node n in Nodes)
                {
                    Console.WriteLine("check node " + n.location);

                    if (n.edges_found == 2)
                    {
                        //if both edges are found it wont do anything
                        Console.WriteLine(" both edges found");

                    }
                    else
                    {
                        bool not_pass = true;
                        while (not_pass && n.edges_found != 2)
                        {
                            long check = n.distance.ElementAt(n.current_min_probable_edge_index).Key;
                            Console.WriteLine("node connected to current probable edge of node " + n.location + " = " + check);
                            while(Nodes[check].edges_found==2) 
                            {
                                // test implement code to remove nodes which have found 2 edges from the sorted dictionary might be quicker?
                                Console.WriteLine("trying to connect to a node with already 2 edges so update  ");
                                n.current_min_probable_edge_index += 1;
                                check = n.distance.ElementAt(n.current_min_probable_edge_index).Key;
                                Console.WriteLine("node connected to current probable edge of node " + n.location + " = " + check);



                            }
                            var f = n;
                            var prev_f = f;
                            f = f.adj_1;
                            // check for loop
                            while (n.location == Nodes[check].distance.ElementAt(Nodes[check].current_min_probable_edge_index).Key && f != null)
                            {

                                Console.WriteLine("f.location =" + f.location);
                                Console.WriteLine("f prev =" + prev_f.location);
                                if (f.adj_1 == prev_f)
                                {
                                    prev_f = f;
                                    f = f.adj_2;
                                    //Console.WriteLine("f.adj_1 == prev f");


                                }
                                else if (f.adj_2 == prev_f)
                                {
                                    prev_f = f;
                                    f = f.adj_1;
                                    //Console.WriteLine("f.adj_2 == prev f");
                                }
                                if (f == null)
                                {

                                    break;

                                }
                                if (f.location == check)
                                {
                                    Console.WriteLine("loops so update possible edge and update checked node");
                                    n.current_min_probable_edge_index += 1;
                                    Nodes[check].current_min_probable_edge_index += 1;
                                    check = n.distance.ElementAt(n.current_min_probable_edge_index).Key;

                                }
                            }

                            if (n.location == Nodes[check].distance.ElementAt(Nodes[check].current_min_probable_edge_index).Key)
                            {




                                //Console.WriteLine("match in nodes " + n.location + "and" + Nodes[check].location);
                                Undirected_Edge temp = new Undirected_Edge(n.location, Nodes[check].location);
                                Undirected_Edge e;
                                
                             
                                if (minimumHamiltonCycle.edges.TryGetValue(temp.id, out e))
                                { }
                                else
                                {
                                    minimumHamiltonCycle.edges.Add(temp.id, temp);
                                    n.edges_found += 1;
                                    Nodes[check].edges_found += 1;
                                    n.current_min_probable_edge_index += 1;
                                    Nodes[check].current_min_probable_edge_index += 1;
                                    //Console.WriteLine(n.edges_found + " edges found for node " + n.location + " and " + Nodes[check].edges_found + " edges found for node " + Nodes[check].location);
                                    add_adjacent_node(n, Nodes[check]);
                                    //Console.WriteLine("adjacent node " + n.location + " is " + Nodes[check].location);
                                    
                                }
                                //Console.WriteLine("no of edges found = " + minimumHamiltonCycle.edges.Count);
                                if (minimumHamiltonCycle.edges.Count == no_of_nodes - 1) // get up to only -1 edges are left
                                {
                                    return;
                                }

                            }
                            else
                            {
                                not_pass = false;

                            }
                        }


                    }

                }
            }
            

        }
        public void based_on_priority() 
        {
            
            
            Dictionary<long, long> prioritised_nodes = new Dictionary<long, long>();
            foreach (Node n in Nodes)
            {
                prioritised_nodes.Add(n.location, n.sum_of_distances);


            }
            bool is_odd_no_of_nodes;
            Console.WriteLine(no_of_nodes);
            float mod = (float)no_of_nodes / 2;
            // since we start from 0 minus one from mod to get center 
            mod = mod - 1;
            
            
            Console.WriteLine("half is " + mod);
            if (no_of_nodes % 2 == 0) 
            {
                
                Console.WriteLine("is even");
                is_odd_no_of_nodes = false;
            }
            else 
            {

                is_odd_no_of_nodes = true;
                Console.WriteLine("is odd");
            
            }

            // sort Nodes based on priority sum of all edges hiher value is higher priority
           
            prioritised_nodes = prioritised_nodes.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach(var entry in prioritised_nodes) 
            {
                Console.WriteLine(entry.Value);
                Console.WriteLine(entry.Key);
            }
           
            for (int i = 0; i<=mod;i++) 
            {
                var a = prioritised_nodes.ElementAt(i).Key;
                var node1 = Nodes[a];
                Console.WriteLine(" node = " + node1.location);
                Node node1_adj;
                long check;
                if (node1.edges_found == 2)
                {

                }
                
                else 
                {
                    int no_edges_to_find_in_first_half = 2;
                    if (is_odd_no_of_nodes && i == mod - 0.5)
                    {
                        no_edges_to_find_in_first_half = 1;

                    }
                    if (!is_odd_no_of_nodes && i == mod )
                    {
                        Console.WriteLine("at this point only 1 edge");
                        Console.WriteLine(" node = " + node1.location +" edges = " + node1.edges_found);
                        no_edges_to_find_in_first_half = 1;

                    }

                    while (node1.edges_found != no_edges_to_find_in_first_half)
                    {
                        
                        check = node1.distance.ElementAt(node1.current_min_probable_edge_index).Key;
                        node1_adj = Nodes[check];
                        Console.WriteLine("nodes picked are node : " + node1.location + " and node : " + node1_adj.location);
                        Console.WriteLine("edges found " + node1.edges_found);
                        while (node1_adj.edges_found == 2)
                        {

                            node1.current_min_probable_edge_index += 1;
                            Console.WriteLine("2 edges already there will lopp so updating");
                            check = node1.distance.ElementAt(node1.current_min_probable_edge_index).Key;
                            node1_adj = Nodes[check];
                            Console.WriteLine(" new nodes picked are node : " + node1.location + " and node : " + node1_adj.location);





                        }

                        var f = node1;
                        var prev_f = f;
                        f = f.adj_1;
                        while (f != null )
                        {
                            //Console.WriteLine("f.location =" + f.location);
                            //Console.WriteLine("f prev =" + prev_f.location);
                            while (node1_adj.edges_found == 2)
                            {

                                node1.current_min_probable_edge_index += 1;
                                Console.WriteLine("2 edges already there will lopp so updating");
                                check = node1.distance.ElementAt(node1.current_min_probable_edge_index).Key;
                                node1_adj = Nodes[check];
                                Console.WriteLine(" new nodes picked are node : " + node1.location + " and node : " + node1_adj.location);





                            }



                            if (f.adj_1 == prev_f)
                            {
                                prev_f = f;
                                f = f.adj_2;
                                //Console.WriteLine("f.adj_1 == prev f");


                            }
                            else if (f.adj_2 == prev_f)
                            {
                                prev_f = f;
                                f = f.adj_1;
                                //Console.WriteLine("f.adj_2 == prev f");
                            }
                            if (f == null)
                            {
                                Console.WriteLine("f is null escaping loop");
                                break;

                            }
                            if (f.location == node1_adj.location )
                            {
                                Console.WriteLine("loops so update possible edge and update checked node");
                                node1.current_min_probable_edge_index += 1;

                                check = node1.distance.ElementAt(node1.current_min_probable_edge_index).Key;
                                node1_adj = Nodes[check];
                                Console.WriteLine(" new nodes picked are node : " + node1.location + " and node : " + node1_adj.location);

                            }


                        }
                        Undirected_Edge temp = new Undirected_Edge(node1.location, node1_adj.location);
                        Undirected_Edge e;
                        if (minimumHamiltonCycle.edges.TryGetValue(temp.id, out e))
                        {
                            //Console.WriteLine("edge already added");
                            node1.current_min_probable_edge_index += 1;
                            check = node1.distance.ElementAt(node1.current_min_probable_edge_index).Key;
                            node1_adj = Nodes[check];
                            Console.WriteLine(" new nodes picked are node : " + node1.location + " and node : " + node1_adj.location);
                        }
                        else
                        {
                            minimumHamiltonCycle.edges.Add(temp.id, temp);
                            node1.edges_found += 1;
                            node1_adj.edges_found += 1;
                            node1.current_min_probable_edge_index += 1;
                            if (node1_adj.distance.ElementAt(node1_adj.current_min_probable_edge_index).Key == node1.location)
                            {
                                node1_adj.current_min_probable_edge_index += 1;

                            }
                            if (minimumHamiltonCycle.edges.Count == no_of_nodes - 1) // get up to only -1 edges are left
                            {
                                return;
                            }
                            Console.WriteLine(node1.edges_found + " edges found for node " + node1.location + " and " + node1_adj.edges_found + " edges found for node " + node1_adj.location);
                            add_adjacent_node(node1, node1_adj);


                        }

                    }
                }


            }
            int no_of_edges_to_find = 1;
            
            Node pivot = Nodes[1];
            // half done now from bottom up
            Console.WriteLine("******************* part 2 start *************");
            Console.WriteLine("mod " + mod);
            
            foreach(var entry in prioritised_nodes) 
            {
                //Console.WriteLine(Nodes[entry.Key].edges_found + " edges found for node " + entry.Key);
                if (Nodes[entry.Key].edges_found == 0) 
                {
                    
                    Console.WriteLine("highest no edges found at node " + entry.Key);
;                   pivot = Nodes[entry.Key];
                    break;
                
                }
            
            
            }
            while (minimumHamiltonCycle.edges.Count != no_of_nodes - 1)// get up to only -1 edges are left
            {
                for (int i = (int)no_of_nodes - 1; i >= mod ; i--)
                {
                    var a = prioritised_nodes.ElementAt(i).Key;
                    var node1 = Nodes[a];
                    Console.WriteLine(" node = " + node1.location);
                    Console.WriteLine(" node edges found  " + node1.edges_found);
                    Node node1_adj;
                    Console.WriteLine("pivot is " + pivot.location);
                    long check;

                    
                   
                    if (node1.edges_found == 2)
                    {


                    }
                    else
                    {
                        if(!is_odd_no_of_nodes &&  node1.location == pivot.location) 
                        {
                            no_of_edges_to_find = 2;
                            i = (int)no_of_nodes;

                        }


                        if (is_odd_no_of_nodes && node1.edges_found == 0 && node1.location == pivot.location )
                        {
                            no_of_edges_to_find = 2;
                            i = (int)no_of_nodes;



                        }
                        while (node1.edges_found != no_of_edges_to_find)
                        {
                            if (node1.edges_found == 2)
                            {
                                break;

                            }

                            check = node1.distance.ElementAt(node1.current_min_probable_edge_index).Key;
                            node1_adj = Nodes[check];
                            Console.WriteLine("nodes picked are node : " + node1.location + " and node : " + node1_adj.location);
                            Console.WriteLine("edges found " + node1.edges_found);
                            while (node1_adj.edges_found == 2)
                            {

                                node1.current_min_probable_edge_index += 1;
                                Console.WriteLine("2 edges already there will lopp so updating");
                                check = node1.distance.ElementAt(node1.current_min_probable_edge_index).Key;
                                node1_adj = Nodes[check];
                                Console.WriteLine(" new nodes picked are node : " + node1.location + " and node : " + node1_adj.location);





                            }
                            var f = node1;
                            var prev_f = f;
                            f = f.adj_1;

                            while (f != null)
                            {
                                //Console.WriteLine("f.location =" + f.location);
                               // Console.WriteLine("f prev =" + prev_f.location);
                                while (node1_adj.edges_found == 2)
                                {

                                    node1.current_min_probable_edge_index += 1;
                                    Console.WriteLine("2 edges already there will lopp so updating");
                                    check = node1.distance.ElementAt(node1.current_min_probable_edge_index).Key;
                                    node1_adj = Nodes[check];




                                }

                                if (f.adj_1 == prev_f)
                                {
                                    prev_f = f;
                                    f = f.adj_2;
                                    //Console.WriteLine("f.adj_1 == prev f");


                                }
                                else if (f.adj_2 == prev_f)
                                {
                                    prev_f = f;
                                    f = f.adj_1;
                                    //Console.WriteLine("f.adj_2 == prev f");
                                }
                                if (f == null)
                                {
                                    //Console.WriteLine("f is null escaping loop");
                                    break;

                                }
                                if (f.location == node1_adj.location)
                                {
                                    Console.WriteLine("loops so update possible edge and update checked node");
                                    node1.current_min_probable_edge_index += 1;

                                    check = node1.distance.ElementAt(node1.current_min_probable_edge_index).Key;
                                    node1_adj = Nodes[check];
                                    Console.WriteLine(" new nodes picked are node : " + node1.location + " and node : " + node1_adj.location);

                                }


                            }
                            Undirected_Edge temp = new Undirected_Edge(node1.location, node1_adj.location);
                            Undirected_Edge e;
                            if (minimumHamiltonCycle.edges.TryGetValue(temp.id, out e))
                            {
                                Console.WriteLine("edge already added");
                                node1.current_min_probable_edge_index += 1;
                                check = node1.distance.ElementAt(node1.current_min_probable_edge_index).Key;
                                node1_adj = Nodes[check];
                                Console.WriteLine(" new nodes picked are node : " + node1.location + " and node : " + node1_adj.location);
                            }
                            else
                            {
                                minimumHamiltonCycle.edges.Add(temp.id, temp);
                                node1.edges_found += 1;
                                node1_adj.edges_found += 1;
                                node1.current_min_probable_edge_index += 1;
                                if (node1_adj.distance.ElementAt(node1_adj.current_min_probable_edge_index).Key == node1.location)
                                {
                                    node1_adj.current_min_probable_edge_index += 1;

                                }
                                if (minimumHamiltonCycle.edges.Count == no_of_nodes - 1) // get up to only -1 edges are left
                                {
                                    return;
                                }
                                Console.WriteLine(node1.edges_found + " edges found for node " + node1.location + " and " + node1_adj.edges_found + " edges found for node " + node1_adj.location);
                                add_adjacent_node(node1, node1_adj);


                            }


                        }
                    }

                }

            }
           
            
                
               
           
            
            



        }
     

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

                        foreach (var item in n.distance)
                        {
                            //Console.WriteLine("[node distance] : " + item);

                        }
                        long check = n.distance.ElementAt(n.index).Key;
                        
                        //nodes[check ]is the other node to check
                        for (int index2 = 1; index2 <= Nodes[check].possible_edges; index2++)// min range 2 only increase if necessary
                        {
                            Console.WriteLine("index " + n.index);


                            
                            long check2 = Nodes[check].distance.ElementAt(index2).Key;
                            Console.WriteLine("node :" + n.location + " at check  : " + n.index + " goes to " + check + " node: " + check + " atcheck :" + index2 + " goes to " + check2);
                            bool notloop = true;
                            // prevents loop and keeps the cycle one edge missing
                            var f = n;
                            f = f.adj_1;
                            var prev_f = n;
                            while (n.location == Nodes[check].distance.ElementAt(index2).Key && f != null)
                            {

                                Console.WriteLine("f.location =" + f.location);
                                Console.WriteLine("f prev =" + prev_f.location);
                                if (f.adj_1 == prev_f)
                                {
                                    prev_f = f;
                                    f = f.adj_2;
                                    Console.WriteLine("f.adj_1 == prev f");


                                }
                                else if (f.adj_2 == prev_f)
                                {
                                    prev_f = f;
                                    f = f.adj_1;
                                    Console.WriteLine("f.adj_2 == prev f");
                                }
                                if (f == null)
                                {
                                    break;

                                }
                                if (f.location == check)
                                {
                                    notloop = false;

                                }
                            }

                            if (n.location == Nodes[check].distance.ElementAt(index2).Key && Nodes[check].edges_found != 2 && notloop)
                            {
                                Console.WriteLine("node1:" + check + " node2:" + check2 + "match");
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
                                { }
                                else
                                {
                                    minimumHamiltonCycle.edges.Add(temp.id, temp);
                                    n.edges_found += 1;
                                    Nodes[check].edges_found += 1;
                                    // Console.WriteLine(n.edges_found + " edges found for node " + n.location + " and node " + Nodes[check].location);
                                    
                                    if (n.adj_1 == null)
                                    {
                                        n.adj_1 = Nodes[check];


                                    }
                                    else
                                    {
                                        n.adj_2 = Nodes[check];

                                    }
                                    

                                    if (Nodes[check].adj_1 == null)
                                    {
                                        Nodes[check].adj_1 = n;

                                    }
                                    else
                                    {
                                        Nodes[check].adj_2 = n;
                                    }
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
            // function to print he hamiltonpath

        }
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
                    if (lastEdgeNodes[0] == null)
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
            lastEdge = new Undirected_Edge(source.location, end.location);
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
                    shortestpath sm = new shortestpath(source, end);
                    sm.weight = shortestdistance[keynode.location];
                    lastEdge.weight = sm.weight;
                    Node i = end;
                    while (i.location != source.location)
                    {
                        Node currentnode = i;
                        i = previousnode[currentnode.location];
                        Undirected_Edge e = new Undirected_Edge(currentnode.location, i.location);
                        sm.pathEdges.Add(e);
                        //Console.WriteLine(currentnode.location + " to " + i.location);
                    }
                    lastEdge.arbitarypath = sm;
                    lastEdge.id += "arbitary";
                    minimumHamiltonCycle.edges.Add(lastEdge.id, lastEdge);
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
            foreach (Undirected_Edge e in ed.arbitarypath.pathEdges)
            {
                Console.WriteLine(e.id);
                Console.WriteLine(e.weight);
            }
        }
        public void printLastEdgeArbitaryPath()
        {
            printArbitaryPath(lastEdge);
        }
        public void store_cycle()
        {
            string path = "D:/Work/Github/Repo/cycle.txt";
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                foreach (KeyValuePair<string, Undirected_Edge> item in minimumHamiltonCycle.edges)
                {
                    sw.WriteLine("node: " + item.Key);
                    sw.WriteLine(" weight:" + item.Value.weight);
                }


                // Console.WriteLine("distances is " + distances);


            }

        }




































    }
}
