using System;
using DataAccess;
using System.Data; // Required for using Dataset , Datatable and Sql  
using System.Data.SqlClient; // Required for Using Sql   
using System.Configuration; // for Using Connection From Web.config  

namespace Tests
{
    [TestClass]
    public class EventCoordinatorClassTests
    {
        private static SqlConnection? _testConnection;
        private static EventCoordinatorClass? _eventCoordinatorUnderTest;
        private static SqlCommand? _testCommand;

        private const string TestCoordinatorEmail = "john.doe@example.com"; // Test email
        private const string TestCoordinatorPassword = "pass1234"; // Test password
        private const string NewCoordinatorPassword = "newPass1234"; // New password for change password tests

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            try
            {
                string connectionString = "Data Source=192.168.0.101;Initial Catalog=TESTdb;Persist Security Info=True;User ID=sa;Password=Slogo74484@;";
                _testConnection = new SqlConnection(connectionString);
                _testConnection.Open();

                // Insert mock data for EventCoordinatorClass testing
                _testCommand = new SqlCommand(
                    "INSERT INTO EventCoordinatorCredentials (EventCoordinatorName, EventCoordinatorEmail, EventCoordinatorPhoneNumber, EventCoordinatorPassword) " +
                    "VALUES ('John Doe', 'john.doe@example.com', '555-1234', 'pass1234')",
                    _testConnection);
                _testCommand.ExecuteNonQuery();

                SqlCommand insertSchool = new SqlCommand(
                    "INSERT INTO SchoolCredentials (SchoolName, SchoolEmail, SchoolPassword, SchoolAddress, SchoolContactPerson, SchoolPhoneNumber) " +
                    "VALUES ('Test School', 'school@test.com', 'schoolPass', '1234 Test St, Test City', 'Jane Doe', '555-5678')",
                    _testConnection);
                insertSchool.ExecuteNonQuery();

                SqlCommand insertFest = new SqlCommand("INSERT INTO FestNames (FestName) VALUES ('TestFest')", _testConnection);
                insertFest.ExecuteNonQuery();

                SqlCommand insertEvent = new SqlCommand(
                    "INSERT INTO EventDetails (FestName, EventName, EventDescription, EventCoordinator, NumberOfParticipants, DateOfTheEvent, TimeOfTheEvent) " +
                    "VALUES ('TestFest', 'TestEvent', 'This is a description of the test event.', 'john.doe@example.com', 50, '2024-01-01', '12:00')",
                    _testConnection);
                insertEvent.ExecuteNonQuery();

                SqlCommand insertScores = new SqlCommand("INSERT INTO SchoolEventScores (FestName, EventName, SchoolEmail, Score) VALUES ('TestFest', 'TestEvent', 'school@test.com', 85)", _testConnection);
                insertScores.ExecuteNonQuery();
                // Initialize the EventCoordinatorClass instance for testing
                _eventCoordinatorUnderTest = new EventCoordinatorClass(TestCoordinatorEmail, TestCoordinatorPassword, NewCoordinatorPassword);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during test setup: " + ex.Message);
                throw; // Re-throw the exception to fail the test setup
            }
        }

        [TestMethod]
        public void LoginCheck_ValidCredentials_ReturnsTrue()
        {
            var result = _eventCoordinatorUnderTest.LoginCheck();
            Assert.AreEqual(1, result, "Login should succeed with correct credentials.");
        }

        [TestMethod]
        public void ChangePassword_ValidCredentials_SuccessfulChange()
        {
            var result = _eventCoordinatorUnderTest.ChangePassword();
            Assert.AreEqual(1, result, "Password change should be successful with correct current password.");
        }

        [TestMethod]
        public void AddEventCoordinatorDetails_NewCoordinatorDetails_ReturnsPositive()
        {
            // Arrange
            EventCoordinatorClass newCoordinator = new EventCoordinatorClass
            {
                EventCoordinatorName = "Jane Coordinator",
                UserEmail = "jane.coord@example.com",
                EventCoordinatorPhoneNumber = "555-6789",
                UserPassword = "coordPassword"
            };

            // Act
            int result = newCoordinator.AddEventCoordinatorDetails();

            // Assert
            Assert.IsTrue(result > 0, "Adding new event coordinator should return a positive integer indicating the number of rows inserted.");
        }

        [TestMethod]
        public void CheckEmail_ExistingEmail_ReturnsTrue()
        {
            // Arrange
            _eventCoordinatorUnderTest.UserEmail = TestCoordinatorEmail;

            // Act
            bool emailExists = _eventCoordinatorUnderTest.CheckEmail();

            // Assert
            Assert.IsTrue(emailExists, "CheckEmail should return true for an existing email.");
        }

        [TestMethod]
        public void ScoresGiven_ExistingScores_ReturnsTrue()
        {
            // Arrange
            _eventCoordinatorUnderTest.FestName = "TestFest";
            _eventCoordinatorUnderTest.EventName = "TestEvent";

            // Act
            bool scoresGiven = _eventCoordinatorUnderTest.ScoresGiven();

            // Assert
            Assert.IsTrue(scoresGiven, "ScoresGiven should return true if scores exist for the given fest and event.");
        }

        [TestCleanup]
        public void CleanupEachTest()
        {
            // Reset the password back to the original after the ChangePassword test
            if (_eventCoordinatorUnderTest != null)
            {
                _testCommand = new SqlCommand("UPDATE EventCoordinatorCredentials SET EventCoordinatorPassword = 'pass1234' WHERE EventCoordinatorEmail = 'john.doe@example.com'", _testConnection);
                _testCommand.ExecuteNonQuery();
            }
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            try
            {
                // Clean up all test data to ensure tests are independent

                // This ensures that all records possibly created with unique prefixes are removed.
                var cleanupCommand = new SqlCommand(
                    "DELETE FROM SchoolCredentials WHERE SchoolEmail LIKE 'uniqueSchool%@test.com';",
                    _testConnection);
                cleanupCommand.ExecuteNonQuery();

                // Optionally, you can also check and drop any leftover entries explicitly if they aren't caught by the pattern.
                cleanupCommand = new SqlCommand(
                    "DELETE FROM SchoolCredentials WHERE SchoolEmail = 'uniqueSchoold9b02866@test.com';",
                    _testConnection);
                cleanupCommand.ExecuteNonQuery();

                cleanupCommand = new SqlCommand("DELETE FROM SchoolEventScores; DELETE FROM EventDetails; DELETE FROM FestNames; DELETE FROM EventCoordinatorCredentials; DELETE FROM SchoolCredentials;", _testConnection);
                cleanupCommand.ExecuteNonQuery();

                // Close the connection
                _testConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during test cleanup: " + ex.Message);
                throw;  // Re-throw to highlight issues in cleanup
            }
        }
    }
}
