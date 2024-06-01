-- this query gives users who are in chats and the related metrics
with user_communications as (
select u.id, trim(u.firstname) as firstname, trim(u.lastname) as lastname,
       (count(c.id) - count(g.id)) as chat_count,
       count(g.id) as group_count,
       (u.customerid is not null) as is_customer,
       (u.staffid is not null) as is_staff
      

from users u join chat_participants cp on u.id = cp.user_id join 
    chat c on cp.chat_id = c.id left join groups g on c.groupid = g.id group by u.id) 

select * from user_communications;
;

-- this query gives data on users who created groups
select u.id, u.firstname, u.lastname,
       count(g.id) as groups_created,
       g.type,
       count(cp.*) as number_of_participants,
       (u.customerid is not null) as is_customer,
       (u.staffid is not null) as is_staff
from groups g join users u on g.creatorid = u.id join chat c on g.id = c.groupid 
join chat_participants cp on c.id = cp.chat_id group by g.type, u.id; 


-- this query can be used to determine number of chats a customer/staff is involved in

select c.id, concat(trim(us.firstname), ' ', trim(us.lastname)) as staff, us.id as staff_user_id , (select concat(trim(firstname), ' ', trim(lastname)) from users u join chat_participants 
    cp on u.id = cp.user_id and cp.chat_id = c.id and u.staffid is null limit 1) as customer,
       (select u.id from users u join chat_participants
           cp on u.id = cp.user_id and cp.chat_id = c.id and u.staffid is null limit 1) as customer_user_id,
    c.date_connected
    
from chat c 
         join chat_participants cps on c.id = cps.chat_id join users us on cps.user_id = us.id and us.staffid is not null 
where c.isgroup is false;