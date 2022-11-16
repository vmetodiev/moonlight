# MoonLight
A handy hacking tool written in a .NET 1.0-compatible code style for accessing MSSQL databases over the network via the DB connection strings. Once connected to the database, the user is allowed to execute native SQL queries.

# Build Instructions
Find the oldest possible VisualStudio and compile it with the oldest possible .NET version.

# Example usage

## Access the prompt
![Screenshot](https://github.com/vmetodiev/moonlight/blob/master/Pictures/ML1.PNG)<br />
![Screenshot](https://github.com/vmetodiev/moonlight/blob/master/Pictures/ML0.PNG)<br />
## Show the connection strings (added before compiling)
![Screenshot](https://github.com/vmetodiev/moonlight/blob/master/Pictures/ML2.PNG)<br />
## Test connection to the database
![Screenshot](https://github.com/vmetodiev/moonlight/blob/master/Pictures/ML3.PNG)<br />
## Enter the connection and execute pure SQL
![Screenshot](https://github.com/vmetodiev/moonlight/blob/master/Pictures/ML4.PNG)<br />
## Add a new connection string (to a newly found DB, for example) 
![Screenshot](https://github.com/vmetodiev/moonlight/blob/master/Pictures/ML5.PNG)<br />

# Microsoft SQL cheatsheet
// Show databases<br />
SELECT * FROM sys.databases;<br />

// Show databases, filtered<br />
SELECT database_id, name FROM sys.databases;<br />

// Show tables<br />
SELECT * FROM information_schema.tables;<br />

// Describe table (column)<br />
select * FROM information_schema.columns WHERE TABLE_NAME='People' ORDER BY ORDINAL_POSITION;<br />
SELECT COLUMN_NAME, DATA_TYPE FROM information_schema.columns WHERE TABLE_NAME='People' ORDER BY ORDINAL_POSITION;<br />

// Get all databases by searching the "master" (other may also work, not only the master)<br />
SELECT database_id, name FROM sys.databases;<br />

// Get all users (principals)<br />
SELECT name from master.sys.server_principals;<br />

// Microsoft docs<br />
https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/ado-net-code-examples<br />

