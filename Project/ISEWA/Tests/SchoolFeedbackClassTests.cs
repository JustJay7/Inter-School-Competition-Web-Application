using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace Tests
{
    [TestClass]
    public class SchoolFeedbackClassTests
    {
        private static SqlConnection? _testConnection;
        private static SchoolFeedbackClass? _feedbackUnderTest;

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            string connectionString = "Data Source=192.168.0.101;Initial Catalog=TESTdb;Persist Security Info=True;User ID=sa;Password=Slogo74484@;";
            _testConnection = new SqlConnection(connectionString);
            _testConnection.Open();

            // Prepare the database with necessary test data
            string setupScript = @"
                INSERT INTO SchoolCredentials (SchoolName, SchoolEmail, SchoolPassword, SchoolAddress, SchoolContactPerson, SchoolPhoneNumber)
                VALUES ('Test School', 'testschool@example.com', 'password', '123 Test Lane', 'John Doe', '555-1234');
                INSERT INTO FestNames (FestName) VALUES ('Test Fest');
                INSERT INTO SchoolFeedback (FestName, SchoolEmail, FestRate, OrganizationOfFestRate, ReturnRate, LikesAboutFestOrEvent, DislikesAboutFestOrEvent, GeneralThoughts) 
                VALUES ('Test Fest', 'testschool@example.com', 5, 4, 'Likely', 'Liked everything', 'Nothing much', 'Was a great event');";
            SqlCommand command = new SqlCommand(setupScript, _testConnection);
            command.ExecuteNonQuery();

            // Instantiate the SchoolFeedbackClass with known test data
            _feedbackUnderTest = new SchoolFeedbackClass("Test Fest", "testschool@example.com");
            _feedbackUnderTest.RateFest = 5;
            _feedbackUnderTest.RateOrganizedFest = 4;
            _feedbackUnderTest.RateFutureFest = "Likely";
            _feedbackUnderTest.SchoolLikes = "Liked everything";
            _feedbackUnderTest.SchoolDislikes = "Nothing much";
            _feedbackUnderTest.GeneralFeedback = "Was a great event";
        }

        [TestMethod]
        public void SubmitFeedback_ShouldReturnPositive()
        {
            int result = _feedbackUnderTest.SubmitFeedback();
            Assert.AreEqual(1, result, "Submitting feedback should be successful and return a positive integer.");
        }

        [TestMethod]
        public void FetchFeedbackDetails_ShouldReturnData()
        {
            DataSet result = _feedbackUnderTest.FetchFeedbackDetails();
            Assert.IsTrue(result.Tables["FeedbackDetails"].Rows.Count > 0, "Fetching feedback details should return data.");
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            string cleanupScript = "DELETE FROM SchoolFeedback; DELETE FROM SchoolCredentials; DELETE FROM FestNames;";
            SqlCommand command = new SqlCommand(cleanupScript, _testConnection);
            command.ExecuteNonQuery();

            _testConnection.Close();
        }
    }
}
