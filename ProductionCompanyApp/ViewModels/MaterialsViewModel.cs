using Microsoft.EntityFrameworkCore;
using ProductionCompanyApp.Data;
using ProductionCompanyApp.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ProductionCompanyApp.ViewModels
{
    public class MaterialsViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _db;
        private readonly int _productId;

        private string _productName = "";
        public string ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged();
            }
        }

        private string _productArticle = "";
        public string ProductArticle
        {
            get => _productArticle;
            set
            {
                _productArticle = value;
                OnPropertyChanged();
            }
        }

        public List<MaterialInfo> Materials { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        // Конструктор с параметрами
        public MaterialsViewModel(AppDbContext db, Product product)
        {
            _db = db;
            _productId = product.Id;
            ProductName = product.Name;
            ProductArticle = product.Article;
            LoadMaterials();
        }

        // Альтернативный конструктор
        public MaterialsViewModel(AppDbContext db, int productId)
        {
            _db = db;
            _productId = productId;
            LoadMaterials();
        }

        private void LoadMaterials()
        {
            // Получаем продукт с данными
            var product = _db.Products
                .Include(p => p.ProductMaterials)
                    .ThenInclude(pm => pm.Material)
                        .ThenInclude(m => m.MaterialType)
                .FirstOrDefault(p => p.Id == _productId);

            if (product == null) return;

            ProductName = product.Name;
            ProductArticle = product.Article;

            Materials.Clear();
            foreach (var pm in product.ProductMaterials)
            {
                if (pm.Material != null)
                {
                    Materials.Add(new MaterialInfo
                    {
                        MaterialName = pm.Material.Name,
                        MaterialType = pm.Material.MaterialType?.Name ?? "Неизвестно",
                        QuantityPerProduct = pm.QuantityPerProduct,
                        Unit = pm.Material.Unit,
                        Cost = pm.Material.Cost,
                        TotalCost = pm.QuantityPerProduct * (double)pm.Material.Cost
                    });
                }
            }

            OnPropertyChanged(nameof(Materials));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MaterialInfo
    {
        public string MaterialName { get; set; } = "";
        public string MaterialType { get; set; } = "";
        public double QuantityPerProduct { get; set; }
        public string Unit { get; set; } = "";
        public decimal Cost { get; set; }
        public double TotalCost { get; set; }

        // Форматированные свойства для отображения
        public string QuantityFormatted => $"{QuantityPerProduct:N2}";
        public string CostFormatted => $"{Cost:N2} ₽";
        public string TotalCostFormatted => $"{TotalCost:N2} ₽";
    }
}