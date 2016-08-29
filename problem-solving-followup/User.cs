using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace problem_solving_followup
{
    [Serializable]
    public class User
    {
        [JsonProperty("Username")]
        public string Name;
        [JsonProperty("Password")]
        public string Password;
        [JsonProperty("Balance")]
        public double Balance;
    }
}
