---Report SQL for leads report
select  extract(year from l.datecreated) as year,
        count(*) as lead_count from lead l group by year;

select  extract(month from l.datecreated) as month,
        extract(year from l.datecreated) as year,
        count(*) as lead_count from lead l where extract(year from l.datecreated) = extract(year from current_date)
                   group by month, year;

select  date(l.datecreated) as date_created,
        count(*) as lead_count from lead l where extract(year from l.datecreated) = extract(year from current_date) and
    extract(month from l.datecreated) = extract(month from current_date)
                                                 
       group by date_created;

select  date(l.datecreated) as date_created,
        extract(hour from l.datecreated) as hour_created,
        count(*) as lead_count from lead l  where l.datecreated = current_date
       group by date_created, hour_created;

select current_date;

---Report SQL for contacts report
select  extract(year from c.datecreated) as year,
        count(*) as contact_count from contact c group by year;

select  extract(month from c.datecreated) as month,
        extract(year from c.datecreated) as year,
        count(*) as contact_count from contact c group by month, year;

select  date(c.datecreated) as date_created,
        count(*) as contact_count from contact c group by date_created;

select  date(c.datecreated) as date_created,
        extract(hour from c.datecreated) as hour_created,
        count(*) as contact_count from contact c group by date_created, hour_created;


---Report SQL for leads converted report
select  extract(year from l.conversiondate) as year,
        count(*) as lead_count from lead  l where l.isdeal group by year;

with month_year as (select  distinct  extract(month from conversiondate) as month,
                extract(year from conversiondate) as year
                from lead where conversiondate is not null and lead.isdeal)
select year, month, (select count(id) from lead l where extract(year from l.conversiondate ) = year 
   and month = extract(month from l.conversiondate ) and l.isdeal and l.conversiondate is not null) as lead_count  
from month_year where year = extract(year from current_date);

with lead_date as (select  distinct  date(conversiondate) as date_converted
                    from lead where conversiondate is not null and lead.isdeal and
                                    extract(month from conversiondate) = extract(month from current_date) and
                       extract(year from conversiondate) = extract(year from current_date))
select date_converted, (select count(id) from lead l where date(l.conversiondate ) = date_converted
                                                    and l.isdeal and l.conversiondate is not null) as lead_count
from lead_date;


with lead_date_hr as (select  distinct  date(conversiondate) as date_converted,
                     (extract(hour from conversiondate)) as hour_converted        
                   from lead where conversiondate is not null and lead.isdeal
                             and conversiondate = current_date)
select date_converted, hour_converted, (select count(id) from lead l where date(l.conversiondate ) = date_converted
                                                        and extract(hour from l.conversiondate) = hour_converted and
                                                                          l.isdeal and l.conversiondate is not null) as lead_count
from lead_date_hr;


-- query staffs and leads gotten and corresponding lead attributes

select u.id, concat(u.firstname,' ', u.lastname) as staff, u.staffid, u.gender, count(l.id) as leadCount from users u left join lead l on u.id = l.ownerid
where u.staffid is not null group by u.id;


-- query staffs and contacts gotten and corresponding contacts attributes

select u.id, concat(u.firstname,' ', u.lastname) as staff, u.staffid, u.gender, count(c.id) as contact_count from users u left join contact c on u.id = c.ownerid
where u.staffid is not null group by u.id;


-- query lead to view lead age
select l.id, l.title, l.company, c.name as lead_contact, c.id as lead_contact_id,   u.staffid, concat(trim(u.firstname), ' ', trim(u.lastname)) as staff, u.gender as staff_gender, extract(days from (current_timestamp - l.datecreated)) as lead_age from  lead l join users u on l.ownerid = u.id join contact c on l.contactid = c.id where isdeal is false and conversiondate is null and
                                                                                                                               u.staffid is not null ;
