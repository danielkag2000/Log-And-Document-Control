
create table subProgramCodes(id int, program_id int, name char(30), description char(200), PRIMARY KEY(id));

insert into subProgramCodes values(1, 1, "ocr", "do ocr");

insert into subProgramCodes values(2, 1, "search file", "search file in the web");

insert into subProgramCodes values(1, 2, "create DB", "do creation of the DB");

select name, description from functionCode;