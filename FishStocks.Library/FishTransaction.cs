using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishStocks.Library
{
    public class FishTransaction
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public Fish Fish { get; set; }
        public DateTime DateEntered { get; set; }

    }
}
