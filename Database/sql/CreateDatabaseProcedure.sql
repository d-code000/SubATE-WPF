CREATE PROCEDURE CreateDatabase
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'БазаАбонентов')
        BEGIN
            CREATE DATABASE БазаАбонентов;
        END
END;