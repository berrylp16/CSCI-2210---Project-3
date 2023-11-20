using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCI_2210___Project_3
{
    /// <summary>
    /// crate class to put in the trucks
    /// </summary>
    public class Crate
    {
        public int Id { get; }
        public double Price { get; }

        public Crate(int id, double price)
        {
            Id = id;
            Random rand = new Random();
            Price = rand.NextDouble() * (500 - 50) + 50;
            Price = price;
        }

        public override string ToString()
        {
            return $"Crate ID: {Id}, Price: ${Price:F2}";
        }
    }
}
