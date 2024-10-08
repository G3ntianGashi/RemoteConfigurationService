USE RemoteConfigurationDatabase;
GO

CREATE OR ALTER PROCEDURE RemoteConfigurationSchema.spUsers_Get
    @UserId INT = NULL ,
    @Active BIT = NULL
AS
BEGIN
    SELECT [Users].[UserId],
        [Users].[FirstName],
        [Users].[LastName],
        [Users].[Email],
        [Users].[Active]
    FROM RemoteConfigurationSchema.Users AS Users 
        WHERE Users.UserId = ISNULL(@UserId, Users.UserId)
            AND ISNULL(Users.Active, 0) = COALESCE(@Active, Users.Active, 0)
END
GO

CREATE OR ALTER PROCEDURE RemoteConfigurationSchema.spUser_UpdateEmail
    @OldEmail NVARCHAR(50),
    @NewEmail NVARCHAR(50)
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM RemoteConfigurationSchema.Auth WHERE Email = @NewEmail)
    BEGIN
        UPDATE RemoteConfigurationSchema.Users 
            SET Email = @NewEmail
            WHERE Email = @OldEmail

        UPDATE RemoteConfigurationSchema.Auth 
            SET Email = @NewEmail
            WHERE Email = @OldEmail
    END   
END
GO

CREATE OR ALTER PROCEDURE RemoteConfigurationSchema.spUser_EditUser
    @FirstName NVARCHAR(50),
	@LastName NVARCHAR(50),
    @Active BIT = 1,
    @UserId INT = NULL
AS
BEGIN
    IF EXISTS (SELECT * FROM RemoteConfigurationSchema.Users WHERE UserId = @UserId)
    BEGIN
        UPDATE RemoteConfigurationSchema.Users 
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    Active = @Active
                WHERE UserId = @UserId
    END
END
GO

CREATE OR ALTER PROCEDURE RemoteConfigurationSchema.spUser_Upsert
    @Email NVARCHAR(50),
    @FirstName NVARCHAR(50),
	@LastName NVARCHAR(50),
    @UserId INT = NULL,
    @Active BIT = 1
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM RemoteConfigurationSchema.Users WHERE UserId = @UserId)
        BEGIN
        IF NOT EXISTS (SELECT * FROM RemoteConfigurationSchema.Users WHERE Email = @Email)
            BEGIN
                DECLARE @OutputUserId INT

                INSERT INTO RemoteConfigurationSchema.Users(
                    [FirstName],
                    [LastName],
                    [Email],
                    [Active]
                ) VALUES (
                    @FirstName,
                    @LastName,
                    @Email,
                    @Active
                )

                SET @OutputUserId = @@IDENTITY
            END
        END
    ELSE 
        BEGIN
            UPDATE RemoteConfigurationSchema.Users 
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    Active = @Active
                WHERE UserId = @UserId
        END
END
GO

CREATE OR ALTER PROCEDURE RemoteConfigurationSchema.spUser_Delete
    @UserId INT
AS
BEGIN
    DECLARE @Email NVARCHAR(50);

    SELECT  @Email = Users.Email
      FROM  RemoteConfigurationSchema.Users
     WHERE  Users.UserId = @UserId;

    DELETE  FROM RemoteConfigurationSchema.Users
     WHERE  Users.UserId = @UserId;

    DELETE  FROM RemoteConfigurationSchema.Auth
     WHERE  Auth.Email = @Email;
END;
GO

CREATE OR ALTER PROCEDURE RemoteConfigurationSchema.spLoginConfirmation_Get
    @Email NVARCHAR(50)
AS
BEGIN
    SELECT [Auth].[PasswordHash],
        [Auth].[PasswordSalt] 
    FROM RemoteConfigurationSchema.Auth AS Auth 
        WHERE Auth.Email = @Email
END;
GO

CREATE OR ALTER PROCEDURE RemoteConfigurationSchema.spRegistration_Upsert
    @Email NVARCHAR(50),
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX)
AS 
BEGIN
    IF NOT EXISTS (SELECT * FROM RemoteConfigurationSchema.Auth WHERE Email = @Email)
        BEGIN
            INSERT INTO RemoteConfigurationSchema.Auth(
                [Email],
                [PasswordHash],
                [PasswordSalt]
            ) VALUES (
                @Email,
                @PasswordHash,
                @PasswordSalt
            )
        END
    ELSE
        BEGIN
            UPDATE RemoteConfigurationSchema.Auth 
                SET PasswordHash = @PasswordHash,
                    PasswordSalt = @PasswordSalt
                WHERE Email = @Email
        END
END
GO