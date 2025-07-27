using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using ISEWA;
using DataAccess;
namespace UnitTesting
{
    [TestClass]
    public class AdminClassTests
    {
        [TestMethod]
        public void LoginCheck_ReturnsOne_ForValidCredentials()
        {
            var mockReader = new Mock<SqlDataReader>();
            mockReader.SetupSequence(x => x.Read()).Returns(true).Returns(false);
            mockReader.Setup(x => x["AdminEmail"]).Returns("admin@example.com");
            mockReader.Setup(x => x["AdminPassword"]).Returns("correctpassword");

            var mockCommand = new Mock<SqlCommand>();
            mockCommand.Setup(cmd => cmd.ExecuteReader()).Returns(mockReader.Object);

            var mockConnection = new Mock<SqlConnection>();
            mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommand.Object);

            var admin = new AdminClass("admin@example.com", "currentpassword", "newpassword");
            Assert.AreEqual(1, admin.LoginCheck());
        }

        [TestMethod]
        public void LoginCheck_ReturnsZero_ForInvalidCredentials()
        {
            var mockReader = new Mock<SqlDataReader>();
            mockReader.Setup(x => x.Read()).Returns(false);

            var mockCommand = new Mock<SqlCommand>();
            mockCommand.Setup(cmd => cmd.ExecuteReader()).Returns(mockReader.Object);

            var mockConnection = new Mock<SqlConnection>();
            mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommand.Object);

            var admin = new AdminClass("admin@example.com", "wrongpassword", "newpassword");
            Assert.AreEqual(0, admin.LoginCheck());
        }

        [TestMethod]
        public void ChangePassword_Success_ReturnsOne()
        {
            var mockCommand = new Mock<SqlCommand>();
            mockCommand.Setup(cmd => cmd.ExecuteNonQuery()).Returns(1);
            mockCommand.Setup(cmd => cmd.Parameters["@Status"].Value).Returns(1);

            var mockConnection = new Mock<SqlConnection>();
            mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommand.Object);

            var admin = new AdminClass("admin@example.com", "currentpassword", "newpassword");
            Assert.AreEqual(1, admin.ChangePassword());
        }

        [TestMethod]
        public void ChangePassword_Failure_ReturnsZero()
        {
            var mockCommand = new Mock<SqlCommand>();
            mockCommand.Setup(cmd => cmd.ExecuteNonQuery()).Returns(1);
            mockCommand.Setup(cmd => cmd.Parameters["@Status"].Value).Returns(0);

            var mockConnection = new Mock<SqlConnection>();
            mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommand.Object);

            var admin = new AdminClass("admin@example.com", "currentpassword", "newpassword");
            Assert.AreEqual(0, admin.ChangePassword());
        }

        [TestMethod]
        public void FetchEventScores_ValidInputs_ReturnsData()
        {
            var mockAdapter = new Mock<SqlDataAdapter>();
            var ds = new DataSet();
            ds.Tables.Add(new DataTable());
            mockAdapter.Setup(a => a.Fill(It.IsAny<DataSet>())).Returns(1);

            var admin = new AdminClass();
            DataSet result = admin.FetchEventScores("Fest1", "Event1", "School1");
            Assert.AreEqual(1, result.Tables[0].Rows.Count);
        }

        [TestMethod]
        public void FetchOverallScores_ValidFestName_ReturnsAggregatedData()
        {
            var mockAdapter = new Mock<SqlDataAdapter>();
            var ds = new DataSet();
            ds.Tables.Add(new DataTable());
            mockAdapter.Setup(a => a.Fill(It.IsAny<DataSet>())).Returns(1);

            var admin = new AdminClass();
            DataSet result = admin.FetchOverallScores("Fest1");
            Assert.AreEqual(1, result.Tables[0].Rows.Count);
        }

        [TestMethod]
        public void FetchSchoolFeedback_SpecificSchool_ReturnsFeedback()
        {
            var mockAdapter = new Mock<SqlDataAdapter>();
            var ds = new DataSet();
            ds.Tables.Add(new DataTable());
            mockAdapter.Setup(a => a.Fill(It.IsAny<DataSet>())).Returns(1);

            var admin = new AdminClass();
            DataSet result = admin.FetchSchoolFeedback("school@example.com");
            Assert.AreEqual(1, result.Tables[0].Rows.Count);
        }

    }
}