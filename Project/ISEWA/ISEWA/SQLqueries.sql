CREATE DATABASE iscwaDB;

USE iscwaDB;

DROP TABLE AdminLoginCredentials;
DROP TABLE EventCoordinatorCredentials;
DROP TABLE EventDetails;
DROP TABLE EventEligibleGrades;
DROP TABLE FestNames;
DROP TABLE ParticipantDetails;
DROP TABLE SchoolCredentials;
DROP TABLE SchoolEventScores;
DROP TABLE SchoolFeedback;

CREATE PROCEDURE AdminChangePassword
(
    @RegisteredEmailID NVARCHAR(50), 
    @CurrentPassword NVARCHAR(50),
    @NewPassword NVARCHAR(50),
    @Status int OUTPUT
)
AS
BEGIN
    -- Check if the current email and password match an existing account
    IF EXISTS(SELECT * FROM AdminLoginCredentials 
              WHERE AdminEmail COLLATE Latin1_general_CS_AS = @RegisteredEmailID 
              AND AdminPassword COLLATE Latin1_general_CS_AS = @CurrentPassword)
    BEGIN
        -- Update the password for the account
        UPDATE AdminLoginCredentials 
        SET AdminPassword = @NewPassword 
        WHERE AdminEmail = @RegisteredEmailID

        -- Indicate success
        SET @Status = 1
    END
    ELSE
    BEGIN
        -- Indicate failure
        SET @Status = 0
    END
    
    -- Return the status value
    RETURN @Status
END

-- Create AdminLoginCredentials table
CREATE TABLE AdminLoginCredentials (
    Id INT IDENTITY(1,1),
    AdminEmail NVARCHAR(50) PRIMARY KEY,
    AdminPassword NVARCHAR(50)
);

-- Create EventCoordinatorCredentials table
CREATE TABLE EventCoordinatorCredentials (
    Id INT IDENTITY(1,1),
    EventCoordinatorName NVARCHAR(50),
    EventCoordinatorEmail NVARCHAR(50) PRIMARY KEY,
    EventCoordinatorPhoneNumber NVARCHAR(50),
    EventCoordinatorPassword NVARCHAR(50)
);

-- Create SchoolCredentials table
CREATE TABLE SchoolCredentials (
    Id INT IDENTITY(1,1),
    SchoolName NVARCHAR(50),
    SchoolEmail NVARCHAR(50) PRIMARY KEY,
    SchoolPassword NVARCHAR(50),
    SchoolAddress NVARCHAR(MAX),
    SchoolContactPerson NVARCHAR(50),
    SchoolPhoneNumber NVARCHAR(50)
);

-- Create FestNames table
CREATE TABLE FestNames (
    Id INT IDENTITY(1,1),
    FestName NVARCHAR(50) PRIMARY KEY
);

-- Create EventDetails table
CREATE TABLE EventDetails (
    Id INT IDENTITY(1,1),
    FestName NVARCHAR(50) FOREIGN KEY REFERENCES FestNames(FestName),
    EventName NVARCHAR(50) PRIMARY KEY,
    EventDescription NVARCHAR(MAX),
    EventCoordinator NVARCHAR(50) FOREIGN KEY REFERENCES EventCoordinatorCredentials(EventCoordinatorEmail),
    NumberOfParticipants INT,
    DateOfTheEvent NVARCHAR(50),
    TimeOfTheEvent NVARCHAR(50)
);

-- Create EventEligibleGrades table
CREATE TABLE EventEligibleGrades (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EventName NVARCHAR(50) FOREIGN KEY REFERENCES EventDetails(EventName),
    EligibleGrades VARCHAR(50)
);

-- Create ParticipantDetails table
CREATE TABLE ParticipantDetails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FestName NVARCHAR(50) FOREIGN KEY REFERENCES FestNames(FestName),
    EventName NVARCHAR(50) FOREIGN KEY REFERENCES EventDetails(EventName),
    SchoolEmail NVARCHAR(50) FOREIGN KEY REFERENCES SchoolCredentials(SchoolEmail),
    ParticipantName NVARCHAR(50),
    ParticipantGender NVARCHAR(50),
    ParticipantGrade NVARCHAR(50)
);

