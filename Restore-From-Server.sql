﻿use [master]

drop database [aspnet-SJAData-dc4d77b6-0cc8-46ff-8f63-284d05abbca7];

RESTORE DATABASE [aspnet-SJAData-dc4d77b6-0cc8-46ff-8f63-284d05abbca7]
FROM DISK = 'C:\Users\trich\Downloads\dashboard2\dashboard2.bak'
WITH MOVE 'dashboard2_data' TO 'C:\Users\trich\aspnet-SJAData-dc4d77b6-0cc8-46ff-8f63-284d05abbca7.mdf',
     MOVE 'dashboard2_log' TO 'C:\Users\trich\aspnet-SJAData-dc4d77b6-0cc8-46ff-8f63-284d05abbca7_log.ldf',
     REPLACE;

exec sp_MSforeachtable "ALTER SCHEMA dbo TRANSFER ? PRINT '? modified' "
