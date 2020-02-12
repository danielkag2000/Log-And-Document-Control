
CREATE TABLE functionCodes(id int, name char(30), description char(200), PRIMARY KEY(id));

insert into functionCodes values(1, "declare", "create a log about somthing");

select name, description from functionCodes;