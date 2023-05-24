using Microsoft.EntityFrameworkCore;

namespace WebAppSST_Test.Models
{
    public class User
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? SunName { get; set; }
    }
}
