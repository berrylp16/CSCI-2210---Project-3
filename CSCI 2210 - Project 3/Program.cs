using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CSCI_2210___Project_3
{
    class Program
    {

        /// <summary>
        /// main method for executing program
        /// </summary>
        static void Main()
        {
            int dockCount = 5;
            Warehouse warehouse = new Warehouse(dockCount);
            warehouse.Run();
        }
    }
}