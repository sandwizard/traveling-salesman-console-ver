# master branch:

if a nodes smallest edge to a node is also the smallest edge to that node from which it is connected to .then it has a high probability of being part of the minimum hamilton cycle .Using this I obtained a very good approximation algorithm with approximation ranging from 1.17 to 1.24
Without drastic change in increase in nodes.

/////A dead end to finding optimal though


# new proposition

basing algorithm on priority

///Priority
We can get the value for how difficult or expensive a node is to get-to by adding the sum of all of its edges of the node.
This give us a ranking of which node is more important to get to at a lower edge weight cause the consequences for not getting to it at a low weight is much higher since than those with low priority since its sum of edges is much lower	



Genesis edge
We can almost always guarantee that the smallest edge of the highest priority node  or the smallest edge within the network will be a part of the minimum Hamilton cycle. (genesis edge)This will give us the starting 2 points and an edge. We will use the concept 
of two travelers sitting at the 2 starting nodes. Planning out their journeys with a relay in between them to help plan out the shortest way to visit all nodes.
With this we will encounter 2 cases one where the 2 travelers each plan to go to different nodes respectively and dice which one of  them should go.or have a clash where they both go to the same node and have do discuss their next course of action.




