using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPZ_Lab2
{
    internal class ProductModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        // Це для красивого відображення в списку (якщо не використовувати Binding)
        public override string ToString()
        {
            return $" {Name} - {Price} грн";
        }
    }
}
