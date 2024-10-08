USE RemoteConfigurationDatabase;
GO

CREATE NONCLUSTERED INDEX fix_Users_Active
    ON RemoteConfigurationSchema.Users (active)
    INCLUDE (Email, FirstName, LastName) --Also Includes UserId because it is our clustered Index 
    WHERE active = 1;
