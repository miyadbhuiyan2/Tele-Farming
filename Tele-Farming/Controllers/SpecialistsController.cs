using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tele_Farming.Data;
using Tele_Farming.Models;

using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Security.Cryptography;
using System.Net.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Tele_Farming.Controllers
{

    
    public class SpecialistsController : Controller
    {
        private readonly TeleFarmingContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private static int SPECIALIST_ID = 0;

        public SpecialistsController(TeleFarmingContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }



        //public async Task<IActionResult> ApproveReject(int? id, int? result)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    Specialist specialist = await _context.Specialist.FindAsync(id);
        //    if (specialist != null)
        //    {
        //        specialist.is_approved = (int)result;
        //        _context.Update(specialist);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction("AdminJoinRequests", "Admins");
        //    }
        //    else
        //        return NotFound();

        //    //return View(specialist);
        //}

        public IActionResult PasswordReset()
        {
            return View();
        }

        public async Task<IActionResult> SendCode(Specialist s)
        {
            if (s.email != null || s.contact_number != null)
            {
                Specialist specialist;
                if (s.email != null)
                {
                    specialist = await _context.Specialist.Where(x => x.email == s.email).FirstOrDefaultAsync();
                    Random random = new System.Random();
                    int code = random.Next(100000, 999999);

                    specialist.PasswordResetCode = code.ToString();

                    _context.Update(specialist);
                    await _context.SaveChangesAsync();

                    SendPasswordResetEmail(specialist.email, specialist.name, code.ToString());
                }
                else
                {
                    specialist = await _context.Specialist.Where(x => x.contact_number == s.contact_number).FirstOrDefaultAsync();
                    Random random = new System.Random();
                    int code = random.Next(100000, 999999);

                    specialist.PasswordResetCode = code.ToString();

                    _context.Update(specialist);
                    await _context.SaveChangesAsync();

                    SendOTPPasswordReset(specialist.name, specialist.contact_number, specialist.PasswordResetCode);
                }
                return RedirectToAction("VerifyPasswordReset", new { id = specialist.specialist_id });
            }
            else
            {
                return RedirectToAction("PasswordReset");
            }
        }

        public async Task<IActionResult> VerifyPasswordReset(int? id)
        {
            var specialist = await _context.Specialist.Where(x => x.specialist_id == id).FirstOrDefaultAsync();
            specialist.PasswordResetCode = null;
            return View(specialist);
        }

        public async Task<IActionResult> VerifyCodePasswordReset(Specialist s)
        {
            var specialist = await _context.Specialist.Where(x => x.specialist_id == s.specialist_id).FirstOrDefaultAsync();
            if (specialist.PasswordResetCode.Equals(s.PasswordResetCode))
                return RedirectToAction("EnterNewPassword", new { id = specialist.specialist_id });
            else
                return RedirectToAction("PasswordReset");
        }

        //public async Task<IActionResult> ForgotPassword(int? id, [Bind("farmer_id,name,contact_number,bkash_number,email,password,ImageFile")] Farmer farmer)
        //{
        //    return RedirectToAction("ForgotPassword", new { id = id });
        //}

        private void SendPasswordResetEmail(string ToEmail, string UserName, string UniqueId)
        {
            // MailMessage class is present is System.Net.Mail namespace
            MailMessage mailMessage = new MailMessage("theorganizationoflife@gmail.com", ToEmail);


            // StringBuilder class is present in System.Text namespace
            StringBuilder sbEmailBody = new StringBuilder();
            sbEmailBody.Append("Dear " + UserName + ",<br/><br/>");
            sbEmailBody.Append("Password Reset Code:");
            sbEmailBody.Append("<br/>"); sbEmailBody.Append(UniqueId);
            sbEmailBody.Append("<br/><br/>");
            sbEmailBody.Append("<b>Pragim Technologies</b>");

            mailMessage.IsBodyHtml = true;

            mailMessage.Body = sbEmailBody.ToString();
            mailMessage.Subject = "Reset Your Password";
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = "telefarmingaustcse40@gmail.com",
                Password = "4Osciencelms"
            };

            smtpClient.EnableSsl = true;
            smtpClient.Send(mailMessage);
        }

        public void SendOTPPasswordReset(String UserName, String ContactNo, String Code)
        {
            TwilioClient.Init("AC3ae3a2818d254fc1ca59bc6cddbac2fd", "bf7b3e10c82bb7a2bbb822d53ce056de");

            if (!ContactNo.StartsWith("+88"))
                ContactNo = ContactNo.Insert(0, "+88");

            var message = MessageResource.Create(
                body: "Hi " + UserName + " Your password reset code is " + Code,
                from: new Twilio.Types.PhoneNumber("+16122940944"),
                to: new Twilio.Types.PhoneNumber(ContactNo)
            );

            Console.WriteLine(message.Sid);
        }

        public async Task<IActionResult> EnterNewPassword(int? id)
        {
            var specialist = await _context.Specialist.Where(x => x.specialist_id == id).FirstOrDefaultAsync();
            return View(specialist);
        }

        public async Task<IActionResult> EnterNewPasswordAndUpdate(Specialist s)
        {
            var specialist = await _context.Specialist.Where(x => x.specialist_id == s.specialist_id).FirstOrDefaultAsync();

            byte[] hashedPasswordWithSalt = GenerateSaltedHash(Encoding.UTF8.GetBytes(s.password), Encoding.UTF8.GetBytes("specialist_salt"));
            specialist.password = Convert.ToBase64String(hashedPasswordWithSalt);

            _context.Update(specialist);
            await _context.SaveChangesAsync();

            return RedirectToAction("SignIn");
        }




        public async Task<IActionResult> CancelTask( int? id, int? s_id )
        {
            var ID = HttpContext.Session.GetInt32("ID");
            if (ID == null || ID != s_id)
                return RedirectToAction("SignIn");

            SPECIALIST_ID = (int)ID;
            ViewData["specialist_id"] = SPECIALIST_ID;

            var meeting = await _context.Meeting
                .Where(x => x.meeting_id == id)
                .Include(x => x.Specialist)
                .Include( x => x.Post )
                .ThenInclude( x => x.Agent )
                .Include(x => x.Post)
                .ThenInclude(x => x.Farmer)            
                .FirstOrDefaultAsync();
            
            var specialist_id = meeting.Specialist.specialist_id;

            meeting.Post.post_status = "Pending";
            _context.Update(meeting);
            await _context.SaveChangesAsync();

            String ContactNo;
            String UserName;
            if (meeting.Post.Agent != null)
            {
                ContactNo = meeting.Post.Agent.contact_number;
                UserName = meeting.Post.Agent.name;
            }
            else
            {
                ContactNo = meeting.Post.Farmer.contact_number;
                UserName = meeting.Post.Farmer.name;
            }

            TwilioClient.Init("AC3ae3a2818d254fc1ca59bc6cddbac2fd", "bf7b3e10c82bb7a2bbb822d53ce056de");

            if (!ContactNo.StartsWith("+88"))
                ContactNo = ContactNo.Insert(0, "+88");

            var message = MessageResource.Create(
                body: "Hi " + UserName + ",\nYour post " + meeting.Post.post_id + " has been cancelled by our specialist " + meeting.Specialist.name + ". The post has been published again.",
                from: new Twilio.Types.PhoneNumber("+16122940944"),
                to: new Twilio.Types.PhoneNumber(ContactNo)
            );


            _context.Meeting.Remove(meeting);
            await _context.SaveChangesAsync();

            return RedirectToAction("TaskList", new { id = specialist_id });
        }

        public async Task<IActionResult> TaskView(int? id, int? s_id)
        {
            var ID = HttpContext.Session.GetInt32("ID");
            if (ID == null || ID != s_id)
                return RedirectToAction("SignIn");

            SPECIALIST_ID = (int)ID;
            ViewData["specialist_id"] = SPECIALIST_ID;

            var meeting = await _context.Meeting
                .Where(x => x.meeting_id == id)
                .Include( x => x.MeetingFailure )
                .Include(x => x.Specialist)
                .Include(x => x.Post)
                .ThenInclude(x => x.Farmer)
                .Include(x => x.Post)
                .ThenInclude(x => x.FarmerDetails)
                .Include(x => x.Post)
                .ThenInclude(x => x.Post_Time)
                .Include(x => x.Post)
                .ThenInclude(x => x.Post_Images)
                .FirstOrDefaultAsync();

            return View( meeting );
        }

        public async Task<IActionResult> TaskList(int? id, String? meeting_type)
        {
            var ID = HttpContext.Session.GetInt32("ID");
            if (ID == null || ID != id)
                return RedirectToAction("SignIn");

            SPECIALIST_ID = (int)ID;
            ViewData["specialist_id"] = SPECIALIST_ID;

            //ViewData["meeting_type"] = meeting_type;
            Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
            if ( meeting_type == "Pending" )
                c.MeetingList = await _context.Meeting.Where(x => x.specialist_id == id && x.meeting_status == "Pending").Include(x => x.Post ).OrderBy( x => x.meeting_time ).ToListAsync();
            else if( meeting_type == "Finished")
            {
                c.MeetingList = await _context.Meeting.Where(x => x.specialist_id == id && x.meeting_status == "Finished").Include(x => x.Post).OrderBy(x => x.meeting_time).ToListAsync();
            }
                
            
            //else if (meeting_type == "Cancelled")
            //return View(await _context.Meeting.Where(x => x.specialist_id == id).Include( x => x.Post).Where( x => x.Post.post_status == "Cancelled").ToListAsync());
            else
                c.MeetingList = await _context.Meeting.Where(x => x.specialist_id == id).Include(x => x.Post).OrderBy(x => x.meeting_time).ToListAsync();
            c.Specialist = await _context.Specialist.Where(x => x.specialist_id == id).FirstOrDefaultAsync();

            ViewData["meeting_type"] = meeting_type;
            c.MeetingList.Reverse();
            return View(c);
        }

        public async Task<IActionResult> ViewSpecialistPost(int? id, int? s_id)
        {
            var ID = HttpContext.Session.GetInt32("ID");
            if (ID == null || ID != s_id)
                return RedirectToAction("SignIn");

            SPECIALIST_ID = (int)ID;
            ViewData["specialist_id"] = SPECIALIST_ID;

            Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
            
            c.Post = await _context.Post
                .Include(m => m.Post_Time)
                .Include(m => m.Post_Images)
                .FirstOrDefaultAsync(m => m.post_id == id);

            c.Meeting = new Meeting();

            c.Specialist = await _context.Specialist
                .FirstOrDefaultAsync(m => m.specialist_id == SPECIALIST_ID);

            return View(
                c
            );
        }

        public async Task<IActionResult> SetMeeting(Post p)
        {
            var ID = HttpContext.Session.GetInt32("ID");
            if ( p == null || (ID == null || ID != p.Meeting.specialist_id ) )              
                return RedirectToAction("SignIn");

            SPECIALIST_ID = (int)ID;
            ViewData["specialist_id"] = SPECIALIST_ID;

            var specialist = await _context.Specialist.Where(x => x.specialist_id == p.Meeting.specialist_id).FirstOrDefaultAsync();

            var post = await _context.Post
                .Where(x => x.post_id == p.Meeting.post_id)
                .Include(x => x.Farmer)
                .Include(x => x.Agent)
                .FirstOrDefaultAsync();
            post.is_accepted = "1";
            post.post_status = "Accepted";
            _context.Update(post);
            await _context.SaveChangesAsync();

            

            String ContactNo;
            if (post.Agent != null )
                ContactNo = post.Agent.contact_number;
            else
                ContactNo = post.Farmer.contact_number;

            String UserName;
            if (post.Agent != null)
                UserName = post.Agent.name;
            else
                UserName = post.Farmer.name;

            if (!ContactNo.StartsWith("+88"))
                ContactNo = ContactNo.Insert(0, "+88");

            TwilioClient.Init("AC3ae3a2818d254fc1ca59bc6cddbac2fd", "bf7b3e10c82bb7a2bbb822d53ce056de");

            var message = MessageResource.Create(
                body: "Hi " + UserName + ",\nYour post " + p.Meeting.post_id + " has been accepted by our specialist " + specialist.name + ". The meeting time will be held at " + p.Meeting.meeting_time,
                from: new Twilio.Types.PhoneNumber("+16122940944"),
                to: new Twilio.Types.PhoneNumber(ContactNo)
            );

            _context.Update(p.Meeting);
            await _context.SaveChangesAsync();
            return RedirectToAction("ForumPage", new { id = p.Meeting.specialist_id });
        }


        public async Task<IActionResult> ForumPage( int? id, Combined_Read_Write_Posts c_time)
        {
            var ID = HttpContext.Session.GetInt32("ID");
            if (ID == null || ID != id)
                return RedirectToAction("SignIn");

            SPECIALIST_ID = (int) ID;
            ViewData["specialist_id"] = SPECIALIST_ID;

            Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
            var specialist = await _context.Specialist
                                        .Where(m => m.specialist_id == id)
                                        .FirstOrDefaultAsync();

            c.Specialist = specialist;

            
            if ( c_time.day != null && c_time.year != null && c_time.month != null )
            {

                if (c_time.hour == null)
                    c_time.hour = "00";

                if (c_time.minute == null)
                    c_time.minute = "00";

                if (c_time.am_pm == null)
                    c_time.am_pm = "PM";


                //if (Convert.ToInt32(c_time.minute) >= 0 && Convert.ToInt32(c_time.minute) < 31)
                //    c_time.minute = "00";
                //else
                //    c_time.minute == "30";

                String filteredTime = c_time.month + " " + c_time.day + " " + c_time.year + " " + c_time.hour + ":" + c_time.minute + ":00 " + c_time.am_pm;
                DateTime filteredDateTime = DateTime.Parse(filteredTime);

                c.PostList = await _context.Post
                                        .Where( m => m.Meeting == null && m.Category.Contains(specialist.Category) )
                                        .Include( m => m.Post_Time.Where( e => e.time.CompareTo(filteredDateTime) == 0 ) )
                                        .Include(m => m.Post_Images)
                                        .OrderBy( x => x.post_time )
                                        .ToListAsync();

                return View(c);
            }
            else
            {
                c.PostList = await _context.Post
                                        .Where(m => m.Meeting == null && m.Category.Contains(specialist.Category))
                                        .Include(m => m.Post_Time)
                                        .Include(m => m.Post_Images)
                                        .OrderBy( x => x.post_time )
                                        .ToListAsync();

                return View(c);
            }
        }

        public async Task<IActionResult> FilterPosts(Combined_Read_Write_Posts c_time)
        {
            String filteredTime = c_time.month + " " + c_time.day + " " + c_time.year +" "+c_time.hour+":"+c_time.minute+":00 "+c_time.am_pm;
            DateTime filteredDateTime = DateTime.Parse(filteredTime);

            if (c_time.day != null && c_time.month != null && c_time.year != null)
            {
                if (c_time.day.Equals("31") && c_time.day.Equals("Jan") && c_time.day.Equals("2021"))
                    return RedirectToAction("SignUp");
            }
            return RedirectToAction("SignIn");
        }

        public async Task<IActionResult> ProfilePage(int? id)
        {
            var ID = HttpContext.Session.GetInt32("ID");
            if (ID == null || ID != id)
                return RedirectToAction("SignIn");

            SPECIALIST_ID = (int)ID;
            ViewData["specialist_id"] = SPECIALIST_ID;


            if (id == null)
            {
                return NotFound();
            }

            var specialist = await _context.Specialist.FindAsync(id);
            if (specialist == null)
            {
                return NotFound();
            }
            return View(specialist);
        }

        


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProfilePage(int id, [Bind("specialist_id,name,contact_number,bkash_number,password,email,ImageFile,Category")] Specialist specialist)
        {

            var ID = HttpContext.Session.GetInt32("ID");
            if (ID == null || ID != id)
                return RedirectToAction("SignIn");

            SPECIALIST_ID = (int)ID;
            ViewData["specialist_id"] = SPECIALIST_ID;

            if (id != specialist.specialist_id)
            {
                return NotFound();
            }

                try
                {
                    var PrevData = await _context.Specialist.FindAsync(specialist.specialist_id);
                    string PreviousImageName = PrevData.profile_picture_path;
                    specialist.Category = PrevData.Category;
                    specialist.is_approved = PrevData.is_approved;

                    if (PreviousImageName != null)
                    {
                        specialist.profile_picture_path = PreviousImageName;
                    }

                    if (specialist.password == null)
                    {
                        specialist.password = PrevData.password;
                    }
                    else
                    {
                        byte[] hashedPasswordWithSalt = GenerateSaltedHash(Encoding.UTF8.GetBytes(specialist.password), Encoding.UTF8.GetBytes("specialist_salt"));
                        specialist.password = Convert.ToBase64String(hashedPasswordWithSalt);
                    }

                    _context.Specialist.Remove(PrevData);

                    if (specialist.ImageFile != null)
                    {

                        if (PreviousImageName != null)
                        {
                            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Image", PreviousImageName);
                            if (System.IO.File.Exists(imagePath))
                                System.IO.File.Delete(imagePath);
                        }
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(specialist.ImageFile.FileName);
                        string extension = Path.GetExtension(specialist.ImageFile.FileName);
                        specialist.profile_picture_path = fileName = fileName + "specialist" + specialist.specialist_id.ToString() + extension;
                        string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await specialist.ImageFile.CopyToAsync(fileStream);
                        }
                    }

                    _context.Update(specialist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialistExists(specialist.specialist_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ForumPage", new { id = specialist.specialist_id });
        }


        public IActionResult SignUp()
        {
            return View();
        }

        public IActionResult SignIn()
        {        
            return View();
        }

        public IActionResult SignOutUser( int? id )
        {
            var ID = HttpContext.Session.GetInt32("ID");
            if (ID == null || ID != id)
                return RedirectToAction("SignIn");

            HttpContext.Session.Remove("ID");
            return RedirectToAction("SignIn");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("email,password")] Specialist objUser)
        {

            if( objUser.email == null || objUser.password == null )
                return RedirectToAction("SignIn");

            byte[] hashedPasswordWithSalt = GenerateSaltedHash(Encoding.UTF8.GetBytes(objUser.password), Encoding.UTF8.GetBytes("specialist_salt"));
            String inputPasswordHashed = Convert.ToBase64String(hashedPasswordWithSalt);

            var specialist = await _context.Specialist
                .FirstOrDefaultAsync(m => m.email == objUser.email && m.password == inputPasswordHashed );
            if (specialist == null)
            {
                return View("SignIn");
            }

            HttpContext.Session.SetInt32("ID", specialist.specialist_id);
            SPECIALIST_ID = specialist.specialist_id;
            ViewData["specialist_id"] = SPECIALIST_ID;
            return RedirectToAction("ForumPage", new { id = specialist.specialist_id });
        }









        // GET: Specialists
        public async Task<IActionResult> Index()
        {
            return View(await _context.Specialist.ToListAsync());
        }

        // GET: Specialists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialist = await _context.Specialist
                .FirstOrDefaultAsync(m => m.specialist_id == id);
            if (specialist == null)
            {
                return NotFound();
            }

            return View(specialist);
        }

        // GET: Specialists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Specialists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("specialist_id,name,contact_number,bkash_number,password,email,CertificateFile,NIDFile,ImageFile,Category,is_approved")] Specialist specialist)
        {
            //if (ModelState.IsValid)
            //{

                byte[] hashedPasswordWithSalt = GenerateSaltedHash(Encoding.UTF8.GetBytes(specialist.password), Encoding.UTF8.GetBytes("specialist_salt"));
                specialist.password = Convert.ToBase64String(hashedPasswordWithSalt);
                specialist.is_approved = 0;

                _context.Add(specialist);
                await _context.SaveChangesAsync();

                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(specialist.ImageFile.FileName);
                string extension = Path.GetExtension(specialist.ImageFile.FileName);
                specialist.profile_picture_path = fileName = fileName + "specialist" + specialist.specialist_id.ToString() + extension;
                string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await specialist.ImageFile.CopyToAsync(fileStream);
                }


                fileName = Path.GetFileNameWithoutExtension(specialist.CertificateFile.FileName);
                extension = Path.GetExtension(specialist.CertificateFile.FileName);
                specialist.certificate_file_path = fileName = fileName + "specialist" + specialist.specialist_id.ToString() + extension;
                path = Path.Combine(wwwRootPath + "/Certificate/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await specialist.ImageFile.CopyToAsync(fileStream);
                }


                fileName = Path.GetFileNameWithoutExtension(specialist.NIDFile.FileName);
                extension = Path.GetExtension(specialist.NIDFile.FileName);
                specialist.nid_file_path = fileName = fileName + "specialist" + specialist.specialist_id.ToString() + extension;
                path = Path.Combine(wwwRootPath + "/Nid/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await specialist.ImageFile.CopyToAsync(fileStream);
                }

                _context.Update(specialist);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(SignIn));
            //}
            //return View(specialist);
        }






        static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] plainTextWithSaltBytes =
              new byte[plainText.Length + salt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }





















        // GET: Specialists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialist = await _context.Specialist.FindAsync(id);
            if (specialist == null)
            {
                return NotFound();
            }
            return View(specialist);
        }

        // POST: Specialists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("specialist_id,name,contact_number,bkash_number,password,email,certificate_file_path,nid_file_path,profile_picture_path,is_approved")] Specialist specialist)
        {
            if (id != specialist.specialist_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specialist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialistExists(specialist.specialist_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(specialist);
        }

        // GET: Specialists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialist = await _context.Specialist
                .FirstOrDefaultAsync(m => m.specialist_id == id);
            if (specialist == null)
            {
                return NotFound();
            }

            return View(specialist);
        }

        // POST: Specialists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialist = await _context.Specialist.FindAsync(id);
            _context.Specialist.Remove(specialist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpecialistExists(int id)
        {
            return _context.Specialist.Any(e => e.specialist_id == id);
        }
    }
}
