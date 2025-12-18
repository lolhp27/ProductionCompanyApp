using Microsoft.EntityFrameworkCore;
using ProductionCompanyApp.Data;
using ProductionCompanyApp.Models;
using ProductionCompanyApp.Services;
using ProductionCompanyApp.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace ProductionCompanyApp.ViewModels
{
    public class ProductListViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _db;
        private readonly ProductCostService _costService;

        public ObservableCollection<ProductViewModel> Products { get; } = new();
        public ProductViewModel? SelectedProduct { get; set; }
        private string? _statusMessage;

        public string? StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ProductListViewModel(AppDbContext db, ProductCostService costService)
        {
            _db = db;
            _costService = costService;
            _ = LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                Products.Clear();
                StatusMessage = "Загрузка данных...";

                var products = await _db.Products
                    .Include(p => p.ProductType)
                    .ToListAsync();

                foreach (var product in products)
                {
                    var cost = await _costService.CalculateProductCostAsync(product.Id);

                    Products.Add(new ProductViewModel
                    {
                        Id = product.Id,
                        Article = product.Article,
                        ProductTypeName = product.ProductType?.Name ?? "Неизвестно",
                        Name = product.Name,
                        MinPartnerPrice = product.MinPartnerPrice,
                        RollWidth = product.RollWidth,
                        CalculatedCost = cost
                    });
                }

                StatusMessage = $"Загружено {Products.Count} продуктов";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка загрузки: {ex.Message}";
                ShowErrorMessage($"Ошибка при загрузке продукции: {ex.Message}");
            }
        }

        public void AddProduct()
        {
            try
            {
                StatusMessage = "Добавление нового продукта...";

                var editWindow = new ProductEditWindow(_db);
                editWindow.Owner = Application.Current.MainWindow;
                editWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                if (editWindow.ShowDialog() == true)
                {
                    _ = RefreshProducts();
                    StatusMessage = "Продукт успешно добавлен";
                    ShowInfoMessage("Продукт успешно добавлен в базу данных");
                }
                else
                {
                    StatusMessage = "Добавление отменено";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при добавлении: {ex.Message}";
                ShowErrorMessage($"Ошибка при открытии окна добавления: {ex.Message}");
            }
        }

        public void EditProduct()
        {
            if (SelectedProduct == null)
            {
                ShowWarningMessage("Выберите продукт для редактирования");
                StatusMessage = "Не выбран продукт для редактирования";
                return;
            }

            try
            {
                StatusMessage = $"Редактирование продукта {SelectedProduct.Article}...";

                // Находим продукт в БД
                var product = _db.Products.Find(SelectedProduct.Id);
                if (product == null)
                {
                    ShowErrorMessage($"Продукт с ID {SelectedProduct.Id} не найден");
                    StatusMessage = "Продукт не найден";
                    return;
                }

                var editWindow = new ProductEditWindow(_db, product);
                editWindow.Owner = Application.Current.MainWindow;
                editWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                bool? dialogResult = editWindow.ShowDialog();

                if (dialogResult == true)
                {
                    // Обновляем список БЕЗ повторного вызова EditProduct
                    _ = RefreshProducts();
                    StatusMessage = "Продукт обновлён";

                    // Показываем сообщение УСПЕХА, а не ОШИБКИ
                    ShowInfoMessage("Изменения успешно сохранены");
                }
                else if (dialogResult == false)
                {
                    StatusMessage = "Редактирование отменено";
                }
                else
                {
                    StatusMessage = "Окно редактирования закрыто";
                }
            }
            catch (Exception ex)
            {
                // ТОЛЬКО здесь показываем ошибку
                StatusMessage = $"Ошибка: {ex.Message}";
                ShowErrorMessage($"Ошибка при редактировании: {ex.Message}");
            }
        }

        public void OpenMaterials()
        {
            if (SelectedProduct == null)
            {
                ShowWarningMessage("Выберите продукт для просмотра материалов");
                StatusMessage = "Не выбран продукт для просмотра материалов";
                return;
            }

            try
            {
                // Находим продукт в БД
                var product = _db.Products.Find(SelectedProduct.Id);
                if (product == null)
                {
                    ShowErrorMessage($"Продукт с ID {SelectedProduct.Id} не найден в базе данных");
                    StatusMessage = "Продукт не найден";
                    return;
                }

                // Создаем окно материалов
                var materialsWindow = new MaterialsWindow(_db, product);
                materialsWindow.Owner = Application.Current.MainWindow;
                materialsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                materialsWindow.ShowDialog();

                StatusMessage = $"Просмотр материалов для {SelectedProduct.Article} завершен";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при открытии окна материалов: {ex.Message}";
                ShowErrorMessage($"Ошибка: {ex.Message}");
            }
        }
        public async Task RefreshProducts()
        {
            await LoadProductsAsync();
        }

        // Методы для показа сообщений (обработка ошибок по методичке)
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowWarningMessage(string message)
        {
            MessageBox.Show(message, "Предупреждение",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ShowInfoMessage(string message)
        {
            MessageBox.Show(message, "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}