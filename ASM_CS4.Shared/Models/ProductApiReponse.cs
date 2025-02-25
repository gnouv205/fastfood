using ASM_CS4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_CS4.Shared.Models
{
    public class ProductApiResponse
    {
        public int TotalItems { get; set; }
        public List<Product> Items { get; set; }
    }

}
