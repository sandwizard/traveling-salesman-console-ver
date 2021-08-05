﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;
using System.Collections;

namespace traveling_salesman_console_ver
{
    public class completeGraph
    {// properties
        static public long[,] distanceMatrix { get; set; }
        static public long smallestEdge_weight;
        public static long no_of_nodes { get; set; }
        public static Node[] lastEdgeNodes = new Node[2];
        static public Node[] Nodes;
        public  static Node[] sorted_nodes;// use static to avoid duplication of nodes
        // the constructor below is run when the class is created
        public completeGraph(long[,] data)
        {
            // check for no of zeros in each row
            distanceMatrix = data;
            no_of_nodes = data.GetLength(0);
            Nodes = new Node[no_of_nodes];
            sorted_nodes = new Node[no_of_nodes];
            Initialise_nodes();

            FindMinHamiltoncycle();
            //based_on_priority();
            lastedge();

        }
        public Node future(Node n)
        {
            Node future = new Node(n);
            // remove candidate 1 from future
            future.c1 = future.candidate_1();
            future.candidate_set.Remove(future.c1.id);
            future.c1 = future.candidate_1();
            
            future.c1 = future.candidate_1();
            future.candidate_set.Remove(future.c1.id);
            Console.WriteLine(" future c is " + future.c1.id);
            return future;


        }

        public class Node
        {

            public long location;
            public int edges_found;
            public int possible_edges;
            public int index = 1;
            public Node adj_1;
            public Node adj_2;
            public int current_min_probable_edge_index = 1;
            public int current_min_probable_edge_index2 = 2;
            public  long sum_of_distances;
            
            // dictionary to represent distance to different locations
            public Dictionary<long, long> distance;

            public Dictionary<long, long> distance;
            // dictionary to represent candidate set
            public Dictionary<long, long> candidate_set;
            public bool has_duplicates;
            public bool has_Edges_of_smae_value;
            public long priority;
            public Dictionary<long, long> future_set;
            public Node c1, c2;
            public Undirected_Edge left_edge;
            public Undirected_Edge right_edge;
            public long id;
            public int edges_found;
            public long sum_of_distances;
            public Node()
            { }
            public Node(Node n)
            {
                has_duplicates = n.has_duplicates;
                candidate_set = new Dictionary<long, long>(n.candidate_set);
                id = n.id;
                distance = n.distance;
                priority = n.priority;

            }
            /// <summary>
            /// in  case candidate was emoved it would give the future c1
            /// </summary>
            /// <returns></returns>
            public Node future_c1 ()
            {
                Node fc = Nodes[this.candidate_set.ElementAt(1).Key];
                if (this.has_duplicates) 
                {
                    fc = Nodes[this.candidate_set.ElementAt(1).Key];
                    long c1_priority = this.candidate_1().priority;
                    var edge_weight = this.candidate_set.ElementAt(0).Value;
                    var edge_weight_compare = this.candidate_set.ElementAt(1).Value;
                    List<Node> candidate_duplicate = new List<Node>();
                    candidate_duplicate.Add(Nodes[this.candidate_set.ElementAt(0).Key]);
                    int i = 1;
                    while (edge_weight == edge_weight_compare) 
                    {
                        candidate_duplicate.Add(Nodes[this.candidate_set.ElementAt(i).Key]);
                        i += 1;
                        try
                        {
                            edge_weight_compare = this.candidate_set.ElementAt(i).Value;
                        }
                        catch (System.ArgumentOutOfRangeException e)
                        {
                        }
                    }
                    // remove c1 from candidate_duplicate
                    candidate_duplicate.Remove(this.candidate_1());
                    // get hightst left priority 
                    fc = candidate_duplicate[0];
                    foreach (Node n in candidate_duplicate) 
                    {
                        if(n.priority > candidate_duplicate[0].priority) 
                        {
                            fc = n;
                        }                                        
                    }

                }
                else 
                {
                    fc = Nodes[this.candidate_set.ElementAt(1).Key];


                }
                
                return fc;


            }
            /// <summary>
            /// return candidate at index  in futur implement a list for duplicate values;
            /// </summary>
            /// <returns></returns>
            public Node candidate_1() 
            {
                if (this.candidate_set.Count == 1) 
                {
                    this.c1 = Nodes[this.candidate_set.ElementAt(0).Key];
                    this.has_duplicates = false;
                    return c1;
                }
                this.c1 = Nodes[this.candidate_set.ElementAt(0).Key];
                //Console.WriteLine(" c1 is" + c1.id);
                var edge_weight = this.candidate_set.ElementAt(0).Value;
                this.has_duplicates = false;
                var edge_weight_compare = this.candidate_set.ElementAt(1).Value;
                int i = 1;
                if (this.candidate_set.Count == 2 && edge_weight == edge_weight_compare) 
                {
                    if (this.c1.priority < Nodes[this.candidate_set.ElementAt(1).Key].priority)
                    {
                        this.c1 = Nodes[this.candidate_set.ElementAt(1).Key];
                    }
                    else 
                    {
                        this.c1 = Nodes[this.candidate_set.ElementAt(0).Key];
                    }
                    if(this.candidate_set.ElementAt(0).Value == this.candidate_set.ElementAt(1).Value)
                    {
                        this.has_duplicates = true;
                    }
                    return c1;

                }
                while (edge_weight == edge_weight_compare) 
                {
                    this.has_duplicates = true;
                    // same value edges found compare priority and return
                    if (this.c1.priority < Nodes[this.candidate_set.ElementAt(i).Key].priority) 
                    {
                        this.c1 = Nodes[this.candidate_set.ElementAt(i).Key];
                    }
                    i+=1;
                    try 
                    {
                        edge_weight_compare = this.candidate_set.ElementAt(i).Value;
                    }
                    catch (System.ArgumentOutOfRangeException e) 
                    {
                    }
                    
                    // means till i candidate set has same values so return that whish has highest priority
                }
                return c1;
                       
            }

