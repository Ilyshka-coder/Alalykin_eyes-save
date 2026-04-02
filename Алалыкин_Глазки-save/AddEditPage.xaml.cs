using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        List<AgentType> agentTypes = new List<AgentType>();
        List<ProductSale> currentSales = new List<ProductSale>();
        Agent currentAgent = new Agent();
        ICollectionView _productsView;
        List<Product> _allProducts;
        bool is_new_agent = false;
        public AddEditPage(Agent agent)
        {
            InitializeComponent();
            if (agent == null)
            {
                agent = new Agent();
                agent.ID = AlalykinEyesEntities.GetContext().Agent.Count() + 1;
                is_new_agent = true;
            }
            else is_new_agent = false;
            currentAgent = agent;
            this.DataContext = agent;

            agentTypes = AlalykinEyesEntities.GetContext().AgentType.ToList();
            currentSales = AlalykinEyesEntities.GetContext().ProductSale.ToList().Where(s => s.AgentID == currentAgent.ID).ToList();


            List<string> agentTypesStr = new List<string>();
            foreach (AgentType agentType in agentTypes) agentTypesStr.Add(agentType.Title);
            int type_i = agentTypes.IndexOf(agent.AgentType);
            AgentTypeCB.ItemsSource = agentTypesStr;
            AgentTypeCB.SelectedIndex = type_i != -1 ? type_i : 0;

            _allProducts = AlalykinEyesEntities.GetContext().Product.ToList();
            _productsView = CollectionViewSource.GetDefaultView(_allProducts);
            ProductComboBox.ItemsSource = _productsView;

            ProductComboBox.Loaded += (s, e) =>
            {
                if (ProductComboBox.Template.FindName("PART_EditableTextBox", ProductComboBox) is TextBox textBox)
                    textBox.TextChanged += ProductComboBox_TextChanged;
            };

            SalesListView.ItemsSource = currentSales;
        }
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var _context = AlalykinEyesEntities.GetContext();
                StringBuilder errors = new StringBuilder();

                // Проверка обязательных полей
                if (string.IsNullOrWhiteSpace(currentAgent.Title))
                    errors.AppendLine("Укажите наименование агента");
                if (string.IsNullOrWhiteSpace(currentAgent.Address))
                    errors.AppendLine("Укажите адрес агента");
                if (string.IsNullOrWhiteSpace(currentAgent.DirectorName))
                    errors.AppendLine("Укажите ФИО директора");
                if (AgentTypeCB.SelectedItem == null)
                    errors.AppendLine("Укажите тип агента");
                else
                {
                    currentAgent.AgentType = agentTypes[AgentTypeCB.SelectedIndex];
                }

                // Priority
                if (string.IsNullOrWhiteSpace(currentAgent.Priority.ToString()))
                    errors.AppendLine("Укажите приоритет агента");
                else
                {
                    int priorityValue;
                    if (int.TryParse(currentAgent.Priority.ToString(), out priorityValue))
                    {
                        currentAgent.Priority = priorityValue;
                    }
                    else
                    {
                        errors.AppendLine("Приоритет должен быть числом");
                    }
                }

                if (currentAgent.Priority <= 0)
                    errors.AppendLine("Укажите положительный приоритет агента");

                if (string.IsNullOrWhiteSpace(currentAgent.INN))
                    errors.AppendLine("Укажите ИНН агента");
                if (string.IsNullOrWhiteSpace(currentAgent.KPP))
                    errors.AppendLine("Укажите КПП агента");
                if (string.IsNullOrWhiteSpace(currentAgent.Phone))
                    errors.AppendLine("Укажите телефон агента");
                else
                {
                    string digitsOnly = new string(currentAgent.Phone.Where(char.IsDigit).ToArray());
                    if (digitsOnly.Length < 10)
                        errors.AppendLine("Телефон должен содержать минимум 10 цифр");
                }

                // Проверка email
                if (string.IsNullOrWhiteSpace(currentAgent.Email))
                {
                    errors.AppendLine("Укажите почту агента");
                }
                else
                {
                    string email = currentAgent.Email.Trim();

                    if (!email.Contains("@"))
                    {
                        errors.AppendLine("Email должен содержать символ @");
                    }
                    else
                    {
                        string[] parts = email.Split('@');
                        if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
                        {
                            errors.AppendLine("Некорректный формат email");
                        }
                        else if (!parts[1].Contains(".ru") && !parts[1].Contains(".com") && !parts[1].Contains(".net") && !parts[1].Contains(".org"))
                        {
                            errors.AppendLine("Email должен быть с доменом .ru, .com или .net или .org");
                        }
                        else if (email.Contains(" ") || email.Contains(";") || email.Contains(","))
                        {
                            errors.AppendLine("Email не должен содержать пробелы или спецсимволы");
                        }
                    }
                }

                // Проверка длины полей
                if (currentAgent.Title != null && currentAgent.Title.Length > 150)
                    errors.AppendLine("Наименование не может быть длиннее 150 символов");
                if (currentAgent.INN != null && currentAgent.INN.Length > 12)
                    errors.AppendLine("ИНН не может быть длиннее 12 символов");
                if (currentAgent.KPP != null && currentAgent.KPP.Length > 9)
                    errors.AppendLine("КПП не может быть длиннее 9 символов");
                if (currentAgent.Phone != null && currentAgent.Phone.Length > 20)
                    errors.AppendLine("Телефон не может быть длиннее 20 символов");

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString());
                    return;
                }

                // СОХРАНЕНИЕ
                if (is_new_agent)
                {
                    _context.Agent.Add(currentAgent);
                }

                // Добавляем продажи в контекст
                foreach (var sale in currentAgent.ProductSale)
                {
                    if (sale.ID == 0)
                    {
                        _context.ProductSale.Add(sale);
                    }
                }

                _context.SaveChanges();
                MessageBox.Show("Информация сохранена");

                Manager.MainFrame.GoBack();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                if (ex.InnerException != null)
                {
                    MessageBox.Show($"Внутренняя ошибка: {ex.InnerException.Message}");
                }
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (is_new_agent)
                {
                    Manager.MainFrame.GoBack();
                    return;
                }
                var _context = AlalykinEyesEntities.GetContext();
                if (currentAgent.ProductSale.Count > 0)
                {
                    MessageBox.Show("Нельзя удалить агента, у которого есть продажи!");
                    return;
                }

                var result = MessageBox.Show("Вы точно хотите удалить агента?", "Внимание",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _context.Agent.Attach(currentAgent);
                    _context.Agent.Remove(currentAgent);
                    _context.SaveChanges();
                    MessageBox.Show("Агент удален");
                    Manager.MainFrame.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}");
            }
        }

        private void ChangePicureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (myOpenFileDialog.ShowDialog() == true)
            {
                try
                {
                    string sourceFile = myOpenFileDialog.FileName;
                    if (!File.Exists(sourceFile))
                    {
                        MessageBox.Show("Исходный файл не найден.");
                        return;
                    }

                    // Определяем папку, куда будем копировать
                    //  string imgsFolder = AppDomain.CurrentDomain.BaseDirectory;

                    // Directory.CreateDirectory(imgsFolder);

                    string fileName = System.IO.Path.GetFileName(sourceFile);
                    //string destPath = Path.Combine(imgsFolder, fileName);

                    //// Если файл с таким именем уже существует – добавляем суффикс
                    //int count = 1;
                    //while (File.Exists(destPath))
                    //{
                    //    string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    //    string ext = Path.GetExtension(fileName);
                    //    string newName = $"{nameWithoutExt}_{count}{ext}";
                    //    destPath = Path.Combine(imgsFolder, newName);
                    //    count++;
                    //}

                    //File.Copy(sourceFile, destPath);

                    // Сохраняем только имя файла (без пути)
                    fileName = "\\agents\\" + fileName;
                    currentAgent.Logo = fileName;

                    // Обновляем изображение на странице
                    LogoImage.Source = new BitmapImage(new Uri(fileName, UriKind.Relative));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при копировании файла: {ex.Message}");
                }
            }
        }


        private void ProductComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = ((TextBox)sender).Text;
            _productsView.Filter = obj =>
            {
                if (string.IsNullOrEmpty(filter)) return true;
                var product = obj as Product;
                return product.Title.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
            };

            // Раскрываем список, если есть результаты и фильтр не пустой
            if (!string.IsNullOrEmpty(filter) && !_productsView.IsEmpty)
                ProductComboBox.IsDropDownOpen = true;
        }

        //методы для работы с продажами
        private void AddSaleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProductComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите продукт", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(CountTextBox.Text, out int count) || count <= 0)
            {
                MessageBox.Show("Введите положительное целое число", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (SaleDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату продажи", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Получаем выбранный продукт
            Product selectedProduct = (Product)ProductComboBox.SelectedItem;

            ProductSale newSale = new ProductSale
            {
                ProductID = selectedProduct.ID,
                ProductCount = count,
                SaleDate = SaleDatePicker.SelectedDate.Value,
                Product = selectedProduct
            };

            currentAgent.ProductSale.Add(newSale);
            currentSales.Add(newSale);
            SalesListView.Items.Refresh();

            // Очистка полей
            CountTextBox.Text = "";
            SaleDatePicker.SelectedDate = DateTime.Today;
            ProductComboBox.SelectedItem = null;
        }

        private void DeleteBtn_Click_1(object sender, RoutedEventArgs e)
        {
            var _context = AlalykinEyesEntities.GetContext();
            Button btn = (Button)sender;
            ProductSale sale = btn.Tag as ProductSale;
            if (sale != null)
            {
                try
                {
                    // Удаляем из контекста, если продажа уже сохранена в БД
                    if (sale.ID != 0)
                    {
                        var saleInContext = _context.ProductSale.Find(sale.ID);
                        if (saleInContext != null)
                        {
                            _context.ProductSale.Remove(saleInContext);
                        }
                    }

                    // Удаляем из коллекций
                    currentAgent.ProductSale.Remove(sale);
                    currentSales.Remove(sale);

                    // Сохраняем изменения
                    _context.SaveChanges();

                    // Обновляем ListView
                    SalesListView.ItemsSource = null;
                    SalesListView.ItemsSource = currentSales;

                    MessageBox.Show("Продажа удалена");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }
    }
    }
}
