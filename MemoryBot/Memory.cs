using System;
using Accord.MachineLearning.DecisionTrees;
using Accord;
using System.Data;
using Accord.Statistics.Filters;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math;

namespace MemoryBot
{
    static class Memory
    {
        public static void AddMemoryItem(string message, string user, string userName)
        {

            MemoryNode MemoryInput = new MemoryNode();
            MemoryInput.UserID = user;
            MemoryInput.UserName = userName;
            MemoryInput.MemoryString = message;

            Utils.SetMemoryAccessType("Memory");
            Utils.ReadMemory();
            Utils.WriteMemory();
        }

        public static void TestAccord()
        {

            /*
             * http://crsouza.com/2012/01/decision-trees-in-c/
             * */


            DataTable data = new DataTable("Memory");

            /*add People names/ID to columns dynamically*/
            data.Columns.Add("Day", "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");

            /*possibly add sentences to this?
            maybe keywords*/
            data.Rows.Add("D1", "Sunny", "Hot", "High", "Weak", "No");
            data.Rows.Add("D1", "Sunny", "Hot", "High", "Weak", "No");
            data.Rows.Add("D2", "Sunny", "Hot", "High", "Strong", "No");
            data.Rows.Add("D3", "Overcast", "Hot", "High", "Weak", "Yes");
            data.Rows.Add("D4", "Rain", "Mild", "High", "Weak", "Yes");
            data.Rows.Add("D5", "Rain", "Cool", "Normal", "Weak", "Yes");
            data.Rows.Add("D6", "Rain", "Cool", "Normal", "Strong", "No");
            data.Rows.Add("D7", "Overcast", "Cool", "Normal", "Strong", "Yes");
            data.Rows.Add("D8", "Sunny", "Mild", "High", "Weak", "No");
            data.Rows.Add("D9", "Sunny", "Cool", "Normal", "Weak", "Yes");
            data.Rows.Add("D10", "Rain", "Mild", "Normal", "Weak", "Yes");
            data.Rows.Add("D11", "Sunny", "Mild", "Normal", "Strong", "Yes");
            data.Rows.Add("D12", "Overcast", "Mild", "High", "Strong", "Yes");
            data.Rows.Add("D13", "Overcast", "Hot", "Normal", "Weak", "Yes");
            data.Rows.Add("D14", "Rain", "Mild", "High", "Strong", "No");

            // Create a new codification codebook to
            // convert strings into integer symbols
            Codification codebook = new Codification(data,"Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");

            /* NO IDEA FOR THIS */
            DecisionVariable[] attributes =
            {
                new DecisionVariable("Outlook", 3), // 3 possible values (Sunny, overcast, rain)
                new DecisionVariable("Temperature", 3), // 3 possible values (Hot, mild, cool)  
                new DecisionVariable("Humidity",    2), // 2 possible values (High, normal)    
                new DecisionVariable("Wind",        2)  // 2 possible values (Weak, strong) 
            };


            /* For possible values, make it one so it narrows to one individual fact about a word*/
            int classCount = 2; // 2 possible output values for playing tennis: yes or no


            DecisionTree tree = new DecisionTree(attributes, classCount);

            // Create a new instance of the ID3 algorithm
            ID3Learning id3learning = new ID3Learning(tree);

            // Translate our training data into integer symbols using our codebook:
            DataTable symbols = codebook.Apply(data);
            int[][] inputs = symbols.ToArray<int>("Outlook", "Temperature", "Humidity", "Wind");
            int[] outputs = symbols.ToIntArray("PlayTennis").GetColumn(0);

            // Learn the training instances!
            id3learning.Run(inputs, outputs);

            /*This is how we will query the memory*/
            int[] query = codebook.Translate("Sunny", "Hot", "High", "Strong");
            int output = tree.Compute(query);


            /*Respond to user*/
            string answer = codebook.Translate("PlayTennis", output); // answer will be "No".
            Console.WriteLine(answer);
        }

    }
}
