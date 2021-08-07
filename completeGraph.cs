using System;
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

        }
        public void Initialise_nodes()
        {
            // creatinf distance list and setting up genesis node
            smallestEdge_weight = Getdistance(0, 1);
            Dictionary<long, long> dist = new Dictionary<long, long>();
            for (long i = 0; i < no_of_nodes; i++)
            {
                for (long j = 0; j < no_of_nodes; j++)
                {

                    dist[j] = Getdistance(i, j);
                    //Console.WriteLine("distance "+j +" = " + dist[j]);
                    if (i != j && dist[j] < smallestEdge_weight)
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
                prioritised_nodes.Add(n.id, n.sum_of_distances);


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

        public Node future(Node n,Node[] state)
        {
            Node future = new Node(n);
            // remove candidate 1 from future
            future.c1 = future.candidate_1(state);
            future.candidate_set.Remove(future.c1.id);
            future.c1 = future.candidate_1(state);
            
            future.c1 = future.candidate_1(state);
            future.candidate_set.Remove(future.c1.id);
            Console.WriteLine(" future c is " + future.c1.id);
            return future;


        }

        public class Node : ICloneable
        {
           
            public int edges_found;
            public int possible_edges;
            public  long sum_of_distances;            
            // dictionary to represent distance to different locations
            public Dictionary<long, long> distance;

            // dictionary to represent candidate set
            public Dictionary<long, long> candidate_set;
            public Dictionary<long, long> imaginary_candidate_set;
            public bool has_duplicates;
            public long priority;
            public Dictionary<long, long> future_set;
            public Node c1, c2;
            public Undirected_Edge left_edge;
            public Undirected_Edge right_edge;
            public long id;

            public Node()
            { }
            public Node(Node n)
            {
                has_duplicates = n.has_duplicates;
                candidate_set = new Dictionary<long, long>(n.candidate_set);
                imaginary_candidate_set = new Dictionary<long, long>(n.candidate_set);
                id = n.id;
                edges_found = n.edges_found;
                distance = n.distance;
                priority = n.priority;
                sum_of_distances = n.sum_of_distances;

            }
            public Object Clone() 
            {
                return new Node(this);
            }
       
            /// <summary>
            /// in  case candidate was emoved it would give the future c1
            /// </summary>
            /// <returns></returns>
            public Node future_c1 (Node[] state)
            {
                Node fc = state[this.candidate_set.ElementAt(1).Key];
                if (this.has_duplicates) 
                {
                    if(this.candidate_set.Count == 2) 
                    {
                        fc = fc = state[this.candidate_set.ElementAt(1).Key];
                        Node fc2 = fc = state[this.candidate_set.ElementAt(0).Key];
                        if(fc.priority >= fc2.priority) 
                        {
                            return fc;
                        }
                        else 
                        {
                            return fc2;
                        }

                    }
                    fc = state[this.candidate_set.ElementAt(1).Key];
                    long c1_priority = this.candidate_1(state).priority;
                    var edge_weight = this.candidate_set.ElementAt(0).Value;
                    var edge_weight_compare = this.candidate_set.ElementAt(1).Value;
                    List<Node> candidate_duplicate = new List<Node>();
                    candidate_duplicate.Add(state[this.candidate_set.ElementAt(0).Key]);
                    int i = 1;
                    while (edge_weight == edge_weight_compare) 
                    {
                        candidate_duplicate.Add(state[this.candidate_set.ElementAt(i).Key]);
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
                    candidate_duplicate.Remove(this.candidate_1(state));
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
                    fc = state[this.candidate_set.ElementAt(1).Key];


                }
                
                return fc;


            }
           
            /// <summary>
            /// return candidate at index  in futur implement a list for duplicate values;
            /// </summary>
            /// <returns></returns>
            public Node candidate_1(Node[] state) 
            {
                if (this.candidate_set.Count == 1) 
                {
                    this.c1 = state[this.candidate_set.ElementAt(0).Key];
                    this.has_duplicates = false;
                    return c1;
                }
                this.c1 = state[this.candidate_set.ElementAt(0).Key];
                //Console.WriteLine(" c1 is" + c1.id);
                var edge_weight = this.candidate_set.ElementAt(0).Value;
                this.has_duplicates = false;
                var edge_weight_compare = this.candidate_set.ElementAt(1).Value;
                int i = 1;
                if (this.candidate_set.Count == 2 && edge_weight == edge_weight_compare) 
                {
                    if (this.c1.priority < state[this.candidate_set.ElementAt(1).Key].priority)
                    {
                        this.c1 = state[this.candidate_set.ElementAt(1).Key];
                    }
                    else 
                    {
                        this.c1 = state[this.candidate_set.ElementAt(0).Key];
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
                    if (this.c1.priority < state[this.candidate_set.ElementAt(i).Key].priority) 
                    {
                        this.c1 = state[this.candidate_set.ElementAt(i).Key];
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

            
            // dictionary to represent distance to different locations

            //node constructor
            
            public Node(long local, Dictionary<long, long> dic)
            {
                this.id = local;
                this.distance = new Dictionary<long, long>(dic);
                this.edges_found = 0;
                this.sum_of_distances = distance.Skip(0).Sum(v => v.Value);
                this.priority = sum_of_distances;
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
                    long weight_to_candidate1 = n.distance[n.candidate_1(Nodes).id];
                    if(weight_to_candidate1 == smallestEdge_weight) 
                    {
                        this.genesisNode = Nodes[n.id];
                        break;                    
                    }                              
                }
                Undirected_Edge genesis_edge = new Undirected_Edge(genesisNode, genesisNode.candidate_1(Nodes),Nodes);
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
            public Hamilton_cycle( Hamilton_cycle h, Node[] imaginary)
            {
                edges = new Dictionary<string, Undirected_Edge>(h.edges);
                left_coner_node = imaginary[h.left_coner_node.id];
                right_coner_node = imaginary[h.right_coner_node.id];

            }


            /// <summary>
            /// ie which edge to pick is determined will switch once half thed edges are found
            /// </summary>
            public void calculate_lr_priorities()
            {     
               
                 this.left_arm_priority = this.left_coner_node.priority;
                 this.right_arm_priority = this.right_coner_node.priority;
                

            }
            /// <summary>
            /// find distance traveler travels traveled left and right respectively
            /// </summary>
            /// <param name="optional_left"></param>
            /// <param name="optional_right"></param>
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
                    //Console.WriteLine("left p is " + left_arm_priority);
                    //Console.WriteLine("right pis " + right_arm_priority);

                    Console.WriteLine("cutoff is " + cutoff);

                    for (int i = 0; i <cutoff; i++) 
                    {
                        //Console.WriteLine("current right hop is " + current_right_hop.id);
                        //Console.WriteLine("current left hop is " + current_left_hop.id);

                       // Console.WriteLine("left p is {0} + {1} " ,left_arm_priority ,current_left_hop.right_edge.weight);
                        //Console.WriteLine("right p is {0} + {1} " ,right_arm_priority, current_right_hop.left_edge.weight);
                        this.left_arm_priority += current_left_hop.right_edge.weight;
                        current_left_hop = current_left_hop.right_edge.right_Node;
                        this.right_arm_priority += current_right_hop.left_edge.weight;
                        current_right_hop = current_right_hop.left_edge.left_Node;
                   

                    }
                    // if edges found is greater than one
                }
                //if (this.left_coner_node.has_duplicates) 
                //{
                //    //Console.WriteLine("has dups pis" );
                //    var edge_weight = this.left_coner_node.candidate_set.ElementAt(0).Value;
                //    var comp_weight = this.left_coner_node.candidate_set.ElementAt(1).Value;
                //    int count = 1;
                //    this.left_arm_priority -= edge_weight;
                //    while (edge_weight == comp_weight) 
                //    {
                        
                //        try 
                //        {   
                            
                //            comp_weight = this.left_coner_node.candidate_set.ElementAt(count).Value;
                //        }
                //        catch (System.ArgumentOutOfRangeException) 
                //        {
                //            break;
                        
                //        }
                //        this.left_arm_priority += edge_weight;
                //        count++;
                //    }

                //}
                //if (this.right_coner_node.has_duplicates)
                //{
                //    //Console.WriteLine("has dups pis");
                //    var edge_weight = this.right_coner_node.candidate_set.ElementAt(0).Value;
                //    var comp_weight = this.right_coner_node.candidate_set.ElementAt(1).Value;
                //    int count = 1;
                //    this.right_arm_priority -= edge_weight;
                //    while (edge_weight == comp_weight)
                //    {
                        
                //        try
                //        {
                            
                //            comp_weight = this.right_coner_node.candidate_set.ElementAt(count).Value;
                //        }
                //        catch (System.ArgumentOutOfRangeException)
                //        {
                //            break;

                //        }
                //        this.right_arm_priority += edge_weight;
                //        count++;
                //    }

                //}

            }
            /// <summary>
            /// adds edge e to hamilton cycle h
            /// </summary>
            /// <param name="h"></param>
            /// <param name="edge"></param>
            public void remove_from_candidaateset(Undirected_Edge e,Node[] imaginary)
            {
                Undirected_Edge f;
                if (this.edges.TryGetValue(e.id, out f))
                {
                    Console.WriteLine(" already exist in cycle");
                }
                else
                {
                    if (e.left_Node.edges_found >= 1 || e.right_Node.edges_found >= 1)
                    {
                        if (e.left_Node.edges_found >= 1)
                        {
                            foreach (Node n in imaginary)
                            {
                                
                                n.candidate_set.Remove(e.left_Node.id);
                            }
                        }
                        else
                        {
                            foreach (Node n in imaginary)
                            {
                                n.candidate_set.Remove(e.right_Node.id);
                            }



                        }

                    }


                }
            }
            public void add_edge_to_cycle(Undirected_Edge e,Node[] state)
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
                    if(this.left_coner_node.id == e.right_Node.id) 
                    {
                        this.left_coner_node = state[e.left_Node.id];
                    
                    }
                    if(this.right_coner_node.id == e.left_Node.id)
                    {
                        this.right_coner_node = state[e.left_Node.id];
                    }
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
                    if (e.left_Node.edges_found >= 1 || e.right_Node.edges_found >= 1)
                    {
                        if (e.left_Node.edges_found >= 1)
                        {
                            foreach (Node n in state)
                            {
                                n.candidate_set.Remove(e.left_Node.id);
                            }
                        }
                        else
                        {
                            foreach (Node n in state)
                            {
                                n.candidate_set.Remove(e.right_Node.id);
                            }



                        }

                    }

                }
            }

        }

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


            public Undirected_Edge() { }

            public Undirected_Edge(Node ln, Node rn,Node[] state)
            {
                this.left_Node = state[ln.id];
                this.right_Node = state[rn.id];
                this.left_Node.right_edge = this;
                this.right_Node.left_edge = this;
                //Console.WriteLine("left node : " + left_Node.id);
                this.left_Node.candidate_set.Remove(right_Node.id);
                this.right_Node.candidate_set.Remove(left_Node.id);
                this.weight = state[this.left_Node.id].distance[this.right_Node.id];

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

        // methods
        // function to get the distance between two nodes 


        // function to initialise nodes and store in Nodes array 



        /// <summary>
        /// sort the edges in order for each node 
        /// </summary>
        /// <param name="nm"></param>
        public void Sort(Node nm)
        {
            nm.distance = nm.distance.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            nm.candidate_set = new Dictionary<long, long>(nm.distance);
            nm.candidate_set.Remove(nm.id);
        }
        // function to check if given data is valid graph


        //fu
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
            Hamilton_cycle pseudoCycle = new Hamilton_cycle();

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

            minimumHamiltonCycle.add_edge_to_cycle(g,Nodes);

            Node left_c = minimumHamiltonCycle.left_coner_node.candidate_1(Nodes);
            Node right_c = minimumHamiltonCycle.right_coner_node.candidate_1(Nodes);
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

            while (minimumHamiltonCycle.edges.Count < no_of_nodes - 2)
            {

                Console.WriteLine("left coner is " + minimumHamiltonCycle.left_coner_node.id);
                Console.WriteLine("right coner is " + minimumHamiltonCycle.right_coner_node.id);
                // first set up candidates for both coner nodes

                left_c = minimumHamiltonCycle.left_coner_node.candidate_1(Nodes);
                right_c = minimumHamiltonCycle.right_coner_node.candidate_1(Nodes);

                Console.WriteLine("left c1 is " + left_c.id);
                Console.WriteLine("right c1 is " + right_c.id);



                // now calculate weight sums
                minimumHamiltonCycle.calculate_lr_priorities_with_weights();

                
                if(left_c.id == right_c.id)
                {
                    Console.WriteLine("c is " + left_c.id);


                    // look to the future 
                    long option_left_value;
                    long option_right_value;
                    long c_id = left_c.id;
                    // if left edge is picked find futur of right
                    if (minimumHamiltonCycle.left_coner_node.candidate_set.Count == 1 || minimumHamiltonCycle.right_coner_node.candidate_set.Count == 1)
                    {
                        Console.WriteLine("cannot look in future nly one candidate left");
                    }
                    else
                    {
                        Node right_future = minimumHamiltonCycle.right_coner_node.future_c1(Nodes);
                        Node left_future = minimumHamiltonCycle.left_coner_node.future_c1(Nodes);

                        Console.WriteLine(" right current c is" + minimumHamiltonCycle.right_coner_node.candidate_1(Nodes).id);
                        Console.WriteLine(" right future c is" + right_future.id);
                        Console.WriteLine(" left current c is" + minimumHamiltonCycle.left_coner_node.candidate_1(Nodes).id);

                        
                        Console.WriteLine(" left future c is" + left_future.id);
                        if (right_future.id == left_future.id) 
                        {
                            option_left_value = Getdistance(minimumHamiltonCycle.left_coner_node.id, c_id) + Getdistance(minimumHamiltonCycle.right_coner_node.id, right_future.id);
                            option_right_value = Getdistance(minimumHamiltonCycle.right_coner_node.id, c_id) + Getdistance(minimumHamiltonCycle.left_coner_node.id, left_future.id);
                            Console.WriteLine("if left picked weight sum = {0} if right is picked weight sum ={1}", option_left_value, option_right_value);

                            if (option_left_value < option_right_value)
                            {
                                e = new Undirected_Edge(Nodes[left_c.id], minimumHamiltonCycle.left_coner_node,Nodes);
                                minimumHamiltonCycle.right_coner_node.candidate_set.Remove(left_c.id);
                                left_c.candidate_set.Remove(minimumHamiltonCycle.right_coner_node.id);

                            }
                            else if (option_left_value == option_right_value)
                            {
                                Console.WriteLine(" c matches and future weights is same find what to do");
                                return minimumHamiltonCycle;


                            }
                            else
                            {
                                e = new Undirected_Edge(minimumHamiltonCycle.right_coner_node, Nodes[right_c.id],Nodes);
                                minimumHamiltonCycle.left_coner_node.candidate_set.Remove(right_c.id);
                                right_c.candidate_set.Remove(minimumHamiltonCycle.left_coner_node.id);

                            }

                        }
                        else 
                        {
                            // future c dont match so compare priority instead
                            if(left_future.priority > right_future.priority) 
                            {
                                // right edge needs to be picked since future of left is more desirable
                                e = new Undirected_Edge(minimumHamiltonCycle.right_coner_node, Nodes[right_c.id],Nodes);
                                minimumHamiltonCycle.left_coner_node.candidate_set.Remove(right_c.id);
                                right_c.candidate_set.Remove(minimumHamiltonCycle.left_coner_node.id);
                                

                            }
                            else if(left_future.priority == right_future.priority) 
                            {
                                Console.WriteLine(" future priorities match find what to do");
                                return minimumHamiltonCycle;
                            
                            }
                            else
                            {
                                e = new Undirected_Edge(Nodes[left_c.id], minimumHamiltonCycle.left_coner_node,Nodes);
                                minimumHamiltonCycle.right_coner_node.candidate_set.Remove(left_c.id);
                                left_c.candidate_set.Remove(minimumHamiltonCycle.right_coner_node.id);



                            }


                        }
                            

                    }



                }
                else
                {
                    // create a copy of state of nodes
                    Node[] imaginary = new Node[no_of_nodes];
                    imaginary = Nodes.Select(a=>(Node)a.Clone()).ToArray();

                    
                    

                    // create a copy of minhamilton cycle
                    Hamilton_cycle imaginary_cycle = new Hamilton_cycle(minimumHamiltonCycle,imaginary);
                    
                    // if initil candidates are not equal create a new imaginary selection on arms till match
                    bool pick_imaginary_left_chain = false;
                    



                    Node imaginary_left_c = imaginary_cycle.left_coner_node.candidate_1(imaginary);

                    Node imaginary_right_c = imaginary_cycle.right_coner_node.candidate_1(imaginary);
    

                    List<Undirected_Edge> left_imaginary_edges = new List<Undirected_Edge>();
                    List<Undirected_Edge> right_imaginary_edges = new List<Undirected_Edge>();
                   
                    
                    Console.WriteLine("imaginary left c {0} and imaginary right c {1}", imaginary_cycle.left_coner_node.id, imaginary_cycle.right_coner_node.id);

                    // create first 2 imaginary edges 
                    
                    
                    while (imaginary_left_c.id != imaginary_right_c.id) 
                    {
                        Console.WriteLine("new left c {0} and new right c {1}", imaginary_cycle.left_coner_node.id, imaginary_cycle.right_coner_node.id);

                        

                        imaginary_cycle.left_arm_priority = imaginary_left_c.priority;
                        imaginary_cycle.right_arm_priority = imaginary_right_c.priority;

                        Console.WriteLine("no match so comparing priority left  p is {0} right p is {1} ",
                        imaginary_cycle.left_arm_priority,
                        imaginary_cycle.right_arm_priority);
                        Console.WriteLine("minimum cycle left candidate is " + minimumHamiltonCycle.left_coner_node.candidate_1(Nodes).id);

                        if (imaginary_cycle.left_arm_priority > imaginary_cycle.right_arm_priority)
                        {
                            // left has higher priority so move left forwadd
                            Console.WriteLine("minimum cycle left candidate is " + minimumHamiltonCycle.left_coner_node.candidate_1(Nodes).id);
                            Undirected_Edge le = new Undirected_Edge(imaginary_left_c, imaginary_cycle.left_coner_node, imaginary);
                            Console.WriteLine(" left prio greater imaginary edge is " +le.id);
                           
                            imaginary_cycle.right_coner_node.candidate_set.Remove(imaginary_left_c.id);
                            
                            imaginary_left_c.candidate_set.Remove(imaginary_cycle.right_coner_node.id);

                            Console.WriteLine(" new imiginary left coner is " + imaginary_cycle.left_coner_node.id);
                            left_imaginary_edges.Add(le);
                            
                            imaginary_cycle.add_edge_to_cycle(le,imaginary);
                            //
                            Console.WriteLine("minimum cycle left candidate is " + minimumHamiltonCycle.left_coner_node.candidate_1(Nodes).id); 
                            
                            

                        }
                        else if (minimumHamiltonCycle.left_arm_priority == minimumHamiltonCycle.right_arm_priority)
                        {
                            Console.WriteLine("both have same prio so add both or not fiuger it out");// for now adding both edges if not same
                            

                        }
                        else
                        {
                            // picking right if right has less weight unless halfpoint reached
                            Undirected_Edge re = new Undirected_Edge(imaginary_cycle.right_coner_node, imaginary_right_c, imaginary);
                            Console.WriteLine(" right prio greater imaginary edge is " + re.id);
                            imaginary_cycle.left_coner_node.candidate_set.Remove(imaginary_right_c.id);
                            imaginary_right_c.candidate_set.Remove(imaginary_cycle.left_coner_node.id);
                            Console.WriteLine("new imiginary right c is " + imaginary_right_c.id);
                            right_imaginary_edges.Add(re);
                            imaginary_cycle.add_edge_to_cycle(re,imaginary);



                        }
                        imaginary_left_c = imaginary_cycle.left_coner_node.candidate_1(imaginary);
                        imaginary_right_c = imaginary_cycle.right_coner_node.candidate_1(imaginary);


                    }

                    Console.WriteLine("printing imaginaary cycle ****");
                    imaginary_cycle.printHamiltonCycle();

                    Console.WriteLine("imaginary left coner is " + imaginary_cycle.left_coner_node.id);
                    Console.WriteLine("imaginary right coner is " + imaginary_cycle.right_coner_node.id);
                    if (imaginary_left_c.id == imaginary_right_c.id)
                    {

                        // look to the future 
                        long option_left_value;
                        long option_right_value;
                        long imaginary_c_id = imaginary_left_c.id;

                        if (imaginary_left_c.candidate_set.Count == 1 || imaginary_right_c.candidate_set.Count == 1)
                        {
                            Console.WriteLine("cannot look in future nly one candidate left");
                        }
                        else 
                        {
                            Node imaginary_right_future = imaginary_cycle.right_coner_node.future_c1(imaginary);
                            Node imaginary_left_future = imaginary_cycle.left_coner_node.future_c1(imaginary);

                  
                            Console.WriteLine(" right current c1  is" + imaginary_right_c.id);
                            Console.WriteLine(" right future c1 is" + imaginary_right_future.id);


                            Console.WriteLine(" left current c1  is" + imaginary_left_c.id);
                            Console.WriteLine(" left future c1 is" + imaginary_left_future.id);


                            if (imaginary_right_future.id == imaginary_left_future.id)
                            {
                                option_left_value = Getdistance(imaginary_cycle.left_coner_node.id, imaginary_c_id) + Getdistance(imaginary_cycle.right_coner_node.id, imaginary_right_future.id);
                                option_right_value = Getdistance(imaginary_cycle.right_coner_node.id, imaginary_c_id) + Getdistance(imaginary_cycle.left_coner_node.id, imaginary_left_future.id);
                                Console.WriteLine("if left picked weight sum = {0} if right is picked weight sum ={1}", option_left_value, option_right_value);

                                if (option_left_value < option_right_value)
                                {
                                    Undirected_Edge le = new Undirected_Edge(imaginary_left_c, imaginary_cycle.left_coner_node, imaginary);
                                    Console.WriteLine(" left prio greater imaginary edge is " + le.id);
                                    imaginary_cycle.right_coner_node.candidate_set.Remove(imaginary_left_c.id);
                                    Console.WriteLine(" new imiginary left coner is " + imaginary_cycle.left_coner_node.id);
                                    left_imaginary_edges.Add(le);
                                    imaginary_cycle.add_edge_to_cycle(le,imaginary);
                                    //pick_imaginary_left_chain = true;
                                    
                                }
                                else if (option_left_value == option_right_value)
                                {
                                    Console.WriteLine(" c matches and future weights is same find what to do");
                                    return minimumHamiltonCycle;


                                }
                                else
                                {
                                    Undirected_Edge re = new Undirected_Edge(imaginary_cycle.right_coner_node, imaginary_right_c, imaginary);
                                    Console.WriteLine(" right prio greater imaginary edge is " + re.id);
                                    imaginary_cycle.left_coner_node.candidate_set.Remove(imaginary_right_c.id);
                                    Console.WriteLine("new imiginary right c is " + imaginary_right_c.id);
                                    right_imaginary_edges.Add(re);
                                    imaginary_cycle.add_edge_to_cycle(re,imaginary);

                                    //pick_imaginary_left_chain = false;

                                }

                            }
                            else
                            {
                                // future c dont match so compare priority instead
                                if (imaginary_left_future.priority > imaginary_right_future.priority)
                                {
                                    Undirected_Edge re = new Undirected_Edge(imaginary_cycle.right_coner_node, imaginary_right_c, imaginary);
                                    Console.WriteLine(" right prio greater imaginary edge is " + re.id);
                                    imaginary_cycle.left_coner_node.candidate_set.Remove(imaginary_right_c.id);
                                    Console.WriteLine("new imiginary right c is " + imaginary_right_c.id);
                                    right_imaginary_edges.Add(re);
                                    imaginary_cycle.add_edge_to_cycle(re,imaginary);

                                    pick_imaginary_left_chain = false;

                                }
                                else if (imaginary_left_future.priority == imaginary_right_future.priority)
                                {
                                    Console.WriteLine(" future priorities match find what to do");
                                    return minimumHamiltonCycle;

                                }
                                else
                                {
                                    Undirected_Edge le = new Undirected_Edge(imaginary_left_c, imaginary_cycle.left_coner_node, imaginary);
                                    Console.WriteLine(" left prio greater imaginary edge is " + le.id);
                                    imaginary_cycle.right_coner_node.candidate_set.Remove(imaginary_left_c.id);
                                    Console.WriteLine(" new imiginary left coner is " + imaginary_cycle.left_coner_node.id);
                                    left_imaginary_edges.Add(le);
                                    imaginary_cycle.add_edge_to_cycle(le,imaginary);
                                    pick_imaginary_left_chain = true;


                                }


                            }
                            
                           


                        }

                    }

                    // add a chain to minimum hamilton cycle
                    if (pick_imaginary_left_chain) 
                    {
                        // pick left chain
                        foreach(var edge in left_imaginary_edges) 
                        {
                            Undirected_Edge ed = new Undirected_Edge(Nodes[edge.left_Node.id], Nodes[edge.right_Node.id], Nodes);
                            minimumHamiltonCycle.right_coner_node.candidate_set.Remove(edge.left_Node.id);
                            Nodes[edge.left_Node.id].candidate_set.Remove(minimumHamiltonCycle.right_coner_node.id);
                            minimumHamiltonCycle.add_edge_to_cycle(ed,Nodes);
                        }

                    
                    
                    }
                    else if(pick_imaginary_left_chain == false)
                    {
                        // pick right chian
                        foreach(var edge in right_imaginary_edges) 
                        {
                            Undirected_Edge ed = new Undirected_Edge(Nodes[edge.left_Node.id], Nodes[edge.right_Node.id], Nodes);
                            minimumHamiltonCycle.left_coner_node.candidate_set.Remove(edge.right_Node.id);
                            Nodes[edge.right_Node.id].candidate_set.Remove(minimumHamiltonCycle.left_coner_node.id);
                            minimumHamiltonCycle.add_edge_to_cycle(ed,Nodes);


                        }

                    
                    }
                    Console.WriteLine(" printinf official cycle");
                    minimumHamiltonCycle.printHamiltonCycle();

                    
                    
                    

                }

                // when half point is reached

                Console.WriteLine("edge created is " + e.id);
                minimumHamiltonCycle.add_edge_to_cycle(e,Nodes);
                if (d != null)
                {
                    //Console.WriteLine("edge created is " + d.id);
                    minimumHamiltonCycle.add_edge_to_cycle(d,Nodes);
                    d = null;
                }

            }
            // last 2 edge 
            e = e = new Undirected_Edge(minimumHamiltonCycle.left_coner_node.candidate_1(Nodes), minimumHamiltonCycle.left_coner_node,Nodes);
            minimumHamiltonCycle.add_edge_to_cycle(e,Nodes);
            e = e = new Undirected_Edge(minimumHamiltonCycle.left_coner_node, minimumHamiltonCycle.right_coner_node,Nodes);
            minimumHamiltonCycle.add_edge_to_cycle(e,Nodes);
            // set up candidates for both and compare weights;

            return minimumHamiltonCycle;

        }





        /// <summary>
        /// returns node connected to Node n at int index of its sorted distances dictionry
        /// </summary>
        /// <param name="index">position in sorted distances  </param>
        /// <param name="n"></param>
        /// <returns></returns>
        public Node getNode(Node n, int index)
            {
                Node n2 = Nodes[n.distance.ElementAt(index).Key];
                return n2;

            }


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


            // function to find the lastnodes without edges


            //public void store_cycle()
            //{
            //    string path = "D:/Work/Github/Repo/cycle.txt";
            //    // Create a file to write to.
            //    using (StreamWriter sw = File.CreateText(path))
            //    {
            //        foreach (KeyValuePair<string, Undirected_Edge> item in minimumHamiltonCycle.edges)
            //        {
            //            sw.WriteLine("node: " + item.Key);
            //            sw.WriteLine(" weight:" + item.Value.weight);
            //        }



            //    }
            //}
        

    }
}
