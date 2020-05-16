
REM Example test script to startup a server and a number of clients

start /B .\Poker\bin\Debug\Poker.exe -n Server -h 2015

start /B .\Poker\bin\Debug\Poker.exe -n Client1 -c 127.0.0.1:2015

start /B .\Poker\bin\Debug\Poker.exe -n Client2 -c 127.0.0.1:2015