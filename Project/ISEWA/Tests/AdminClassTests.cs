using System;
using DataAccess;
using System.Data; // Required for using Dataset , Datatable and Sql  
using System.Data.SqlClient; // Required for Using Sql   
using System.Configuration; // for Using Connection From Web.config  

namespace Tests
{
    [TestClass]
    public class AdminClassTests
    {
        private static SqlConnection? _testConnection;
        private static AdminClass? _adminUnderTest;

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            try
            {
                string connectionString = "Data Source=192.168.165.13;Initial Catalog=TESTdb;Persist Security Info=True;User ID=sa;Password=Slogo74484@;";
                _testConnection = new SqlConnection(connectionString);
                _testConnection.Open();

                // Inserting Test Data
                SqlCommand insertAdminCreds = new SqlCommand(
                    "INSERT INTO AdminLoginCredentials (AdminEmail, AdminPassword) " +
                    "VALUES ('admin@test.com', 'adminPassword')",
                    _testConnection);
                insertAdminCreds.ExecuteNonQuery();

                SqlCommand insertCoordinator = new SqlCommand(
                    "INSERT INTO EventCoordinatorCredentials (EventCoordinatorName, EventCoordinatorEmail, EventCoordinatorPhoneNumber, EventCoordinatorPassword) " +
                    "VALUES ('John Doe', 'john.doe@example.com', '555-1234', 'pass1234')",
                    _testConnection);
                insertCoordinator.ExecuteNonQuery();

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

                SqlCommand insertParticipants = new SqlCommand(
                    "INSERT INTO ParticipantDetails (FestName, EventName, SchoolEmail, ParticipantName, ParticipantGender, ParticipantGrade) " +
                    "VALUES ('TestFest', 'TestEvent', 'school@test.com', 'Jane Student', 'Female', 'Grade 10')",
                    _testConnection);
                insertParticipants.ExecuteNonQuery();

                SqlCommand insertScores = new SqlCommand(
                    "INSERT INTO SchoolEventScores (FestName, EventName, SchoolEmail, Score) " +
                    "VALUES ('TestFest', 'TestEvent', 'school@test.com', 85)",
                    _testConnection);
                insertScores.ExecuteNonQuery();

                SqlCommand insertGrades = new SqlCommand(
                    "INSERT INTO EventEligibleGrades (EventName, EligibleGrades) " +
                    "VALUES ('TestEvent', 'Grades 9-12')",
                    _testConnection);
                insertGrades.ExecuteNonQuery();

                SqlCommand insertFeedback = new SqlCommand(
                    "INSERT INTO SchoolFeedback (FestName, SchoolEmail, FestRate, OrganizationOfFestRate, ReturnRate, LikesAboutFestOrEvent, DislikesAboutFestOrEvent, GeneralThoughts) " +
                    "VALUES ('TestFest', 'school@test.com', 5, 5, 'Likely', 'Well organized', 'None', 'Looking forward to next year!')",
                    _testConnection);
                insertFeedback.ExecuteNonQuery();

                _adminUnderTest = new AdminClass("admin@test.com", "adminPassword", "newPassword");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during test setup: " + ex.Message);
                throw;  // Re-throw the exception to fail the test setup
            }
        }

        [TestMethod]
        public void LoginCheck_ValidCredentials_ReturnsTrue()
        {
            var result = _adminUnderTest.LoginCheck();
            Assert.AreEqual(1, result, "Login should succeed with correct credentials.");
        }

        [TestMethod]
        public void ChangePassword_ValidCredentials_SuccessfulChange()
        {
            var result = _adminUnderTest.ChangePassword();
            Assert.AreEqual(1, result, "Password change should be successful with correct current password.");
        }

        [TestMethod]
        public void FetchEventScores_ValidParameters_ReturnsDataSet()
        {
            var result = _adminUnderTest.FetchEventScores("TestFest", "TestEvent", "school@test.com");
            Assert.IsTrue(result.Tables[0].Rows.Count > 0, "Should return event scores data for valid parameters.");
        }

        [TestMethod]
        public void FetchOverallScores_ValidFest_ReturnsDataSet()
        {
            var result = _adminUnderTest.FetchOverallScores("TestFest");
            Assert.IsTrue(result.Tables[0].Rows.Count > 0, "Should return overall scores for the fest.");
        }

        [TestMethod]
        public void FetchSchoolFeedback_ValidSchool_ReturnsDataSet()
        {
            var result = _adminUnderTest.FetchSchoolFeedback("school@test.com");
            Assert.IsTrue(result.Tables[0].Rows.Count > 0, "Should return feedback for the given school email.");
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            try
            {
                // Clean up all test data to ensure tests are independent
                var cleanupCommand = new SqlCommand("DELETE FROM SchoolEventScores; DELETE FROM SchoolFeedback; DELETE FROM ParticipantDetails; DELETE FROM EventeligibleGrades; DELETE FROM EventDetails; DELETE FROM FestNames; DELETE FROM EventCoordinatorCredentials; DELETE FROM SchoolCredentials; DELETE FROM AdminLoginCredentials;", _testConnection);
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
