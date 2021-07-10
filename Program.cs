using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel.Design.Serialization;


namespace traveling_salesman_console_ver
{
    class Program
    {
        

        static public  long[,] DistanceMatrix = {
        {0, 2451, 713, 1018, 1631, 1374, 2408, 213, 2571, 875, 1420, 2145, 1972},
        {2451, 0, 1745, 1524, 831, 1240, 959, 2596, 403, 1589, 1374, 357, 579},
        {713, 1745, 0, 355, 920, 803, 1737, 851, 1858, 262, 940, 1453, 1260},
        {1018, 1524, 355, 0, 700, 862, 1395, 1123, 1584, 466, 1056, 1280, 987},
        {1631, 831, 920, 700, 0, 663, 1021, 1769, 949, 796, 879, 586, 371},
        {1374, 1240, 803, 862, 663, 0, 1681, 1551, 1765, 547, 225, 887, 999},
        {2408, 959, 1737, 1395, 1021, 1681, 0, 8000, 678, 1724, 1891, 1114, 701},
        {213, 2596, 851, 1123, 1769, 1551, 8000, 0, 2699, 1038, 1605, 2300, 2099},
        {2571, 403, 1858, 1584, 949, 1765, 678, 2699, 0, 1744, 1645, 653, 600},
        {875, 1589, 262, 466, 796, 547, 1724, 1038, 1744, 0, 679 ,1272, 1162},
        {1420, 1374, 940, 1056, 879, 225, 1891, 1605, 1645, 679, 0, 1017, 1200},
        {2145, 357, 1453, 1280, 586, 887, 1114, 2300, 653, 1272, 1017, 0, 504},
        {1972, 579, 1260, 987, 371, 999, 701, 2099, 600, 1162, 1200, 504, 0},
      };
        
        static void Main(string[] args)
        {
            //GenerateGraph graph = new GenerateGraph(1979);
            GenerateGraph graph = new GenerateGraph();
            graph.from_tsp_data("D:/Work/Github/Repo/lu980.tsp");
            //for (int i = 0; i < graph.coordinate_graph.GetLength(0); i++)
            //{
            //    for (int j = 0; j < graph.coordinate_graph.GetLength(1); j++)
            //    {
            //        Console.Write(graph.coordinate_graph[i, j] + "\t");
            //    }
            //    Console.WriteLine();
            //}
            //for (int i = 0; i < graph.distance_matrix.GetLength(0); i++)
            //{
            //    for (int j = 0; j < graph.distance_matrix.GetLength(1); j++)
            //    {
            //        Console.Write(graph.distance_matrix[i, j] + "\t");
            //    }
            //    Console.WriteLine();
            //}

            Console.WriteLine("start now");
            completeGraph c = new completeGraph(graph.distance_matrix);
            //Console.WriteLine("end now");
            //c.printMinHamiltonPath();
            c.minimumHamiltonCycle.totalweight();
            c.printLastEdgeArbitaryPath();


        }
    }
}
