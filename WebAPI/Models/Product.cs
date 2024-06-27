using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Product
    {
        [Key]
        public int pid { get; set; }

        public string Pimage { get; set; }

        public string Pname { get; set; }

        public string Pcat { get; set; }
        public int Price { get; set; }

    }
}
