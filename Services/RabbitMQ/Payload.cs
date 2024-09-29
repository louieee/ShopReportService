using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ReportApp.Data.Services;

public class ReportPayload
{
    [JsonProperty("action")]
    public string Action { get; set; }
    
    [JsonProperty("data_type")]
    public string DataType { get; set; }
    
    [JsonProperty("data")]
    public string Data { get; set; }
    
}

public class UserPayload{

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("first_name")]
    public string FirstName { get; set; }

    [JsonProperty("last_name")]
    public string LastName { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("is_customer")]
    public bool IsCustomer { get; set; }

    [JsonProperty("is_admin")]
    public bool IsAdmin { get; set; }

    [JsonProperty("is_staff")]
    public bool IsStaff { get; set; }

    [JsonProperty("display_name")]
    public string DisplayName { get; set; }

    [JsonProperty("profile_pic")]
    public string ProfilePic { get; set; }

    [JsonProperty("gender")]
    public string Gender { get; set; }

    [JsonProperty("date_of_birth")]
    public string DateOfBirth { get; set; }

    [JsonProperty("customer_id")]
    public int? CustomerId { get; set; }

    [JsonProperty("staff_id")]
    public int? StaffId { get; set; }

    [JsonProperty("admin_id")]
    public int? AdminId { get; set; }
}

class ChatPayload {

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("is_group")]
    public bool IsGroup { get; set; }

    [JsonProperty("participants")]
    public List<int> Participants { get; set; }

    [JsonProperty("group_id")]
    public string? GroupId { get; set; }

    [JsonProperty("group_name")]
    public string? GroupName { get; set; }

    [JsonProperty("group_type")]
    public string? GroupType { get; set; }

    [JsonProperty("group_creator")]
    public int? GroupCreator { get; set;}

}


class ContactPayload{
    [JsonProperty("id")]
    public int Id {get; set;}

    [JsonProperty("ownerId")]
    public int OwnerId {get; set;}

    [JsonProperty("name")]
    public string Name {get; set;}

    
}

class LeadPayload{
    [JsonProperty("id")]
    public int Id {get; set;}

    [JsonProperty("title")]
    public string Title {get; set;}

    [JsonProperty("contact_id")]
    public int ContactId {get; set;}

    [JsonProperty("owner_id")]
    public int OwnerId {get; set;}

    [JsonProperty("source")]
    public string Source {get; set;}

    [JsonProperty("nurturing_status")]
    public string NurturingStatus {get; set;}

    [JsonProperty("is_deal")]
    public bool IsDeal {get; set;}

    [JsonProperty("company")]
    public string Company {get; set;}

    [JsonProperty("conversion_date")]
    public DateTime? ConversionDate {get; set;}


}

class OrderPayload{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("product_id")]
    public int ProductId { get; set; }

    [JsonProperty("sale_id")]
    public int SaleId { get; set; }

    [JsonProperty("staff_id")]
    public int? StaffId { get; set; }

    [JsonProperty("quantity")]
    public int Quantity { get; set; }

    [JsonProperty("delivered")]
    public bool Delivered { get; set; }

    [JsonProperty("date_delivered")]
    public DateTime? DateDelivered { get; set; }

}

class ProductPayload{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("brand")]

    public string Brand { get; set; }

    [JsonProperty("inventory")]

    public string Inventory { get; set; }

    [JsonProperty("price")]
    public float Price { get; set; }

    [JsonProperty("quantity")]
    public int Quantity { get; set; }
}

class SalePayload{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("paid")]
    public bool Paid { get; set; }

    [JsonProperty("customer_id")]
    public int CustomerId { get; set; }

    [JsonProperty("date_ordered")]
    public DateTime DateOrdered { get; set; }

    [JsonProperty("date_paid")]
    public DateTime? DatePaid { get; set; }

    [JsonProperty("location")]
    public string Location { get; set; }
}

/*

user payload
{
  "action": "user_data",
  "data_type": "user",
  "data": {
    "id": 12345,
    "first_name": "John",
    "last_name": "Doe",
    "email": "john.doe@example.com",
    "is_customer": true,
    "is_admin": false,
    "is_staff": false,
    "display_name": "John Doe",
    "profile_pic": "https://example.com/profile.jpg",
    "gender": "male",
    "date_of_birth": "1990-01-01",
    "customer_id": 123,
    "staff_id": null,
    "admin_id": null
  }
}

chat payload

{
  "action": "chat_data",
  "data_type": "chat",
  "data": {
    "id": "uuid1234567890abcdef",
    "is_group": true,
    "participants": [1, 2, 3],
    "group_id": "uuid1234567890abcdef",
    "group_name": "My Group",
    "group_type": "private",
    "group_creator": 1
  }
}

contact payload
{
  "action": "contact_data",
  "data_type": "contact",
  "data": {
    "id": 10,
    "owner_id": 5,
    "name": "John Smith",
    "email": "john.smith@example.com",
    "phone_number": "+1234567890",
    "address": "123 Main Street, City, State"
  }
}

lead payload
{
  "action": "lead_data",
  "data_type": "lead",
  "data": {
    "id": 20,
    "title": "New Lead",
    "contact_id": 10,
    "owner_id": 5,
    "source": "website",
    "nurturing_status": "new",
    "is_deal": false,
    "company": "Acme Inc.",
    "conversion_date": null
  }
}

order payload

{
  "action": "order_data",
  "data_type": "order",
  "data": {
    "id": 50,
    "product_id": 25,
    "sale_id": 30,
    "staff_id": 2,
    "quantity": 2,
    "delivered": true,
    "date_delivered": "2024-09-30T00:00:00Z",
    "order_status": "pending",
    "total_amount": 100.00
  }
}

product payload

{
  "action": "product_data",
  "data_type": "product",
  "data": {
    "id": 25,
    "name": "T-Shirt",
    "brand": "Acme",
    "inventory": "in_stock",
    "price": 19.99,
    "quantity": 100,
    "description": "A comfortable and stylish t-shirt",
    "category": "apparel"
  }
}

sale payload

{
  "action": "sale_data",
  "data_type": "sale",
  "data": {
    "id": 30,
    "paid": true,
    "customer_id": 1,
    "date_ordered": "2024-09-28T10:00:00Z",
    "date_paid": "2024-09-28T10:30:00Z",
    "location": "New York",
    "discount_applied": false,
    "discount_amount": 0.00
  }
}




*/