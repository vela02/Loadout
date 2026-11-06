/* =====================================================================
   Loadout – seed podataka (igre, kategorije, ocjene, vijesti)
   Skripta je IDEMPOTENTNA: može se pokrenuti više puta, ne pravi duplikate
   (provjerava po nazivu igre / kategorije / vijesti).
   Slike: službene Steam header slike (cdn.cloudflare.steamstatic.com).
   Sistemski zahtjevi i cijene su okvirni podaci za testiranje.
   ===================================================================== */
SET NOCOUNT ON;
BEGIN TRANSACTION;

-- 1) KATEGORIJE

IF NOT EXISTS (SELECT 1 FROM GameCategory WHERE Name = 'Action')
    INSERT INTO GameCategory (Name, ParentCategoryId, IsEnabled, IsDeleted) VALUES ('Action', NULL, 1, 0);
IF NOT EXISTS (SELECT 1 FROM GameCategory WHERE Name = 'RPG')
    INSERT INTO GameCategory (Name, ParentCategoryId, IsEnabled, IsDeleted) VALUES ('RPG', NULL, 1, 0);
IF NOT EXISTS (SELECT 1 FROM GameCategory WHERE Name = 'Shooter')
    INSERT INTO GameCategory (Name, ParentCategoryId, IsEnabled, IsDeleted) VALUES ('Shooter', NULL, 1, 0);
IF NOT EXISTS (SELECT 1 FROM GameCategory WHERE Name = 'Adventure')
    INSERT INTO GameCategory (Name, ParentCategoryId, IsEnabled, IsDeleted) VALUES ('Adventure', NULL, 1, 0);
IF NOT EXISTS (SELECT 1 FROM GameCategory WHERE Name = 'Strategy')
    INSERT INTO GameCategory (Name, ParentCategoryId, IsEnabled, IsDeleted) VALUES ('Strategy', NULL, 1, 0);
IF NOT EXISTS (SELECT 1 FROM GameCategory WHERE Name = 'Simulation')
    INSERT INTO GameCategory (Name, ParentCategoryId, IsEnabled, IsDeleted) VALUES ('Simulation', NULL, 1, 0);
IF NOT EXISTS (SELECT 1 FROM GameCategory WHERE Name = 'Sports')
    INSERT INTO GameCategory (Name, ParentCategoryId, IsEnabled, IsDeleted) VALUES ('Sports', NULL, 1, 0);
IF NOT EXISTS (SELECT 1 FROM GameCategory WHERE Name = 'Racing')
    INSERT INTO GameCategory (Name, ParentCategoryId, IsEnabled, IsDeleted) VALUES ('Racing', NULL, 1, 0);
IF NOT EXISTS (SELECT 1 FROM GameCategory WHERE Name = 'Indie')
    INSERT INTO GameCategory (Name, ParentCategoryId, IsEnabled, IsDeleted) VALUES ('Indie', NULL, 1, 0);
IF NOT EXISTS (SELECT 1 FROM GameCategory WHERE Name = 'Horror')
    INSERT INTO GameCategory (Name, ParentCategoryId, IsEnabled, IsDeleted) VALUES ('Horror', NULL, 1, 0);