-- Create SchoolFeedback table
CREATE TABLE SchoolFeedback (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FestName NVARCHAR(50) FOREIGN KEY REFERENCES FestNames(FestName),
    SchoolEmail NVARCHAR(50) FOREIGN KEY REFERENCES SchoolCredentials(SchoolEmail),
    FestRate INT,
    OrganizationOfFestRate INT,
    ReturnRate NVARCHAR(50),
    LikesAboutFestOrEvent NVARCHAR(MAX),
    DislikesAboutFestOrEvent NVARCHAR(MAX),
    GeneralThoughts NVARCHAR(MAX)
);

-- Create SchoolEventScores table
CREATE TABLE SchoolEventScores (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FestName NVARCHAR(50) FOREIGN KEY REFERENCES FestNames(FestName),
    EventName NVARCHAR(50) FOREIGN KEY REFERENCES EventDetails(EventName),
    SchoolEmail NVARCHAR(50) FOREIGN KEY REFERENCES SchoolCredentials(SchoolEmail),
    Score INT
);


INSERT INTO AdminLoginCredentials (AdminEmail, AdminPassword) VALUES
('erik.someone@gmail.com', 'dedsef');

INSERT INTO EventCoordinatorCredentials (EventCoordinatorName, EventCoordinatorEmail, EventCoordinatorPhoneNumber, EventCoordinatorPassword) VALUES
('Amanda Hans', 'amanda.hans@gmail.com', '9323450002', 'dedsef'),
('Anne Gathion', 'annegathion140@gmail.com', '9648009263', 'dedsef'),
('Erik Someone', 'erik.someone@yahoo.in', '9908789045', 'dedsef'),
('Eri Singh', 'erisingh@gmail.com', '991181182', 'dedsef'),
('James John', 'james.john@gmail.com', '9999000876', 'dedsef'),
('Jeffery', 'jeffery.malhotra@myschool.in', '9333374653', 'dedsef'),
('Michael', 'michael.someone@gmail.com', '9327530947', 'dedsef'),
('Piper McLean', 'piperpmclean21@bing.com', '9326490989', 'dedsef');

INSERT INTO SchoolCredentials (SchoolName, SchoolEmail, SchoolPassword, SchoolAddress, SchoolContactPerson, SchoolPhoneNumber) VALUES
('Ananta Public School', 'anantasschool@gmail.com', 'fe5546df-d703-4a0d-827f-2f998720a1a8', 'Near Wallstreet Park', 'Ananta Sanchez', '9310540438'),
('Archie''s School for Young Children', 'archie@gmail.com', 'dedsef', 'mystreet', 'Archie', '9873425112'),
('Mount Olympus''s School for kids', 'bhumika@mountolympus.com', 'dedsef', 'Downtown Avenue', 'Bhumika', '9300045532'),
('National York School', 'jermylynch@nationalyork.com', 'nyschool74484', 'Opposite Mount Olympus', 'Jermy Lynch', '9300045532'),
('Bellweather School', 'lexi.bellweather@myschool.com', 'dedsef', '55th belewards street', 'Lexi', '967758909'),
('Lily''s Magical School', 'lily.zinnia@gmail.com', 'dedsef', 'Magical School, Magical Place', 'Lily', '9763253600'),
('Louise''s International School', 'louise.gathion@gmail.com', 'dedsef', '21st street, ABC sector', 'Louise Gathion', '9872365998'),
('Phillips International School', 'philip.phineas@gmail.com', 'dedsef', 'Near Philips Mall Centre', 'Philip Phineas', '8873460221');

INSERT INTO FestNames (FestName) VALUES
('Annual Year 2024'),
('Fun Fest!'),
('Get-Set-Go!'),
('Play Fest 2024');

