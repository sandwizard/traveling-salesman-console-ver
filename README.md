# quick aproximation algorith for Traveling sales man pproblem
an fast  algorith to solve traveling salesman with apronimation around 1.1 - 1.26 

link to test data:
https://www.math.uwaterloo.ca/tsp/data/index.html

# Performance


according to https://www.math.uwaterloo.ca/tsp/world/lulog.html
  luxmberg computational log -lu980 

  Optimal Value:  11340
  
  Solution Method:  concorde (default settings), QSopt LP solver
  
  Solution Time:  1681.68 seconds, AMD Athlon 1.33 GHz


my algorithm

sollution : 12894

sollution time :few millisec on an modern cpu

after underclocking to test it still completed in a few sec you can run to confirm




# How it works
Approach I use to solve tsp

-considering we have to find the minimum Hamiltonian Cycle it helps to ignore the starting node .this is because each node is eventually visited thus making the start and end nodes irrelevant.

with this flexibility i take each node and sort and store its edge information . i.e node connected to and weight of the edge.this completes the preparatory stages


# FindingHamiltoncycle

The idea is to find the 2 minimum value edges of a node which are also minimum value edges of other node connected by the same edge . this confirms that this edge is contained in the minimum Hamiltonian path. this is done repetitively for every node.

Using the previously sorted list of edges of each node we check if the first 2 edges are also the minimum of the other node connected by that edge. We only check the first two to keep the algo from going through unnecessary edges . but if the desired edge is not found the search range in incremented by one every time .i have removed redundant checking of nodes by creating a object for the Hamilton_cycle called minimumHamiltonCycle and given each edge a unique id if the edge already exist it will not be processed, and if 2 edges for a node have already been found it will not be that node will not be checked.

This is done for up-to the last edge excluding the last edge .

# lastedge

This is because the last edge directly connects the remaining node but its weight may be extremely large or small "Lucky" . if its weight happens to be large and is taken as an edge for the cycle this makes all previously selected minimum edges useless .Instead i find an arbitrary path between the two remaining nodes using Dijkstra shortest path which may or may not be the direct connection and store the path taken and its cumulative weight.

This concludes finding the Hamiltonian path all other functions are utility functions for printing values ,except for Dijkstra and sort.

for storing most of the data i use dictionaries for their quick look up.