-- 2) IGRE (+ nasumične ocjene 3–5 zvjezdica za svaku novu igru)
DECLARE @gameId INT;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Cyberpunk 2077')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Cyberpunk 2077', 'Open-world action RPG set in Night City, a megalopolis obsessed with power, glamour and body modification.', 59.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/1091500/header.jpg', 'RPG', '2020-12-10', 'CD Projekt Red', 'CD Projekt',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'RPG'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 3, DATEADD(day, -115, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -72, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -53, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Elden Ring')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Elden Ring', 'Challenging action RPG set in the vast Lands Between, created in collaboration with George R. R. Martin.', 69.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/1245620/header.jpg', 'RPG', '2022-02-25', 'FromSoftware', 'Bandai Namco',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'RPG'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 3, DATEADD(day, -120, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -259, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -14, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -288, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -102, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -280, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -215, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Red Dead Redemption 2')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Red Dead Redemption 2', 'Epic tale of outlaw Arthur Morgan and the Van der Linde gang at the end of the Wild West era.', 59.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/1174180/header.jpg', 'Action', '2019-12-05', 'Rockstar Games', 'Rockstar Games',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Action'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 5, DATEADD(day, -82, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -217, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -175, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -143, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Grand Theft Auto V')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Grand Theft Auto V', 'Three very different criminals risk everything in a series of daring heists across Los Santos.', 29.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/271590/header.jpg', 'Action', '2015-04-14', 'Rockstar North', 'Rockstar Games',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Action'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i5-4460 / AMD Ryzen 3 1200
RAM: 8 GB
GPU: GTX 970 / Radeon RX 470
Storage: 60 GB

Recommended:
CPU: Intel Core i7-7700 / AMD Ryzen 5 3600
RAM: 16 GB
GPU: RTX 2060 / Radeon RX 5700');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -195, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -50, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -184, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -177, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Baldur''s Gate 3')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Baldur''s Gate 3', 'Story-rich party-based RPG set in the Dungeons & Dragons universe, where every choice matters.', 59.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/1086940/header.jpg', 'RPG', '2023-08-03', 'Larian Studios', 'Larian Studios',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'RPG'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -283, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -151, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -186, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -296, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -99, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -36, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -24, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Hades')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Hades', 'Rogue-like dungeon crawler: defy the god of the dead as you hack and slash out of the Underworld.', 24.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/1145360/header.jpg', 'Indie', '2020-09-17', 'Supergiant Games', 'Supergiant Games',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Indie'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i3 / AMD Ryzen 3
RAM: 4 GB
GPU: GTX 750 Ti / Radeon R7 360
Storage: 10 GB

Recommended:
CPU: Intel Core i5 / AMD Ryzen 5
RAM: 8 GB
GPU: GTX 1050 Ti');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -195, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -143, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -233, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -187, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Stardew Valley')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Stardew Valley', 'Inherit your grandfather''s old farm, grow crops, raise animals and become part of a small town community.', 14.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/413150/header.jpg', 'Simulation', '2016-02-26', 'ConcernedApe', 'ConcernedApe',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Simulation'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i3 / AMD Ryzen 3
RAM: 4 GB
GPU: GTX 750 Ti / Radeon R7 360
Storage: 10 GB

Recommended:
CPU: Intel Core i5 / AMD Ryzen 5
RAM: 8 GB
GPU: GTX 1050 Ti');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -37, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -88, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -274, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -126, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Hollow Knight')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Hollow Knight', 'Hand-drawn action adventure through a vast, ruined kingdom of insects and heroes.', 14.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/367520/header.jpg', 'Indie', '2017-02-24', 'Team Cherry', 'Team Cherry',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Indie'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i3 / AMD Ryzen 3
RAM: 4 GB
GPU: GTX 750 Ti / Radeon R7 360
Storage: 10 GB

Recommended:
CPU: Intel Core i5 / AMD Ryzen 5
RAM: 8 GB
GPU: GTX 1050 Ti');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 5, DATEADD(day, -113, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -167, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -29, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -118, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Terraria')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Terraria', 'Dig, fight, explore and build in a 2D sandbox world full of adventure.', 9.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/105600/header.jpg', 'Indie', '2011-05-16', 'Re-Logic', 'Re-Logic',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Indie'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i3 / AMD Ryzen 3
RAM: 4 GB
GPU: GTX 750 Ti / Radeon R7 360
Storage: 10 GB