            /// <summary>
            /// return candidate at indes 2 in future implement a list for duplicate values;
            /// </summary>
            /// <returns></returns>
            public Node candidate_2() 
            {
                this.c2 = Nodes[this.candidate_set.ElementAt(1).Key];
                var c1_weight = this.candidate_set.ElementAt(0).Value;
                var c2_weight = this.candidate_set.ElementAt(1).Value;
                var edge_weight_compare = this.candidate_set.ElementAt(2).Value;
                int i = 2;
                while (c1_weight == c2_weight)
                {
                    // same value edges found compare priority and return
                 
                    this.c2 = Nodes[this.candidate_set.ElementAt(i).Key];
                    c2_weight = this.candidate_set.ElementAt(i).Value;
                                     
                    i += 1;
                    try 
                    {
                        edge_weight_compare = this.candidate_set.ElementAt(i).Value;
                    }
                    catch (System.ArgumentOutOfRangeException e) 
                    {
                    
                    
                    }
                     // means till i candidate set has same values so return that whish has highest priority
                }
                // check for duplicated within c2 set
                while(c2_weight == edge_weight_compare) 
                {
                    if (this.c2.priority < Nodes[this.candidate_set.ElementAt(i).Key].priority)
                    {
                        this.c2 = Nodes[this.candidate_set.ElementAt(i).Key];
                    }
                    i += 1;
                    try 
                    {
                        edge_weight_compare = this.candidate_set.ElementAt(i).Value;
                    }
                    catch(System.ArgumentOutOfRangeException)
                    {

                    }
                   


                }  
                return c2;
            
            }
            // dictionary to represent distance to different locations


            //node constructor
            
            public Node(long local, Dictionary<long, long> dic)
            {
                this.location = local;
                this.distance = new Dictionary<long, long>(dic);
                this.edges_found = 0;
                this.sum_of_distances = distance.Skip(0).Sum(v => v.Value); 
                //Console.WriteLine("sum for node :" + this.location + " =  " + sum_of_distances);
            }

            /// <summary>
            /// set candidate 1 of node to candidate
            /// </summary>
            /// <param name="node"></param>
            /// <param name="candidate"></param>
            /// <returns></returns>
        }

        // methods
        // function to get the distance between two nodes 
        static public long Getdistance(long from, long to)
        {
            return distanceMatrix[from, to];

        }
        public class Hamilton_cycle
        {

