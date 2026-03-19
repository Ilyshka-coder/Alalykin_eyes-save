using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Алалыкин_Глазки_save
{
    /// <summary>
    /// Логика взаимодействия для ChangePriority.xaml
    /// </summary>
    public partial class ChangePriority : Window
    {
        public int NewPriority { get; private set; }

        public ChangePriority(int defaultPriority)
        {
            InitializeComponent();
            PriorityTextBox.Text = defaultPriority.ToString();
            PriorityTextBox.Focus();
            PriorityTextBox.SelectAll();
        }

        private void PriorityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(PriorityTextBox.Text, out int priority))
            {
                NewPriority = priority;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректное числовое значение.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
