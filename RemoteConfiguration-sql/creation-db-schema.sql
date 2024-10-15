CREATE DATABASE RemoteConfigurationDatabase;
GO

USE RemoteConfigurationDatabase;
GO

CREATE SCHEMA RemoteConfigurationSchema;
GO

DROP TABLE IF EXISTS RemoteConfigurationSchema.Users
CREATE TABLE RemoteConfigurationSchema.Users
(
    UserId INT IDENTITY(1, 1) PRIMARY KEY
	, Email NVARCHAR(50)
	, ApiKey UNIQUEIDENTIFIER
    , FirstName NVARCHAR(50)
    , LastName NVARCHAR(50)
    , Active BIT
);

DROP TABLE IF EXISTS RemoteConfigurationSchema.Auth
CREATE TABLE RemoteConfigurationSchema.Auth
(
	Email NVARCHAR(50) PRIMARY KEY,
	PasswordHash VARBINARY(MAX),
	PasswordSalt VARBINARY(MAX)
)

DROP TABLE IF EXISTS RemoteConfigurationSchema.Configuration
CREATE TABLE RemoteConfigurationSchema.Configuration
(
	Id INT IDENTITY(1, 1) PRIMARY KEY
	, ApiKey UNIQUEIDENTIFIER
	, KeyIdentifier NVARCHAR(50)
	, ConfigData NVARCHAR(MAX)
)