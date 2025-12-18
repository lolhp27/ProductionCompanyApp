using Microsoft.EntityFrameworkCore;
using ProductionCompanyApp.Data;
using ProductionCompanyApp.Models;
using System;
using System.Linq;

namespace ProductionCompanyApp.Services
{
    public class MaterialCalculationService
    {
        private readonly AppDbContext _db;

        public MaterialCalculationService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Расчет требуемого количества материала для производства (Модуль 4)
        /// ТОЧНО ПО МЕТОДИЧКЕ:
        /// 1. Потребность = параметры × коэффициент типа продукции
        /// 2. Добавить процент брака материала
        /// 3. Учитывать остаток на складе
        /// 4. Вернуть целое количество
        /// 5. При ошибке вернуть -1
        /// </summary>
        public int CalculateRequiredMaterial(
            int productTypeId,
            int materialTypeId,
            int productCount,
            double parameter1,
            double parameter2,
            double stockQuantity)
        {
            // 5. В случае неверных данных вернуть –1
            if (productCount <= 0 || parameter1 <= 0 || parameter2 <= 0 || stockQuantity < 0)
            {
                return -1;
            }

            try
            {
                // Получаем данные из БД
                var productType = _db.ProductTypes.Find(productTypeId);
                var materialType = _db.MaterialTypes.Find(materialTypeId);

                if (productType == null || materialType == null)
                {
                    return -1;
                }

                // 1. Рассчитать потребность = параметры × коэффициент типа продукции
                double requirementPerProduct = parameter1 * parameter2 * productType.Coefficient;

                if (requirementPerProduct <= 0)
                {
                    return -1;
                }

                // Общая потребность для всего количества продукции
                double totalRequirement = requirementPerProduct * productCount;

                // 2. Добавить процент брака материала
                // Получаем процент брака из material_type.defect_percent
                double defectPercent = materialType.DefectPercent; // Уже в процентах (например 0.70)

                // Преобразуем процент в коэффициент: 0.70% = 0.007
                double defectCoefficient = defectPercent / 100.0;

                // Увеличиваем потребность с учётом брака
                double totalWithDefect = totalRequirement * (1.0 + defectCoefficient);

                // 3. Учитывать остаток на складе
                // Вычитаем то, что уже есть на складе
                double needToBuy = totalWithDefect - stockQuantity;

                // Если материала достаточно на складе
                if (needToBuy <= 0)
                {
                    return 0;
                }

                // 4. Вернуть целое количество закупаемого материала
                // Округляем ВВЕРХ до целого
                int result = (int)Math.Ceiling(needToBuy);

                return result;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Улучшенный метод с логированием для отладки
        /// </summary>
        public string CalculateWithDetails(
            int productTypeId,
            int materialTypeId,
            int productCount,
            double parameter1,
            double parameter2,
            double stockQuantity)
        {
            try
            {
                var productType = _db.ProductTypes.Find(productTypeId);
                var materialType = _db.MaterialTypes.Find(materialTypeId);

                if (productType == null || materialType == null)
                {
                    return "Ошибка: не найден тип продукции или материала";
                }

                // 1. Потребность на единицу
                double perProduct = parameter1 * parameter2 * productType.Coefficient;
                double totalNeeded = perProduct * productCount;

                // 2. Процент брака
                double defectPercent = materialType.DefectPercent;
                double defectCoefficient = defectPercent / 100.0;
                double totalWithDefect = totalNeeded * (1.0 + defectCoefficient);

                // 3. Учёт остатка
                double needToBuy = totalWithDefect - stockQuantity;

                // Формируем подробный отчёт
                string report = $"📊 ДЕТАЛЬНЫЙ РАСЧЁТ:\n" +
                               $"========================\n" +
                               $"Тип продукции: {productType.Name} (коэфф: {productType.Coefficient})\n" +
                               $"Тип материала: {materialType.Name} (брак: {defectPercent}%)\n" +
                               $"Количество продукции: {productCount} шт.\n" +
                               $"Параметры: {parameter1} × {parameter2}\n" +
                               $"Потребность на ед.: {perProduct:F2}\n" +
                               $"Общая потребность: {totalNeeded:F2}\n" +
                               $"С учётом брака ({defectPercent}%): {totalWithDefect:F2}\n" +
                               $"Остаток на складе: {stockQuantity:F2}\n" +
                               $"Требуется закупить: {Math.Max(0, needToBuy):F2}\n" +
                               $"========================\n";

                if (needToBuy <= 0)
                {
                    report += $"✅ Результат: Материала достаточно (0)";
                }
                else
                {
                    int result = (int)Math.Ceiling(needToBuy);
                    report += $"📦 Результат: Нужно закупить {result} ед.";
                }

                return report;
            }
            catch (Exception ex)
            {
                return $"Ошибка расчёта: {ex.Message}";
            }
        }

        /// <summary>
        /// Упрощённый метод для тестирования с конкретным продуктом
        /// </summary>
        public (int Result, string Details) CalculateForProduct(int productId, int productCount, double stockQuantity)
        {
            try
            {
                var product = _db.Products
                    .Include(p => p.ProductType)
                    .Include(p => p.ProductMaterials)
                        .ThenInclude(pm => pm.Material)
                            .ThenInclude(m => m.MaterialType)
                    .FirstOrDefault(p => p.Id == productId);

                if (product == null)
                {
                    return (-1, "Продукт не найден");
                }

                // Берём первый материал продукта
                var productMaterial = product.ProductMaterials.FirstOrDefault();
                if (productMaterial == null)
                {
                    return (-1, "У продукта нет материалов");
                }

                // Параметры: ширина рулона и 1 (стандартная длина)
                double parameter1 = (double)product.RollWidth;
                double parameter2 = 1.0;

                // Выполняем расчёт
                int result = CalculateRequiredMaterial(
                    product.ProductTypeId,
                    productMaterial.Material.MaterialTypeId,
                    productCount,
                    parameter1,
                    parameter2,
                    stockQuantity);

                // Генерируем детали
                string details = CalculateWithDetails(
                    product.ProductTypeId,
                    productMaterial.Material.MaterialTypeId,
                    productCount,
                    parameter1,
                    parameter2,
                    stockQuantity);

                return (result, details);
            }
            catch (Exception ex)
            {
                return (-1, $"Ошибка: {ex.Message}");
            }
        }
    }
}