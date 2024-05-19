
class TabulatedElements
{    
    
    public static Dictionary<string, (double weight, double pricePerUnit, int quantity, double shippingCost, double shippingRevenue, double profit, string deliveryStatus)> LoadData()
    {
        var itemData = new Dictionary<string, (double, double, int, double, double, double, string)>();

        try{
            using (var reader = new StreamReader(@"C:\Users\ACER\Downloads\DAA\.project\projDAA\final_csv-12545.csv")){
                reader.ReadLine(); // Skip header line

                while (!reader.EndOfStream){
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    string itemID = values[0];
                    string deliveryStatus = values[7];

                    if (!double.TryParse(values[1], out double weight) ||
                        !double.TryParse(values[2], out double pricePerUnit) ||
                        !int.TryParse(values[3], out int quantity) ||
                        !double.TryParse(values[4], out double shippingCost) ||
                        !double.TryParse(values[5], out double shippingRevenue) ||
                        !double.TryParse(values[6], out double profit))
                    {
                        Console.WriteLine("Invalid data format in line: " + line);
                        continue;
                    }

                    itemData[itemID] = (
                        weight, 
                        Math.Round(pricePerUnit, 2), 
                        quantity, 
                        Math.Round(shippingCost, 2), 
                        Math.Round(shippingRevenue, 2), 
                        profit,
                        deliveryStatus
                    );
                }
            }
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine("File Not Found: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }

        return itemData;
    }

    static void CheckToDeliveredItems(string filePath){
    Console.WriteLine("ItemID   | Weight  | PricePerUnit  | Quantity | ShippingCost | ShippingRevenue | LogisticsProfit  | DeliveryStatus |");
    try
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                // Trim whitespace and perform case-insensitive comparison
                if (values[7].Trim().Equals("True", StringComparison.OrdinalIgnoreCase))
                {
                    string formattedLine = string.Format(
                        "{0,-8} | {1,-7} | {2,-13} | {3,-8} | {4,-12} | {5,-15} | {6,-16} | {7,-14} |",
                        values[0].Trim(), // ItemID
                        values[1].Trim(), // Weight
                        values[2].Trim(), // PricePerUnit
                        values[3].Trim(), // Quantity
                        values[4].Trim(), // ShippingCost
                        values[5].Trim(), // ShippingRevenue
                        values[6].Trim(), // LogisticsProfit
                        values[7].Trim()  // DeliveryStatus
                    );
                    Console.WriteLine(formattedLine);
                }
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}


    public static int ChooseDeliveryMode()
    {
        bool status = true;
        int carryingCapacity = 0;

        while (status)
        {
            Console.Clear();
            Console.WriteLine("\n 1: Small Trucks  | Van, Pickup Truck (500 to 2000kg)");
            Console.WriteLine(" 2: Medium Trucks | Box or Light Duty Truck (2000 to 5000kg)");
            Console.WriteLine(" 3: Large Trucks  | Semi-trailer or Heavy-duty (5000 to 10000kg)");

            try
            {
                Console.Write("\nChoice: ");
                int truck = int.Parse(Console.ReadLine());

                if (truck < 1 || truck > 3)
                {
                    Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
                    continue;
                }

                Console.Write("Enter Carrying Capacity: ");
                carryingCapacity = int.Parse(Console.ReadLine());

                if (truck == 1 && (carryingCapacity < 500 || carryingCapacity > 2000))
                {
                    Console.WriteLine("Invalid carrying capacity for Small Trucks. Please enter a value between 500 and 2000.");
                }
                else if (truck == 2 && (carryingCapacity < 2000 || carryingCapacity > 5000))
                {
                    Console.WriteLine("Invalid carrying capacity for Medium Trucks. Please enter a value between 2000 and 5000.");
                }
                else if (truck == 3 && (carryingCapacity < 5000 || carryingCapacity > 10000))
                {
                    Console.WriteLine("Invalid carrying capacity for Large Trucks. Please enter a value between 5000 and 25000.");
                }
                else
                {
                    status = false;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input. Please enter numeric values.");
                System.Threading.Thread.Sleep(1000); // Pause for 1 second to show the error message
            }
        }

        return carryingCapacity;
    }

    static void UpdateDeliveryStatus(Dictionary<int, (int, int)> chosenItemData){
        string filePath = @"C:\Users\ACER\Downloads\DAA\.project\projDAA\super_final_csv-12545_.csv";
        string tempFilePath = @"C:\Users\ACER\Downloads\DAA\.project\projDAA\temp.csv";

        try{
            using (var reader = new StreamReader(filePath))
            using (var writer = new StreamWriter(tempFilePath)){
                
                string header = reader.ReadLine();
                writer.WriteLine(header);

                while (!reader.EndOfStream){
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    foreach (var item in chosenItemData){
                        if (item.Key == int.Parse(values[0])){
                            values[7] = "True";
                        }
                    }
                    writer.WriteLine(string.Join(",", values));
                }
            }

            File.Delete(filePath);
            File.Move(tempFilePath, filePath);

            Console.WriteLine("Delivery status updated successfully.");
            
        }catch (Exception e){
            Console.WriteLine(e);
        }
    }

    
    static List<int> FindSelectedItems(int[,] K, int[] wt, int m, int n)
    {
        List<int> selectedItems = new List<int>();
        int w = m;

        for (int i = n; i > 0 && w > 0; i--)
        {
            if (K[i, w] != K[i - 1, w])
            {
                selectedItems.Add(i);
                w -= wt[i];
            }
        }

        selectedItems.Reverse();
        return selectedItems;
    }


    


    static void DisplaySolutions(List<int> idSelectedItems, int[] Weight, int[] Profit, int m){
        int countWeights = 0;
        int countProfit = 0;
        var chosenItemData = new Dictionary<int, (int, int)>();

        Console.WriteLine("{0,-6} | {1,-8} | {2,-7} |", "Item", "Weight", "Profit");
        Console.WriteLine(new string('-', 28)); // A line separator for better readability

        foreach (int item in idSelectedItems){
            Console.WriteLine("{0,-6} | {1,-8} | {2,-7} |", item, Weight[item], Profit[item]);
            countWeights += Weight[item];
            countProfit += Profit[item];
            chosenItemData[item] = (Weight[item], Profit[item]);
        }

        UpdateDeliveryStatus(chosenItemData);

        
        Console.WriteLine("\nSummary:");
        Console.WriteLine("{0,-18} : {1}", "Carrying Capacity", m);
        Console.WriteLine("{0,-18} : {1}", "Total Weights", countWeights);
        Console.WriteLine("{0,-18} : {1}", "Total Profit", countProfit);
        Console.WriteLine();
    }

    static int[,] Knapsack(int m){ 
        var dataset = LoadData();
        int n = dataset.Count;
        int[] Weight = new int[n + 1];
        int[] Profit = new int[n + 1];
        int index = 1;
        int[,] K_Table = new int[n + 1, m + 1];

        foreach (var item in dataset){
            if(item.Value.deliveryStatus == "True"){
                continue;
            }else{
                Weight[index] = (int)item.Value.weight; //fetching the weights and profits
                Profit[index] = (int)item.Value.profit;
            }
            index++;
        }


        for (int i = 0; i <= n; i++){
            for (int w = 0; w <= m; w++){
                if (i == 0 || w == 0){
                    K_Table[i, w] = 0;
                }
                else if (Weight[i] <= w){
                    K_Table[i, w] = Math.Max(Profit[i] + K_Table[i - 1, w - Weight[i]], K_Table[i - 1, w]);
                }
                else{
                    K_Table[i, w] = K_Table[i - 1, w];
                }
            }
        }   

        List<int> idSelectedItems = FindSelectedItems(K_Table, Weight, m, n);
        DisplaySolutions(idSelectedItems, Weight, Profit, m);
        return K_Table;
    }
    
    static void Main(string[] args)
    {
        bool exit = false;
        while (!exit){
            Console.WriteLine("\nChoose Action: ");
            Console.WriteLine("  1: Run Knapsack");
            Console.WriteLine("  2: Check Items To Be Delivered");
            Console.WriteLine("  3: Exit");
            Console.Write("\n\n\t Action: ");

            int userAction;
            while (!int.TryParse(Console.ReadLine(), out userAction) || userAction < 1 || userAction > 3){
                Console.WriteLine("\n  Invalid input. Please enter a number between 1 and 3.");
            }

            switch (userAction){
                case 1:
                    int carryingCapacity = ChooseDeliveryMode();
                    int[,] K_Table = Knapsack(carryingCapacity);
                    break;
                case 2:
                    CheckToDeliveredItems(@"C:\Users\ACER\Downloads\DAA\.project\projDAA\super_final_csv-12545_.csv");
                    break;
                case 3:
                    exit = true;
                    break;
            }
        }
    }
   
}



    