            public Node left_coner_node;
            public Node genesisNode;
            public bool match_left;
            public bool match_right;
            public Node right_coner_node;
            public long lr_chains_divider_index;
            public long left_arm_priority;
            public long right_arm_priority;
            // returns the minimum entry in matrix
            public Undirected_Edge findGenesis_edge() 
            {
                // for each entry of smallest edge find nodes on priority                
                // first find genesis Node
                foreach(Node n in sorted_nodes) 
                {
                    long weight_to_candidate1 = n.distance[n.candidate_1().id];
                    if(weight_to_candidate1 == smallestEdge_weight) 
                    {
                        this.genesisNode = Nodes[n.id];
                        break;                    
                    }                              
                }
                Undirected_Edge genesis_edge = new Undirected_Edge(genesisNode, genesisNode.candidate_1());
                return genesis_edge;
            }


         

            public Dictionary<string, Undirected_Edge> edges;
            public long totalweight()
            {
                long total = edges.Skip(0).Sum(v => v.Value.weight);
                Console.WriteLine("total weight" + total);
                return total;
            }

            /// <summary>
            /// prints the edges of given hamiltoncycle h
            /// </summary>
            /// <param name="h"></par
            public void printHamiltonCycle()
            {
                foreach (KeyValuePair<string, Undirected_Edge> item in this.edges)
                {
                    Console.WriteLine("node: " + item.Key);
                    Console.WriteLine(" weight:" + item.Value.weight);
                }
            }

            public Hamilton_cycle()
            {
                edges = new Dictionary<string, Undirected_Edge>();

            }


            /// <summary>
            /// ie which edge to pick is determined will switch once half thed edges are found
            /// </summary>
            public void calculate_lr_priorities()
            {     
               
                 this.left_arm_priority = this.left_coner_node.priority;
                 this.right_arm_priority = this.right_coner_node.priority;
                

            }
            public void calculate_lr_priorities_with_weights()                
            {
                Node left_c = this.left_coner_node.c1;
                Node right_c = this.right_coner_node.c1;
                int no_of_edges_found = this.edges.Count;
                
                if(no_of_edges_found == 1) 
                {
                    this.left_arm_priority = Getdistance(this.left_coner_node.id, left_c.id);
                    this.right_arm_priority = Getdistance(this.right_coner_node.id, right_c.id);

                }
                else 
                { // find cut off ie if even or odd
                    long cutoff;
                    if(no_of_edges_found% 2 == 0) 
                    {
                        //even
                        cutoff = no_of_edges_found / 2;
                    }
                    else
                    {
                        cutoff = (no_of_edges_found - 1) / 2;
                    }
                    //starting priorities is equal to the weight of the c1 to left coner node
                    this.left_arm_priority = Getdistance(this.left_coner_node.id, left_c.id);
                    this.right_arm_priority = Getdistance(this.right_coner_node.id, right_c.id);
                    // loop and add edge weights till cutoff
                    Node current_left_hop = Nodes[this.left_coner_node.id];
                    Node current_right_hop = Nodes[this.right_coner_node.id];
                   
                    Console.WriteLine("cutoff is " + cutoff);

                    for (int i = 0; i <cutoff; i++) 
                    {
                        Console.WriteLine("current right hop is " + current_right_hop.id);
                        Console.WriteLine("current left hop is " + current_left_hop.id);

                        this.left_arm_priority += current_left_hop.right_edge.weight;
                        current_left_hop = current_left_hop.right_edge.right_Node;
                        this.right_arm_priority += current_right_hop.left_edge.weight;
                        current_right_hop = current_right_hop.left_edge.left_Node;
                        Console.WriteLine("left p is " + left_arm_priority);
                        Console.WriteLine("right pis " + right_arm_priority);


                    }
                    // if edges found is greater than one
                }
                if (this.left_coner_node.has_duplicates) 
                {
                    Console.WriteLine("has dups pis" );
                    var edge_weight = this.left_coner_node.candidate_set.ElementAt(0).Value;
                    var comp_weight = this.left_coner_node.candidate_set.ElementAt(1).Value;
                    int count = 1;
                    this.left_arm_priority -= edge_weight;
                    while (edge_weight == comp_weight) 
                    {
                        
                        try 
                        {   
                            
                            comp_weight = this.left_coner_node.candidate_set.ElementAt(count).Value;
                        }
                        catch (System.ArgumentOutOfRangeException) 
                        {
                            break;
                        
                        }
                        this.left_arm_priority += edge_weight;
                        count++;
                    }

                }
                if (this.right_coner_node.has_duplicates)
                {
                    Console.WriteLine("has dups pis");
                    var edge_weight = this.right_coner_node.candidate_set.ElementAt(0).Value;
                    var comp_weight = this.right_coner_node.candidate_set.ElementAt(1).Value;
                    int count = 1;
                    this.right_arm_priority -= edge_weight;
                    while (edge_weight == comp_weight)
                    {
                        
                        try
                        {
                            
                            comp_weight = this.right_coner_node.candidate_set.ElementAt(count).Value;
                        }
                        catch (System.ArgumentOutOfRangeException)
                        {
                            break;

                        }
                        this.right_arm_priority += edge_weight;
                        count++;
                    }

                }

            }
            /// <summary>
            /// adds edge e to hamilton cycle h
            /// </summary>
            /// <param name="h"></param>
            /// <param name="edge"></param>
            public void add_edge_to_cycle(Undirected_Edge e)
            {
                Undirected_Edge f; // used to hold output of search
                if (this.edges.TryGetValue(e.id, out f))
                {
                    Console.WriteLine(" already exist in cycle");
                }
                else
                {
                    this.edges.Add(e.id, e);
                    e.left_Node.edges_found += 1;
                    e.right_Node.edges_found += 1;
                    e.left_Node.candidate_set.Remove(e.right_Node.id);
                    e.right_Node.candidate_set.Remove(e.left_Node.id);
                    // update conors
                    if(e.right_Node.id == this.left_coner_node.id) 
                    {
                        this.left_coner_node = e.left_Node;                                           
                    }
                    else if(e.left_Node.id == this.right_coner_node.id) 
                    {
                        this.right_coner_node = e.right_Node;
                        Console.WriteLine(" her");
                    }
                    // node from candidate set
                    // remove from all sets if 2 edges found
                    if (e.left_Node.edges_found == 2 || e.right_Node.edges_found == 2)
                    {
                        if (e.left_Node.edges_found == 2)
                        {
                            foreach (Node n in Nodes)
                            {
                                n.candidate_set.Remove(e.left_Node.id);
                            }
                        }
                        else
                        {
                            foreach (Node n in Nodes)
                            {
                                n.candidate_set.Remove(e.right_Node.id);
                            }



                        }

                    }

                }
            }

        }
        public Hamilton_cycle minimumHamiltonCycle = new Hamilton_cycle();
        public class Undirected_Edge
        {

