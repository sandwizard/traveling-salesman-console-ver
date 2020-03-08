using System;
using System.Collections.Generic;
using System.Text;

namespace traveling_salesman_console_ver
{
    namespace Datamodel
    {
        class DataModel
        {
            public long[,] DistanceMatrix = {
        {0, 2451, 713, 1018, 1631, 1374, 2408, 213, 2571, 875, 1420, 2145, 1972},
        {2451, 0, 1745, 1524, 831, 1240, 959, 2596, 403, 1589, 1374, 357, 579},
        {713, 1745, 0, 355, 920, 803, 1737, 851, 1858, 262, 940, 1453, 1260},
        {1018, 1524, 355, 0, 700, 862, 1395, 1123, 1584, 466, 1056, 1280, 987},
        {1631, 831, 920, 700, 0, 663, 1021, 1769, 949, 796, 879, 586, 371},
        {1374, 1240, 803, 862, 663, 0, 1681, 1551, 1765, 547, 225, 887, 999},
        {2408, 959, 1737, 1395, 1021, 1681, 0, 2493, 678, 1724, 1891, 1114, 701},
        {213, 2596, 851, 1123, 1769, 1551, 2493, 0, 2699, 1038, 1605, 2300, 2099},
        {2571, 403, 1858, 1584, 949, 1765, 678, 2699, 0, 1744, 1645, 653, 600},
        {875, 1589, 262, 466, 796, 547, 1724, 1038, 1744, 0, 679, 1272, 1162},
        {1420, 1374, 940, 1056, 879, 225, 1891, 1605, 1645, 679, 0, 1017, 1200},
        {2145, 357, 1453, 1280, 586, 887, 1114, 2300, 653, 1272, 1017, 0, 504},
        {1972, 579, 1260, 987, 371, 999, 701, 2099, 600, 1162, 1200, 504, 0},
      };
            public int VehicleNumber = 1;
            public int Depot = 0;
            //0. New York 1. Los Angeles 2. Chicago 3. Minneapolis 4. Denver 5. Dallas 6. Seattle 7. Boston
            //8.San Francisco 9. St.Louis 10. Houston 11. Phoenix 12. Salt Lake City

            struct node
            {   // location of node
                int location;
                // dictionary to represent distance to different locations
                Dictionary<int, int> distance = new Dictionary<int, int>();
            }

            // function to get distance 
            int Getdistance(int from, int to)
            {
                return DistanceMatrix[from, to];


            }

            int no_nodes = DistanceMatrix.GetLength(0);
            // create a struct for each node


            void initialise_nodes()
            {
                for (int i = 0; i < no_nodes; i++)
                {
               // create a temp node struct and initialise it and add to an structure array
                }

            }

            // step 1 sort in ascending order the values and store in arrays or structs
            // for each vertex



           //create a data structure for each node

           void sort(int from)
            {
                

            }

        }
    }


}
