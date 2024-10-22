create database ChatVideoDB

create table Users
(
	UserId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	FullName nvarchar(100)  NOT NULL,
	DateOfBirth date,
	Gender int,
	Email nvarchar(100) Unique not null,
	PhoneNumber nvarchar(25) Unique not null,
	Password varchar(255),
	Role int default 1,
	IsBlocked BIT NOT NULL DEFAULT 0,
	WrongPasswordCount int DEFAULT 0,
	CreatedDate datetime
)

create table Friends
(
	UserId UNIQUEIDENTIFIER,
	FriendUserId UNIQUEIDENTIFIER,
	Status int default 0,
	CreatedDate datetime,
	PRIMARY KEY (UserId, FriendUserId),
	FOREIGN KEY(UserId) REFERENCES Users(UserId) ON DELETE NO ACTION ON UPDATE NO ACTION,
    FOREIGN KEY(FriendUserId) REFERENCES Users(UserId) ON DELETE NO ACTION ON UPDATE NO ACTION
)
create table Chats
(
	ChatId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	UserId UNIQUEIDENTIFIER NOT NULL,
	FriendUserId UNIQUEIDENTIFIER NOT NULL,
	CreatedDate datetime,
	ModifiedDate datetime,
	UNIQUE(UserId, FriendUserId),
	FOREIGN KEY(UserId) REFERENCES Users(UserId) ON DELETE NO ACTION ON UPDATE NO ACTION,
    FOREIGN KEY(FriendUserId) REFERENCES Users(UserId) ON DELETE NO ACTION ON UPDATE NO ACTION
)

create table VideoCalls
(
	VideoCallId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	StartTime datetime NOT NULL,
	EndTime datetime,
	ChatId UNIQUEIDENTIFIER
	FOREIGN KEY(ChatId) REFERENCES Chats(ChatId) ON Update CASCADE ON DELETE CASCADE
)

create table Messages
(
	MessageId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	ChatId UNIQUEIDENTIFIER NOT NULL,
	SenderId UNIQUEIDENTIFIER NOT NULL,
	ReceiverId UNIQUEIDENTIFIER NOT NULL,
	Content ntext NOT NULL,
	TimeStamp datetime NOT NULL,
	IsDeleted bit DEFAULT 0,
	IsRead bit DEFAULT 0,

	FOREIGN KEY(ChatId) REFERENCES Chats(ChatId) ON DELETE NO ACTION ON UPDATE NO ACTION,
	FOREIGN KEY(SenderId) REFERENCES Users(UserId) ON DELETE NO ACTION ON UPDATE NO ACTION,
	FOREIGN KEY(SenderId) REFERENCES Users(UserId) ON DELETE NO ACTION ON UPDATE NO ACTION
)

create table Sessions
(
	SessionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	UserId UNIQUEIDENTIFIER NOT NULL,
	Token varchar(1000) NOT NULL,
	ExpirationDate datetime,
	CreatedDate datetime,

	FOREIGN KEY(UserId) REFERENCES Users(UserId) ON Update CASCADE ON DELETE CASCADE
)