Recommended:
CPU: Intel Core i5 / AMD Ryzen 5
RAM: 8 GB
GPU: GTX 1050 Ti');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -34, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -109, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -291, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Celeste')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Celeste', 'Precise platformer about climbing a mountain and the struggles you face along the way.', 19.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/504230/header.jpg', 'Indie', '2018-01-25', 'Maddy Makes Games', 'Maddy Makes Games',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Indie'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i3 / AMD Ryzen 3
RAM: 4 GB
GPU: GTX 750 Ti / Radeon R7 360
Storage: 10 GB

Recommended:
CPU: Intel Core i5 / AMD Ryzen 5
RAM: 8 GB
GPU: GTX 1050 Ti');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -136, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -72, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -127, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -288, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -276, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Portal 2')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Portal 2', 'Award-winning puzzle game with a single-player story and a separate co-op campaign.', 9.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/620/header.jpg', 'Adventure', '2011-04-18', 'Valve', 'Valve',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Adventure'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i3 / AMD Ryzen 3
RAM: 4 GB
GPU: GTX 750 Ti / Radeon R7 360
Storage: 10 GB

Recommended:
CPU: Intel Core i5 / AMD Ryzen 5
RAM: 8 GB
GPU: GTX 1050 Ti');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 5, DATEADD(day, -113, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -71, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -261, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -253, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -47, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Half-Life 2')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Half-Life 2', 'Classic first-person shooter: Gordon Freeman fights the alien Combine in City 17.', 9.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/220/header.jpg', 'Shooter', '2004-11-16', 'Valve', 'Valve',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Shooter'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i3 / AMD Ryzen 3
RAM: 4 GB
GPU: GTX 750 Ti / Radeon R7 360
Storage: 10 GB

Recommended:
CPU: Intel Core i5 / AMD Ryzen 5
RAM: 8 GB
GPU: GTX 1050 Ti');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 3, DATEADD(day, -217, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -33, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -198, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'DOOM Eternal')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('DOOM Eternal', 'Fast-paced FPS where you rip and tear through demon hordes across dimensions.', 39.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/782330/header.jpg', 'Shooter', '2020-03-20', 'id Software', 'Bethesda Softworks',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Shooter'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i5-4460 / AMD Ryzen 3 1200
RAM: 8 GB
GPU: GTX 970 / Radeon RX 470
Storage: 60 GB

Recommended:
CPU: Intel Core i7-7700 / AMD Ryzen 5 3600
RAM: 16 GB
GPU: RTX 2060 / Radeon RX 5700');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 5, DATEADD(day, -59, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -275, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -137, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -175, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -58, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -151, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Sekiro: Shadows Die Twice')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Sekiro: Shadows Die Twice', 'Carve your own path as a shinobi seeking revenge in a reimagined late 1500s Sengoku Japan.', 59.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/814380/header.jpg', 'Action', '2019-03-21', 'FromSoftware', 'Activision',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Action'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i5-4460 / AMD Ryzen 3 1200
RAM: 8 GB
GPU: GTX 970 / Radeon RX 470
Storage: 60 GB

Recommended:
CPU: Intel Core i7-7700 / AMD Ryzen 5 3600
RAM: 16 GB
GPU: RTX 2060 / Radeon RX 5700');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -260, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -55, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -153, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -260, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -102, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -79, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Dark Souls III')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Dark Souls III', 'Dark fantasy action RPG known for its punishing but rewarding combat.', 59.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/374320/header.jpg', 'RPG', '2016-04-11', 'FromSoftware', 'Bandai Namco',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'RPG'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i5-4460 / AMD Ryzen 3 1200
RAM: 8 GB
GPU: GTX 970 / Radeon RX 470
Storage: 60 GB

Recommended:
CPU: Intel Core i7-7700 / AMD Ryzen 5 3600
RAM: 16 GB
GPU: RTX 2060 / Radeon RX 5700');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -166, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -251, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -10, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -58, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -186, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Monster Hunter: World')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Monster Hunter: World', 'Hunt colossal monsters in a living ecosystem, alone or with up to three friends.', 29.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/582010/header.jpg', 'Action', '2018-08-09', 'Capcom', 'Capcom',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Action'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i5-4460 / AMD Ryzen 3 1200
RAM: 8 GB
GPU: GTX 970 / Radeon RX 470
Storage: 60 GB

