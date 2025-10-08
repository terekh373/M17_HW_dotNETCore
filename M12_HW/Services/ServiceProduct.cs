using Microsoft.EntityFrameworkCore;
using M12_HW.Models;

namespace M12_HW.Services
{
    public interface IServiceProducts
    {
        Task<Product?> CreateAsync(Product? product); // Асинхронний метод створення продукту
        Task<IEnumerable<Product>> ReadAsync(); // Асинхронний метод отримання всіх продуктів
        Task<Product?> GetByIdAsync(int id); // Асинхронний метод отримання продукту з його ID
        Task<Product?> UpdateAsync(int id, Product? product); // Асинхронний метод оновлення продукту
        Task<bool> DeleteAsync(int id); // Асинхронний метод видалення продукту по ID
        Task<IEnumerable<Product>> GetCartAsync(); // Асинхронний метод отримання продуктів у кошику
        Task AddToCartAsync(int productId); // Асинхронний метод додавання продукту до кошика по його ID
        Task<bool> RemoveFromCartAsync(int productId); // Асинхронний метод видалення продукту з кошика по його ID
        Task ClearCartAsync(); // Асинхронний метод очищення кошика
    }

    public class ServiceProducts : IServiceProducts
    {
        private readonly ProductStoreContext _productStoreContext; // Хранит контекст базы данных для работы с продуктами
        private readonly ILogger<ServiceProducts> _logger; // Логгер для записи событий и ошибок
        private readonly List<Product> _cart = new(); // Корзина в памяти


        // Конструктор класса, который принимает контекст и логгер через внедрение зависимостей
        public ServiceProducts(ProductStoreContext productContext, ILogger<ServiceProducts> logger)
        {
            _productStoreContext = productContext; // Инициализация контекста базы данных
            _logger = logger; // Инициализация логгера
        }

        // Методы для работы с корзиной
        public async Task<IEnumerable<Product>> GetCartAsync()
        {
            return await Task.FromResult(_cart);
        }

        // Метод для добавления продукта в корзину по его ID
        public async Task AddToCartAsync(int productId)
        {
            var product = await _productStoreContext.Products.FindAsync(productId);
            if (product != null)
            {
                _cart.Add(product);
            }
        }

        // Метод для удаления продукта из корзины по его ID
        public async Task<bool> RemoveFromCartAsync(int productId)
        {
            var product = _cart.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                _cart.Remove(product);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        // Метод для очистки корзины
        public async Task ClearCartAsync()
        {
            _cart.Clear();
            await Task.CompletedTask;
        }

        // Метод для создания нового продукта
        public async Task<Product?> CreateAsync(Product? product)
        {
            // Проверка, является ли продукт нулевым
            if (product == null)
            {
                _logger.LogWarning("Попытка создать продукт с нулевым значением."); // Логирование предупреждения
                return null; // Возврат нуля, если продукт нулевой
            }

            // Добавление продукта в контекст базы данных
            await _productStoreContext.Products.AddAsync(product);
            // Сохранение изменений в базе данных
            await _productStoreContext.SaveChangesAsync();
            return product; // Возврат созданного продукта
        }

        // Метод для удаления продукта по его ID
        public async Task<bool> DeleteAsync(int id)
        {
            // Поиск продукта в базе данных по его ID
            var product = await _productStoreContext.Products.FindAsync(id);
            // Если продукт не найден, вернуть false
            if (product == null)
            {
                return false;
            }

            // Удаление продукта из контекста базы данных
            _productStoreContext.Products.Remove(product);
            // Сохранение изменений в базе данных
            await _productStoreContext.SaveChangesAsync();
            return true; // Возврат true, если продукт успешно удален
        }

        // Метод для получения продукта по его ID
        public async Task<Product?> GetByIdAsync(int id)
        {
            // Поиск продукта в базе данных по его ID
            return await _productStoreContext.Products.FindAsync(id);
        }


        // Метод для получения всех продуктов из базы данных
        public async Task<IEnumerable<Product>> ReadAsync()
        {
            // Возврат списка всех продуктов
            return await _productStoreContext.Products.ToListAsync();
        }

        // Метод для обновления существующего продукта
        public async Task<Product?> UpdateAsync(int id, Product? product)
        {
            // Проверка, является ли продукт нулевым или идентификатор не совпадает
            if (product == null || id != product.Id)
            {
                _logger.LogWarning($"Несоответствие идентификатора продукта. Ожидался {id}, получен {product?.Id}."); // Логирование предупреждения
                return null; // Возврат нуля, если есть несоответствие
            }

            try
            {
                // Обновление продукта в контексте базы данных
                _productStoreContext.Products.Update(product);
                // Сохранение изменений в базе данных
                await _productStoreContext.SaveChangesAsync();
                return product; // Возврат обновленного продукта
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Логирование ошибки при обновлении продукта
                _logger.LogError(ex, "Ошибка при обновлении продукта с идентификатором {Id}.", id);
                return null; // Возврат нуля в случае ошибки
            }
        }

    }
}
