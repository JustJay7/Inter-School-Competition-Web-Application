using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess;
using System.Data.SqlClient;
using System.Configuration;

namespace Tests
{
    [TestClass]
    public class EventClassTests
    {
        private static SqlConnection? _testConnection;
        private static EventClass? _eventUnderTest;

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            string connectionString = "Data Source=192.168.165.13;Initial Catalog=TESTdb;Persist Security Info=True;User ID=sa;Password=Slogo74484@;";
            _testConnection = new SqlConnection(connectionString);
            _testConnection.Open();

            // Setup necessary data for the tests
            var setupCommands = new[]
            {
                "INSERT INTO EventCoordinatorCredentials (EventCoordinatorName, EventCoordinatorEmail, EventCoordinatorPhoneNumber, EventCoordinatorPassword) VALUES ('Jay Malhotra', 'coordinator@test.com', '9310678930', 'password123')",
                "INSERT INTO EventCoordinatorCredentials (EventCoordinatorName, EventCoordinatorEmail, EventCoordinatorPhoneNumber, EventCoordinatorPassword) VALUES ('Uliana R', 'new_coordinator@test.com', '9393455090', 'password321')",
                "INSERT INTO FestNames (FestName) VALUES ('SampleFest')"
            };

            foreach (var cmdText in setupCommands)
            {
                using (var cmd = new SqlCommand(cmdText, _testConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            _eventUnderTest = new EventClass("SampleFest", "SampleEvent", "Sample description", "coordinator@test.com", "100", "28-05-2024", "12 pm - 2 pm");
        }

        [TestMethod]
        public void AddFestDetails_ShouldReturnPositive()
        {
            string uniqueFestName = "NewFest_";

            // Initialize the EventClass with the unique fest name
            EventClass newFest = new EventClass
            {
                FestName = uniqueFestName
            };

            var result = newFest.AddFestDetails();
            Assert.IsTrue(result > 0, "Adding fest details should be successful and return a positive integer.");
        }

        [TestMethod]
        public void AddEventDetails_ShouldReturnPositive()
        {
            var result = _eventUnderTest.AddEventDetails();
            Assert.IsTrue(result > 0, "Adding event details should be successful and return a positive integer.");
        }

        [TestMethod]
        public void UpdateEventDetails_ShouldReturnPositive()
        {
            // Setup a known event and capture the ID for updating
            string eventNameToUpdate = "UniqueEvent_ExampleName";
            string festName = "SampleFest";
            InsertKnownEvent(festName, eventNameToUpdate);

            // Retrieve the ID of the event to update
            int eventId = GetEventIdByName(eventNameToUpdate);

            // Initialize EventClass instance with known ID and updated details
            _eventUnderTest = new EventClass(eventId, festName, eventNameToUpdate, "Updated description", "new_coordinator@test.com", "150", "28-05-2024", "12 pm - 2 pm");

            // Attempt to update the event details
            var result = _eventUnderTest.UpdateEventDetails();
            Assert.IsTrue(result > 0, "Updating event details should be successful and return a positive integer.");
        }


        [TestMethod]
        public void CheckFestExists_ShouldReturnTrue()
        {
            var result = _eventUnderTest.CheckFestExists();
            Assert.IsTrue(result, "Check fest exists should return true when the fest exists.");
        }

        [TestMethod]
        public void CheckEventExists_ShouldReturnTrue()
        {
            var result = _eventUnderTest.CheckEventExists();
            Assert.IsTrue(result, "Check event exists should return true when the event exists.");
        }

        [TestMethod]
        public void AddEligibleGrades_ShouldReturnPositive()
        {
            string[] grades = { "Grade 10", "Grade 11" };
            var result = _eventUnderTest.AddEligibleGrades(_eventUnderTest.EventName, grades);
            Assert.AreEqual(1, result, "Adding eligible grades should be successful and return 1.");
        }

        // Helper method to insert a known event into the database and return its ID
        private void InsertKnownEvent(string festName, string eventName)
        {
            string insertSql = "INSERT INTO EventDetails (FestName, EventName, EventDescription, EventCoordinator, NumberOfParticipants, DateOfTheEvent, TimeOfTheEvent) VALUES (@FestName, @EventName, 'Initial description', 'coordinator@test.com', '100', '01-01-2024', '10 am - 12 pm');";
            using (var conn = new SqlConnection("Data Source=192.168.0.101;Initial Catalog=TESTdb;Persist Security Info=True;User ID=sa;Password=Slogo74484@;"))
            {
                using (var insertCmd = new SqlCommand(insertSql, conn))
                {
                    insertCmd.Parameters.AddWithValue("@FestName", festName);
                    insertCmd.Parameters.AddWithValue("@EventName", eventName);
                    conn.Open();
                    insertCmd.ExecuteNonQuery();
                }
            }
        }

        private int GetEventIdByName(string eventName)
        {
            string selectSql = "SELECT Id FROM EventDetails WHERE EventName = @EventName";
            using (var conn = new SqlConnection("Data Source=192.168.0.101;Initial Catalog=TESTdb;Persist Security Info=True;User ID=sa;Password=Slogo74484@;"))
            {
                using (var selectCmd = new SqlCommand(selectSql, conn))
                {
                    selectCmd.Parameters.AddWithValue("@EventName", eventName);
                    conn.Open();
                    object result = selectCmd.ExecuteScalar();
                    return (result != null) ? Convert.ToInt32(result) : -1;  // Return -1 if no result found
                }
            }
        }


        [ClassCleanup]
        public static void Cleanup()
        {
            var cleanupCommands = new[]
            {
                "DELETE FROM EventEligibleGrades",
                "DELETE FROM EventDetails",
                "DELETE FROM FestNames",
                "DELETE FROM EventCoordinatorCredentials"
            };

            foreach (var cmdText in cleanupCommands)
            {
                using (var cmd = new SqlCommand(cmdText, _testConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            _testConnection.Close();
        }
    }
}
