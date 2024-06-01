-- other data can be extracted from here concerning product sales

select extract(year from s.datepaid) as year_sold,
       count(*) as products_sold
from product p join orders o on p.id = o.productid join sale s on o.saleid = s.id and s.paid = true
 group by year_sold;

select s.datepaid as date_sold, extract(hour from s.datepaid) as hour_sold,
       count(*) as products_sold
from product p join orders o on p.id = o.productid join sale s on o.saleid = s.id and s.paid = true
and s.datepaid = current_date
group by s.datepaid;

-- this can be used to query products delivered by staff queried by any duration

select (concat(trim(u.firstname), ' ', trim(u.lastname))) as staff,
       count(p.id) as products_delivered
from product p join orders o on p.id = o.productid and o.delivered = true join sale s on o.saleid = s.id and s.paid = true
               join users u on o.staffid = u.staffid
where datedelivered <= '2025-02-01'
group by staff;

--  this view can be used for the product demography reports
create view orders_view as
select      o.id,
            o.staffid,
        concat(trim(u.firstname), ' ', trim(u.lastname)) as customer,
        (extract(year from (current_date))-extract(year from u.date_of_birth)) as age,
       u.gender as gender,
       s.location as location,
       p.id as product_id,
       p.name as product,
            o.quantity,
       o.delivered as delivered,
            s.paid as paid,
        o.datedelivered,
        s.datepaid
    
from product p join orders o on p.id = o.productid  join sale s on o.saleid = s.id join users u on s.customerid = u.customerid
order by o.id;

select * from product_customer_view;

select orders_view.product,  count(*) as  products_ordered, sum(quantity) as total_quantity  from orders_view 
                 where gender ilike 'female%'  
                 group by product ;

select gender, product, count(*) as  products_bought from product_customer_view group by gender, product;

select location, product, count(*) as  products_bought from product_customer_view group by location, product;

select location, count(*) as  products_bought from product_customer_view group by location, product;




create view product_speed_report as
with product_speed_data as (
select p.id as product_id, p.name, p.quantity, extract(days from (current_timestamp - p.date_added)) as age ,
sum(o.quantity) as qty_sold, round(sum(o.quantity)/cast(p.quantity as numeric), 2) as sell_through_ratio
from product p join orders o on p.id = o.productid and o.delivered
    group by p.id, o.quantity)

select *, round(qty_sold/age) as product_sold_per_day, round(qty_sold/(age/qty_sold), 2) as sale_speed from product_speed_data order by sale_speed desc;
;
select * from product_speed_report;
