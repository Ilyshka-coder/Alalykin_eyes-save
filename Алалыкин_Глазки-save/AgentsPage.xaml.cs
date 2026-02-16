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
    /// Логика взаимодействия для AgentsPage.xaml
    /// </summary>
    public partial class AgentsPage : Page
    {
        public AgentsPage()
        {
            InitializeComponent();
            var currentAgent = AlalykinEyesEntities1.GetContext().Agent.ToList();
            AgentListView.ItemsSource = currentAgent;
            ComboType.SelectedIndex = 0;
            SortType.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void SortType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }
        private void UpdateAgents()
        {
            var currentAgents = AlalykinEyesEntities1.GetContext().Agent.ToList();
            if (ComboType.SelectedIndex == 0)
            {
                currentAgents = currentAgents.Where(p => (p.AgentTypeTitle != "")).ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                currentAgents = currentAgents.Where(p => (p.AgentTypeTitle == "МФО")).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentAgents = currentAgents.Where(p => (p.AgentTypeTitle == "ООО")).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentAgents = currentAgents.Where(p => (p.AgentTypeTitle == "ЗАО")).ToList();
            }
            if (ComboType.SelectedIndex == 4)
            {
                currentAgents = currentAgents.Where(p => (p.AgentTypeTitle == "МКК")).ToList();
            }
            if (ComboType.SelectedIndex == 5)
            {
                currentAgents = currentAgents.Where(p => (p.AgentTypeTitle == "ОАО")).ToList();
            }
            if (ComboType.SelectedIndex == 6)
            {
                currentAgents = currentAgents.Where(p => (p.AgentTypeTitle == "ПАО")).ToList();
            }
            if (SortType.SelectedIndex == 0)
            {
                currentAgents = currentAgents.ToList();
            }
            if (SortType.SelectedIndex == 1)
            {
                currentAgents = currentAgents.OrderBy(p => p.Title).ToList();
            }
            if (SortType.SelectedIndex == 2)
            {
                currentAgents = currentAgents.OrderByDescending(p => p.Title).ToList();
            }
            if (SortType.SelectedIndex == 3)
            {
                currentAgents = currentAgents.OrderBy(p => p.Discount).ToList();
            }
            if (SortType.SelectedIndex == 4)
            {
                currentAgents = currentAgents.OrderByDescending(p => p.Discount).ToList();
            }
            if (SortType.SelectedIndex == 5)
            {
                currentAgents = currentAgents.OrderBy(p => p.Priority).ToList();
            }
            if (SortType.SelectedIndex == 6)
            {
                currentAgents = currentAgents.OrderByDescending(p => p.Priority).ToList();
            }
            currentAgents = currentAgents.Where(p => (p.Title.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.PhoneNum.Contains(TBoxSearch.Text) || p.Email.ToLower().Contains(TBoxSearch.Text.ToLower()))).ToList();
            AgentListView.ItemsSource = currentAgents.ToList();
            AgentListView.ItemsSource = currentAgents;
        }
    }
}
