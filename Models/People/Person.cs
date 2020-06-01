using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OBPostupy.Models.People
{
    /// <summary>
    /// Data model for Person
    /// </summary>
    public class Person
    {
        public int ID { get; set; }
        public string RegNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [JsonIgnore]
        public List<Results.PersonResult> PersonResults { get;}

        public string UserId { get; set; }
        
    }
}
