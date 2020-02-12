CREATE TABLE flowTable (doc_id INT NOT NULL, pipe_id INT NOT NULL, cprocess INT NOT NULL, pipe_index INT NOT NULL, start_time DATETIME NOT NULL, avaliable INT NOT NULL, cround INT NOT NULL, PRIMARY KEY(doc_id, pipe_id));
CREATE TABLE eventTable (event_id INT PRIMARY KEY IDENTITY (1,1), event_date DATETIME NOT NULL, event_type INT NOT NULL, program_id INT NOT NULL, sub_program_id INT NOT NULL, description CHAR(200) NOT NULL);
CREATE TABLE functionCodes(id INT PRIMARY KEY IDENTITY (1,1), func_name char(30) NOT NULL, func_description char(200) NOT NULL);
CREATE TABLE pipeCode(id INT PRIMARY KEY IDENTITY (1,1), flow char(100) NOT NULL);
CREATE TABLE programCodes(id int, prog_name char(30) NOT NULL, prog_description char(200) NOT NULL, PRIMARY KEY(id));
CREATE TABLE subProgramCodes(id int, program_id int, prog_name char(30) NOT NULL, prog_description char(200) NOT NULL, PRIMARY KEY(id, program_id));

