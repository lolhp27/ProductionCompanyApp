using ProductionCompanyApp.Data;
using ProductionCompanyApp.Models;
using ProductionCompanyApp.ViewModels;
using System.Windows;

namespace ProductionCompanyApp.Views
{
    public partial class MaterialsWindow : Window
    {
        private readonly MaterialsViewModel _viewModel;

        // Правильный конструктор
        public MaterialsWindow(AppDbContext db, Product product)
        {
            InitializeComponent();

            // Создаем ViewModel с нужными параметрами
            _viewModel = new MaterialsViewModel(db, product);
            DataContext = _viewModel;
        }

        // Альтернативный конструктор для тестов
        public MaterialsWindow(MaterialsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}