using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess;
using System.Data.SqlClient;

namespace Tests
{
    [TestClass]
    public class SchoolClassTests
    {
        private static SqlConnection? _testConnection;
        private static SchoolClass? _schoolUnderTest;
        private static SqlCommand? _testCommand;

        private const string TestSchoolEmail = "schoolUnique@test.com";  // Ensure this email is unique in your database
        private const string TestSchoolPassword = "schoolPassword";
        private const string NewSchoolPassword = "newSchoolPassword";

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            try
            {
                string connectionString = "Data Source=192.168.0.101;Initial Catalog=TESTdb;Persist Security Info=True;User ID=sa;Password=Slogo74484@;";
                _testConnection = new SqlConnection(connectionString);
                _testConnection.Open();

                // Clean existing data to avoid conflicts
                SqlCommand cleanupCmd = new SqlCommand(
                    "DELETE FROM SchoolEventScores; " +
                    "DELETE FROM ParticipantDetails; " +
                    "DELETE FROM EventDetails; " +
                    "DELETE FROM FestNames; " +
                    "DELETE FROM SchoolCredentials; " +
                    "DELETE FROM EventCoordinatorCredentials;",
                    _testConnection);
                cleanupCmd.ExecuteNonQuery();

                // Insert event coordinator data
                _testCommand = new SqlCommand(
                    "INSERT INTO EventCoordinatorCredentials (EventCoordinatorName, EventCoordinatorEmail, EventCoordinatorPhoneNumber, EventCoordinatorPassword) " +
                    "VALUES ('John Doe', 'coordinator@example.com', '555-1234', 'pass1234')",
                    _testConnection);
                _testCommand.ExecuteNonQuery();

                // Insert school credentials
                _testCommand = new SqlCommand(
                    "INSERT INTO SchoolCredentials (SchoolName, SchoolEmail, SchoolPassword, SchoolAddress, SchoolContactPerson, SchoolPhoneNumber) " +
                    "VALUES ('Test School', 'schoolUnique@test.com', 'schoolPassword', '123 School St', 'John Doe', '555-1234')",
                    _testConnection);
                _testCommand.ExecuteNonQuery();

                // Insert fest names
                _testCommand = new SqlCommand("INSERT INTO FestNames (FestName) VALUES ('TestFest')", _testConnection);
                _testCommand.ExecuteNonQuery();

                // Insert event details
                _testCommand = new SqlCommand(
                    "INSERT INTO EventDetails (FestName, EventName, EventDescription, EventCoordinator, NumberOfParticipants, DateOfTheEvent, TimeOfTheEvent) " +
                    "VALUES ('TestFest', 'TestEvent', 'Description', 'coordinator@example.com', 100, '2024-12-25', '10:00 AM')",
                    _testConnection);
                _testCommand.ExecuteNonQuery();

                var insertParticipant = new SqlCommand(
                    "INSERT INTO ParticipantDetails (FestName, EventName, SchoolEmail, ParticipantName, ParticipantGender, ParticipantGrade) " +
                    "VALUES ('TestFest', 'TestEvent', 'schoolUnique@test.com', 'Student Unique', 'Female', 'Grade 10')",
                _testConnection);
                insertParticipant.ExecuteNonQuery();

                var cmd = new SqlCommand(
                    "INSERT INTO SchoolEventScores (FestName, EventName, SchoolEmail, Score) " +
                    "VALUES ('TestFest', 'TestEvent', 'schoolUnique@test.com', 9)",
                _testConnection);
                cmd.ExecuteNonQuery();

                // Initialize the SchoolClass instance for testing
                _schoolUnderTest = new SchoolClass("TestEvent", "schoolUnique@test.com");
                _schoolUnderTest.SchoolName = "Test School";
                _schoolUnderTest.SchoolAddress = "123 School St";
                _schoolUnderTest.SchoolContactPerson = "John Doe";
                _schoolUnderTest.SchoolPhoneNumber = "555-1234";
                _schoolUnderTest.UserEmail = "schoolUnique@test.com";
                _schoolUnderTest.UserPassword = "schoolPassword";
                _schoolUnderTest.CurrentPassword = "schoolPassword";
                _schoolUnderTest.NewPassword = "newSchoolPassword";
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
            int result = _schoolUnderTest.LoginCheck();
            Assert.AreEqual(1, result, "Login should succeed with correct credentials.");
        }

        [TestMethod]
        public void ChangePassword_ValidCredentials_SuccessfulChange()
        {
            int result = _schoolUnderTest.ChangePassword();
            Assert.AreEqual(1, result, "Password change should be successful with correct current password.");
        }

        [TestMethod]
        public void AddSchoolDetails_NewSchoolDetails_ReturnsPositive()
        {
            // Arrange
            SchoolClass newSchool = new SchoolClass
            {
                SchoolName = "New Unique School",
                UserEmail = "newUniqueSchool@test.com",
                UserPassword = "newUniquePassword",
                SchoolAddress = "456 New School St",
                SchoolContactPerson = "New Contact",
                SchoolPhoneNumber = "555-6789"
            };

            // Act
            int result = newSchool.AddSchoolDetails();

            // Assert
            Assert.IsTrue(result > 0, "Adding new school details should be successful.");
        }


        [TestMethod]
        public void CheckEmail_ExistingEmail_ReturnsTrue()
        {
            bool result = _schoolUnderTest.CheckEmail();
            Assert.IsTrue(result, "Check email should return true for existing email.");
        }

        [TestMethod]
        public void RegisteredEvent_ExistingEvent_ReturnsTrue()
        {
            bool result = _schoolUnderTest.RegisteredEvent();
            Assert.IsTrue(result, "Registered event check should return true for existing registration.");
        }

        [TestMethod]
        public void FetchEventDetails_ValidFest_ReturnsDataSet()
        {
            var result = _schoolUnderTest.FetchEventDetails("TestFest");
            Assert.IsTrue(result.Tables[0].Rows.Count > 0, "Should return event details for the valid fest.");
        }

        [TestMethod]
        public void ViewScores_ValidParameters_ReturnsDataSet()
        {
            var result = _schoolUnderTest.ViewScores("TestFest", "schoolUnique@test.com");
            Assert.IsTrue(result.Tables[0].Rows.Count > 0, "Should return scores for valid parameters.");
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            try
            {
                // Clean up the database to remove mock data
                _testCommand = new SqlCommand(
                    "DELETE FROM SchoolEventScores; DELETE FROM ParticipantDetails; DELETE FROM EventDetails; DELETE FROM FestNames; DELETE FROM SchoolCredentials; DELETE FROM EventCoordinatorCredentials;",
                    _testConnection);
                _testCommand.ExecuteNonQuery();

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
