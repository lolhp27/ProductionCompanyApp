using ProductionCompanyApp.Data;
using ProductionCompanyApp.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ProductionCompanyApp.ViewModels
{
    public class ProductEditViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _db;
        private Product _product;
        private bool _isEditMode;

        public int Id { get; set; }

        private string _article = "";
        public string Article
        {
            get => _article;
            set
            {
                _article = value;
                OnPropertyChanged();
            }
        }

        private string _name = "";
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private decimal _minPartnerPrice;
        public decimal MinPartnerPrice
        {
            get => _minPartnerPrice;
            set
            {
                _minPartnerPrice = value;
                OnPropertyChanged();
            }
        }

        private decimal _rollWidth;
        public decimal RollWidth
        {
            get => _rollWidth;
            set
            {
                _rollWidth = value;
                OnPropertyChanged();
            }
        }

        public List<ProductType> ProductTypes { get; set; } = new();

        private ProductType _selectedProductType;
        public ProductType SelectedProductType
        {
            get => _selectedProductType;
            set
            {
                _selectedProductType = value;
                OnPropertyChanged();
            }
        }

        public string WindowTitle => _isEditMode ? "Редактирование продукции" : "Добавление продукции";

        public event PropertyChangedEventHandler? PropertyChanged;

        public ProductEditViewModel(AppDbContext db, Product product = null)
        {
            _db = db;
            _product = product;
            _isEditMode = product != null;

            LoadData();
        }

        private void LoadData()
        {
            // Загружаем типы продукции
            ProductTypes = _db.ProductTypes.ToList();

            if (_isEditMode)
            {
                // Заполняем поля из существующего продукта
                Id = _product.Id;
                Article = _product.Article;
                Name = _product.Name;
                MinPartnerPrice = _product.MinPartnerPrice;
                RollWidth = _product.RollWidth;
                SelectedProductType = ProductTypes.FirstOrDefault(pt => pt.Id == _product.ProductTypeId);
            }
            else
            {
                // Значения по умолчанию для нового продукта
                MinPartnerPrice = 0;
                RollWidth = 0;
                SelectedProductType = ProductTypes.FirstOrDefault();
            }
        }

        public bool Validate()
        {
            // Валидация данных
            if (string.IsNullOrWhiteSpace(Article))
            {
                MessageBox.Show("Артикул не может быть пустым", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("Наименование не может быть пустым", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (SelectedProductType == null)
            {
                MessageBox.Show("Выберите тип продукции", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (MinPartnerPrice < 0)
            {
                MessageBox.Show("Стоимость не может быть отрицательной", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (RollWidth < 0)
            {
                MessageBox.Show("Ширина рулона не может быть отрицательной", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Проверка уникальности артикула (только для нового продукта)
            if (!_isEditMode && _db.Products.Any(p => p.Article == Article))
            {
                MessageBox.Show("Продукт с таким артикулом уже существует", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public bool Save()
        {
            try
            {
                if (_isEditMode)
                {
                    // Обновляем существующий продукт
                    _product.Article = Article;
                    _product.Name = Name;
                    _product.MinPartnerPrice = MinPartnerPrice;
                    _product.RollWidth = RollWidth;
                    _product.ProductTypeId = SelectedProductType.Id;
                }
                else
                {
                    // Создаем новый продукт
                    var newProduct = new Product
                    {
                        Article = Article,
                        Name = Name,
                        MinPartnerPrice = MinPartnerPrice,
                        RollWidth = RollWidth,
                        ProductTypeId = SelectedProductType.Id
                    };

                    _db.Products.Add(newProduct);
                }

                _db.SaveChanges();
                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}