using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPZ_Lab2 // Ваша назва проекту
{
    public class Dish
    {
        // Властивості з get/set (Вимога викладача)
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageEmoji { get; set; } // Використаємо емодзі замість картинок для простоти

        // Конструктор для швидкого створення
        public Dish(string name, string desc, decimal price, string emoji)
        {
            Name = name;
            Description = desc;
            Price = price;
            ImageEmoji = emoji;
        }
    }
}