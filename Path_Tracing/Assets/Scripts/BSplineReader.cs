
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PointsReader {
    public static List<Vector3> ReadFile(string path)
    {
        List<Vector3> points = new List<Vector3>();
        try
        {   // Open the text file using a stream reader.
            using (StreamReader sr = new StreamReader(path))
            {

                // Read the stream to a string, and write the string to the console.
                string line = string.Empty;
                while((line = sr.ReadLine()) != null)
                {
                    string[] words = line.Split(' ');
                    Vector3 point = new Vector3(float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]));
                    points.Add(point);
                }
                Console.WriteLine(line);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
        return points;
    }
}
