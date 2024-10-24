create database VideoCallDB

use VideoCallDB

CREATE TABLE Users
(
    UserId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FullName NVARCHAR(100) NOT NULL,
    DateOfBirth DATE,
    Gender INT,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PhoneNumber NVARCHAR(25) UNIQUE NOT NULL,
    Password VARCHAR(255),
    Role INT DEFAULT 1,
    IsBlocked BIT NOT NULL DEFAULT 0,
    WrongPasswordCount INT DEFAULT 0,
    CreatedDate DATETIME NOT NULL,
    ModifiedDate DATETIME NOT NULL
);

CREATE TABLE Friends
(
    UserId UNIQUEIDENTIFIER NOT NULL,
    FriendUserId UNIQUEIDENTIFIER NOT NULL,
    Status INT DEFAULT 0,
    CreatedDate DATETIME NOT NULL,
    ModifiedDate DATETIME NOT NULL,
    PRIMARY KEY (UserId, FriendUserId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (FriendUserId) REFERENCES Users(UserId)
);

CREATE TABLE Groups
(
    GroupId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    GroupName NVARCHAR(100) NOT NULL,
    CreatedDate DATETIME NOT NULL,
    ModifiedDate DATETIME NOT NULL
);

CREATE TABLE GroupMembers
(
    GroupId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    JoinedDate DATETIME NOT NULL,
	Nickname nvarchar(255),
    PRIMARY KEY (GroupId, UserId),
    FOREIGN KEY (GroupId) REFERENCES Groups(GroupId) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

CREATE TABLE PrivateChats
(
    ChatId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    PartnerId UNIQUEIDENTIFIER NOT NULL,
    CreatedDate DATETIME NOT NULL,
    ModifiedDate DATETIME NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE NO ACTION,
    FOREIGN KEY (PartnerId) REFERENCES Users(UserId) ON DELETE NO ACTION,
    UNIQUE(UserId, PartnerId)
);

CREATE TABLE PrivateMessages
(
    MessageId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ChatId UNIQUEIDENTIFIER NOT NULL,
    SenderId UNIQUEIDENTIFIER NOT NULL,
    Content NTEXT NOT NULL,
    TimeStamp DATETIME NOT NULL,
    IsDeleted BIT DEFAULT 0,
    IsRead BIT DEFAULT 0,
    FOREIGN KEY (ChatId) REFERENCES PrivateChats(ChatId) ON DELETE CASCADE,
    FOREIGN KEY (SenderId) REFERENCES Users(UserId) ON DELETE NO ACTION
);

CREATE TABLE GroupMessages
(
    MessageId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    GroupId UNIQUEIDENTIFIER NOT NULL,
    SenderId UNIQUEIDENTIFIER NOT NULL,
    Content NTEXT NOT NULL,
    TimeStamp DATETIME NOT NULL,
    IsDeleted BIT DEFAULT 0,
    IsRead BIT DEFAULT 0,
    FOREIGN KEY (GroupId) REFERENCES Groups(GroupId) ON DELETE CASCADE,
    FOREIGN KEY (SenderId) REFERENCES Users(UserId) ON DELETE NO ACTION
);

CREATE TABLE PrivateVideoCalls
(
    VideoCallId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ChatId UNIQUEIDENTIFIER NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME,
	Caller UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (ChatId) REFERENCES PrivateChats(ChatId) ON DELETE CASCADE,
    FOREIGN KEY (Caller) REFERENCES Users(UserId) ON DELETE CASCADE
);

CREATE TABLE GroupVideoCalls
(
    VideoCallId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    GroupId UNIQUEIDENTIFIER NOT NULL,
	Caller UNIQUEIDENTIFIER NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME,
	FOREIGN KEY (Caller) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (GroupId) REFERENCES Groups(GroupId) ON DELETE CASCADE
);

CREATE TABLE GroupCallParticipants
(
    VideoCallId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    JoinTime DATETIME NOT NULL,
    LeaveTime DATETIME,
	PRIMARY KEY (VideoCallId, UserId),
    FOREIGN KEY (VideoCallId) REFERENCES GroupVideoCalls(VideoCallId) ON DELETE NO ACTION,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

CREATE TABLE Sessions
(
    SessionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Token VARCHAR(1000) NOT NULL,
    ExpirationDate DATETIME,
    CreatedDate DATETIME NOT NULL,
    ModifiedDate DATETIME NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON UPDATE CASCADE ON DELETE CASCADE
);



EXEC Proc_Users_Insert 
    @UserId = 'f1a800ff-0255-4a90-a1eb-a81c25aa663d', 
    @FullName = N'Trần Thị Lan Anh', 
    @DateOfBirth = '1999-09-08', 
    @Gender = 1,  
    @Email = 'lananhhz@gmail.com', 
    @PhoneNumber = '0886126621', 
    @Password = 'Abc@123456', 
    @Role = 1, 
    @CreatedDate = '2024-10-23 19:35:47.003', 
    @ModifiedDate = '2024-10-23 19:35:47.003';

	select * from Users

CREATE PROCEDURE Proc_Users_Insert
    @UserId UNIQUEIDENTIFIER,
    @FullName NVARCHAR(100),
    @DateOfBirth DATE,
    @Gender INT,
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(25),
    @Password VARCHAR(20),
    @Role INT,
    @CreatedDate DATETIME,
    @ModifiedDate DATETIME
AS
BEGIN
	-- Mã hóa mật khẩu
    DECLARE @PasswordHash VARBINARY(64);
    SET @PasswordHash = HASHBYTES('SHA2_256', @Password);

    -- Thêm bản ghi mới vào bảng User
    INSERT INTO Users(UserId, FullName, DateOfBirth, Gender, Email, PhoneNumber, Password, Role, CreatedDate, ModifiedDate)
    VALUES (@UserId, @FullName, @DateOfBirth, @Gender, @Email, @PhoneNumber, @PasswordHash, @Role, @CreatedDate, @ModifiedDate);

    -- Trả về thông báo thành công
    SELECT 'User added successfully' AS Message;
END;

CREATE PROCEDURE Proc_Sessions_Insert
    @SessionId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @Token VARCHAR(1000),
    @ExpirationDate DATETIME,
    @CreatedDate DATETIME,
    @ModifiedDate DATETIME
AS
BEGIN
    INSERT INTO Sessions(SessionId, UserId, Token, ExpirationDate, CreatedDate, ModifiedDate)
    VALUES (@SessionId, @UserId, @Token, @ExpirationDate, @CreatedDate, @ModifiedDate);
END;

CREATE PROCEDURE Proc_Users_CheckLogin
    @EmailOrPhoneNumber NVARCHAR(100),
    @Password VARCHAR(20)
AS
BEGIN
	-- Mã hóa mật khẩu
    DECLARE @PasswordHash VARBINARY(64);
    SET @PasswordHash = HASHBYTES('SHA2_256', @Password);

	SELECT * FROM View_Users vu 
	WHERE vu.UserId = (
		SELECT UserId FROM Users u
		WHERE (@EmailOrPhoneNumber = u.Email
		OR @EmailOrPhoneNumber = u.PhoneNumber)
		AND @PasswordHash = u.Password
	)
END;

EXEC Proc_Users_CheckLogin @EmailOrPhoneNumber = 'lanphamthu10@gmail.com', @Password = 'Abc@123456';

CREATE PROCEDURE Proc_Sessions_Delete
    @SessionId UNIQUEIDENTIFIER
AS
BEGIN
	DELETE FROM Sessions where SessionId = @SessionId
END;