            public Node Node1;
            public Node Node2;
            public long node1;
            public long node2;

            public Node ln;
            public Node rn;
            public Node left_Node;
            public Node right_Node;

            public long weight;
            public string id;           
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


            public Undirected_Edge() { }
            public Undirected_Edge(Node ln, Node rn)
            {
                this.left_Node = Nodes[ln.id];
                this.right_Node = Nodes[rn.id];
                this.left_Node.right_edge = this;
                this.right_Node.left_edge = this;
                //Console.WriteLine("left node : " + left_Node.id);
                this.left_Node.candidate_set.Remove(right_Node.id);
                this.right_Node.candidate_set.Remove(left_Node.id);
                this.weight = Nodes[this.left_Node.id].distance[this.right_Node.id];
               
                // gives a uniquie id concatenation of smallest value node and highest value node
                this.id = string.Format("{0} :{1}", left_Node.id, right_Node.id);

            }
        }

        // if there ia row in matrix more than one zero in thematrix and if no of rows is not equal to no of collumns
        //exceptions

        /// <summary>
        /// implement removal of duplicate nodes ie with zero values in future
        /// </summary>
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

        public long Getdistance(long from, long to)
        {
            return distanceMatrix[from, to];
        }


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
            // creatinf distance list and setting up genesis node
            smallestEdge_weight = Getdistance(0,1);
            Dictionary<long, long> dist = new Dictionary<long, long>();
            for (long i = 0; i < no_of_nodes; i++)
            {
                for (long j = 0; j < no_of_nodes; j++)
                {
                    
                    dist[j] = Getdistance(i, j);
                    //Console.WriteLine("distance "+j +" = " + dist[j]);
                    if(i != j && dist[j] < smallestEdge_weight) 
                    {
                        smallestEdge_weight = dist[j];
                        
                        //Console.WriteLine(" smallest edge is " + smallestEdge_weight);
                    }
                    
                }
                Node temp = new Node(i, dist);
                Sort(temp);
                Nodes[i] = temp;
            }
            Dictionary<long, long> prioritised_nodes = new Dictionary<long, long>();
            foreach (Node n in Nodes)
            {
                prioritised_nodes.Add(n.location, n.sum_of_distances);


            }
            //Console.WriteLine("not overflowing here");
            prioritised_nodes = prioritised_nodes.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            int k = 0;
            foreach (var entry in prioritised_nodes)
            {
                sorted_nodes[k] = Nodes[entry.Key];
                k++;

            }
        }


