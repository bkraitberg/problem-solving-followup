using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace problem_solving_followup
{
    [Serializable]
    public class Transaction
    {
        [JsonProperty("UserName")]
        public string UserName;
        [JsonProperty("ProductName")]
        public string ProductName;
        [JsonProperty("Quantity")]
        public int Quantity;
        [JsonProperty("Date")]
        public DateTime Date;
    }
}
