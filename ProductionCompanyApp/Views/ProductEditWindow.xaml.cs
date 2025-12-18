using ProductionCompanyApp.Data;
using ProductionCompanyApp.Models;
using ProductionCompanyApp.ViewModels;
using System.Windows;

namespace ProductionCompanyApp.Views
{
    public partial class ProductEditWindow : Window
    {
        private readonly ProductEditViewModel _viewModel;

        public ProductEditWindow(AppDbContext db, Product product = null)
        {
            InitializeComponent();
            _viewModel = new ProductEditViewModel(db, product);
            DataContext = _viewModel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Validate() && _viewModel.Save())
            {
                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}