        /// <summary>
        /// sort the edges in order for each node 
        /// </summary>
        /// <param name="nm"></param>
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



        /// <summary>
        /// finds and returns minimum hailtonion cylce 
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        public Hamilton_cycle FindMinHamiltonCycle() 
        {
            // this is the minimum hamilton cycle

            Array.ForEach<Node>(sorted_nodes, p => Console.WriteLine(p.id + " " + p.sum_of_distances));
            Hamilton_cycle minimumHamiltonCycle = new Hamilton_cycle();

            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%% statr%%%%%%%%%%%%%%%%%");
            // first get first priority node i.e genesis node
            Undirected_Edge g = minimumHamiltonCycle.findGenesis_edge();
            // when creating edge remove it from candidate set
            Console.WriteLine(" genesis edge is " + g.id);
            // find candidates for genesis node
            // *********  if it has duplicates then c1 has to hold all values implement in future
            // set up initial left and right conor node
            minimumHamiltonCycle.left_coner_node = Nodes[g.left_Node.id];
            minimumHamiltonCycle.right_coner_node = Nodes[g.right_Node.id];
            // addig edge 
            minimumHamiltonCycle.add_edge_to_cycle(g);

            Node left_c = minimumHamiltonCycle.left_coner_node.candidate_1();
            Node right_c = minimumHamiltonCycle.right_coner_node.candidate_1();
            Console.WriteLine("left c1 is " + left_c.id);
            Console.WriteLine("right c1 is " + right_c.id);

            /// implement duplicates
            // first 2 edges
            
            // decisions made for firstt edge
            Undirected_Edge e = new Undirected_Edge();
            Undirected_Edge d = new Undirected_Edge();
            d = null;
            // later find 2 cycles one from staring left and one from staring right
            // o
           

            // after first 2 edges are picked
            // checking if conors aur updated
            long half_point;
            if (no_of_nodes % 2 == 0)
            {
                //even
                half_point = no_of_nodes / 2;
            }
            else
            {
                half_point = (no_of_nodes - 1) / 2;
            }

            while (minimumHamiltonCycle.edges.Count < no_of_nodes-2 ) 
            {
                
                Console.WriteLine("left coner is " + minimumHamiltonCycle.left_coner_node.id);
                Console.WriteLine("right coner is " + minimumHamiltonCycle.right_coner_node.id);
                // first set up candidates for both coner nodes

                left_c = minimumHamiltonCycle.left_coner_node.candidate_1();
                right_c = minimumHamiltonCycle.right_coner_node.candidate_1();

                Console.WriteLine("left c1 is " + left_c.id);
                Console.WriteLine("right c1 is " + right_c.id);

                

                // now calculate weight sums
                minimumHamiltonCycle.calculate_lr_priorities_with_weights();

                if (left_c.id == right_c.id)
                {
                    Console.WriteLine("c is " + left_c.id);
                    

                    // look to the future 
                    long option_left_value;
                    long option_right_value;
                    long c_id = left_c.id;
                    // if left edge is picked find futur of right
                    if(minimumHamiltonCycle.left_coner_node.candidate_set.Count == 1 || minimumHamiltonCycle.right_coner_node.candidate_set.Count == 1)
                    {
                        Console.WriteLine("cannot look in future nly one candidate left");
                    }
                    else
                    {
                        Node right_future = minimumHamiltonCycle.right_coner_node.future_c1();
                        Node left_future = minimumHamiltonCycle.left_coner_node.future_c1();

                        Console.WriteLine(" right current c is" + minimumHamiltonCycle.right_coner_node.candidate_1().id);
                        Console.WriteLine(" right future c is" + right_future.id);
                        Console.WriteLine(" left current c is" + minimumHamiltonCycle.left_coner_node.candidate_1().id);


                        Console.WriteLine(" left future c is" + right_future.id);

                        option_left_value = Getdistance(minimumHamiltonCycle.left_coner_node.id, c_id) + Getdistance(minimumHamiltonCycle.right_coner_node.id, right_future.id);
                        option_right_value = Getdistance(minimumHamiltonCycle.right_coner_node.id, c_id) + Getdistance(minimumHamiltonCycle.left_coner_node.id, left_future.id);
                        Console.WriteLine("if left picked weight sum = {0} if right is picked weight sum ={1}", option_left_value, option_right_value);

                        if (option_left_value < option_right_value)
                        {
                            e = new Undirected_Edge(Nodes[left_c.id], minimumHamiltonCycle.left_coner_node);
                            minimumHamiltonCycle.right_coner_node.candidate_set.Remove(left_c.id);
                            left_c.candidate_set.Remove(minimumHamiltonCycle.right_coner_node.id);

                        }
                        else if (option_left_value == option_right_value)
                        {
                            Console.WriteLine(" c atches and future weights is same");
                            

                        }
                        else
                        {
                            e = new Undirected_Edge(minimumHamiltonCycle.right_coner_node, Nodes[right_c.id]);
                            minimumHamiltonCycle.left_coner_node.candidate_set.Remove(right_c.id);
                            right_c.candidate_set.Remove(minimumHamiltonCycle.left_coner_node.id);

                        }

                    }
                   
                    

                }
                else 
                {
                    Console.WriteLine("left weight sum is {0} right weightsum is {1} ",
                    minimumHamiltonCycle.left_arm_priority,
                    minimumHamiltonCycle.right_arm_priority);
                    // calculate half point 

                    // before half point is reached
                    //if (minimumHamiltonCycle.edges.Count >= half_point)
                    //{
                    //    // swap prio 
                    //    var temp = minimumHamiltonCycle.left_arm_priority;
                    //    minimumHamiltonCycle.left_arm_priority = minimumHamiltonCycle.right_arm_priority;
                    //    minimumHamiltonCycle.right_arm_priority = temp;

                    //}

                    if (minimumHamiltonCycle.left_arm_priority < minimumHamiltonCycle.right_arm_priority)
                    {
                        // left has less weight so pick left unless swaped;
                        e = new Undirected_Edge(Nodes[left_c.id], minimumHamiltonCycle.left_coner_node);
                        minimumHamiltonCycle.right_coner_node.candidate_set.Remove(left_c.id);
                        left_c.candidate_set.Remove(minimumHamiltonCycle.right_coner_node.id);

                    }
                    else if (minimumHamiltonCycle.left_arm_priority == minimumHamiltonCycle.right_arm_priority)
                    {
                        Console.WriteLine("both have same prio so add both or not fiuger it out");// for now adding both edges if not same
                        if (left_c != right_c)
                        {
                            // adding both edges
                            e = new Undirected_Edge(Nodes[left_c.id], minimumHamiltonCycle.left_coner_node);
                            d = new Undirected_Edge(minimumHamiltonCycle.right_coner_node, Nodes[right_c.id]);
                            minimumHamiltonCycle.right_coner_node.candidate_set.Remove(left_c.id);
                            left_c.candidate_set.Remove(minimumHamiltonCycle.right_coner_node.id);
                            minimumHamiltonCycle.left_coner_node.candidate_set.Remove(right_c.id);
                            right_c.candidate_set.Remove(minimumHamiltonCycle.left_coner_node.id);
                        }

                    }
                    else
                    {
                        // picking right if right has less weight unless halfpoint reached
                        e = new Undirected_Edge(minimumHamiltonCycle.right_coner_node, Nodes[right_c.id]);
                        minimumHamiltonCycle.left_coner_node.candidate_set.Remove(right_c.id);
                        right_c.candidate_set.Remove(minimumHamiltonCycle.left_coner_node.id);

                    }

                }
                
                // when half point is reached
                
                Console.WriteLine("edge created is " + e.id);
                minimumHamiltonCycle.add_edge_to_cycle(e);
                if(d != null) 
                {
                    Console.WriteLine("edge created is " + d.id);
                    minimumHamiltonCycle.add_edge_to_cycle(d);
                    d = null;
                }

            }
            // last 2 edge 
            e = e = new Undirected_Edge(minimumHamiltonCycle.left_coner_node.candidate_1(),minimumHamiltonCycle.left_coner_node);
            minimumHamiltonCycle.add_edge_to_cycle(e);
            e = e = new Undirected_Edge(minimumHamiltonCycle.left_coner_node, minimumHamiltonCycle.right_coner_node);
            minimumHamiltonCycle.add_edge_to_cycle(e);
            // set up candidates for both and compare weights;

            return minimumHamiltonCycle;
        
        }
        
        

       
       

