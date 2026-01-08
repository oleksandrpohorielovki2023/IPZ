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
    public partial class PaymentWindow : Window
    {
        public PaymentWindow(decimal amountToPay)
        {
            InitializeComponent();

            // Показуємо передану суму на екрані
            AmountText.Text = $"{amountToPay} грн";
        }

        private void Button_Pay_Click(object sender, RoutedEventArgs e)
        {
            // 1. Валідація (чи заповнені поля)
            if (string.IsNullOrWhiteSpace(CardNumberTxt.Text) ||
                string.IsNullOrWhiteSpace(ExpiryTxt.Text) ||
                string.IsNullOrWhiteSpace(CvvTxt.Text))
            {
                MessageBox.Show("Заповніть дані картки!", "Помилка");
                return;
            }

            // 2. Тут може бути логіка перевірки карти...

            MessageBox.Show("Оплата успішна!", "Банк", MessageBoxButton.OK, MessageBoxImage.Information);

            // --- ГОЛОВНА ЗМІНА ---
            // Це повідомляє вікну замовлення, що все добре, і можна зберігати замовлення
            this.DialogResult = true;
            // ---------------------

            this.Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
