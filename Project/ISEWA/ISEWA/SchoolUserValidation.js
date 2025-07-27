// JavaScript source code
function SchoolUserValidation()
{
    var SchoolName, SchoolAddress, SchoolPhoneNumber, ValidatePhoneNumber, SchoolContactPersonsName, SchoolEmail, ValidateEmail, SchoolPassword, SchoolConfirmPassword;

    ValidatePhoneNumber = /^[1-9]{1}[0-9]{9}$/;
    ValidateEmail = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([com\co\.\in])+$/;
    SchoolName = document.getElementById("txtSchoolName").value.trim();
    SchoolAddress = document.getElementById("txtSchoolAddress").value.trim();
    SchoolPhoneNumber = document.getElementById("txtSchoolPhoneNumber").value.trim();
    SchoolContactPersonsName = document.getElementById("txtSchoolContactPersonName").value.trim();
    SchoolEmail = document.getElementById("txtSchoolContactEmail").value.trim();
    SchoolPassword = document.getElementById("txtCreatePassword").value.trim();
    SchoolConfirmPassword = document.getElementById("txtConfirmPassword").value.trim();

    if (SchoolName == '' && SchoolAddress == '' && SchoolPhoneNumber == '' && SchoolContactPersonsName == '' && SchoolEmail == '' && SchoolPassword == '' && SchoolConfirmPassword == '')
    {
        alert("Please fill the form!");
        return false;
    }
    if (SchoolName == '')
    {
        alert("Please Enter Your School Name!");
        return false;
    }
    if (SchoolAddress == '')
    {
        alert("Please Enter Your School Address!");
        return false;
    }
    if (SchoolPhoneNumber == '')
    {
        alert("Please Enter Your School's Phone Number!");
        return false;
    }
    if (SchoolPhoneNumber != '') {
        if (!SchoolPhoneNumber.match(ValidatePhoneNumber))
        {
            alert("Invalid Phone Number.");
            return false;
        }
    }
    if (SchoolContactPersonsName == '') {
        alert("Please Enter Your School's Contact Person's Name!");
        return false;
    }
    if (SchoolEmail == '')
    {
        alert("Please enter your school's contact email.");
        return false;
    }
    if (SchoolEmail != '') {
        if (!SchoolEmail.match(ValidateEmail))
        {
            alert("Invalid Email ID.");
            return false;
        }
    }
    if (SchoolPassword == '') {
        alert("Please Create A Password.");
        return false;
    }
    if (SchoolPassword != '' && SchoolConfirmPassword == '')
    {
        alert("Please Confirm the Password.");
        return false;
    }
    if (SchoolPassword != SchoolConfirmPassword) {
        alert("The Passwords don't match. Please try again.");
        return false;
    }
   
    return true;
}