INSERT INTO EventDetails (FestName, EventName, EventDescription, EventCoordinator, NumberOfParticipants, DateOfTheEvent, TimeOfTheEvent) VALUES
('Annual Year 2024', 'All you can Eat!', 'Participants will have access to a buffet where each participant will get 10 minutes to eat as much as they can.', 'james.john@gmail.com', 3, '28-05-2024', '12 pm - 2 pm'),
('Annual Year 2024', 'Basketball Fever', 'Colleges will compete against each other in various matches of basketball. Does your college have what it takes to take home the trophy?', 'james.john@gmail.com', 5, '24-05-2024', '10 am - 12 pm'),
('Annual Year 2024', 'Bottle Flip', 'Each participant has 5 tries to land 3 bottle flips in a row.', 'annegathion140@gmail.com', 2, '01/06/2024', '2 pm - 4 pm'),
('Annual Year 2024', 'Chess', 'Each participant will go up against another from a different college. The process will keep repeating until only 2 remain. Who will win the finals?', 'piperpmclean21@bing.com', 1, '28/05/2024', '8 am - 10 am'),
('Annual Year 2024', 'Doodling', 'Participants will need to make a doodle revolving around their favorite cartoon. Participants will be provided with the necessary equipment.', 'james.john@gmail.com', 2, '28-05-2024', '8 am - 10 am'),
('Annual Year 2024', 'Gaming', 'Participants will compete in a game of monopoly. Whoever has the most amount of money after an individual goes bankrupt, wins.', 'james.john@gmail.com', 1, '24-05-2024', '8 am - 10 am'),
('Play Fest 2024', 'Merry-Go-Round', 'Each participant will get to play 3 sets of Merry-Go-Round', 'amanda.hans@gmail.com', 2, '20/06/2024', '12 pm - 2 pm'),
('Annual Year 2024', 'Painting', 'Participants will be provided with a theme on which they have to make a meaningful and impactful painting.', 'james.john@gmail.com', 3, '28-05-2024', '12 pm - 2 pm'),
('Annual Year 2024', 'Singing at the Bonfire', 'Participants have to come up with their own song about climate change and its negative effects. The duration of the song cannot be more than 2 minutes.', 'james.john@gmail.com', 2, '27-05-2024', '2 pm - 4 pm');

INSERT INTO EventEligibleGrades (EventName, EligibleGrades) VALUES
('All you can Eat!', 'Grade 6'),
('All you can Eat!', 'Grade 7'),
('All you can Eat!', 'Grade 8'),
('Painting', 'Grade 6'),
('Painting', 'Grade 7'),
('Painting', 'Grade 8'),
('Doodling', 'Grade 6'),
('Doodling', 'Grade 7'),
('Doodling', 'Grade 8'),
('Doodling', 'Grade 9'),
('Doodling', 'Grade 10'),
('Doodling', 'Grade 11'),
('Doodling', 'Grade 12'),
('Singing at the Bonfire', 'Grade 6'),
('Singing at the Bonfire', 'Grade 7'),
('Singing at the Bonfire', 'Grade 8'),
('Gaming', 'Grade 9'),
('Gaming', 'Grade 10'),
('Gaming', 'Grade 11'),
('Gaming', 'Grade 12'),
('Bottle Flip', 'Grade 6'),
('Bottle Flip', 'Grade 7'),
('Basketball Fever', 'Grade 6'),
('Basketball Fever', 'Grade 7'),
('Basketball Fever', 'Grade 8'),
('Merry-Go-Round', 'Grade 6'),
('Merry-Go-Round', 'Grade 7'),
('Chess', 'Grade 6'),
('Chess', 'Grade 7');

