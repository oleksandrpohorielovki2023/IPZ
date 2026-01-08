using FastFoodDelivery;
using System;
using System.Collections.Generic;
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

namespace IPZ_Lab2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        // Створюємо нового користувача для цього вікна
        public User NewUser { get; set; }

        public RegisterWindow()
        {
            InitializeComponent();

            NewUser = new User();
            // Прив'язуємо дані вікна до об'єкта NewUser
            this.DataContext = NewUser;
        }

        private void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Отримуємо паролі з PasswordBox (вони не прив'язуються автоматично з міркувань безпеки)
                string pass = RegPassBox.Password;
                string confirm = RegConfirmPassBox.Password;
                string login = RegEmailTxt.Text;

                // 2. Перевірка на порожні поля (Name і Login вже в NewUser завдяки Binding)
                if (string.IsNullOrEmpty(NewUser.Name) || string.IsNullOrEmpty(NewUser.Login))
                {
                    throw new Exception("Будь ласка, заповніть Ім'я та Email.");
                }

                if (string.IsNullOrEmpty(pass))
                {
                    throw new Exception("Введіть пароль.");
                }

                // 3. Перевірка співпадіння паролів
                if (pass != confirm)
                {
                    throw new Exception("Паролі не співпадають!");
                }

                // 4. Записуємо пароль у об'єкт
                NewUser.Password = pass;

                // --- ТУТ МАЄ БУТИ КОД ЗБЕРЕЖЕННЯ В БАЗУ ДАНИХ ---
                // Формуємо рядок: REGISTER|логін|пароль
                string request = $"REGISTER|{login}|{pass}";

                // Відправляємо через наш клас ServerClient
                string response = ServerClient.SendRequest(request);

                // 5. Перевіряємо, що відповів сервер
                if (response == "REGISTER_SUCCESS")
                {
                    MessageBox.Show("Акаунт успішно створено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close(); // Закриваємо вікно тільки якщо сервер сказав "ОК"
                }
                else
                {
                    // Якщо сервер повернув ERROR або щось інше
                    throw new Exception($"Сервер відхилив реєстрацію. Відповідь: {response}");
                }
            }

            catch (Exception ex)
            {
                // Вимога: обробка помилок через try-catch
                MessageBox.Show($"Помилка реєстрації: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
