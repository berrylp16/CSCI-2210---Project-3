﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCI_2210___Project_3
{
    public class Crate
    {
        public string Id { get; }
        public double Price { get; }

        Crate(string id)
        {
            Id = id;
            Random rand = new Random();
            Price = rand.NextDouble() * (500 - 50) + 50;
        }

        public override string ToString()
        {
            return $"Crate ID: {Id}, Price: ${Price:F2}";
        }
    }
}