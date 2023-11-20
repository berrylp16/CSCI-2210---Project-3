using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace CSCI_2210___Project_3
{
   /// <summary>
   /// warehouse class that controls what the docks do and controls how the trucks unload the crates
   /// </summary>
    public class Warehouse
    {
        public List<Dock> Docks { get; }
        public Queue<Truck> Entrance { get; }
        

        public Warehouse(int dockCount)
        {
            if (dockCount > 15)
            {
                throw new ArgumentException("The maximum number of docks is 15.");
            }

            Docks = new List<Dock>();
            for (int i = 1; i <= dockCount; i++)
            {
                Docks.Add(new Dock($"Dock{i}"));
            }

            Entrance = new Queue<Truck>();
        }

        public Truck GenerateRandomTruck()
        {
            string[] driverNames = { "Luke Berry", "Joe Rutherford", "Jake Gillenwater", "John Doe", "Jeff Bezos" };
            string[] companyNames = { "BTD6", "Fortnite Shopping Cart Delivery", "UPS", "FedEx", "US Mail" };
            Random random = new Random();
            string Driver = driverNames[random.Next(driverNames.Length)];
            string DeliveryCompany = companyNames[random.Next(companyNames.Length)];

            Truck truck = new Truck(Driver, DeliveryCompany);

            int numberCrate = random.Next(1, 16);
            for(int i = 0; i < numberCrate; i++)
            {
                int id = random.Next(1, 100);
                double cratePrice = random.Next(50, 501); // Random price between $50 and $500
                Crate crate = new Crate(id, cratePrice);
                truck.Load(crate);
            }

            return truck;
        }

        private int GetTruckArrivalProb(int time)
        {
            int peakTime = 50;
            int standardDeviation = 20;

            double prob1 = Math.Exp(-Math.Pow(time - peakTime, 2) / (2 * Math.Pow(standardDeviation, 2)));
            prob1 *= 100;
            int prob = Convert.ToInt32(prob1);

            return prob;
        }
        string filePath = "crate_unloading_log.csv";
        public void GenerateReport(string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("Metric,Value"); // Write header

                    int numDocksOpen = Docks.Count;
                    int longestLine = Docks.Max(dock => dock.Line.Count);
                    int totalTrucksProcessed = Docks.Sum(dock => dock.TotalTrucks);
                    int totalCratesUnloaded = Docks.Sum(dock => dock.TotalCrates);
                    double totalValueOfCratesUnloaded = Docks.Sum(dock => dock.TotalSales);
                    double averageValueOfEachCrate = totalValueOfCratesUnloaded / totalCratesUnloaded;
                    double averageValueOfEachTruck = totalValueOfCratesUnloaded / totalTrucksProcessed;
                    int totalTimeInUse = Docks.Sum(dock => dock.TimeInUse);
                    int totalTimeNotInUse = Docks.Sum(dock => dock.TimeNotInUse);
                    double averageTimeInUse = (double)totalTimeInUse / numDocksOpen;
                    double totalOperatingCost = Docks.Sum(dock => dock.OperatingCost);
                    double totalRevenue = totalValueOfCratesUnloaded - totalOperatingCost;

                    // Write the data to the CSV file
                    WriteCsvLine(writer, "Number of Docks Open", numDocksOpen);
                    WriteCsvLine(writer, "Longest Line at a Loading Dock", longestLine);
                    WriteCsvLine(writer, "Total Trucks Processed", totalTrucksProcessed);
                    WriteCsvLine(writer, "Total Crates Unloaded", totalCratesUnloaded);
                    WriteCsvLine(writer, "Total Value of Crates Unloaded", totalValueOfCratesUnloaded);
                    WriteCsvLine(writer, "Average Value of Each Crate Unloaded", averageValueOfEachCrate);
                    WriteCsvLine(writer, "Average Value of Each Truck Unloaded", averageValueOfEachTruck);
                    WriteCsvLine(writer, "Total Time in Use", totalTimeInUse);
                    WriteCsvLine(writer, "Total Time Not in Use", totalTimeNotInUse);
                    WriteCsvLine(writer, "Average Time in Use per Dock", averageTimeInUse);
                    WriteCsvLine(writer, "Total Operating Cost of Docks", totalOperatingCost);
                    WriteCsvLine(writer, "Total Revenue of the Warehouse", totalRevenue);
                }

                Console.WriteLine($"Data written to {filePath} successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to CSV file: {ex.Message}");
            }
        }

        private void WriteCsvLine(StreamWriter writer, string metric, object value)
        {
            writer.WriteLine($"{metric},{value}");
        }
        public void Run()
        {
            Random randy = new Random();

            

            for (int time = 1; time <= 48; time++)
            {
                // Handle truck arrivals within the same time increment
                foreach (Dock dock in Docks)
                {
                    int prob = GetTruckArrivalProb(time);
                    if (randy.Next(100) < prob)
                    {
                        Truck newTruck = GenerateRandomTruck();
                        bool truckAddedToDock = false;

                        // Try to add the truck to an available dock
                        if (!dock.IsInUse())
                        {
                            dock.JoinLine(newTruck);
                            Console.WriteLine($"Time {time}: Truck arrived -- \nDriver: {newTruck.Driver}\nCompany: {newTruck.DeliveryCompany}");
                            truckAddedToDock = true;
                        }

                        if (!truckAddedToDock)
                        {
                            // If no available docks, the truck waits in the Entrance queue
                            Entrance.Enqueue(newTruck);
                            Console.WriteLine($"Time {time}: Truck arrived -- (Waiting.....)  \nDriver: {newTruck.Driver}\nCompany: {newTruck.DeliveryCompany}");
                        }
                    }
                }
                // Unload one crate from each dock
                foreach (Dock dock in Docks)
                {
                    if (dock.Line.Count > 0)
                    {
                        Truck currentTruck = dock.SendOff();
                        if (currentTruck != null && !dock.CrateUnloaded)
                        {
                           
                            
                                double Price = randy.Next(50, 501);

                                Crate crate = currentTruck.Unload();
                                dock.UnloadCrate(Price);
                                Console.WriteLine($"\nTime {time}: Unloaded crate from Truck --\nDriver: {currentTruck.Driver}\nCompany: {currentTruck.DeliveryCompany}\nCrate Price: ${Price:F2}");

                                string scenario = dock.Line.Count > 0
                                    ? "A crate was unloaded, but the truck still has more crates to unload"
                                    : "A crate was unloaded, Another truck is already in the Dock";
                                // Log the unloading scenario here

                                dock.CrateUnloaded = true;
                                dock.CurrentTruck = currentTruck;
                            
                        }
                    }
                    else if (dock.CrateUnloaded)
                    {
                        Truck currentTruck = dock.CurrentTruck;
                        if (currentTruck != null)
                        {
                            dock.CrateUnloaded = false;
                            dock.Line.Enqueue(currentTruck);
                        }
                    }
                }
            }

            GenerateReport("crate_unloading_log.csv");
        }
    }
}
