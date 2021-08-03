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
        public long[,] distanceMatrix { get; set; }
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
            sorted_nodes = new Node[no_of_nodes];
            Initialise_nodes();
            //FindApproxHamiltoncycle();
            //lastedge();
          
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

        /// <summary>
        ///  no
        /// </summary>
        public class Node
        {
            public Node c1;
            public Node c2;
            public Node centerNode;
            public Undirected_Edge centerEdge;
            public Undirected_Edge left_edge;
            public Undirected_Edge right_edge;
            public long id;
            public int edges_found;
            public int possible_edges;
            public int index = 1;
            public Node adj_1;
            public Node adj_2;
            public int current_min_probable_edge_index = 1;
            public int current_min_probable_edge_index2 = 2;
            public long sum_of_distances;
            /// <summary>
            /// return candidate at index 1
            /// </summary>
            /// <returns></returns>
            public Node candidate_1() 
            {
                this.c1 = Nodes[this.candidate_set.ElementAt(0).Key];
                return c1;
                       
            }

            /// <summary>
            /// return candidate at indes 2
            /// </summary>
            /// <returns></returns>
            public Node candidate_2() 
            {
                this.c2 = Nodes[this.candidate_set.ElementAt(1).Key];
                return c2;
            
            }
            public long priority;


            // dictionary to represent distance to different locations
            public Dictionary<long, long> distance;
            public Dictionary<long, long> candidate_set;
            //node constructor
            public Node()
            { }
            public Node(long local, Dictionary<long, long> dic)
            {
                this.id = local;
                this.distance = new Dictionary<long, long>(dic);
                
                this.edges_found = 0;
                this.possible_edges = 2;
                this.sum_of_distances = distance.Skip(0).Sum(v => v.Value); 
                //Console.WriteLine("sum for node :" + this.location + " =  " + sum_of_distances);
            }
            /// <summary>
            /// set candidate 1 of node to candidate
            /// </summary>
            /// <param name="node"></param>
            /// <param name="candidate"></param>
            /// <returns></returns>
            public void setCandidate_1(Node candidate)
            {
                candidate = Nodes[candidate.id];
                this.c1 = candidate;
                //Console.WriteLine("node {0} c1 = {1} ", this.id, this, candidate_1);
            }
            /// <summary>
            /// set candidate 2 of node to candidate
            /// </summary>
            /// <param name="node"></param>
            /// <param name="candidate"></param>
            /// <returns></returns>
            public void setCandidate_2( Node candidate)
            {
                candidate = Nodes[candidate.id];
                this.c2 = candidate;
                //Console.WriteLine("node {0} c2 = {1} ", this.id, this, candidate_2);

            }
        }
        public Undirected_Edge lastEdge { get; set; }
        public class Hamilton_cycle
        {
            public Node genesisNode() 
            {
                Node n = sorted_nodes[0];
                n = Nodes[n.id];
                return n;

            }
            public bool stuck_left;
            public bool stuck_right;
            public bool searchleft;
            public bool cycle_incomplete;
            public Node left_coner_node;
          
            public Node right_coner_node;
            public long lr_chains_divider_index;
            public long left_arm_priority;
            public long right_arm_priority;
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
            /// <summary>
            /// search right arm
            /// </summary>
            /// <param name="minimumHamiltonCycle"></param>
            public void search_right_arm()
            {
                Node node_searching_edge_for;
                /////// search right conner
                if (this.searchleft == false)
                {
                    node_searching_edge_for = this.right_coner_node;
                    //Console.WriteLine(" ######## SEARCHING FOR NODE {0} ########", node_searching_edge_for.id);
                    // will always have only one candidate cause one edge is found
                    // set candidate of node being checked
                    SetCandidates(node_searching_edge_for);
                    //Console.WriteLine("seting candidates for candidate and checking loops");
                    SetCandidates(node_searching_edge_for.candidate_1());
                    
                    //Console.WriteLine("phase over");
                    // set candidates for c1 of node being checked
                    // perform check      
                    if (node_searching_edge_for.id == node_searching_edge_for.candidate_1().candidate_1().id
                        ||
                        node_searching_edge_for.id == node_searching_edge_for.candidate_1().candidate_2().id)
                    {
                        //Console.WriteLine("implement here");
                        //Console.WriteLine("picking right");
                        Node node_to_add_edge_to = this.right_coner_node;
                        Undirected_Edge right_edge_to_add = new Undirected_Edge(node_to_add_edge_to, node_to_add_edge_to.candidate_1());
                        //Console.WriteLine("right edge is " + right_edge_to_add.id + "  weight : " + right_edge_to_add.weight);
                        this.add_edge_to_cycle(right_edge_to_add);
                        // update right coners after edge
                        this.right_coner_node = this.right_coner_node.right_edge.right_Node;
                        this.right_coner_node = Nodes[this.right_coner_node.id];
                        
                        this.calculate_lr_priorities();
                         this.stuck_left = false;
                        this.stuck_right = false;
                    }
                    else
                    {
                        Console.WriteLine(" stuck right");
                        this.stuck_right = true;
                        this.searchleft = false;
                        // switch to the other end
                    }

                }
            }
            /// <summary>
            /// searh the left arm for 
            /// </summary>
            /// <param name="minimumHamiltonCycle"></param>
            public void search_left_arm()
            {
                /////// search left conner
                Node node_searching_edge_for;

                if (this.searchleft)
                {

                    node_searching_edge_for = this.left_coner_node;
                    //Console.WriteLine(" ######## SEARCHING FOR NODE {0} ########", node_searching_edge_for.id);
                    // will always have only one candidate cause one edge is found

                    // set candidate of node being checked
                    SetCandidates(node_searching_edge_for); // sets candidate and performs loop test 
                    // set candidtes for candidate of node searching edge for
                    //Console.WriteLine("seting candidates for candidate and checking loops");
                    SetCandidates(node_searching_edge_for.candidate_1()); // set candidate for candidates and doloop test 2


                    // perform check 


                    if (node_searching_edge_for.id == node_searching_edge_for.candidate_1().candidate_1().id ||
                        node_searching_edge_for.id == node_searching_edge_for.candidate_1().candidate_2().id)
                    {
                        // edge is picked
                        //Console.WriteLine("match so picking left");
                        Node node_to_add_edge_to = this.left_coner_node;
                        Undirected_Edge left_edge_to_add = new Undirected_Edge(node_to_add_edge_to.candidate_1(), node_to_add_edge_to);
                        //Console.WriteLine("left edge is " + left_edge_to_add.id + "  weight : " + left_edge_to_add.weight);
                        this.add_edge_to_cycle(left_edge_to_add);


                        // update left coners after edge
                        this.left_coner_node = this.left_coner_node.left_edge.left_Node;
                        this.left_coner_node = Nodes[this.left_coner_node.id];

                        this.stuck_left = false;
                        this.stuck_right = false;

                        this.calculate_lr_priorities();


                    }
                    else
                    {
                        Console.WriteLine(" stuck left");
                        this.stuck_left = true;
                        this.searchleft = false;
                        // switch to the other end


                    }
                }


            }

            /// <summary>
            /// test if candidate of coner node creates a loop with only one candidate edge
            /// </summary>
            /// <param name="h"></param>
            /// <param name="n"></param>

            public void loop_test_2( Node nc1, Node nc2)
            {

                Node n;
                nc1= Nodes[nc1.id];
                nc2 = Nodes[nc2.id];
                //Console.WriteLine("   loop test 2");
                Node node_in_chain;  // will be updated
                if (this.searchleft)
                {
                    n = this.left_coner_node.candidate_1();
                    n = Nodes[n.id];
                    //Console.WriteLine(" node n is" + n.id);
                    //Console.WriteLine("searching left" + h.searchleft);
                    node_in_chain = this.left_coner_node;
                    node_in_chain = node_in_chain.right_edge.right_Node;
                    //Console.WriteLine("left coner node " + node_in_chain.id);                    
                    // start to scroll from here
                    while (node_in_chain != null)
                    {
                        // scroll right                        
                        //Console.WriteLine(" startes checkin from node {0} with {1} adn {2}", node_in_chain.id, nc1.id,nc2.id);
                        if (nc1.id == node_in_chain.id )
                        {
                            n.candidate_set.Remove(node_in_chain.id);
                            Console.WriteLine(" {0} node lops so removed from candidate set ", node_in_chain.id);
                            this.SetCandidates(n);
                        }
                        if (nc2.id == node_in_chain.id )
                        {
                            n.candidate_set.Remove(node_in_chain.id);
                           // Console.WriteLine(" {0} node lops so removed from candidate set ", node_in_chain.id);
                            this.SetCandidates(n);
                        }
                        //if(nc1.id == node_in_chain.candidate_1().id) 
                        //{
                        //    n.candidate_set.Remove(node_in_chain.candidate_1().id);
                        //    this.SetCandidates(n);
                        //}
                        //if (nc2.id == node_in_chain.id )
                        //{
                        //    n.candidate_set.Remove(node_in_chain.id);
                        //    // Console.WriteLine(" {0} node lops so removed from candidate set ", node_in_chain.id);
                        //    this.SetCandidates(n);
                        //}
                        try
                        {
                            node_in_chain = node_in_chain.right_edge.right_Node;

                        }
                        catch (NullReferenceException e)
                        {

                            node_in_chain = null;

                        }                      
                    }
                   // Console.WriteLine("loop_test_2 test over");
                }
                if (this.searchleft == false)
                {
                    n = this.right_coner_node.candidate_1();
                    n = Nodes[n.id];
                    //Console.WriteLine(" node n is" + n.id);
                    node_in_chain = this.right_coner_node;
                    // start to scroll from here
                    node_in_chain = node_in_chain.left_edge.left_Node;
                    while (node_in_chain != null)
                    {
                        // scrool left

                        //Console.WriteLine(" startes checkin from node {0} with {1}", node_in_chain.id, nc1.id);
                        if (nc1.id == node_in_chain.id )
                        {
                            n.candidate_set.Remove(node_in_chain.id);
                            //Console.WriteLine(" {0} node lops so removed from candidate set ", node_in_chain.id);
                            this.SetCandidates(n);
                        }
                        if (nc2.id == node_in_chain.id )
                        {
                            n.candidate_set.Remove(node_in_chain.id);
                            //Console.WriteLine(" {0} node lops so removed from candidate set ", node_in_chain.id);
                            this.SetCandidates(n);
                        }
                        try
                        {
                            node_in_chain = node_in_chain.left_edge.left_Node;

                        }
                        catch (NullReferenceException e)
                        {
                            node_in_chain = null;
                        }
                        

                    }


                }

            }

            /// <summary>
            /// sets candidate 1 and 2 for node n
            /// </summary>
            /// <param name="n"></param>
            /// <returns></returns>
            public void SetCandidates(Node n)
            {
                n = Nodes[n.id];
                // implement test for loops 
                if (n.edges_found == 2)
                {

                    // if 2 edges are found candidates will not be searched for any more
                    
                    

                }
                else if (n.edges_found == 1)
                {
                    // only one candidate always
                    
                    n.c1 = n.candidate_1();
                    n.c2 = null;
                    //Console.WriteLine("node {0} c1 = {1} ", n.id, n.c1.id);
                    //Console.WriteLine("loop test 1");
                    loop_test_1(n.c1);

                }
                else if (n.edges_found == 0)
                {
                    
                    n.c1 = n.candidate_1();
                    n.c2 = n.candidate_2();

                    //Console.WriteLine("node {0} c1 = {1} c2 = {2}", n.id, n.c1.id, n.c2.id);
                    //Console.WriteLine("loop test 2");
                    loop_test_2(n.c1, n.c2);
                    
                    

                }




            }
            /// <summary>
            /// returns a bool for if 
            /// </summary>
            /// <param name="n"></param>
            /// <returns></returns>
            public void loop_test_1( Node n)
            {
                //Console.WriteLine("   loop test 1 ");
                //Console.WriteLine(" node n is" + n.id);
                //make sure working on static 
                n = Nodes[n.id];
                Node node_in_chain;  // will be updated
                if (this.searchleft)
                {
                    //Console.WriteLine("searching left" + this.searchleft);
                    node_in_chain = this.left_coner_node;

                    //Console.WriteLine("left coner node " + node_in_chain.id);
                    node_in_chain = node_in_chain.right_edge.right_Node;
                    //Console.WriteLine("right of coner node " + node_in_chain.id);
                    // start to scroll from here
                    while (node_in_chain != null )
                    {
                        // scroll right
                        //Console.WriteLine(" startes checkin node {0} from node {1}", node_in_chain.id, n.id);
                        if (n.id == node_in_chain.id)
                        {
                            
                            this.left_coner_node.candidate_set.Remove(n.id);
                            //Console.WriteLine(" {0} node lops so removed from candidate set ", node_in_chain.id);
                            SetCandidates(this.left_coner_node);
                        }
                        try
                        {
                            node_in_chain = node_in_chain.right_edge.right_Node;
                            
                        }
                        catch (NullReferenceException e)
                        {
                            
                            node_in_chain = null;

                        }


                    }
                    //Console.WriteLine("notValidCompleteGraph looping here");
                    // first scroll test passed now find sets fr the candidate again
                   
                    

                }
                //Console.WriteLine("searching left" + this.searchleft);
                if (this.searchleft == false)
                {
                    //Console.WriteLine("searching left" + this.searchleft);
                    node_in_chain = this.right_coner_node;
                    node_in_chain = node_in_chain.left_edge.left_Node;
                    //Console.WriteLine("righ coner node " + node_in_chain.id);

                    // start to scroll from here
                    while (node_in_chain != null)
                    {
                        // scrool left


                        //Console.WriteLine("startes checkin from node {0} with {1}", node_in_chain.id, n.id);
                        if (n.id == node_in_chain.id)
                        {
                            this.right_coner_node.candidate_set.Remove(n.id);
                            //Console.WriteLine(" {0} node lops so removed from candidate set ", node_in_chain.id);
                            SetCandidates(this.right_coner_node);
                        }
                        try
                        {
                            node_in_chain = node_in_chain.left_edge.left_Node;
                            
                        }
                        catch (NullReferenceException e)
                        {
                            node_in_chain = null;
                        }

                    }
                    // first scroll test passed now find sets fr the candidate again
                  


                }
            }
            
            /// <summary>
            /// compute the priorities of left arm and right arm
            /// </summary>
            /// <param name="h"></param>
            public void calculate_lr_priorities()
            {
                // cut chain in 2
                float no_of_edges_found = this.edges.Count;
                float no_of_nodes_in_chain = no_of_edges_found + 1;
                // if not genesis
                if(no_of_edges_found == 2 ) 
                {
                    this.left_arm_priority = this.left_coner_node.priority;
                    this.right_arm_priority = this.right_coner_node.priority;
                } 
                else if(no_of_edges_found > 2) 
                {
                    Node left_hop_point = Nodes[this.left_coner_node.id];
                    Node right_hop_point = Nodes[this.right_coner_node.id];
                    if (no_of_edges_found % 2 ==0) // even no of edges
                    {

                        //Console.WriteLine("even");
                        this.right_arm_priority = this.right_coner_node.priority;
                        //Console.WriteLine(" lc node is " + this.left_coner_node.id + "rc node is" + this.right_coner_node.id);
                        float center_node_indes = no_of_edges_found / 2;
                        left_hop_point = Nodes[left_hop_point.right_edge.right_Node.id];
                        right_hop_point = Nodes[right_hop_point.left_edge.left_Node.id];
                        this.left_arm_priority = this.left_coner_node.priority;
                        for(int hop = 0; hop < center_node_indes-1; hop++) 
                        {
                            //Console.WriteLine("current l  prio is {0} and rl is {1}", this.left_arm_priority, this.right_arm_priority);
                            this.left_arm_priority += left_hop_point.priority;
                            //Console.WriteLine(" node {0} prio {1} added to lr of {1} ", left_hop_point.id ,left_hop_point.priority,this.left_arm_priority);
                            this.right_arm_priority += right_hop_point.priority;
                            //Console.WriteLine(" node {0} prio {1} added to rp of {1} ", right_hop_point.id,right_hop_point.priority,this.right_arm_priority);
                            left_hop_point = Nodes[left_hop_point.right_edge.right_Node.id];
                            right_hop_point = Nodes[right_hop_point.left_edge.left_Node.id];
                        
                        }


                    }
                    else 
                    {
                        this.left_arm_priority = this.left_coner_node.priority;
                        this.right_arm_priority = this.right_coner_node.priority;
                        //Console.WriteLine("odd no_of Nodes is " + no_of_nodes_in_chain);
                        //Console.WriteLine(" lc node is " + this.left_coner_node.id + "rc node is" + this.right_coner_node.id);
                        
                        float center_node_indes = no_of_nodes_in_chain/ 2;
                        center_node_indes -= 1;
                        left_hop_point = Nodes[left_hop_point.right_edge.right_Node.id];
                        right_hop_point = Nodes[right_hop_point.left_edge.left_Node.id];
                        //Console.WriteLine("center node indice is" + center_node_indes);
                        for (int hop = 1; hop < center_node_indes; hop++)
                        {
                            this.left_arm_priority += left_hop_point.priority;
                            //Console.WriteLine(" node {0} prio {1} added to lr ", left_hop_point.id, left_hop_point.priority);
                            this.right_arm_priority += right_hop_point.priority;
                            //Console.WriteLine(" node {0} prio {1} added to rp ", right_hop_point.id ,right_hop_point.priority);
                            left_hop_point = Nodes[left_hop_point.right_edge.right_Node.id];
                            right_hop_point = Nodes[right_hop_point.left_edge.left_Node.id];
                            
                            
                        }


                    }
                
                
                }
                else 
                {
                    //Console.WriteLine("currently {0} edges found and {1} nodes in chain ", no_of_edges_found, no_of_nodes_in_chain);
                    // initial edges is genesis edge
                    Undirected_Edge left_edge = this.edges.ElementAt(0).Value;
                    Undirected_Edge right_edge = this.edges.ElementAt(0).Value;

                    while (left_edge != null || right_edge != null && this.cycle_incomplete) // till conner nodes in chain
                    {
                        Node left_node = left_edge.left_Node;
                        Node right_node = right_edge.right_Node;
                        this.left_coner_node = left_node;
                        this.right_coner_node = right_node;
                        // initial lp and rp                
                        //Console.WriteLine(" left node is " + left_node.id);
                        //Console.WriteLine(" right node is " + right_node.id);
                        this.left_arm_priority = left_node.priority;
                        this.right_arm_priority = right_node.priority;

                        //Console.WriteLine(" left priority is " + this.left_arm_priority);
                        //Console.WriteLine(" right priority is " + this.right_arm_priority);
                        left_edge = left_node.left_edge;
                        right_edge = right_node.right_edge;

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
                    e.left_Node.candidate_set.Remove(e.right_node_id);
                    e.right_Node.candidate_set.Remove(e.left_node_id);
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
        //public Hamilton_cycle minimumHamiltonCycle = new Hamilton_cycle();
        /// <summary>
        /// returns the weight of the edge of two nodes n1 and n2
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>

        public class Undirected_Edge
        {
            public Node ln;
            public Node rn;
            public Node left_Node;
            public Node right_Node;
            public long left_node_id;
            public long right_node_id;
            public long weight;
            public string id;
            public bool mark;
            long temp;
            public shortestpath arbitarypath;
         
            public Undirected_Edge(Node ln, Node rn)
            {
                this.left_Node = Nodes[ln.id];
                this.right_Node = Nodes[rn.id];
                this.left_Node.right_edge = this;
                this.right_Node.left_edge = this;
                this.left_node_id = left_Node.id;
                //Console.WriteLine("left node : " + left_Node.id);
                this.right_node_id = right_Node.id;

                this.weight = Nodes[left_node_id].distance[right_node_id];
               
                // gives a uniquie id concatenation of smallest value node and highest value node
                this.id = string.Format("{0} :{1}", left_node_id, right_node_id);
            }
            public Undirected_Edge(long destination, long source)
            {
                this.left_node_id = destination;
                this.right_node_id = source;
                
                this.weight = Nodes[left_node_id].distance[right_node_id];
                
                this.left_Node = Nodes[left_node_id];
                this.right_Node = Nodes[right_node_id];
                if (left_node_id > right_node_id)
                {
                    temp = this.left_node_id;
                    this.left_node_id = right_node_id;
                    this.right_node_id = temp;
                }
                // gives a uniquie id concatenation of smallest value node and highest value node
                this.id = string.Format("{0}:{1}", left_node_id, right_node_id);
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
        /// <summary>
        /// returns edge weight of nodes n1 and n2
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public long get_weight(Node n1, Node n2)
        {
            long weight = Nodes[n1.id].distance[n2.id];
            return weight;

        }
        /// <summary>
        /// sorts the edges of each node in ascending and sets priority
        /// </summary>
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
            Dictionary<long, long> prioritised_nodes = new Dictionary<long, long>();
            foreach (Node n in Nodes)
            {
                prioritised_nodes.Add(n.id, n.sum_of_distances);
                n.priority = n.sum_of_distances;


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
        // function to sort node distance data
        public void Sort(Node nm)
        {
            nm.distance = nm.distance.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            nm.candidate_set = new Dictionary<long, long>(nm.distance);
            nm.candidate_set.Remove(nm.id);
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

        // after dealing with chain restart ftom fisrt priority to finish dealing with it
        // thing to implement in new ver

        // imaginary loop 

        // candidate variables



        // relation bettwwn candidates also need to be considered no imaginary loop in between the candidates 

        // update candidates aftter finding loop and edges
        // if loop on length of one c_edge node  eg 12 -C_edge-8 -edge-1-edge-11-c_edge then c_eges for both are not considered and new c edge is updated

        // and cedge of c node is also considered
        // while blacklisting make sure to update c_edges of blacklisted c_node and make sure to remove it from all 3 participating nodes indefinitly

        // if loop iof edge node to oficaial node only c_edge of edge node is updated 




        // for now update insted of blacklist c_node if both c_1 and c_2 of c_node are the nodes in chain
        // or 

        //  if only c_1 is in chain and c1 od c_node is equal to the c1 of node being checked then update               #do this more probable            



        // if no check confirm found cut in half sum priorities and pik edge of higher priority

        //public void printMinHamiltonPath()
        //{
        //    foreach (KeyValuePair<string, Undirected_Edge> item in minimumHamiltonCycle.edges)
        //    {
        //        Console.WriteLine("node: " + item.Key);
        //        Console.WriteLine(" weight:" + item.Value.weight);
        //    }
        //}

        public Hamilton_cycle MinimumHamiltonCycle = new Hamilton_cycle();// aprox
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
            Node genesis_node = sorted_nodes[0];
            Console.WriteLine("genesis node is : " + genesis_node.id);

            genesis_node = minimumHamiltonCycle.genesisNode();
            // set genesis candidate 1

           

            Console.WriteLine("genesis node c1 is = "+ genesis_node.candidate_1().id);

            minimumHamiltonCycle.stuck_left = false;
            minimumHamiltonCycle.stuck_right = false;
            // get genesis edge 
            Node node_conected_to_genesis = Nodes[genesis_node.candidate_1().id];  // 1 sice 0 is self
            Console.WriteLine("node connected to genesiss node : " + node_conected_to_genesis.id);
            Undirected_Edge genesis_edge = new Undirected_Edge(genesis_node , node_conected_to_genesis);
            Console.WriteLine("genesis edge is " + genesis_edge.id + "  weight : "+ genesis_edge.weight);

            
            
            
           
            //adding edge to cycle and  calculate initial priorities
            // wil also remove the nodes from their candidatee sets
            minimumHamiltonCycle.add_edge_to_cycle(genesis_edge);

            Console.WriteLine("node c1 is = "+ genesis_node.candidate_1().id);
            // set initial left coner and right coner nodes
            minimumHamiltonCycle.left_coner_node = minimumHamiltonCycle.genesisNode();
            minimumHamiltonCycle.right_coner_node = node_conected_to_genesis;

            Console.WriteLine("left coner Node : " + minimumHamiltonCycle.left_coner_node.id);
            Console.WriteLine("right coner Node : " + minimumHamiltonCycle.right_coner_node.id);
            //printHamiltonCycle(minimumHamiltonCycle);

            // remove candidate since edge is added 
            
                                  
            // initially will always search right
            minimumHamiltonCycle.searchleft = false;
            minimumHamiltonCycle.calculate_lr_priorities();

            Console.WriteLine("l priority is " + minimumHamiltonCycle.left_arm_priority);
            Console.WriteLine("r priority is " + minimumHamiltonCycle.right_arm_priority);
            // following need to be in loop

           
            
            
            while (minimumHamiltonCycle.edges.Count < no_of_nodes - 2) 
            {
                //Console.WriteLine("l priority is " + minimumHamiltonCycle.left_arm_priority);
                //Console.WriteLine("r priority is " + minimumHamiltonCycle.right_arm_priority);
                if (minimumHamiltonCycle.left_arm_priority >= minimumHamiltonCycle.right_arm_priority)
                {
                    minimumHamiltonCycle.searchleft = true;
                }
                else 
                {
                    minimumHamiltonCycle.searchleft = false;
                }
                if (minimumHamiltonCycle.stuck_left == true && minimumHamiltonCycle.stuck_right == false) 
                {
                    minimumHamiltonCycle.searchleft = false;
                }
                else if (minimumHamiltonCycle.stuck_left == false && minimumHamiltonCycle.stuck_right == true)
                {
                    minimumHamiltonCycle.searchleft = true;
                }

                //Console.WriteLine("search left is" + minimumHamiltonCycle.searchleft);

                if (minimumHamiltonCycle.searchleft)
                {
                    //Console.WriteLine(" searching left");
                    minimumHamiltonCycle.search_left_arm();
                    //Console.WriteLine(" left coner node {0} c1 {1} ", minimumHamiltonCycle.left_coner_node.id,
                    //minimumHamiltonCycle.left_coner_node.candidate_1().id);
                    //Console.WriteLine(" node {0} c1 {1} c2 {2}", minimumHamiltonCycle.left_coner_node.candidate_1().id,
                    //minimumHamiltonCycle.left_coner_node.candidate_1().candidate_1().id, minimumHamiltonCycle.left_coner_node.candidate_1().candidate_2().id);

                }
                else
                {
                    
                    //Console.WriteLine(" searching right");
                    minimumHamiltonCycle.search_right_arm();
                    //Console.WriteLine(" node {0} c1 {1} c2 {2}", minimumHamiltonCycle.left_coner_node.candidate_1().id,
                    //minimumHamiltonCycle.left_coner_node.candidate_1().candidate_1().id, minimumHamiltonCycle.left_coner_node.candidate_1().candidate_2().id);


                }
                


                if (minimumHamiltonCycle.stuck_left == true && minimumHamiltonCycle.stuck_right == true)
                {
                    if (minimumHamiltonCycle.left_arm_priority >= minimumHamiltonCycle.right_arm_priority)
                    {   //// for left
                        //add left edge
                        //Console.WriteLine("stuck so picking left");
                        Node node_to_add_edge_to = Nodes[minimumHamiltonCycle.left_coner_node.id];
                        Undirected_Edge left_edge_to_add = new Undirected_Edge(node_to_add_edge_to.candidate_1(), node_to_add_edge_to);
                        //Console.WriteLine("left edge is " + left_edge_to_add.id + "  weight : " + left_edge_to_add.weight);
                        minimumHamiltonCycle.add_edge_to_cycle( left_edge_to_add);
                        

                        // update left coners after edge
                        minimumHamiltonCycle.left_coner_node = Nodes[minimumHamiltonCycle.left_coner_node.left_edge.left_Node.id];
                        //Console.WriteLine(" new left conor is {0} and right conor node is  {1} " , minimumHamiltonCycle.left_coner_node.id , minimumHamiltonCycle.right_coner_node.id);


                        minimumHamiltonCycle.calculate_lr_priorities();
                        // find new priorities
                        // decide search left


                    }
                    else
                    {
                        // pick right edge
                        //Console.WriteLine("stuck so picking right");
                        Node node_to_add_edge_to = Nodes[minimumHamiltonCycle.right_coner_node.id];
                        Undirected_Edge right_edge_to_add = new Undirected_Edge(node_to_add_edge_to,node_to_add_edge_to.candidate_1());
                        //Console.WriteLine("right edge is " + right_edge_to_add.id + "  weight : " + right_edge_to_add.weight);
                        minimumHamiltonCycle.add_edge_to_cycle(right_edge_to_add);


                        // update right coners after edge
                        minimumHamiltonCycle.right_coner_node = Nodes[minimumHamiltonCycle.right_coner_node.right_edge.right_Node.id];
                        //Console.WriteLine(" new right conor is {0} and left conor node is {1} " , minimumHamiltonCycle.right_coner_node.id , minimumHamiltonCycle.left_coner_node.id);


                        minimumHamiltonCycle.calculate_lr_priorities();



                    }
                    // reset stuck left and stuck right
                    minimumHamiltonCycle.stuck_left = false;
                    minimumHamiltonCycle.stuck_right = false;


                }



                //printHamiltonCycle(minimumHamiltonCycle);

            }

            // stuck on both sides 

            // at hisi point only one node left so jyt create 2 edges for it

            Console.WriteLine(" creating edge for lase node");
            Node left_coner_node = Nodes[minimumHamiltonCycle.left_coner_node.id];
            Undirected_Edge last_left_edge_to_add = new Undirected_Edge(minimumHamiltonCycle.left_coner_node.candidate_1(), left_coner_node);
            

            Node right_coner_node = Nodes[minimumHamiltonCycle.right_coner_node.id];
            Undirected_Edge last_right_edge_to_add = new Undirected_Edge(right_coner_node, minimumHamiltonCycle.right_coner_node.candidate_1());
            // find the priorities of let and right arm
            minimumHamiltonCycle.add_edge_to_cycle(last_left_edge_to_add);
            minimumHamiltonCycle.add_edge_to_cycle(last_right_edge_to_add);

            return minimumHamiltonCycle;
        
        }
        
        

       
       
        public long get_node_priority(Node n) 
        {
            long p = n.priority;
            return p;
        
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

        class NodeComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return -(new CaseInsensitiveComparer()).Compare(((Node)x).sum_of_distances, ((Node)y).sum_of_distances);
            }
        }
        public void FindApproxHamiltoncycle()
        {

            
            //Console.WriteLine(" un sorted ######");
            //Array.ForEach<Node>(Nodes, p => Console.WriteLine(p.location + " " + p.sum_of_distances));
            Console.WriteLine("sorted ######");
            Array.ForEach<Node>(sorted_nodes, p => Console.WriteLine(p.id + " " + p.sum_of_distances));





            foreach (Node n in sorted_nodes)
            {

                Console.WriteLine("check node " + n.id);
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
                            Console.WriteLine("node :" + n.id + " at check  : " + n.index + " goes to " + check + " node: " + check + " atcheck :" + index2 + " goes to " + check2);
                            bool notloop = true;
                            // prevents loop and keeps the cycle one edge missing
                            var f = n;
                            f = f.adj_1;
                            var prev_f = n;
                            while (n.id == Nodes[check].distance.ElementAt(index2).Key && f != null)
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
                                if (f.id == check)
                                {
                                    notloop = false;

                                }
                            }

                            if (n.id == Nodes[check].distance.ElementAt(index2).Key && Nodes[check].edges_found != 2 && notloop)
                            {
                                Console.WriteLine("node1:" + check + " node2:" + check2 + "match");
                                n.index++;
                                Undirected_Edge temp = new Undirected_Edge(check, check2);
                                Undirected_Edge e;
                               
                                //Console.WriteLine("id:" + temp.id);
                                

                                
                                // below code prevents duplicates
                                if (MinimumHamiltonCycle.edges.Count == no_of_nodes - 1) // get up to only -1 edges are left
                                {
                                    return;
                                }
                                if (MinimumHamiltonCycle.edges.TryGetValue(temp.id, out e))
                                { }
                                else
                                {
                                    MinimumHamiltonCycle.edges.Add(temp.id, temp);
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
            if (MinimumHamiltonCycle.edges.Count < no_of_nodes - 1)
            {
                FindApproxHamiltoncycle();
            }
            // function to print he hamiltonpath

        }
        /// <summary>
        /// prints the edges of given hamiltoncycle h
        /// </summary>
        /// <param name="h"></par
        public void printHamiltonCycle(Hamilton_cycle h)
        {
            foreach (KeyValuePair<string, Undirected_Edge> item in h.edges)
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
            lastEdge = new Undirected_Edge(source.id, end.id);
            bool[] visited = new bool[no_of_nodes];
            Node[] previousnode = new Node[no_of_nodes];
            foreach (Node n in Nodes)
            {
                // initialise
                shortestdistance.Add(n.id, graph[source.id, n.id]);
                visited[n.id] = false;
                previousnode[n.id] = source;
            }
            visited[source.id] = true;
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
                        keyvalue = shortestdistance[keynode.id];
                    }
                }
                visited[keynode.id] = true;
                if (keynode.id == end.id)
                {
                    //Console.WriteLine("distance fronm " + source.location + " to " + end.location);
                    //Console.WriteLine(shortestdistance[keynode.location]);
                    shortestpath sm = new shortestpath(source, end);
                    sm.weight = shortestdistance[keynode.id];
                    lastEdge.weight = sm.weight;
                    Node i = end;
                    while (i.id != source.id)
                    {
                        Node currentnode = i;
                        i = previousnode[currentnode.id];
                        Undirected_Edge e = new Undirected_Edge(currentnode.id, i.id);
                        sm.pathEdges.Add(e);
                        //Console.WriteLine(currentnode.location + " to " + i.location);
                    }
                    lastEdge.arbitarypath = sm;
                    lastEdge.id += "arbitary";
                    MinimumHamiltonCycle.edges.Add(lastEdge.id, lastEdge);
                }
                foreach (Node n in Nodes)
                {
                    long newdistance = keyvalue + graph[keynode.id, n.id];
                    long olddistance = shortestdistance[n.id];
                    if (newdistance < olddistance)
                    {
                        shortestdistance[n.id] = newdistance;
                        previousnode[n.id] = keynode;
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
                foreach (KeyValuePair<string, Undirected_Edge> item in MinimumHamiltonCycle.edges)
                {
                    sw.WriteLine("node: " + item.Key);
                    sw.WriteLine(" weight:" + item.Value.weight);
                }


                // Console.WriteLine("distances is " + distances);


            }

        }




































    }
}