        /// <summary>
        /// returns node connected to Node n at int index of its sorted distances dictionry
        /// </summary>
        /// <param name="index">position in sorted distances  </param>
        /// <param name="n"></param>
        /// <returns></returns>
        public Node getNode(Node n,int index) 
        {
            Node n2 = Nodes[n.distance.ElementAt(index).Key];
            return n2;

        }
      
>>>>>>> Stashed changes

        /// <summary>
        /// use to sort nodes
        /// </summary>
        class NodeComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return -(new CaseInsensitiveComparer()).Compare(((Node)x).sum_of_distances, ((Node)y).sum_of_distances);
            }
        }
<<<<<<< Updated upstream
        public void FindMinHamiltoncycle()
        {

            
            //Console.WriteLine(" un sorted ######");
            //Array.ForEach<Node>(Nodes, p => Console.WriteLine(p.location + " " + p.sum_of_distances));
            //Console.WriteLine("sorted ######");
            //Array.ForEach<Node>(sorted_nodes, p => Console.WriteLine(p.location + " " + p.sum_of_distances));





            foreach (Node n in sorted_nodes)
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
                            //Console.WriteLine("index " + n.index);

                            

                            long check2 = Nodes[check].distance.ElementAt(index2).Key;
                            //Console.WriteLine("node :" + n.location + " at check  : " + n.index + " goes to " + check + " node: " + check + " atcheck :" + index2 + " goes to " + check2);
                            bool notloop = true;
                            // prevents loop and keeps the cycle one edge missing
                            var f = n;
                            f = f.adj_1;
                            var prev_f = n;
                            while (n.location == Nodes[check].distance.ElementAt(index2).Key && f != null)
                            {

                                //Console.WriteLine("f.location =" + f.location);
                                //Console.WriteLine("f prev =" + prev_f.location);
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
                                    notloop = false;

                                }
                            }

                            if (n.location == Nodes[check].distance.ElementAt(index2).Key && Nodes[check].edges_found != 2 && notloop)
                            {
                                //Console.WriteLine("node1:" + check + " node2:" + check2 + "match");
                                n.index++;
                                Undirected_Edge temp = new Undirected_Edge(check, check2);
                                Undirected_Edge e;
                               
                                //Console.WriteLine("id:" + temp.id);
                                

                                
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
                                    //Console.WriteLine("node1:" + check+ " node2:" + n.location + "match");
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
                if (n.edges_found == 2)
                {
                    n.possible_edges = 0;
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
=======

       
        // function to find the lastnodes without edges
>>>>>>> Stashed changes

        //public void store_cycle()
        //{
        //    string path = "D:/Work/Github/Repo/cycle.txt";
        //    // Create a file to write to.
        //    using (StreamWriter sw = File.CreateText(path))
        //    {
        //        foreach (KeyValuePair<string, Undirected_Edge> item in MinimumHamiltonCycle.edges)
        //        {
        //            sw.WriteLine("node: " + item.Key);
        //            sw.WriteLine(" weight:" + item.Value.weight);
        //        }
       //        // Console.WriteLine("distances is " + distances);
        //    }
        //}




































    }
}
