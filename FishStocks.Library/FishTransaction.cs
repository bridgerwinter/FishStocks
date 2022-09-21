using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishStocks.Library
{
    public class FishTransaction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public DateTime DateEntered { get; set; }

    }
}
