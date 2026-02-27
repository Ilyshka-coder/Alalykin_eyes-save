using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Алалыкин_Глазки_save
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Agent _currentAgent = new Agent();
        public AddEditPage(Agent SelectedAgent)
        {
            InitializeComponent();
            if (SelectedAgent != null)
                _currentAgent = SelectedAgent;
            DataContext = _currentAgent;
            ComboBoxType.SelectedIndex = _currentAgent.AgentTypeID-1;
            if (_currentAgent.AgentTypeID>0) DeleteBtn.Visibility = Visibility.Visible;
            else DeleteBtn.Visibility = Visibility.Hidden;
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                _currentAgent.Logo = myOpenFileDialog.FileName;
                LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxType.Text == "МФО") _currentAgent.AgentTypeID = 1;
            if (ComboBoxType.Text == "ООО") _currentAgent.AgentTypeID = 2;
            if (ComboBoxType.Text == "ЗАО") _currentAgent.AgentTypeID = 3;
            if (ComboBoxType.Text == "МКК") _currentAgent.AgentTypeID = 4;
            if (ComboBoxType.Text == "ОАО") _currentAgent.AgentTypeID = 5;
            if (ComboBoxType.Text == "ПАО") _currentAgent.AgentTypeID = 6;
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentAgent.Title))
                errors.AppendLine("Укажите наименование агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.Address))
                errors.AppendLine("Укажите адрес агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.DirectorName))
                errors.AppendLine("Укажите ФИО директора");
            if (ComboBoxType.SelectedItem==null)
                errors.AppendLine("Укажите тип агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.Priority.ToString()))
                errors.AppendLine("Укажите приоритет агента");
            if (_currentAgent.Priority<=0)
                errors.AppendLine("Укажите положительный приоритет агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.INN))
                errors.AppendLine("Укажите ИНН агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.KPP))
                errors.AppendLine("Укажите КПП агента");
            if (string.IsNullOrWhiteSpace(_currentAgent.Phone))
                errors.AppendLine("Укажите телефон агента");
            else
            {
                string ph = _currentAgent.Phone.Replace("(", "").Replace("-", "").Replace(")", "").Replace(" ", "").Replace("+", "");
                if (ph.Length <= 1 || (ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11 || (ph[1] == '3' && ph.Length != 12))
                    errors.AppendLine("Укажите правильно телефон агента");
            }
            if (string.IsNullOrWhiteSpace(_currentAgent.Email))
                errors.AppendLine("Укажите почту агента");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if (_currentAgent.ID == 0)
                AlalykinEyesEntities1.GetContext().Agent.Add(_currentAgent);
            try
            {
                AlalykinEyesEntities1.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentAgent = (sender as Button).DataContext as Agent;
            var currentProductSale = AlalykinEyesEntities1.GetContext().ProductSale.ToList();
            currentProductSale = currentProductSale.Where(p => p.AgentID == currentAgent.ID).ToList();
            if (currentProductSale.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление");
            else
            {
                if (MessageBox.Show("Вы хотите удаленить агента?","Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        AlalykinEyesEntities1.GetContext().Agent.Remove(currentAgent);
                        AlalykinEyesEntities1.GetContext().SaveChanges();
                        Manager.MainFrame.GoBack();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }
    }
}
