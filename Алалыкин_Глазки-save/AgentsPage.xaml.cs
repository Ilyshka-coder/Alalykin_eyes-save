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
            this.Loaded += AgentsPage_Loaded;
        }
        private void AgentsPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateAgents();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
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
            currentAgents = currentAgents.Where(p => (p.Title.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Phone.Replace("+","").Replace(" ","").Replace("(","").Replace(")","").Replace("-","").Contains(TBoxSearch.Text.Replace("+", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "")) || p.Email.ToLower().Contains(TBoxSearch.Text.ToLower()))).ToList();
            AgentListView.ItemsSource = currentAgents.ToList();
            AgentListView.ItemsSource = currentAgents;
            TableList = currentAgents;
            ChangePage(0, 0);
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;
        int ObjectsOnPage = 10;
        List<Agent> CurrentPageList = new List<Agent>();
        List<Agent> TableList;
        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;
            CountPage = (CountRecords + ObjectsOnPage-1) / ObjectsOnPage;

            int newPage = CurrentPage;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage < CountPage)
                    newPage = selectedPage.Value;
                else
                    return;
            }
            else
            {
                switch (direction)
                {
                    case 1: newPage = CurrentPage - 1; break;
                    case 2: newPage = CurrentPage + 1; break;
                }
            }

            if (newPage < 0 || newPage >= CountPage)
                return;

            CurrentPage = newPage;

            int startIndex = CurrentPage * ObjectsOnPage;
            int endIndex = Math.Min(startIndex + ObjectsOnPage, CountRecords);

            for (int i = startIndex; i < endIndex; i++)
                CurrentPageList.Add(TableList[i]);

            PageListBox.Items.Clear();
            for (int i = 1; i <= CountPage; i++)
                PageListBox.Items.Add(i);

            PageListBox.SelectedIndex = CurrentPage;
            AgentListView.ItemsSource = CurrentPageList;
            AgentListView.Items.Refresh();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Agent));
        }
    }
}
