INSERT INTO ASPNETUSERROLES (UserId, RoleId)
VALUES
    (
        (SELECT Id FROM ASPNETUSERS WHERE Email = 'me@example.com'),
        (SELECT Id FROM ASPNETROLES WHERE Name = 'PropertyManager')
    );