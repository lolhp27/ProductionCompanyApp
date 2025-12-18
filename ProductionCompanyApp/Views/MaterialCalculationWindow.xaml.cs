using Microsoft.EntityFrameworkCore;
using ProductionCompanyApp.Data;
using ProductionCompanyApp.Models;
using ProductionCompanyApp.Services;
using System;
using System.Linq;
using System.Windows;

namespace ProductionCompanyApp.Views
{
    public partial class MaterialCalculationWindow : Window
    {
        private readonly AppDbContext _db;
        private readonly MaterialCalculationService _calculationService;

        public MaterialCalculationWindow(AppDbContext db)
        {
            InitializeComponent();
            _db = db;
            _calculationService = new MaterialCalculationService(db);
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Загружаем продукты
                var products = _db.Products
                    .Include(p => p.ProductType)
                    .ToList();
                ProductComboBox.ItemsSource = products;
                ProductComboBox.SelectedIndex = 0;

                // Загружаем типы материалов
                var materialTypes = _db.MaterialTypes.ToList();
                MaterialTypeComboBox.ItemsSource = materialTypes;
                MaterialTypeComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем выбранные значения
                var selectedProduct = ProductComboBox.SelectedItem as Product;
                var selectedMaterialType = MaterialTypeComboBox.SelectedItem as MaterialType;

                if (selectedProduct == null || selectedMaterialType == null)
                {
                    ResultTextBlock.Text = "Выберите продукт и тип материала";
                    return;
                }

                // Парсим входные данные
                if (!int.TryParse(ProductCountTextBox.Text, out int productCount) || productCount <= 0)
                {
                    ResultTextBlock.Text = "Введите корректное количество продукции (>0)";
                    return;
                }

                if (!double.TryParse(Parameter1TextBox.Text, out double parameter1) || parameter1 <= 0)
                {
                    ResultTextBlock.Text = "Введите корректный параметр 1 (>0)";
                    return;
                }

                if (!double.TryParse(Parameter2TextBox.Text, out double parameter2) || parameter2 <= 0)
                {
                    ResultTextBlock.Text = "Введите корректный параметр 2 (>0)";
                    return;
                }

                if (!double.TryParse(StockQuantityTextBox.Text, out double stockQuantity) || stockQuantity < 0)
                {
                    ResultTextBlock.Text = "Введите корректный остаток на складе (≥0)";
                    return;
                }

                // Выполняем расчёт
                int result = _calculationService.CalculateRequiredMaterial(
                    selectedProduct.ProductTypeId,
                    selectedMaterialType.Id,
                    productCount,
                    parameter1,
                    parameter2,
                    stockQuantity);

                // Отображаем результат
                if (result == -1)
                {
                    ResultTextBlock.Text = "Ошибка в расчёте. Проверьте входные данные.";
                }
                else if (result == 0)
                {
                    ResultTextBlock.Text = $"✅ Материала достаточно на складе.\n" +
                                          $"Дополнительная закупка не требуется.";
                }
                else
                {
                    ResultTextBlock.Text = $"📦 Требуется закупить: {result} единиц материала.\n" +
                                          $"📊 Параметры: {parameter1:N2} × {parameter2:N2} × коэфф. = {selectedProduct.ProductType?.Coefficient:N2}\n" +
                                          $"🎯 Продукция: {selectedProduct.Name} ({productCount} шт.)";
                }
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Ошибка: {ex.Message}";
                MessageBox.Show($"Ошибка расчёта: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}