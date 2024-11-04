-- Create the database if it does not already exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'OT_Assessment_DB')
BEGIN
    CREATE DATABASE OT_Assessment_DB;
END
GO

-- Switch to the new database
USE OT_Assessment_DB;
GO

-- Create the Players table
CREATE TABLE Players (
    accountId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    username NVARCHAR(50) NOT NULL,
    email NVARCHAR(100) NOT NULL,
    createdDate DATETIME2 NOT NULL
);

-- Create the Providers table
CREATE TABLE Providers (
    providerId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    name NVARCHAR(100) NOT NULL
);

-- Create the TransactionTypes table
CREATE TABLE TransactionTypes (
    transactionTypeId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    name NVARCHAR(50) NOT NULL
);

-- Create the Games table
CREATE TABLE Games (
    gameId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    theme NVARCHAR(50) NULL,
    ProviderId UNIQUEIDENTIFIER NULL,
    CONSTRAINT FK_Games_Providers_ProviderId FOREIGN KEY (ProviderId) REFERENCES Providers(providerId)
);

-- Create the CasinoWagers table
CREATE TABLE CasinoWagers (
    wagerId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    GameId UNIQUEIDENTIFIER NULL,
    ProviderId UNIQUEIDENTIFIER NULL,
    AccountId UNIQUEIDENTIFIER NULL,
    externalReferenceId UNIQUEIDENTIFIER NULL,
    TransactionTypeId UNIQUEIDENTIFIER NULL,
    transactionId UNIQUEIDENTIFIER NULL,
    brandId UNIQUEIDENTIFIER NULL,
    amount FLOAT NULL,
    username NVARCHAR(MAX) NULL,
    createdDateTime DATETIME2 NULL,
    numberOfBets INT NULL,
    countryCode NVARCHAR(MAX) NULL,
    sessionData NVARCHAR(MAX) NULL,
    duration BIGINT NULL,
    CONSTRAINT FK_CasinoWagers_Games_GameId FOREIGN KEY (GameId) REFERENCES Games(gameId),
    CONSTRAINT FK_CasinoWagers_Players_AccountId FOREIGN KEY (AccountId) REFERENCES Players(accountId),
    CONSTRAINT FK_CasinoWagers_Providers_ProviderId FOREIGN KEY (ProviderId) REFERENCES Providers(providerId),
    CONSTRAINT FK_CasinoWagers_TransactionTypes_TransactionTypeId FOREIGN KEY (TransactionTypeId) REFERENCES TransactionTypes(transactionTypeId)
);

-- Create indexes for the CasinoWagers table
CREATE INDEX IX_CasinoWagers_AccountId ON CasinoWagers (AccountId);
CREATE INDEX IX_CasinoWagers_GameId ON CasinoWagers (GameId);
CREATE INDEX IX_CasinoWagers_ProviderId ON CasinoWagers (ProviderId);
CREATE INDEX IX_CasinoWagers_TransactionTypeId ON CasinoWagers (TransactionTypeId);

-- Create index for the Games table
CREATE INDEX IX_Games_ProviderId ON Games (ProviderId);
