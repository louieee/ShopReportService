-- Report SQL for customer report
select  extract(year from c.DateJoined) as year,
        count(*) as customer_count from customer c join users u on c.id = u.customerid group by year;

select  extract(month from c.DateJoined) as month,  
        extract(year from c.DateJoined) as year,
        count(*) as customer_count from customer c join users u on c.id = u.customerid 
                                   where extract(year from c.DateJoined) = extract(year from current_date)
                                   group by month, year;

select  date(c.datejoined) as date_joined,
    count(*) as customer_count from customer c join users u on c.id = u.customerid
where extract(year from c.DateJoined) = extract(year from current_date) and extract(month from c.DateJoined) = extract(month from current_date)
                               group by date_joined;

select  date(c.datejoined) as date_joined,
    extract(hour from c.datejoined) as hour_joined,
    count(*) as customer_count from customer c join users u on c.id = u.customerid                      
                               group by date_joined, hour_joined
;

---Report SQL for staff report
select  extract(year from c.DateJoined) as year,
        count(*) as staff_count from staff c join users u on c.id = u.staffid group by year;

select  extract(month from c.DateJoined) as month,
        extract(year from c.DateJoined) as year,
        count(*) as staff_count from staff c join users u on c.id = u.staffid
where extract(year from c.DateJoined) = extract(year from current_date)
                                group by month, year;

select  date(c.datejoined) as date_joined,
    count(*) as staff_count from staff c join users u on c.id = u.staffid
where extract(year from c.DateJoined) = extract(year from current_date) and extract(month from c.DateJoined) = extract(month from current_date)
                            group by date_joined;

select  date(c.datejoined) as date_joined,
    extract(hour from c.datejoined) as hour_joined,
    count(*) as staff_count from staff c join users u on c.id = u.staffid
                            where c.datejoined >= current_date
                            group by date_joined, hour_joined;

