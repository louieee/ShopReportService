namespace ReportApp.Data.Services;

using ReportApp.Data.Models.communication;
using ReportApp.Data.Models.crm;
using ReportService.Models;

public class CRMRepository
    {
        private readonly DataContext _DbContext;
        
        public CRMRepository(DataContext context)
        {
            _DbContext = context;
            
        }

        public void CreateContact(Contact contact){
            var existingContact = _DbContext.Contacts.First(c => c.Id == contact.Id);
            if (existingContact != null) return;
            _DbContext.Contacts.Add(contact); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
        }
        public void UpdateContact(Contact contact){
            var existingContact = _DbContext.Contacts.First(c => c.Id == contact.Id);
            if (existingContact == null) return;
            _DbContext.Contacts.Update(contact); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
        }
        public void DeleteContact(Contact contact){
            var existingContact = _DbContext.Contacts.First(c => c.Id == contact.Id);
            if (existingContact!= null)
            {
                _DbContext.Contacts.Remove(existingContact);
                _DbContext.SaveChanges();
            }
        }
        public void CreateLead(Lead lead){
            var existingLead = _DbContext.Leads.First(l => l.Id == lead.Id);
            if (existingLead != null) return;
            _DbContext.Leads.Add(lead); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
        }
        public void UpdateLead(Lead lead){
            var existingLead = _DbContext.Leads.First(l => l.Id == lead.Id);
            if (existingLead == null) return;
            _DbContext.Leads.Update(lead); // Add the new instance to the context
            _DbContext.SaveChanges(); // Save changes to the database
        }
        public void DeleteLead(Lead lead){
            var existingLead = _DbContext.Leads.First(l => l.Id == lead.Id);
            if (existingLead!= null)
            {
                _DbContext.Leads.Remove(existingLead);
                _DbContext.SaveChanges();
            }
        }
    }
