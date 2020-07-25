# traveling-salesman-console-ver
an algorith to solve traveling salesman for a completegraph in polynomial time
note >would be helpful if someone could confirm


# How it works
--completeGraph.cs has the main functionality
  takes a matrix argument 
  it takea a complete graph and takes each node and finds 2 minimumum value edges which are also the minimum value edge of the other nodes connected by that edge.This way we get     the 2 minimum paths on for entrence and one for exit.While also considering the next nodee in the path.  For the last edge it finds a arbitary path using the shortest path         algo.since the last edge connectes two nodes there is no way to check if the direct edge that connects the is the best one by normal mean. i.e the value may be extremely large     or low. therefore we find the shortest path between the last two nodes givig us an arbitary path through other nodes if the direct path is large. fell free to change the data to   test this out i.e increase the last edge value to be large 
    
  
## methods

   # printMinHamiltonPath();  
      -- prints the hamilton path in the format of edge data              // not printed in order of the path taken considering each node is visited direction is not important               eg.                                                                    since it forms  a circular path compilation of all edges form the cycle
            Node1:Node2
             weight ...
                                                        
          
      
   # minimumHamiltonCycle.totalweight();
    -- returns the total weight of the graph of type long and prints it
    
   # printLastEdgeArbitaryPath();
    --prints the path taken by the last edge    // 
    #####################   NOTICE ########
       will provide a gif to show full functionaliy in the future
