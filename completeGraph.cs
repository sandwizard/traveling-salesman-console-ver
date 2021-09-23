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
                //foreach(Node n in sorted_nodes) 
                //{
                //    long weight_to_candidate1 = n.distance[n.candidate_1(Nodes).id];
                //    if(weight_to_candidate1 == smallestEdge_weight) 
                //    {
                //        this.genesisNode = Nodes[n.id];
                //        break;                    
                //    }                              
                //}
                Node node = sorted_nodes[0];
                Node n2 = node.candidate_1(Nodes);

                
                Undirected_Edge genesis_edge = new Undirected_Edge(node, n2,Nodes);
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
            foreach (var node in Nodes)
            {
                Console.WriteLine("-----------------Candidate set of " + node.id + "------------------------------");
                foreach (var c in node.candidate_set)
                {
                    Console.WriteLine("node {0} -to -{1} weight = {2} ", node.id, c.Key, c.Value);
                }
            }
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
            /// implement new iteraation from here
            /// 
            Console.WriteLine("edges found" + minimumHamiltonCycle.right_coner_node.edges_found);
            Node[] imaginary = new Node[no_of_nodes];
            imaginary = Nodes.Select(a => (Node)a.Clone()).ToArray();
            Hamilton_cycle imaginary_cycle = new Hamilton_cycle(minimumHamiltonCycle, imaginary);
            while (imaginary_cycle.edges.Count < no_of_nodes - 2)
            {
                // setting up imaginary cycle
                
                
                Node imaginary_left_c = imaginary_cycle.left_coner_node.candidate_1(imaginary);
                Node imaginary_right_c = imaginary_cycle.right_coner_node.candidate_1(imaginary);
                Console.WriteLine("imaginary left c {0} and imaginary right c {1}", imaginary_left_c.id, imaginary_right_c.id);
                while(imaginary_left_c!= imaginary_right_c) 
                {
                    //compare priority of edges
                    if(imaginary_cycle.left_coner_node.priority > imaginary_cycle.right_coner_node.priority) 
                    {
                        //extend left
                        Undirected_Edge l_edge = new Undirected_Edge(
                                imaginary_left_c, imaginary_cycle.left_coner_node, imaginary);
                        imaginary_cycle.add_edge_to_cycle(l_edge, imaginary);

                        // updating left candidate
                        imaginary_left_c = imaginary_cycle.left_coner_node.candidate_1(imaginary);
                        Console.WriteLine(" imaginary chain edge is " + l_edge.id);
                        Console.WriteLine(" new imaginary left candidate is " + imaginary_left_c.id);

                    }
                    else if(imaginary_cycle.left_coner_node.priority == imaginary_cycle.right_coner_node.priority) 
                    {
                        // not sure what to do yet
                        Console.WriteLine("pioritiry values are same");
                        return minimumHamiltonCycle;
                    }
                    else 
                    {
                        //extending right
                        Undirected_Edge r_edge = new Undirected_Edge(
                              imaginary_cycle.right_coner_node, imaginary_right_c, imaginary);
                        imaginary_cycle.add_edge_to_cycle(r_edge, imaginary);
                        Console.WriteLine(" imaginary chain edge is " + r_edge.id);
                        imaginary_right_c = imaginary_cycle.right_coner_node.candidate_1(imaginary);

                    }                                                                            
                   
                }

                if (imaginary_left_c.id == imaginary_right_c.id)
                {
                    Console.WriteLine(" same candidate **** " + imaginary_left_c.id);
                    //compare future edge weights
                    // left future  
                    Node right_future_candidate = imaginary_cycle.right_coner_node.future_c1(imaginary);
                   
                    // right future 
                    Node left_future_candidate = imaginary_cycle.left_coner_node.future_c1(imaginary);
                    


                    
    

                    // find 4 edges  goodness values and compare
                    // if left goes to candidate(case1)
                    long left_to_candidate_goodness = imaginary_left_c.priority / Getdistance(imaginary_cycle.left_coner_node.id, imaginary_left_c.id);
                    long right_to_future_goodness = right_future_candidate.priority / Getdistance(imaginary_cycle.right_coner_node.id, right_future_candidate.id);
                    Console.WriteLine("left future: right goes to {0} and left edge goodness {1} and right edge goodness {2}", right_future_candidate.id, left_to_candidate_goodness,right_to_future_goodness);

                    long left_goodness = left_to_candidate_goodness + right_to_future_goodness;

                    // if right goes to candidate(case2)
                    long right_to_candidate_goodness = imaginary_right_c.priority / Getdistance(imaginary_cycle.right_coner_node.id, imaginary_right_c.id);
                    long left_to_future_goodness = left_future_candidate.priority / Getdistance(imaginary_cycle.left_coner_node.id, left_future_candidate.id);
                    Console.WriteLine("right future: left goes to {0} and left edge goodness {1} and right edge goodness {2}", right_future_candidate.id, left_to_future_goodness, right_to_candidate_goodness);
                    
                    // find smallest goodness value



                    if ((left_to_candidate_goodness < right_to_candidate_goodness && left_to_candidate_goodness < left_to_future_goodness)
                        ||
                        (right_to_future_goodness < right_to_candidate_goodness && right_to_future_goodness < left_to_future_goodness))

                    {
                        //left to cantidate case has lowest goodness edge 
                        Console.WriteLine("left discarded");
                        imaginary[imaginary_cycle.left_coner_node.id].candidate_set.Remove(imaginary_left_c.id);
                    
                    }
                    else 
                    {
                        //right to cantidate case has lowest goodness edge 
                        Console.WriteLine("right discarded");
                        imaginary[imaginary_cycle.right_coner_node.id].candidate_set.Remove(imaginary_left_c.id);
                    }
                    


                }




            }


            //######################################


            return imaginary_cycle;

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
