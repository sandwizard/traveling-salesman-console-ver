# traveling-salesman-console-ver
an algorith to solve traveling salesman for a completegraph in polynomial time
note >would be helpful if someone could confirm


# How it works
Approach I use to solve tsp

-considering we have to find the minimum Hamiltonian Cycle it helps to ignore the starting node .this is because each node is eventually visited thus making the start and end nodes irrelevant.

with this flexibility i take each node and sort and store its edge information . i.e node connected to and and weight of the edge this sorted list which can be done in polynomial time .this completes the preparatory stages

-- if you look at program .cs in my file it creates a new class of completeGraph.cs through which it checks weather the given graph is a valid complete Graph and runs three function which find the Hamiltonian Path.

Initialise_nodes() //sorts the edges of each node and stores it.I know this is in polynomial time .

FindHamiltoncycle() // as the name suggest explanation below. This is where i need the most help.

lastedge() // the last edge is not found using the same approach but by using shortest path willexplain below

______FindHamiltoncycle()_________

The idea is to find the 2 minimum value edges of a node which are also minimum value edges of other node connected by the same edge . this confirms that this edge is contained in the minimum Hamiltonian path. this is done repetitively for every node.

Using the previously sorted list of edges of each node we check if the first 2 edges are also the minimum of the other node connected by that edge. We only check the first two to keep the algo from going through unnecessary edges . but if the desired edge is not found the search range in incremented by one every time .i have removed redundant checking of nodes by creating a object for the Hamilton_cycle called minimumHamiltonCycle and given each edge a unique id if the edge already exist it will not be processed, and if 2 edges for a node have already been found it will not be that node will not be checked.

This is done for up-to the last edge excluding the last edge .

____lastedge()_____

This is because the last edge directly connects the remaining node but its weight may be extremely large or small "Lucky" . if its weight happens to be large and is taken as an edge for the cycle this makes all previously selected minimum edges useless .Instead i find an arbitrary path between the two remaining nodes using Dijkstra shortest path which may or may not be the direct connection and store the path taken and its cumulative weight.

This concludes finding the Hamiltonian path all other functions are utility functions for printing values ,except for Dijkstra and sort.

for storing most of the data i use dictionaries for their quick look up.

Files to look at.

-----program.cs

------completeGraph.cs

-- its a visual studio solution so you can clone it and test it on different data. if the data is not a valid complete graph it will throw an exception.

I am just a student and don't know much about finding time complexity so help would be much appreciated if some one helped calculate it

the three methods mentioned above run sequentially so the time-complexity should be the sum of those three .The second method is conditionally recursive.check code.
  
## utility methods

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
