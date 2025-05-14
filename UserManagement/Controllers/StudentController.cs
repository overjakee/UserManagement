using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UserManagement.Data;
using UserManagement.Models;

namespace UserManagement.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDBContext _db;
        public StudentController(ApplicationDBContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult LoadUser(string userNameSearch,string emailSearch, string nameSearch, string surnameSearch)
        {
            IEnumerable<User> allUser = _db.User;
            if((userNameSearch != "" && userNameSearch != null) || (emailSearch != "" && emailSearch != null) ||(nameSearch != "" && nameSearch != null) || (surnameSearch != "" && surnameSearch != null))
            {
                if(userNameSearch != "" && userNameSearch != null)
                {
                    allUser = allUser.Where(x => x.UserName.Contains(userNameSearch)).ToList();
                }

                if (emailSearch != "" && emailSearch != null)
                {
                    allUser = allUser.Where(x => x.Email.Contains(emailSearch)).ToList();
                }

                if (nameSearch != "" && nameSearch != null)
                {
                    allUser = allUser.Where(x => x.Name_TH.Contains(nameSearch)).ToList();
                }

                if (surnameSearch != "" && surnameSearch != null)
                {
                    allUser = allUser.Where(x => x.Surname_TH.Contains(surnameSearch)).ToList();
                }

                if (allUser.Count() == 0)
                {
                    return Json(new { isSuccess = false, errorMessage = "ไม่พบข้อมูลตามเงื่อนไขนี้", data = allUser });
                }
            }

            List<object> dataConvert = new List<object>();
            foreach (var user in allUser)
            {
                string EffectiveStartDateMonth = user.EffectiveStartDate.Month.ToString().Length == 1 ? "0" + user.EffectiveStartDate.Month.ToString() : user.EffectiveStartDate.Month.ToString();
                string EffectiveStartDateDay = user.EffectiveStartDate.Day.ToString().Length == 1 ? "0" + user.EffectiveStartDate.Day.ToString() : user.EffectiveStartDate.Day.ToString();

                string EffectiveEndtDateMonth = user.EffectiveFinishDate.Month.ToString().Length == 1 ? "0" + user.EffectiveFinishDate.Month.ToString() : user.EffectiveFinishDate.Month.ToString();
                string EffectiveEndDateDay = user.EffectiveFinishDate.Day.ToString().Length == 1 ? "0" + user.EffectiveFinishDate.Day.ToString() : user.EffectiveFinishDate.Day.ToString();

                dataConvert.Add(new 
                {
                    ID = user.ID,
                    UserName = user.UserName,
                    email = user.Email,
                    Name_TH = user.Name_TH,
                    Surname_TH = user.Surname_TH,
                    Name_EN = user.Name_EN,
                    Surname_EN = user.Surname_EN,
                    Password = Decrypt(user.Password, user.SaltKey),
                    GroupId = user.GroupId,
                    EffectiveStartDate = $"{user.EffectiveStartDate.Year}-{EffectiveStartDateMonth}-{EffectiveStartDateDay}",
                    EffectiveFinishDate = $"{user.EffectiveFinishDate.Year}-{EffectiveEndtDateMonth}-{EffectiveEndDateDay}"
                });
            }


            return Json(new { isSuccess = true , errorMessage = "", data = dataConvert });
        }

        public ActionResult GetGroupUser()
        {
            try
            {
                IEnumerable<UserGroup> allListUserGroup = _db.UserGroup;

                if(allListUserGroup.Count() == 0)
                {
                    return Json(new { isSuccess = false, errorMessage = "ไม่พบข้อมูล UserGroup" });
                }

                return Json(new { isSuccess = true, errorMessage = "" , listUserGroup = allListUserGroup});
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, errorMessage = ex.Message });
            }
        }

        public ActionResult AddUser(string UserName,string Email,string Name_TH,string Name_EN,string Password,int GroupId,
            DateTime? EffectiveStartDate, DateTime? EffectiveFinishDate)
        {
            string NameUserForm = "NameUserForm";
            string EmailForm = "EmailForm";
            string EmployeeNameTHForm = "EmployeeNameTHForm";
            string EmployeeNameENForm = "EmployeeNameENForm";
            string PasswordForm = "PasswordForm";
            string UserGroupForm = "UserGroupForm";
            string DateFromForm = "DateFromForm";
            string DateToForm = "DateToForm";

            try
            {
                IEnumerable<User> allUser = _db.User;


                // Validation UserName
                if (UserName == null || UserName == "")
                {
                    return Json(new { isSuccess = false, errorMessage = "กรุณากรอก Username" , inputName = NameUserForm });
                }
                if (UserName.Length > 100)
                {
                    return Json(new { isSuccess = false, errorMessage = "Username ต้องไม่เกิน 100 ตัวอักษร", inputName = NameUserForm });
                }
                if (!Regex.IsMatch(UserName, "^[A-Za-z0-9]*$"))
                {
                    return Json(new { isSuccess = false, errorMessage = "Format Username ไม่ถูกต้อง", inputName = NameUserForm });
                }
                if (allUser.Where(x => x.UserName == UserName).Count() > 0)
                {
                    return Json(new { isSuccess = false, errorMessage = "Username นี้มีในระบบแล้ว", inputName = NameUserForm });
                }

                // Validation Email
                if (Email == null || Email == "")
                {
                    return Json(new { isSuccess = false, errorMessage = "กรุณากรอก Email", inputName = EmailForm });
                }
                if (Email.Length > 100)
                {
                    return Json(new { isSuccess = false, errorMessage = "Email ต้องไม่เกิน 100 ตัวอักษร", inputName = EmailForm });
                }
                if (!Regex.IsMatch(Email, "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$"))
                {
                    return Json(new { isSuccess = false, errorMessage = "Format Email ไม่ถูกต้อง", inputName = EmailForm });
                }

                // Validation Name_TH
                if (Name_TH == null || Name_TH == "")
                {
                    return Json(new { isSuccess = false, errorMessage = "กรุณากรอก ขื่อ-นามสกุล (TH)", inputName = EmployeeNameTHForm });
                }
                if (Name_TH.Length > 100)
                {
                    return Json(new { isSuccess = false, errorMessage = "ขื่อ-นามสกุล (TH) ต้องไม่เกิน 100 ตัวอักษร", inputName = EmployeeNameTHForm });
                }

                var listNameTH = Name_TH.Split(" ");

                if (!Regex.IsMatch(Name_TH, "^[ก-๏\\s]+$") || listNameTH.Length > 2)
                {
                    return Json(new { isSuccess = false, errorMessage = "Format ขื่อ-นามสกุล (TH) ไม่ถูกต้อง", inputName = EmployeeNameTHForm });
                }

                string fistNameTH = listNameTH[0];
                string lastNameTH = listNameTH[1];

                // Validation Name_EN
                if (Name_EN == null || Name_EN == "")
                {
                    return Json(new { isSuccess = false, errorMessage = "กรุณากรอก ขื่อ-นามสกุล (EN)", inputName = EmployeeNameENForm });
                }
                if (Name_TH.Length > 100)
                {
                    return Json(new { isSuccess = false, errorMessage = "ขื่อ-นามสกุล (EN) ต้องไม่เกิน 100 ตัวอักษร", inputName = EmployeeNameENForm });
                }

                var listNameEN = Name_EN.Split(" ");

                if (!Regex.IsMatch(Name_EN, "^[a-zA-Z\\s]+$") || listNameEN.Length > 2)
                {
                    return Json(new { isSuccess = false, errorMessage = "Format ขื่อ-นามสกุล (EN) ไม่ถูกต้อง", inputName = EmployeeNameENForm });
                }

                string fistNameEN = listNameEN[0];
                string lastNameEN = listNameEN[1];


                // Validation Password

                if (Password == null ||  Password == "")
                {
                    return Json(new { isSuccess = false, errorMessage = "กรุณากรอก Password", inputName = PasswordForm });
                }
                if (Password.Length < 8)
                {
                    return Json(new { isSuccess = false, errorMessage = "Password ต้องไม่น้อยกว่า 8 ตัวอักษร", inputName = PasswordForm });
                }

                if (Password.Length > 13)
                {
                    return Json(new { isSuccess = false, errorMessage = "Password ต้องไม่เกิน 13 ตัวอักษร", inputName = PasswordForm });
                }

                if (!Regex.IsMatch(Password, "(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*_+\\-=.])^[a-zA-Z0-9!@#$%^&*_+\\-=.]*$"))
                {
                    return Json(new { isSuccess = false, errorMessage = "Format Password ไม่ถูกต้อง", inputName = PasswordForm });
                }

                // Validation GroupId
                var userGroup = _db.UserGroup;

                if (userGroup.Count() == 0)
                {
                    return Json(new { isSuccess = false, errorMessage = "ไม่พบข้อมูลกลุ่มผู้ใช้ระบบ", inputName = UserGroupForm });
                }

                if (userGroup.Where(x => x.ID == GroupId).Count() == 0)
                {
                    return Json(new { isSuccess = false, errorMessage = "ไม่พบข้อมูลกลุ่มผู้ใช้ระบบ", inputName = UserGroupForm });
                }

                // Validation EffectiveStartDate
                if (EffectiveStartDate == null)
                {
                    return Json(new { isSuccess = false, errorMessage = "กรุณากรอก วัน/เวลาที่ใช้งานระบบได้", inputName = DateFromForm });
                }

                // Validation EffectiveFinishDate

                if (EffectiveFinishDate == null)
                {
                    return Json(new { isSuccess = false, errorMessage = "กรุณากรอก วัน/เวลาที่สิ้นสุดใช้งานระบบได้", inputName = DateToForm });
                }

                if (EffectiveStartDate >= EffectiveFinishDate)
                {
                    return Json(new { isSuccess = false, errorMessage = "กรุณากรอก วัน/เวลาที่สิ้นสุดใช้งานระบบได้ ให้มากกว่า วัน/เวลาที่ใช้งานระบบได้" });
                }

                Guid saltKey = Guid.NewGuid();

                string passwordEncrypt = Encrypt(Password, saltKey.ToString());

                User user = new User()
                {
                    UserName = UserName,
                    Email = Email,
                    Name_TH = fistNameTH,
                    Surname_TH = lastNameTH,
                    Name_EN = fistNameEN,
                    Surname_EN = lastNameEN,
                    Password = passwordEncrypt,
                    SaltKey = saltKey.ToString(),
                    GroupId = GroupId,
                    EffectiveStartDate = EffectiveStartDate.Value,
                    EffectiveFinishDate = EffectiveFinishDate.Value,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };
                if (ModelState.IsValid)
                {
                    _db.User.Add(user);
                    _db.SaveChanges();
                }

                return Json(new { isSuccess = true, errorMessage = "" });
            }
            catch (Exception ex) 
            {
                return Json(new { isSuccess = false, errorMessage = ex.Message });
            }
        }

        private string Encrypt(string clearText , string EncryptionKey)
        {
            Random rand = new Random();
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                byte[] IV = new byte[15];
                rand.NextBytes(IV);
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, IV);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(IV) + Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        private string Decrypt(string cipherText, string EncryptionKey)
        {
            byte[] IV = Convert.FromBase64String(cipherText.Substring(0, 20));
            cipherText = cipherText.Substring(20).Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, IV);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        public ActionResult UpdateUser(int Id ,string UserName, string UserNameOrginal ,string Email, string Name_TH, string Name_EN, int GroupId,
            DateTime? EffectiveStartDate, DateTime? EffectiveFinishDate)
        {
            try
            {
                IEnumerable<User> allUser = _db.User;

                var updateData = _db.User.Find(Id);

                if (updateData == null)
                {
                    return Json(new { isSuccess = false, errorMessage = "ไม่พบข้อมูลที่ต้องการอัพเดต" });
                }

                // Validation UserName
                if(UserName != UserNameOrginal)
                {
                    if (UserName == null || UserName == "")
                    {
                        return Json(new { isSuccess = false, errorMessage = "กรุณากรอก Username" });
                    }
                    if (UserName.Length > 100)
                    {
                        return Json(new { isSuccess = false, errorMessage = "Username ต้องไม่เกิน 100 ตัวอักษร" });
                    }
                    if (!Regex.IsMatch(UserName, "^[A-Za-z0-9]*$"))
                    {
                        return Json(new { isSuccess = false, errorMessage = "Format Username ไม่ถูกต้อง" });
                    }
                    if (allUser.Where(x => x.UserName == UserName).Count() > 0)
                    {
                        return Json(new { isSuccess = false, errorMessage = "Username นี้มีในระบบแล้ว" });
                    }
                }

                // Validation Email
                if (Email == null || Email == "")
                {
                    return Json(new { isSuccess = false, errorMessage = "กรุณากรอก Email" });
                }
                if (Email.Length > 100)
                {
                    return Json(new { isSuccess = false, errorMessage = "Email ต้องไม่เกิน 100 ตัวอักษร" });
                }
                if (!Regex.IsMatch(Email, "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$"))
                {
                    return Json(new { isSuccess = false, errorMessage = "Format Email ไม่ถูกต้อง" });
                }

                // Validation Name_TH
                if (Name_TH == null || Name_TH == "")
                {
                    return Json(new { isSuccess = false, errorMessage = "กรุณากรอก ขื่อ-นามสกุล (TH)" });
                }
                if (Name_TH.Length > 100)
                {
                    return Json(new { isSuccess = false, errorMessage = "ขื่อ-นามสกุล (TH) ต้องไม่เกิน 100 ตัวอักษร" });
                }

                var listNameTH = Name_TH.Split(" ");

                if (!Regex.IsMatch(Name_TH, "^[ก-๏\\s]+$") || listNameTH.Length > 2)
                {
                    return Json(new { isSuccess = false, errorMessage = "Format ขื่อ-นามสกุล (TH) ไม่ถูกต้อง" });
                }

                string fistNameTH = listNameTH[0];
                string lastNameTH = listNameTH[1];

                // Validation Name_EN
                if (Name_EN == null || Name_EN == "")
                {
                    return Json(new { isSuccess = false, errorMessage = "กรุณากรอก ขื่อ-นามสกุล (EN)" });
                }
                if (Name_TH.Length > 100)
                {
                    return Json(new { isSuccess = false, errorMessage = "ขื่อ-นามสกุล (EN) ต้องไม่เกิน 100 ตัวอักษร" });
                }

                var listNameEN = Name_EN.Split(" ");

                if (!Regex.IsMatch(Name_EN, "^[a-zA-Z\\s]+$") || listNameEN.Length > 2)
                {
                    return Json(new { isSuccess = false, errorMessage = "Format ขื่อ-นามสกุล (EN) ไม่ถูกต้อง" });
                }

                string fistNameEN = listNameEN[0];
                string lastNameEN = listNameEN[1];

                // Validation GroupId
                var userGroup = _db.UserGroup;

                if (userGroup.Count() == 0)
                {
                    return Json(new { isSuccess = false, errorMessage = "ไม่พบข้อมูลกลุ่มผู้ใช้ระบบ" });
                }

                if (userGroup.Where(x => x.ID == GroupId).Count() == 0)
                {
                    return Json(new { isSuccess = false, errorMessage = "ไม่พบข้อมูลกลุ่มผู้ใช้ระบบ" });
                }

                // Validation EffectiveStartDate
                if (EffectiveStartDate == null)
                {
                    return Json(new { isSuccess = false, errorMessage = "กรุณากรอก วัน/เวลาที่ใช้งานระบบได้" });
                }

                // Validation EffectiveFinishDate

                if (EffectiveFinishDate == null)
                {
                    return Json(new { isSuccess = false, errorMessage = "กรุณากรอก วัน/เวลาที่สิ้นสุดใช้งานระบบได้" });
                }

                if (EffectiveStartDate >= EffectiveFinishDate)
                {
                    return Json(new { isSuccess = false, errorMessage = "กรุณากรอก วัน/เวลาที่สิ้นสุดใช้งานระบบได้ ให้มากกว่า วัน/เวลาที่ใช้งานระบบได้" });
                }

                updateData.UserName = UserName;
                updateData.Email = Email;
                updateData.Name_TH = fistNameTH;
                updateData.Surname_TH = lastNameTH;
                updateData.Name_EN = fistNameEN;
                updateData.Surname_EN = lastNameEN;
                updateData.GroupId = GroupId;
                updateData.EffectiveStartDate = EffectiveStartDate.Value;
                updateData.EffectiveFinishDate = EffectiveFinishDate.Value;
                updateData.ModifiedDate = DateTime.Now;

                _db.User.Update(updateData);
                _db.SaveChanges();

                return Json(new { isSuccess = true, errorMessage = "" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, errorMessage = ex.Message });
            }
        }

        public ActionResult DeleteUser(int id)
        {
            JsonResult jsonResult = null;

            if (id == null || id == 0)
            {
                return Json(new { isSuccess = false, errorMessage = "ข้อมูลสำหรับใช้ลบไม่ถูกต้อง" });
            }
            var result = _db.User.Find(id);
            if (result == null)
            {
                return Json(new { isSuccess = false, errorMessage = "ไม่พบข้อมูลที่จะลบ" });
            }

            _db.User.Remove(result);
            _db.SaveChanges();

            return Json(new { isSuccess = true, errorMessage = ""});
        }

    }
}
