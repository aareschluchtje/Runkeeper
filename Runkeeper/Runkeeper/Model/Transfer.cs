using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runkeeper.Model
{
    public class Transfer
    {
        public Transfer(DataHandler datahandler)
        {
            this.data = datahandler;
        }

        public DataHandler data { get; set; }
    }
}
