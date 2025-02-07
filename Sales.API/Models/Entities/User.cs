using Sales.API.Models.Enum;
using System.Data;
using System.Net;
using System.Xml.Linq;

namespace Sales.API.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        
        public Name Name { get; set; }
        public Address Address { get; set; }

        public string Phone { get; set; }

      
        public Status Status { get; set; }

    
        public Role Role { get; set; }
    }
}
