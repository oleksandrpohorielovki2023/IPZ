using IPZ_Lab2;
using System;
using System.Windows;

namespace FastFoodDelivery
{
    public partial class MainWindow : Window
    {
        // Створюємо об'єкт користувача
        public User CurrentUser { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // Ініціалізуємо користувача
            CurrentUser = new User();

            // Встановлюємо DataContext. Тепер XAML знає, звідки брати дані для {Binding}
            this.DataContext = CurrentUser;
        }

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            string login = CurrentUser.Login; // Ваші назви полів можуть відрізнятися
            string password = CurrentUser.Password;

            // Формуємо команду за нашим протоколом: "LOGIN|login|password"
            string request = $"LOGIN|{login}|{password}";

            // ВІДПРАВЛЯЄМО НА СЕРВЕР
            string response = ServerClient.SendRequest(request);

            // Перевіряємо відповідь сервера
            if (response == "SUCCESS")
            {
                // Успіх
                OrderWindow orderWin = new OrderWindow();
                orderWin.Show();
                this.Close();
            }
            else if (response == "FAIL")
            {
                MessageBox.Show("Невірний логін або пароль (відхилено сервером).", "Помилка");
            }
            else
            {
                // Якщо сервер не відповів або сталася помилка
                // (Повідомлення вже показав клас ServerClient)
            }
        }

        private void Button_OpenRegister_Click(object sender, RoutedEventArgs e) // Назва методу може бути вашою
        {
            // Створюємо вікно реєстрації
            RegisterWindow regWindow = new RegisterWindow();

            // Показуємо його як діалогове (користувач не може клацати по іншим вікнам, доки не закриє це)
            regWindow.ShowDialog();


        }
    }
}