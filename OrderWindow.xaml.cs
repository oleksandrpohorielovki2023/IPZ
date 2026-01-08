using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IPZ_Lab2 // Переконайтеся, що тут ваша назва проекту
{
    public partial class OrderWindow : Window
    {
        // Список доступних страв (База даних)
        List<Dish> availableDishes;

        // Загальна сума
        decimal currentTotal = 0;


        public OrderWindow()
        {
            InitializeComponent();
            LoadDishes();
        }

        // 1. Завантажуємо страви у випадаючий список (ComboBox)
        // 1. Завантажуємо страви ІЗ СЕРВЕРА
        private void LoadDishes()
        {
            try
            {
                // Ініціалізуємо список
                availableDishes = new List<Dish>();

                // --- ЗАПИТ ДО СЕРВЕРА ---
                string response = ServerClient.SendRequest("GET_PRODUCTS");

                // Перевірка на помилки
                if (string.IsNullOrEmpty(response) || response.StartsWith("ERROR"))
                {
                    MessageBox.Show("Не вдалося отримати меню з сервера.");
                    return;
                }

                // Розбираємо відповідь сервера
                // Сервер шле: "Страва1...~Страва2...~Страва3..."
                string[] productsData = response.Split('~');

                foreach (string row in productsData)
                {
                    if (string.IsNullOrWhiteSpace(row)) continue;

                    // Кожна страва: "Назва;Опис;Ціна;Емодзі"
                    string[] parts = row.Split(';');

                    if (parts.Length >= 4)
                    {
                        // Створюємо об'єкт Dish із даних сервера
                        string name = parts[0];
                        string desc = parts[1];

                        // Пробуємо перетворити ціну (заміна крапки на кому, якщо треба)
                        if (!decimal.TryParse(parts[2], out decimal price))
                        {
                            price = 0;
                        }

                        // Додаємо в список (Ваш конструктор Dish має приймати ці параметри)
                        // Припускаю, що у вас конструктор: Dish(Name, Description, Price, Emoji)
                        availableDishes.Add(new Dish(name, desc, price));
                    }
                }

                // Налаштовуємо ComboBox (випадаючий список)
                DishComboBox.ItemsSource = availableDishes;

                if (availableDishes.Count > 0)
                {
                    DishComboBox.SelectedIndex = 0; // Обрати першу страву
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка завантаження меню: " + ex.Message);
            }
        }

        // 2. Кнопка "Додати в список"
        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Отримуємо обрану страву
                Dish selectedDish = (Dish)DishComboBox.SelectedItem;

                // Отримуємо кількість (перетворюємо текст у число)
                if (!int.TryParse(CountTextBox.Text, out int count) || count <= 0)
                {
                    MessageBox.Show("Введіть коректну кількість (число більше 0)!");
                    return;
                }

                // Рахуємо вартість за цю позицію
                decimal itemTotal = selectedDish.Price * count;

                // Формуємо красивий рядок для списку
                string lineItem = $"{selectedDish.Name} x {count} шт. = {itemTotal} грн";

                // Додаємо рядок у ListBox праворуч
                CartListBox.Items.Add(lineItem);

                // Оновлюємо загальну суму
                currentTotal += itemTotal;
                TotalText.Text = $"{currentTotal} грн";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }
        }

        // 3. Кнопка "Підтвердити"
        private void Button_Checkout_Click(object sender, RoutedEventArgs e)
        {
            // 1. Перевірка на порожній кошик
            if (CartListBox.Items.Count == 0)
            {
                MessageBox.Show("Кошик порожній!");
                return;
            }

            PaymentWindow payWin = new PaymentWindow(currentTotal);

            bool? result = payWin.ShowDialog();

            if (result == true)
            {
                // === ОПЛАТА ПРОЙШЛА УСПІШНО, ТЕПЕР ЗБЕРІГАЄМО НА СЕРВЕР ===

                try
                {
                    // Збираємо текст замовлення
                    string allItems = "";
                    foreach (var item in CartListBox.Items)
                    {
                        allItems += item.ToString() + "; ";
                    }

                    // Формуємо запит
                    string request = $"ORDER|{allItems}|{currentTotal}";

                    // Відправляємо
                    string response = ServerClient.SendRequest(request);

                    if (response == "ORDER_SAVED")
                    {
                        MessageBox.Show("Замовлення відправлено на кухню!", "Готово");

                        // Очищаємо кошик
                        CartListBox.Items.Clear();
                        currentTotal = 0;
                        TotalText.Text = "0 грн";
                        CountTextBox.Text = "1";
                    }
                    else
                    {
                        MessageBox.Show("Помилка збереження на сервері: " + response);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка з'єднання: " + ex.Message);
                }
            }
            else
            {
                // Якщо користувач закрив вікно оплати хрестиком або кнопкою "Скасувати"
                MessageBox.Show("Оплату скасовано. Замовлення не оформлено.");
            }
        }

        // Додайте цей клас, якщо його ще немає або він неповний
        public class Dish
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }

            public Dish(string name, string description, decimal price)
            {
                Name = name;
                Description = description;
                Price = price;
            }

            // Щоб у списку замовлення красиво відображалася назва
            public override string ToString()
            {
                return Name;
            }
        }

        private void Button_Remove_Click(object sender, RoutedEventArgs e)
        {
            // 1. Перевірка: чи вибрано щось у списку?
            if (CartListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Будь ласка, виберіть страву зі списку, яку хочете видалити.");
                return;
            }

            try
            {
                // 2. Отримуємо текст вибраного рядка
                // Приклад: "Чізбургер x 2 шт. = 240 грн"
                string selectedItemText = CartListBox.SelectedItem.ToString();

                // 3. Витягуємо ціну з цього рядка
                // Нам треба те, що після знаку "="
                string[] parts = selectedItemText.Split('=');

                if (parts.Length == 2)
                {
                    // parts[1] буде " 240 грн"
                    string priceString = parts[1].Replace("грн", "").Trim();

                    if (decimal.TryParse(priceString, out decimal priceToRemove))
                    {
                        // 4. Віднімаємо від загальної суми
                        currentTotal -= priceToRemove;

                        // Оновлюємо напис
                        TotalText.Text = $"{currentTotal} грн";
                    }
                }

                // 5. Видаляємо рядок із ListBox
                // Видаляємо саме за індексом, бо так надійніше
                CartListBox.Items.RemoveAt(CartListBox.SelectedIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при видаленні: " + ex.Message);
            }
        }
    }
    }