Recommended:
CPU: Intel Core i7-7700 / AMD Ryzen 5 3600
RAM: 16 GB
GPU: RTX 2060 / Radeon RX 5700');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -44, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -249, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -36, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -273, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -65, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Forza Horizon 5')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Forza Horizon 5', 'Open-world racing across the vibrant landscapes of Mexico with hundreds of cars.', 59.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/1551360/header.jpg', 'Racing', '2021-11-09', 'Playground Games', 'Xbox Game Studios',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Racing'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 5, DATEADD(day, -271, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -217, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -109, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -277, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Resident Evil 4')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Resident Evil 4', 'Remake of the survival horror classic: Leon S. Kennedy on a mission to rescue the president''s daughter.', 39.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/2050650/header.jpg', 'Horror', '2023-03-24', 'Capcom', 'Capcom',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Horror'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -265, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -232, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -62, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -127, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'God of War')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('God of War', 'Kratos and his son Atreus journey through the realm of Norse gods and monsters.', 49.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/1593500/header.jpg', 'Action', '2022-01-14', 'Santa Monica Studio', 'PlayStation PC',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Action'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 3, DATEADD(day, -284, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -118, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -113, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -4, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Marvel''s Spider-Man Remastered')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Marvel''s Spider-Man Remastered', 'Swing through a vibrant open-world New York as an experienced Peter Parker.', 59.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/1817070/header.jpg', 'Action', '2022-08-12', 'Insomniac Games', 'PlayStation PC',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Action'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 3, DATEADD(day, -17, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -170, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -37, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Hogwarts Legacy')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Hogwarts Legacy', 'Open-world action RPG set in the wizarding world of the 1800s.', 59.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/990080/header.jpg', 'RPG', '2023-02-10', 'Avalanche Software', 'Warner Bros. Games',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'RPG'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -296, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -243, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -125, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -243, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -209, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -98, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -49, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Cities: Skylines')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Cities: Skylines', 'Modern city-building simulation: plan traffic, zoning, services and grow your city.', 27.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/255710/header.jpg', 'Simulation', '2015-03-10', 'Colossal Order', 'Paradox Interactive',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Simulation'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i3 / AMD Ryzen 3
RAM: 4 GB
GPU: GTX 750 Ti / Radeon R7 360
Storage: 10 GB

Recommended:
CPU: Intel Core i5 / AMD Ryzen 5
RAM: 8 GB
GPU: GTX 1050 Ti');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 5, DATEADD(day, -211, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -240, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -28, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Sid Meier''s Civilization VI')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Sid Meier''s Civilization VI', 'Turn-based strategy: build an empire to stand the test of time.', 59.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/289070/header.jpg', 'Strategy', '2016-10-21', 'Firaxis Games', '2K',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Strategy'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i5-4460 / AMD Ryzen 3 1200
RAM: 8 GB
GPU: GTX 970 / Radeon RX 470
Storage: 60 GB

Recommended:
CPU: Intel Core i7-7700 / AMD Ryzen 5 3600
RAM: 16 GB
GPU: RTX 2060 / Radeon RX 5700');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 3, DATEADD(day, -56, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -128, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -99, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Valheim')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Valheim', 'Brutal co-op survival and exploration game inspired by Viking culture.', 19.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/892970/header.jpg', 'Adventure', '2021-02-02', 'Iron Gate AB', 'Coffee Stain Publishing',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Adventure'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i3 / AMD Ryzen 3
RAM: 4 GB
GPU: GTX 750 Ti / Radeon R7 360
Storage: 10 GB

