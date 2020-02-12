-- subProgramCodes inserts
insert into subProgramCodes values(1, 1, 'ocr', 'do ocr');
insert into subProgramCodes values(2, 1, 'search file', 'search file in the web');
insert into subProgramCodes values(1, 2, 'create DB', 'do creation of the DB');

-- programCodes inserts
insert into programCodes values(1, 'crowler', 'search for documents in the web');
insert into programCodes values(2, 'DB', 'do DB');

-- functionCodes inserts
insert into functionCodes values('Declare', 'create a log about somthing');
insert into functionCodes values('Subscribe', 'subscribe for documents updates');
insert into functionCodes values('Unsubscribe', 'unsubscribe for documents updates');
insert into functionCodes values('GetDocument', 'get the document from the queue');
insert into functionCodes values('AddDocPipe', 'add the sub configration pipe of a document');
insert into functionCodes values('ResetDocPipe', 'reset the sub configration pipe for the document');
insert into functionCodes values('FinishWithDoc', 'declare that the process finished with the document');
insert into functionCodes values('DeleteDocPipe', 'delete a document in a spesefic pipe');
insert into functionCodes values('FinishWithDoc', 'declare that the process finished with the document');
insert into functionCodes values('DeleteDocPipe', 'delete a document in a spesefic pipe');
insert into functionCodes values('ShowProcessQueue', 'show the processes that are waiting in the queue');
insert into functionCodes values('GetDocumentById', 'delete a document in a spesefic pipe');

-- pipeCode inserts
insert into pipeCode values('1,3,2');
insert into pipeCode values('1,2');


