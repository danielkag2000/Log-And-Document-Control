import socket

HOST = '127.0.0.1'  # Standard loopback interface address (localhost)
PORT = 52616        # Port to listen on (non-privileged ports are > 1023)

str_xml_decleare = b"<Request><ProgramID>1</ProgramID><SubProgramID>1</SubProgramID><Function><FunctionID>1</FunctionID><Param>hello world</Param></Function></Request>"

str_xml_subscribe = b"<Request><ProgramID>1</ProgramID><SubProgramID>1</SubProgramID><Function><FunctionID>2</FunctionID><Param>127.0.0.1</Param><Param>5005</Param><Param>1</Param></Function></Request>"

#str_xml = b"<Request><ProgramID>1</ProgramID><SubProgramID>1</SubProgramID><Function><FunctionID>5</FunctionID><Param>4232</Param><Param>1</Param></Function></Request>"

#str_xml = b"<Request><ProgramID>1</ProgramID><SubProgramID>1</SubProgramID><Function><FunctionID>4</FunctionID><Param>1</Param></Function></Request>"

#str_xml = b"<Request><ProgramID>1</ProgramID><SubProgramID>1</SubProgramID><Function><FunctionID>7</FunctionID><Param>4232</Param><Param>1</Param></Function></Request>"


# ----------------------------------------------------------#

# send simple log function
with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.connect((HOST, PORT))
    s.sendall(str_xml_decleare)
    data = s.recv(4024)

print('Received', repr(data))


# use subscribe function
with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.connect((HOST, PORT))
    s.sendall(str_xml_subscribe)
    data = s.recv(4024)

print('Received', repr(data))
