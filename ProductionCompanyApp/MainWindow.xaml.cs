using ProductionCompanyApp.ViewModels;
using System;
using System.Windows;

namespace ProductionCompanyApp
{
    public partial class MainWindow : Window
    {
        private ProductListViewModel _viewModel;

        // ПУСТОЙ конструктор обязателен для StartupUri
        public MainWindow()
        {
            InitializeComponent();

            // Создаем ViewModel внутри конструктора
            var db = Data.DbContextFactory.CreateDbContext();
            var costService = new Services.ProductCostService(db);
            _viewModel = new ProductListViewModel(db, costService);
            DataContext = _viewModel;
        }

        // Остальные методы без изменений
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel?.AddProduct();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel?.EditProduct();
        }

        private void MaterialsButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel?.OpenMaterials();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                await _viewModel.RefreshProducts();
            }
        }

        private void ProductsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
        private void CalculationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var db = Data.DbContextFactory.CreateDbContext();
                var calculationWindow = new Views.MaterialCalculationWindow(db);
                calculationWindow.Owner = this;
                calculationWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                calculationWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия окна расчёта: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}