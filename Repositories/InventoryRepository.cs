

namespace ReportApp.Data.Services;

using ReportApp.Data.Models.inventory;
using ReportApp.Data.Models.crm;
using ReportService.Models;

public class InventoryRepository
    {
        private readonly DataContext _DbContext;
        
        public InventoryRepository(DataContext context)
        {
            _DbContext = context;
            
        }

        public void CreateOrder(Order order){
            var existingOrder = _DbContext.Orders
                .FirstOrDefault(o => o.Id == order.Id);
            if (existingOrder != null) return;
            _DbContext.Orders.Add(order); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
        }
        public void UpdateOrder(Order order){
            var existingOrder = _DbContext.Orders
                .FirstOrDefault(o => o.Id == order.Id);
            if (existingOrder == null) return;
            _DbContext.Orders.Update(order); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
        }
        public void DeleteOrder(Order order){
            var existingOrder = _DbContext.Orders
                .FirstOrDefault(o => o.Id == order.Id);
            if (existingOrder!= null)
            {
                _DbContext.Orders.Remove(existingOrder);
                _DbContext.SaveChanges();
            }
        }
        public void CreateProduct(Product product){
            var existingProduct = _DbContext.Products
                .FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null) return;
            _DbContext.Products.Add(product); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
        }
        public void UpdateProduct(Product product){
            var existingProduct = _DbContext.Products
                .FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct == null) return;
            _DbContext.Products.Update(product); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
        }
        public void DeleteProduct(Product product){
            var existingProduct = _DbContext.Products
                .FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct!= null)
            {
                _DbContext.Products.Remove(existingProduct);
                _DbContext.SaveChanges();
            }
        }
        public void CreateSale(Sale sale){
            var existingSale = _DbContext.Sales
                .FirstOrDefault(s => s.Id == sale.Id);
            if (existingSale != null) return;
            _DbContext.Sales.Add(sale); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
        }
        public void UpdateSale(Sale sale){
            var existingSale = _DbContext.Sales
                .FirstOrDefault(s => s.Id == sale.Id);
            if (existingSale == null) return;
            _DbContext.Sales.Update(sale); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
        }
        public void DeleteSale(Sale sale){
            var existingSale = _DbContext.Sales
                .FirstOrDefault(s => s.Id == sale.Id);
            if (existingSale!= null)
            {
                _DbContext.Sales.Remove(existingSale);
                _DbContext.SaveChanges();
            }
        }
    }