Recommended:
CPU: Intel Core i5 / AMD Ryzen 5
RAM: 8 GB
GPU: GTX 1050 Ti');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 5, DATEADD(day, -94, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -143, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -237, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -128, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Disco Elysium')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Disco Elysium', 'Groundbreaking detective RPG with a unique dialogue system and choices that matter.', 39.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/632470/header.jpg', 'RPG', '2019-10-15', 'ZA/UM', 'ZA/UM',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'RPG'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i3 / AMD Ryzen 3
RAM: 4 GB
GPU: GTX 750 Ti / Radeon R7 360
Storage: 10 GB

Recommended:
CPU: Intel Core i5 / AMD Ryzen 5
RAM: 8 GB
GPU: GTX 1050 Ti');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 5, DATEADD(day, -26, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -277, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -8, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Subnautica')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Subnautica', 'Underwater survival adventure on an alien ocean planet.', 29.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/264710/header.jpg', 'Adventure', '2018-01-23', 'Unknown Worlds', 'Unknown Worlds',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Adventure'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i5-4460 / AMD Ryzen 3 1200
RAM: 8 GB
GPU: GTX 970 / Radeon RX 470
Storage: 60 GB

Recommended:
CPU: Intel Core i7-7700 / AMD Ryzen 5 3600
RAM: 16 GB
GPU: RTX 2060 / Radeon RX 5700');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -249, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -247, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -110, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'It Takes Two')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('It Takes Two', 'Genre-bending co-op adventure built purely for two players.', 39.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/1426210/header.jpg', 'Adventure', '2021-03-26', 'Hazelight Studios', 'Electronic Arts',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Adventure'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i5-4460 / AMD Ryzen 3 1200
RAM: 8 GB
GPU: GTX 970 / Radeon RX 470
Storage: 60 GB

Recommended:
CPU: Intel Core i7-7700 / AMD Ryzen 5 3600
RAM: 16 GB
GPU: RTX 2060 / Radeon RX 5700');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 3, DATEADD(day, -233, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -147, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -217, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -285, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -250, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -80, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Lies of P')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Lies of P', 'Souls-like action game inspired by the story of Pinocchio, set in a dark Belle Epoque world.', 59.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/1627720/header.jpg', 'Action', '2023-09-19', 'Round8 Studio', 'Neowiz',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Action'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -278, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -32, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -161, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -30, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Black Myth: Wukong')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Black Myth: Wukong', 'Action RPG rooted in Chinese mythology, inspired by Journey to the West.', 59.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/2358720/header.jpg', 'Action', '2024-08-20', 'Game Science', 'Game Science',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Action'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 5, DATEADD(day, -272, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -81, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -30, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Helldivers 2')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Helldivers 2', 'Co-op third-person shooter: spread managed democracy across the galaxy.', 39.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/553850/header.jpg', 'Shooter', '2024-02-08', 'Arrowhead Game Studios', 'PlayStation PC',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Shooter'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 3, DATEADD(day, -62, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -292, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -127, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -297, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -21, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -42, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -215, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Dead Cells')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Dead Cells', 'Rogue-lite metroidvania action platformer with fast, punishing combat.', 24.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/588650/header.jpg', 'Indie', '2018-08-07', 'Motion Twin', 'Motion Twin',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Indie'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i3 / AMD Ryzen 3
RAM: 4 GB
GPU: GTX 750 Ti / Radeon R7 360
Storage: 10 GB

Recommended:
CPU: Intel Core i5 / AMD Ryzen 5
RAM: 8 GB
GPU: GTX 1050 Ti');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 5, DATEADD(day, -136, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -203, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -68, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -154, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -235, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -162, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -38, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Slay the Spire')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Slay the Spire', 'Deck-building rogue-like: craft a unique deck and climb the Spire.', 24.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/646570/header.jpg', 'Strategy', '2019-01-23', 'Mega Crit', 'Mega Crit',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Strategy'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i3 / AMD Ryzen 3
RAM: 4 GB
GPU: GTX 750 Ti / Radeon R7 360
Storage: 10 GB

