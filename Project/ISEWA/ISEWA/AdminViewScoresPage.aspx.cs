using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data; // Required for using Dataset , Datatable and Sql  
using System.Data.SqlClient; // Required for Using Sql   
using System.Configuration; // for Using Connection From Web.config  
using DataAccess;

using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Ionic.Zip;


namespace ISEWA
{
    public partial class AdminViewScoresPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string constrr = ConfigurationManager.ConnectionStrings["mycon"].ToString();
            // connection string  
            SqlConnection conn = new SqlConnection(constrr);
            conn.Open();

            if (!this.IsPostBack)
            {
                string mycoon = ConfigurationManager.ConnectionStrings["mycon"].ConnectionString;
                using (SqlConnection coonn = new SqlConnection(mycoon))
                {
                    using (SqlCommand cmmd = new SqlCommand("SELECT FestName FROM FestNames"))
                    {
                        cmmd.CommandType = CommandType.Text;
                        cmmd.Connection = coonn;
                        coonn.Open();
                        ddlFestName.DataSource = cmmd.ExecuteReader();
                        ddlFestName.DataTextField = "FestName";
                        ddlFestName.DataBind();
                        coonn.Close();
                    }
                }
                //ddlFestName.Items.Insert(0, new ListItem("All Fests", "All Fests"));
            }
            lblAdminEmail.Text = Session["AdminEmail"].ToString();
        }

        private void RankSchools(DataTable dt)
        {
            string constr = ConfigurationManager.ConnectionStrings["mycon"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();
            string qry = "SELECT COUNT (EventName) FROM EventDetails WHERE FestName = '" + ddlFestName.SelectedValue + "'";
            SqlCommand com = new SqlCommand(qry, con);
            Int32 TotalEvents = Convert.ToInt32(com.ExecuteScalar());

            int var = dt.Rows.Count;
            int MaximumScore = 10 * TotalEvents;
            double AddTotal;
            double SchoolScore;
            double SchoolEvent;
            List<string> TopSchools = new List<string>();
            List<double> TopWinners = new List<double>();
            double[] ARRTopWinners = new double[var];
            string[] ARRTopSchools = new string[var];
            double temp = 0;
            string temp2 = "";

            foreach (DataRow row in dt.Rows)
            {
                SchoolScore = (double.Parse(row["OverallScore"].ToString())) / MaximumScore * 0.6;
                SchoolEvent = (double.Parse(row["NumberOfEvents"].ToString())) / TotalEvents * 0.4;
                AddTotal = SchoolScore + SchoolEvent;
                TopWinners.Add(AddTotal);
                TopSchools.Add(row["SchoolEmail"].ToString());
                ARRTopWinners = TopWinners.ToArray();
                ARRTopSchools = TopSchools.ToArray();
            }
            for (int i = 0; i <= ARRTopWinners.Length - 1; i++)
            {
                for (int j = i + 1; j < ARRTopWinners.Length; j++)
                {
                    if (ARRTopWinners[i] < ARRTopWinners[j])
                    {
                        temp = ARRTopWinners[i];
                        ARRTopWinners[i] = ARRTopWinners[j];
                        ARRTopWinners[j] = temp;

                        temp2 = ARRTopSchools[i];
                        ARRTopSchools[i] = ARRTopSchools[j];
                        ARRTopSchools[j] = temp2;
                    }
                }
            }
            LblSchoolName1.Visible = true;
            LblSchoolName2.Visible = true;
            LblSchoolName3.Visible = true;
            btnCalculateWinners.Visible = false;
            btnDownloadWinnerCertificates.Visible = true;   // new button, we will add in ASPX

            LblSchoolName1.Text = ARRTopSchools[0].ToString();
            LblSchoolName2.Text = ARRTopSchools[1].ToString();
            LblSchoolName3.Text = ARRTopSchools[2].ToString();

            // store winner emails and scores for later use
            Session["WinnerEmails"] = ARRTopSchools;
            Session["WinnerScores"] = ARRTopWinners;
            Session["SelectedFestName"] = ddlFestName.SelectedValue;
        }

        private void Search()
        {
            AdminClass objFetchFest = new AdminClass();
            DataSet ds = objFetchFest.FetchOverallScores(ddlFestName.SelectedValue);
            if (ds.Tables[0].Rows.Count > 0)
            {
                gvOverallScore.DataSource = ds;
                gvOverallScore.DataBind();
                gvOverallScore.Visible = true;
                
                btnEventScores.Visible = true;
                lblTop3Winners.Visible = true;
                lbl1.Visible = true;
                lbl2.Visible = true;
                lbl3.Visible = true;
                LblSchoolName1.Visible = false;
                LblSchoolName2.Visible = false;
                LblSchoolName3.Visible = false;
                btnCalculateWinners.Visible = true;

                lblErrorMessage.Visible = false;
            }
            else
            {
                gvOverallScore.Visible = false;
                lblTop3Winners.Visible = false;
                lbl1.Visible = false;
                lbl2.Visible = false;
                lbl3.Visible = false;
                LblSchoolName1.Visible = false;
                LblSchoolName2.Visible = false;
                LblSchoolName3.Visible = false;
                btnCalculateWinners.Visible = false;

                lblErrorMessage.Visible = true;
                lblErrorMessage.Text = "No Data was available for the selected Fest.";
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.Search();
        }

        protected void btnEventScores_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminViewEventScoresPage.aspx");
        }

        protected void btnLogoutAdmin_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminLoginPage.aspx");
        }

        protected void btnReturnToPreviousPage_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminHomePage.aspx");
        }

        protected void btnHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("HomePage.aspx");
        }

        protected void btnCalculateWinners_Click(object sender, EventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings["mycon"].ToString();
            // connection string  
            SqlConnection con = new SqlConnection(constr);
            con.Open();
            string qry = "SELECT SchoolEmail, SUM(Score) As OverallScore, COUNT (DISTINCT SchoolEventScores.EventName) As NumberOfEvents FROM SchoolEventScores " +
                "WHERE FestName = '" + ddlFestName.SelectedValue + "' GROUP BY SchoolEmail ORDER BY OverallScore DESC";
            SqlCommand com = new SqlCommand(qry, con);
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable ds = new DataTable();
            da.Fill(ds);  //fill dataset  
            gvOverallScore.DataSource = ds;   //assigning datasource to the gridview  
            gvOverallScore.DataBind();

            RankSchools(ds);
        }

        protected void btnDownloadWinnerCertificates_Click(object sender, EventArgs e)
        {
            string[] winnerEmails = Session["WinnerEmails"] as string[];
            if (winnerEmails == null || winnerEmails.Length == 0)
            {
                lblErrorMessage.Visible = true;
                lblErrorMessage.Text = "Please calculate the Top 3 winners first.";
                return;
            }

            string festName = Session["SelectedFestName"] as string ?? ddlFestName.SelectedValue;

            using (MemoryStream zipStream = new MemoryStream())
            {
                using (ZipFile zip = new ZipFile())
                {
                    for (int i = 0; i < 3 && i < winnerEmails.Length; i++)
                    {
                        string schoolEmail = winnerEmails[i];
                        string rankText = GetRankText(i + 1);

                        string schoolName;
                        DataTable eventScores;

                        GetSchoolCertificateData(festName, schoolEmail, out schoolName, out eventScores);

                        if (eventScores == null || eventScores.Rows.Count == 0)
                            continue;

                        byte[] pdfBytes = CreateCertificatePdfBytes(schoolName, schoolEmail, festName, rankText, eventScores);

                        string safeSchoolName = MakeSafeFileName(schoolName);
                        string entryName = safeSchoolName + "_" + (i + 1).ToString() + "Place.pdf";

                        zip.AddEntry(entryName, pdfBytes);
                    }

                    zip.Save(zipStream);
                }

                byte[] zipBytes = zipStream.ToArray();
                Response.Clear();
                Response.ContentType = "application/zip";
                Response.AddHeader("content-disposition", "attachment;filename=" + MakeSafeFileName(festName) + "_WinnersCertificates.zip");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(zipBytes);
                Response.End();
            }
        }

        private string GetRankText(int rank)
        {
            switch (rank)
            {
                case 1: return "1st";
                case 2: return "2nd";
                case 3: return "3rd";
                default: return rank + "th";
            }
        }

        private void GetSchoolCertificateData(string festName, string schoolEmail, out string schoolName, out DataTable eventScores)
        {
            schoolName = schoolEmail;
            eventScores = new DataTable();

            string constr = ConfigurationManager.ConnectionStrings["mycon"].ToString();
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                // get school name
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 SchoolName FROM SchoolCredentials WHERE SchoolEmail = @Email", con))
                {
                    cmd.Parameters.AddWithValue("@Email", schoolEmail);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        schoolName = result.ToString();
                    }
                }

                // get event wise scores for this fest
                using (SqlCommand cmd2 = new SqlCommand(
                    "SELECT EventName, Score FROM SchoolEventScores WHERE FestName = @FestName AND SchoolEmail = @Email ORDER BY EventName", con))
                {
                    cmd2.Parameters.AddWithValue("@FestName", festName);
                    cmd2.Parameters.AddWithValue("@Email", schoolEmail);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd2))
                    {
                        da.Fill(eventScores);
                    }
                }
            }
        }

        private byte[] CreateCertificatePdfBytes(string schoolName, string schoolEmail, string festName, string rankText, DataTable eventScores)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4.Rotate(), 50f, 50f, 120f, 50f);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                // attach our background event
                writer.PageEvent = new CertificateBgHelper();

                doc.Open();
                doc.Add(Chunk.NEWLINE);

                // fonts
                BaseColor darkBlue = new BaseColor(2, 55, 138);
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 22, darkBlue);
                Font subFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, darkBlue);
                Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 14, BaseColor.BLACK);
                Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);

                // MAIN HEADING
                Paragraph title = new Paragraph("CERTIFICATE OF ACHIEVEMENT", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                doc.Add(title);

                doc.Add(new Paragraph("\n\n"));   // More top spacing

                // CERTIFICATE TEXT
                Paragraph txt1 = new Paragraph($"This is to certify that ", normalFont);
                txt1.Alignment = Element.ALIGN_CENTER;
                doc.Add(txt1);

                Paragraph schoolPara = new Paragraph(schoolName, boldFont);
                schoolPara.Alignment = Element.ALIGN_CENTER;
                doc.Add(schoolPara);

                // Fetch rank details & score details
                int totalScore = eventScores.AsEnumerable().Sum(row => row.Field<int>("Score"));
                int totalEvents = eventScores.Rows.Count;

                Paragraph txt2 = new Paragraph(
                    $"has achieved {rankText} position by participating in {totalEvents} events and achieving a score of {totalScore}!",
                    normalFont);
                txt2.Alignment = Element.ALIGN_CENTER;
                doc.Add(txt2);

                doc.Add(Chunk.NEWLINE);

                // EVENT SUMMARY TABLE
                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 70;
                table.HorizontalAlignment = Element.ALIGN_CENTER;
                table.SetWidths(new float[] { 1f, 5f, 2f });

                AddHeaderCell(table, "S.No");
                AddHeaderCell(table, "Event Name");
                AddHeaderCell(table, "Score");

                int serial = 1;
                foreach (DataRow row in eventScores.Rows)
                {
                    AddBodyCell(table, serial.ToString());
                    AddBodyCell(table, row["EventName"].ToString());
                    AddBodyCell(table, row["Score"].ToString());
                    serial++;
                }

                doc.Add(table);
                doc.Add(Chunk.NEWLINE);

                PdfPTable signTable = new PdfPTable(3);
                signTable.WidthPercentage = 100;
                signTable.SetWidths(new float[] { 1f, 1f, 1f });

                AddSignature(signTable, "~/Certificates/sign1.jpg", "Mr. Jay Malhotra\nEvent Organizer");
                AddSignature(signTable, "~/Certificates/sign2.png", "Mr. Suryesh Pandey\nMinister of Relations @Student Cabinet");
                AddSignature(signTable, "~/Certificates/sign3.png", "Mr. Viraal Saini\nHead Judge");

                doc.Add(signTable);

                doc.Close();
                return ms.ToArray();
            }
        }

        private void AddSignature(PdfPTable table, string imgPath, string text)
        {
            PdfPCell cell = new PdfPCell();
            cell.Border = Rectangle.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_TOP;


            string fullPath = Server.MapPath(imgPath);
            if (File.Exists(fullPath))
            {
                iTextSharp.text.Image sign = iTextSharp.text.Image.GetInstance(fullPath);
                sign.ScaleToFit(120f, 60f);
                cell.AddElement(sign);
            }

            Paragraph p = new Paragraph(text, FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK));
            p.Alignment = Element.ALIGN_CENTER;
            cell.AddElement(p);

            table.AddCell(cell);
        }


        private void AddHeaderCell(PdfPTable table, string text)
        {
            Font font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = BaseColor.WHITE;
            cell.Padding = 5;
            table.AddCell(cell);
        }

        private void AddBodyCell(PdfPTable table, string text)
        {
            Font font = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);
        }

        private string MakeSafeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c.ToString(), "_");
            }
            return name.Replace(" ", "_");
        }

    }

}