INSERT INTO ParticipantDetails (FestName, EventName, SchoolEmail, ParticipantName, ParticipantGender, ParticipantGrade) VALUES
('Annual Year 2024', 'Painting', 'jermylynch@nationalyork.com', 'Mishel', 'Female', 'Grade 6'),
('Annual Year 2024', 'Painting', 'jermylynch@nationalyork.com', 'Zack', 'Male', 'Grade 8'),
('Annual Year 2024', 'Painting', 'lexi.bellweather@myschool.com', 'Jason', 'Male', 'Grade 6'),
('Annual Year 2024', 'Painting', 'lexi.bellweather@myschool.com', 'Piper Weasely', 'Female', 'Grade 7'),
('Annual Year 2024', 'Painting', 'lexi.bellweather@myschool.com', 'Manny', 'Male', 'Grade 6'),
('Annual Year 2024', 'Painting', 'lexi.bellweather@myschool.com', 'Piper Weasely', 'Female', 'Grade 7'),
('Annual Year 2024', 'Doodling', 'anantasschool@gmail.com', 'Annabeth', 'Female', 'Grade 12'),
('Annual Year 2024', 'Doodling', 'anantasschool@gmail.com', 'Hazel', 'Female', 'Grade 12'),
('Annual Year 2024', 'Doodling', 'bhumika@mountolympus.com', 'Reyna', 'Female', 'Grade 12'),
('Annual Year 2024', 'Doodling', 'bhumika@mountolympus.com', 'Nico', 'Male', 'Grade 12'),
('Annual Year 2024', 'Painting', 'bhumika@mountolympus.com', 'Megan Molly', 'Female', 'Grade 8'),
('Annual Year 2024', 'Painting', 'bhumika@mountolympus.com', 'Harry James', 'Male', 'Grade 8'),
('Annual Year 2024', 'Painting', 'bhumika@mountolympus.com', 'Percy', 'Male', 'Grade 7'),
('Annual Year 2024', 'Singing at the Bonfire', 'philip.phineas@gmail.com', 'Erik', 'Male', 'Grade 7'),
('Annual Year 2024', 'Singing at the Bonfire', 'philip.phineas@gmail.com', 'Peter Parker', 'Male', 'Grade 8'),
('Annual Year 2024', 'Singing at the Bonfire', 'jermylynch@nationalyork.com', 'Zack', 'Male', 'Grade 6'),
('Annual Year 2024', 'Singing at the Bonfire', 'jermylynch@nationalyork.com', 'Erik', 'Male', 'Grade 6'),
('Annual Year 2024', 'Gaming', 'jermylynch@nationalyork.com', 'Percy Jackson', 'Male', 'Grade 10'),
('Annual Year 2024', 'Gaming', 'anantasschool@gmail.com', 'Nick', 'Male', 'Grade 7'),
('Annual Year 2024', 'Gaming', 'bhumika@mountolympus.com', 'Jason', 'Male', 'Grade 7'),
('Annual Year 2024', 'Basketball Fever', 'philip.phineas@gmail.com', 'Tyson', 'Male', 'Grade 6'),
('Annual Year 2024', 'Basketball Fever', 'philip.phineas@gmail.com', 'Vikram', 'Male', 'Grade 6'),
('Annual Year 2024', 'Basketball Fever', 'philip.phineas@gmail.com', 'Jay', 'Male', 'Grade 7'),
('Annual Year 2024', 'All you can Eat!', 'philip.phineas@gmail.com', 'Slogo', 'Male', 'Grade 6'),
('Annual Year 2024', 'All you can Eat!', 'lexi.bellweather@myschool.com', 'Diyan', 'Male', 'Grade 7'),
('Annual Year 2024', 'All you can Eat!', 'lexi.bellweather@myschool.com', 'Diyan', 'Female', 'Grade 8'),
('Annual Year 2024', 'All you can Eat!', 'lexi.bellweather@myschool.com', 'Daniel', 'Male', 'Grade 6'),
('Annual Year 2024', 'Basketball Fever', 'lexi.bellweather@myschool.com', 'Jason', 'Male', 'Grade 7'),
('Annual Year 2024', 'Basketball Fever', 'lexi.bellweather@myschool.com', 'Slogo', 'Male', 'Grade 6'),
('Annual Year 2024', 'Doodling', 'lexi.bellweather@myschool.com', 'Erik', 'Female', 'Grade 6'),
('Annual Year 2024', 'Doodling', 'lexi.bellweather@myschool.com', 'Slogo', 'Male', 'Grade 10'),
('Annual Year 2024', 'Gaming', 'lexi.bellweather@myschool.com', 'Jason', 'Female', 'Grade 9'),
('Annual Year 2024', 'Gaming', 'lily.zinnia@gmail.com', 'Erik', 'Male', 'Grade 6'),
('Annual Year 2024', 'Bottle Flip', 'lily.zinnia@gmail.com', 'Sonam', 'Female', 'Grade 6'),
('Annual Year 2024', 'Gaming', 'lily.zinnia@gmail.com', 'Erik', 'Female', 'Grade 9'),
('Annual Year 2024', 'All you can Eat!', 'lily.zinnia@gmail.com', 'Erik', 'Female', 'Grade 7'),
('Annual Year 2024', 'All you can Eat!', 'lily.zinnia@gmail.com', 'Slogo', 'Male', 'Grade 8'),
('Annual Year 2024', 'All you can Eat!', 'lily.zinnia@gmail.com', 'Crainer', 'Male', 'Grade 8'),
('Annual Year 2024', 'Painting', 'jermylynch@nationalyork.com', 'Percy', 'Male', 'Grade 7'),
('Annual Year 2024', 'Painting', 'philip.phineas@gmail.com', 'Erik', 'Male', 'Grade 7'),
('Annual Year 2024', 'Painting', 'philip.phineas@gmail.com', 'Slogo', 'Male', 'Grade 6'),
('Annual Year 2024', 'Painting', 'philip.phineas@gmail.com', 'Crainer', 'Female', 'Grade 6');