Recommended:
CPU: Intel Core i5 / AMD Ryzen 5
RAM: 8 GB
GPU: GTX 1050 Ti');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 5, DATEADD(day, -52, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -38, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -276, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Outer Wilds')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Outer Wilds', 'Open-world mystery about a solar system trapped in an endless time loop.', 24.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/753640/header.jpg', 'Adventure', '2020-06-18', 'Mobius Digital', 'Annapurna Interactive',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Adventure'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i3 / AMD Ryzen 3
RAM: 4 GB
GPU: GTX 750 Ti / Radeon R7 360
Storage: 10 GB

Recommended:
CPU: Intel Core i5 / AMD Ryzen 5
RAM: 8 GB
GPU: GTX 1050 Ti');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 5, DATEADD(day, -36, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -126, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -190, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -146, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'The Elder Scrolls V: Skyrim Special Edition')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('The Elder Scrolls V: Skyrim Special Edition', 'Award-winning open-world RPG: the Dragonborn''s battle against the World-Eater.', 39.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/489830/header.jpg', 'RPG', '2016-10-28', 'Bethesda Game Studios', 'Bethesda Softworks',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'RPG'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i5-4460 / AMD Ryzen 3 1200
RAM: 8 GB
GPU: GTX 970 / Radeon RX 470
Storage: 60 GB

Recommended:
CPU: Intel Core i7-7700 / AMD Ryzen 5 3600
RAM: 16 GB
GPU: RTX 2060 / Radeon RX 5700');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 5, DATEADD(day, -271, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -5, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -284, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -154, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Fallout 4')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Fallout 4', 'Post-apocalyptic open-world RPG set in the ruins of Boston.', 19.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/377160/header.jpg', 'RPG', '2015-11-10', 'Bethesda Game Studios', 'Bethesda Softworks',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'RPG'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i5-4460 / AMD Ryzen 3 1200
RAM: 8 GB
GPU: GTX 970 / Radeon RX 470
Storage: 60 GB

Recommended:
CPU: Intel Core i7-7700 / AMD Ryzen 5 3600
RAM: 16 GB
GPU: RTX 2060 / Radeon RX 5700');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -55, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -284, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -80, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Mass Effect Legendary Edition')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Mass Effect Legendary Edition', 'Remastered trilogy of the acclaimed sci-fi saga of Commander Shepard.', 59.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/1328670/header.jpg', 'RPG', '2021-05-14', 'BioWare', 'Electronic Arts',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'RPG'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i5-4460 / AMD Ryzen 3 1200
RAM: 8 GB
GPU: GTX 970 / Radeon RX 470
Storage: 60 GB

Recommended:
CPU: Intel Core i7-7700 / AMD Ryzen 5 3600
RAM: 16 GB
GPU: RTX 2060 / Radeon RX 5700');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -136, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -259, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -251, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -129, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -27, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'EA SPORTS FC 24')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('EA SPORTS FC 24', 'Football simulation with HyperMotionV technology, Ultimate Team and Career mode.', 69.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/2195250/header.jpg', 'Sports', '2023-09-29', 'EA Vancouver', 'Electronic Arts',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Sports'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 5, DATEADD(day, -2, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -171, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -67, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'F1 23')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('F1 23', 'Official Formula 1 game with the Braking Point story mode and all official teams.', 69.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/2108330/header.jpg', 'Racing', '2023-06-16', 'Codemasters', 'Electronic Arts',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Racing'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -5, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -58, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -39, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -77, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -280, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Euro Truck Simulator 2')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Euro Truck Simulator 2', 'Travel across Europe as king of the road, delivering cargo and building your company.', 19.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/227300/header.jpg', 'Simulation', '2012-10-18', 'SCS Software', 'SCS Software',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Simulation'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i3 / AMD Ryzen 3
RAM: 4 GB
GPU: GTX 750 Ti / Radeon R7 360
Storage: 10 GB

