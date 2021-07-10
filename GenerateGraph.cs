using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace traveling_salesman_console_ver
{
    public class GenerateGraph
    {
        int no_of_entries { get; set; }
        public long[,] distance_matrix { get; set; }
        public string name { get; set; }
        public int dimensions { get; set; }

        public double[,] coordinate_graph{ get; set; }

        public GenerateGraph()
        {
            
        }

        public GenerateGraph(int no_of_entries) 
        {
            this.no_of_entries = no_of_entries;

            this.distance_matrix = GenerateRandMatrix();
        
        }

        public long[,] GenerateRandMatrix() 
        {
            var rand = new Random();
            long[,] matrix = new long[no_of_entries, no_of_entries];
            
            int j = 0;
            for (int i =0; i < no_of_entries;i++) 
            {
                
                j = i;
                matrix[i, j] = 0;
                if (j != 0) 
                {
                    
                    while (j >= 1)
                    {
                        int num = rand.Next();
                        j = j - 1;
                        matrix[j, i] = num;
                        matrix[i, j] = num;

                    }
                }
                else
                {
                }
      
            }
            return matrix;
        
        }

    
        public double[,] GetCoordinateGraph( string path) 
        {
            string[] text = File.ReadAllLines(path);
            foreach (var line in text[0..10]) 
            {                
                //Console.WriteLine(line);
                string[] entries = line.Split(':',' ');
                string entry = entries[0];               
                //Console.WriteLine(entries);               
                // can expand later
                if (entry=="NAME")
                {
                    name = entries[3];
                    Console.WriteLine("this is name of locaion " +name);
                }
                else if (entry == "DIMENSION")
                {
                    dimensions = Int32.Parse(entries[3]);
                    Console.WriteLine("this is dimensions " + dimensions);
                }
                else if (entry == "COMMENT" || entry == "TYPE" || entry == "EDGE_WEIGHT_TYPE" || entry == "NODE_COORD_SECTION" )
                {
                    Console.WriteLine("doing nothing ");
                }
                else 
                { 
                }
            }
            
            double[,] coordinates = new double[dimensions, 2];

            foreach (var line in text) 
            {
                
                Console.WriteLine(line);
                string[] entries = line.Split(' ');
                string entry = entries[0];
                if (entry == "EOF"|| entry == "COMMENT" || entry == "TYPE" || entry == "EDGE_WEIGHT_TYPE" || entry == "NODE_COORD_SECTION" ||entry == "DIMENSION" || entry == "NAME") 
                {
                    //Console.WriteLine("do nothing");
                }
                else 
                {

                    //Console.WriteLine("this is entey 0 " + entries[0]);
                    int node = Int32.Parse(entries[0]) - 1;
                    //Console.WriteLine("this is entey 1 " + entries[1]);
                    coordinates[node, 0] = double.Parse(entries[1]);
                    //Console.WriteLine("this is entey 2 " + entries[2]);
                    coordinates[node, 1] = double.Parse(entries[2]);


                }

            }         
            return coordinates;

            //Console.WriteLine(text);
        }
        public void from_tsp_data(string path) 
        {
             
            coordinate_graph = GetCoordinateGraph(path);
            string distance_matrix_path = "D:/Work/Github/Repo/" + name + ".txt";
            if (File.Exists(distance_matrix_path))
            {
                distance_matrix = Get_distance_matrix_from_txt(distance_matrix_path);

            }
            else 
            {
                distance_matrix = Generate_TSP_Matrix();

                store_distance_matrix();

            }

        }

        public long[,] Generate_TSP_Matrix() 
        {
            long[,] matrix = new long[dimensions,dimensions];

            for(int i = 0; i < dimensions; i++ )
            {
                double x1 = coordinate_graph[i, 0];
                long x1l = Convert.ToInt64(x1);
                double y1 = coordinate_graph[i, 1];
                long y1l = Convert.ToInt64(y1);
                for (int j = 0; j < dimensions; j++) 
                {
                    if(i == j) 
                    {
                        // weight is zero
                        matrix[i, j] = 0; 
                    }
                    else if (j<i) 
                    {
                        matrix[i, j] = matrix[j, i];           
                    }
                    else 
                    {
                        double x2 = coordinate_graph[j, 0];
                        long x2l = Convert.ToInt64(x2);
                        double y2 = coordinate_graph[j, 1];
                        long y2l = Convert.ToInt64(y2);
                        // learn fast inverse and come up with something similar
                        double distance = Math.Sqrt((Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)));
                        //double distance = Math.Sqrt((Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)));
                        matrix[i, j] = Convert.ToInt64(distance);
                    }
              
                }                
            
            }

            return matrix;

        }

        public void store_distance_matrix() 
        {
            string path = "D:/Work/Github/Repo/" + name + ".txt";            
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                for (int i = 0; i < dimensions; i++) 
                {
                    string distances = "";
                    for (int j = 0; j < dimensions; j++) 
                    {
                        distances +=   distance_matrix[i,j]+" ";          
                    }
                    // Console.WriteLine("distances is " + distances);
                    sw.WriteLine(distances);
                }
                                            
            }
    
        }

        public long[,] Get_distance_matrix_from_txt(string path) 
        {
            long[,] matrix = new long[dimensions, dimensions];

            string[] text = File.ReadAllLines(path);
            
            for (int i =0; i < dimensions; i++) 
            {
                string[] entries = text[i].Split(" ");
                //Console.WriteLine("line " + text[i]);
                for(int j= 0; j< dimensions; j++) 
                {
                   // Console.WriteLine("entry " + entries[j]);
                    matrix[i, j] = long.Parse(entries[j]);

                    

                }
            }
            return matrix;

        }
         


            
    }

    
}