INSERT INTO SchoolEventScores (FestName, EventName, SchoolEmail, Score) VALUES
('Annual Year 2024', 'Basketball Fever', 'philip.phineas@gmail.com', 7),
('Annual Year 2024', 'Gaming', 'anantasschool@gmail.com', 8),
('Annual Year 2024', 'Gaming', 'jermylynch@nationalyork.com', 9),
('Annual Year 2024', 'Singing at the Bonfire', 'jermylynch@nationalyork.com', 9),
('Annual Year 2024', 'Singing at the Bonfire', 'philip.phineas@gmail.com', 6),
('Annual Year 2024', 'Doodling', 'anantasschool@gmail.com', 6),
('Annual Year 2024', 'Doodling', 'bhumika@mountolympus.com', 9),
('Annual Year 2024', 'All you can Eat!', 'philip.phineas@gmail.com', 7),
('Annual Year 2024', 'All you can Eat!', 'lexi.bellweather@myschool.com', 1),
('Annual Year 2024', 'Basketball Fever', 'lexi.bellweather@myschool.com', 1),
('Annual Year 2024', 'Doodling', 'lexi.bellweather@myschool.com', 7),
('Annual Year 2024', 'Gaming', 'lexi.bellweather@myschool.com', 2),
('Annual Year 2024', 'Painting', 'bhumika@mountolympus.com', 8),
('Annual Year 2024', 'Painting', 'jermylynch@nationalyork.com', 4),
('Annual Year 2024', 'Painting', 'lexi.bellweather@myschool.com', 4),
('Annual Year 2024', 'Painting', 'philip.phineas@gmail.com', 2);

INSERT INTO SchoolFeedback (FestName, SchoolEmail, FestRate, OrganizationOfFestRate, ReturnRate, LikesAboutFestOrEvent, DislikesAboutFestOrEvent, GeneralThoughts) VALUES
('Fun Fest!', 'louise.gathion@gmail.com', 5, 5, 'Yes', 'Organized', 'Harsh Judges', 'We loved the fest'),
('Fun Fest!', 'jermylynch@nationalyork.com', 4, 5, 'Yes', 'It was Fun', 'None', 'We will be back!'),
('Annual Year 2024', 'bhumika@mountolympus.com', 4, 5, 'No', 'None', 'Unorganized', 'None.'),
('Annual Year 2024', 'jermylynch@nationalyork.com', 4, 5, 'Maybe', 'Variety of events', 'None', 'None.');