Recommended:
CPU: Intel Core i5 / AMD Ryzen 5
RAM: 8 GB
GPU: GTX 1050 Ti');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -76, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -221, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -66, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Phasmophobia')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Phasmophobia', 'Four-player co-op psychological horror: investigate paranormal activity.', 13.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/739630/header.jpg', 'Horror', '2020-09-18', 'Kinetic Games', 'Kinetic Games',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Horror'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i3 / AMD Ryzen 3
RAM: 4 GB
GPU: GTX 750 Ti / Radeon R7 360
Storage: 10 GB

Recommended:
CPU: Intel Core i5 / AMD Ryzen 5
RAM: 8 GB
GPU: GTX 1050 Ti');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -184, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -108, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -128, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Dead by Daylight')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Dead by Daylight', 'Asymmetrical multiplayer horror: one killer hunts four survivors.', 19.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/381210/header.jpg', 'Horror', '2016-06-14', 'Behaviour Interactive', 'Behaviour Interactive',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Horror'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i5-4460 / AMD Ryzen 3 1200
RAM: 8 GB
GPU: GTX 970 / Radeon RX 470
Storage: 60 GB

Recommended:
CPU: Intel Core i7-7700 / AMD Ryzen 5 3600
RAM: 16 GB
GPU: RTX 2060 / Radeon RX 5700');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -80, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -122, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -84, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Cyberpunk 2077: Phantom Liberty')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Cyberpunk 2077: Phantom Liberty', 'Spy-thriller expansion for Cyberpunk 2077 set in the new district of Dogtown.', 29.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/2138330/header.jpg', 'RPG', '2023-09-26', 'CD Projekt Red', 'CD Projekt',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'RPG'),
            1, 0, 1, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 5, DATEADD(day, -211, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -128, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -137, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -82, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Elden Ring: Shadow of the Erdtree')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Elden Ring: Shadow of the Erdtree', 'Massive expansion that takes players to the Land of Shadow.', 39.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/2778580/header.jpg', 'RPG', '2024-06-21', 'FromSoftware', 'Bandai Namco',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'RPG'),
            1, 0, 1, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 5, DATEADD(day, -114, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -103, GETDATE())), (NULL, @gameId, 5, DATEADD(day, -236, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'The Witcher 3: Blood and Wine')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('The Witcher 3: Blood and Wine', 'Final expansion for The Witcher 3 set in the sunny duchy of Toussaint.', 19.99, 'https://cdn.cloudflare.steamstatic.com/steam/apps/378649/header.jpg', 'RPG', '2016-05-31', 'CD Projekt Red', 'CD Projekt',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'RPG'),
            1, 0, 1, NULL, N'Minimum:
OS: Windows 10 64-bit
CPU: Intel Core i5-4460 / AMD Ryzen 3 1200
RAM: 8 GB
GPU: GTX 970 / Radeon RX 470
Storage: 60 GB

Recommended:
CPU: Intel Core i7-7700 / AMD Ryzen 5 3600
RAM: 16 GB
GPU: RTX 2060 / Radeon RX 5700');
    SET @gameId = SCOPE_IDENTITY();
    INSERT INTO Rating (UserId, GameId, Stars, CreatedAt) VALUES (NULL, @gameId, 4, DATEADD(day, -205, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -169, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -143, GETDATE())), (NULL, @gameId, 3, DATEADD(day, -36, GETDATE())), (NULL, @gameId, 4, DATEADD(day, -143, GETDATE()));
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'The Witcher IV')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('The Witcher IV', 'The next saga in The Witcher series. Pre-order now (release date is a placeholder).', 69.99, NULL, 'RPG', '2027-10-15', 'CD Projekt Red', 'CD Projekt',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'RPG'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'The Elder Scrolls VI')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('The Elder Scrolls VI', 'The next chapter of The Elder Scrolls. Pre-order now (release date is a placeholder).', 69.99, NULL, 'RPG', '2028-06-01', 'Bethesda Game Studios', 'Bethesda Softworks',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'RPG'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
END;

IF NOT EXISTS (SELECT 1 FROM Game WHERE Title = 'Intergalactic: The Heretic Prophet')
BEGIN
    INSERT INTO Game (Title, Description, Price, ImageUrl, Genre, ReleaseDate, Developer, Publisher, CategoryId,
                      IsEnabled, IsDeleted, ContentType, TrailerUrl, SystemRequirements)
    VALUES ('Intergalactic: The Heretic Prophet', 'Sci-fi action adventure from Naughty Dog. Pre-order now (release date is a placeholder).', 69.99, NULL, 'Action', '2027-09-01', 'Naughty Dog', 'PlayStation PC',
            (SELECT TOP 1 Id FROM GameCategory WHERE Name = 'Action'),
            1, 0, 0, NULL, N'Minimum:
OS: Windows 10/11 64-bit
CPU: Intel Core i7-6700 / AMD Ryzen 5 1600
RAM: 12 GB
GPU: GTX 1060 6GB / Radeon RX 580
Storage: 70 GB SSD

Recommended:
CPU: Intel Core i7-12700 / AMD Ryzen 7 7800X3D
RAM: 16 GB
GPU: RTX 3070 / Radeon RX 6800 XT
Storage: 70 GB SSD');
    SET @gameId = SCOPE_IDENTITY();
END;

-- 3) GLOBALNE VIJESTI (UserId = NULL -> vide ih svi; IsActive = 1 -> prikazuju se u feedu)
IF NOT EXISTS (SELECT 1 FROM Notification WHERE Title = N'Black Friday Sale' AND UserId IS NULL)
    INSERT INTO Notification (UserId, Title, Message, Type, IsRead, IsActive, CreatedAt)
    VALUES (NULL, N'Black Friday Sale', 'Up to 60% off on selected RPG and action titles this weekend only!', 'Sale', 0, 1, DATEADD(hour, -1, GETDATE()));
IF NOT EXISTS (SELECT 1 FROM Notification WHERE Title = N'New releases every Friday' AND UserId IS NULL)
    INSERT INTO Notification (UserId, Title, Message, Type, IsRead, IsActive, CreatedAt)
    VALUES (NULL, N'New releases every Friday', 'Check the catalog every Friday for fresh titles and DLC.', 'Info', 0, 1, DATEADD(hour, -21, GETDATE()));
IF NOT EXISTS (SELECT 1 FROM Notification WHERE Title = N'Scheduled maintenance' AND UserId IS NULL)
    INSERT INTO Notification (UserId, Title, Message, Type, IsRead, IsActive, CreatedAt)
    VALUES (NULL, N'Scheduled maintenance', 'The store will be unavailable on Sunday from 02:00 to 04:00 due to server maintenance.', 'Maintenance', 0, 1, DATEADD(hour, -41, GETDATE()));
IF NOT EXISTS (SELECT 1 FROM Notification WHERE Title = N'Pre-orders are open' AND UserId IS NULL)
    INSERT INTO Notification (UserId, Title, Message, Type, IsRead, IsActive, CreatedAt)
    VALUES (NULL, N'Pre-orders are open', 'The Witcher IV and The Elder Scrolls VI are now available for pre-order.', 'Info', 0, 1, DATEADD(hour, -61, GETDATE()));

COMMIT TRANSACTION;

-- Pregled rezultata
SELECT COUNT(*) AS BrojIgara FROM Game WHERE IsDeleted = 0;
SELECT g.Title, g.Genre, g.Price, g.ReleaseDate, CAST(AVG(CAST(r.Stars AS decimal(3,1))) AS decimal(3,1)) AS Ocjena
FROM Game g LEFT JOIN Rating r ON r.GameId = g.Id
GROUP BY g.Title, g.Genre, g.Price, g.ReleaseDate
ORDER BY g.Title;
