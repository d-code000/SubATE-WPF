CREATE PROCEDURE CreateSubscriberTable
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'Абонент')
        BEGIN
            CREATE TABLE Абонент (
                 Id INT IDENTITY (1, 1) PRIMARY KEY,
                 Имя NVARCHAR(100) NOT NULL,
                 Телефон NVARCHAR(16),
                 Премиум BIT,
                 Тип NVARCHAR(50),
                 ДатаРегистрации DATE
            );
        END
END;