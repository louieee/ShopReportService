namespace ReportApp.Data.Services;
using ReportService.Models;

public class AccountRepository
    {
        private readonly DataContext _DbContext;
        
        public AccountRepository(DataContext context)
        {
            _DbContext = context;
            
        }

        public void CreateUser(User user)
        {
            var existingUser = _DbContext.Users.First(u => u.Id == user.Id);
            if (existingUser != null) return;
            _DbContext.Users.Add(user); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
        }

        public void CreateStaff(Staff staff)
        {
            var existingStaff = _DbContext.Staffs.First(s => s.Id == staff.Id);
            if (existingStaff != null) return;
           _DbContext.Staffs.Add(staff); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
     
        }
        public void CreateCustomer(Customer customer)
        {
            var existingCustomer = _DbContext.Customers.First(c => c.Id == customer.Id);
            if (existingCustomer != null) return;
           _DbContext.Customers.Add(customer); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
     
        }
        public void UpdateUser(User user)
        {
            var existingUser = _DbContext.Users.First(u => u.Id == user.Id);
            if (existingUser == null) return;
            _DbContext.Users.Update(user); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
        }

        public void UpdateStaff(Staff staff)
        {
           var existingStaff = _DbContext.Staffs.First(s => s.Id == staff.Id);
            if (existingStaff == null) return;
           _DbContext.Staffs.Update(staff); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
     
        }
        public void UpdateCustomer(Customer customer)
        {
           
            var existingCustomer = _DbContext.Customers.First(c => c.Id == customer.Id);
            if (existingCustomer == null) return;
           _DbContext.Customers.Update(customer); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
        }
        public void DeleteUser(User user)
        {
            var existingUser = _DbContext.Users.First(u => u.Id == user.Id);
            if (existingUser!= null)
            {
                _DbContext.Users.Remove(existingUser);
                _DbContext.SaveChanges();
            }   
        }

        public void DeleteStaff(Staff staff)
        {
           var existingStaff = _DbContext.Staffs.First(s => s.Id == staff.Id);
            if (existingStaff != null)
            {
                _DbContext.Staffs.Remove(existingStaff);
                _DbContext.SaveChanges();
            }
     
        }
        public void DeleteCustomer(Customer customer)
        {
           var existingCustomer = _DbContext.Customers.First(c => c.Id == customer.Id);
            if (existingCustomer != null)
            {
                _DbContext.Customers.Remove(existingCustomer);
                _DbContext.SaveChanges();
            }
     
        }
        
    }
