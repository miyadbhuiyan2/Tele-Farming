using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tele_Farming.Data;
using Tele_Farming.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Tele_Farming.Controllers
{
    public class FarmersController : Controller
    {
        private readonly TeleFarmingContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private static int FARMER_ID = 0;

        public FarmersController(TeleFarmingContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult LandingPage()
        {    
            return View();
        }

        public IActionResult SelectNavigationTypePage()
        {
            return View();
        }


        public IActionResult PasswordReset()
        {
            return View();
        }

        public async Task<IActionResult> SendCode( Farmer f )
        {
            if( f.email != null || f.contact_number != null )
            {
                Farmer farmer;
                if (f.email != null)
                {
                    farmer = await _context.Farmer.Where(x => x.email == f.email).FirstOrDefaultAsync();
                    Random random = new System.Random();
                    int code = random.Next(100000, 999999);

                    farmer.PasswordResetCode = code.ToString();

                    _context.Update(farmer);
                    await _context.SaveChangesAsync();

                    SendPasswordResetEmail(farmer.email, farmer.name, code.ToString());
                }
                else
                {
                    farmer = await _context.Farmer.Where(x => x.contact_number == f.contact_number).FirstOrDefaultAsync();
                    Random random = new System.Random();
                    int code = random.Next(100000, 999999);

                    farmer.PasswordResetCode = code.ToString();

                    _context.Update(farmer);
                    await _context.SaveChangesAsync();

                    SendOTPPasswordReset(farmer.name, farmer.contact_number, farmer.PasswordResetCode);
                }
                return RedirectToAction("VerifyPasswordReset", new { id = farmer.farmer_id });
            }
            else
            {
                return RedirectToAction("PasswordReset");
            }
        }

        public async Task<IActionResult> VerifyPasswordReset( int? id )
        {
            var farmer = await _context.Farmer.Where(x => x.farmer_id == id).FirstOrDefaultAsync();
            farmer.PasswordResetCode = null;
            return View(farmer);
        }

        public async Task<IActionResult> VerifyCodePasswordReset( Farmer f )
        {
            var farmer = await _context.Farmer.Where(x => x.farmer_id == f.farmer_id).FirstOrDefaultAsync();
            if( farmer.PasswordResetCode.Equals(f.PasswordResetCode) )
                return RedirectToAction("EnterNewPassword", new { id = farmer.farmer_id } );
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

        public void SendOTPPasswordReset( String UserName, String ContactNo, String Code )
        {
            TwilioClient.Init("AC3ae3a2818d254fc1ca59bc6cddbac2fd", "bf7b3e10c82bb7a2bbb822d53ce056de");

            if (!ContactNo.StartsWith("+88"))
                ContactNo = ContactNo.Insert(0, "+88");

            var message = MessageResource.Create(
                body: "Hi "+UserName+" Your password reset code is "+Code,
                from: new Twilio.Types.PhoneNumber("+16122940944"),
                to: new Twilio.Types.PhoneNumber(ContactNo)
            );

            Console.WriteLine(message.Sid);
        }

        public async Task<IActionResult> EnterNewPassword( int? id )
        {
            var farmer = await _context.Farmer.Where(x => x.farmer_id == id).FirstOrDefaultAsync();
            return View(farmer);
        }

        public async Task<IActionResult> EnterNewPasswordAndUpdate( Farmer f )
        {
            var farmer = await _context.Farmer.Where(x => x.farmer_id == f.farmer_id ).FirstOrDefaultAsync();

            byte[] hashedPasswordWithSalt = GenerateSaltedHash(Encoding.UTF8.GetBytes(f.password), Encoding.UTF8.GetBytes("farmer_salt"));
            farmer.password = Convert.ToBase64String(hashedPasswordWithSalt);

            _context.Update(farmer);
            await _context.SaveChangesAsync();

            return RedirectToAction("SignIn");
        }


        public async Task<IActionResult> MeetingFailed(int? id, int? f_id)
        {

            var ID = HttpContext.Session.GetInt32("F_ID");
            if (ID == null || ID != f_id)
                return RedirectToAction("SignIn");

            FARMER_ID = (int)ID;
            ViewData["farmer_id"] = FARMER_ID;

            Complain complain = new Complain();

            complain.time = DateTime.Now;
            complain.is_resolved = 0;
            complain.category = "Meeting Failed";

            complain.MeetingFailure = new MeetingFailure();
            complain.MeetingFailure.meeting_id = (int)id;

            _context.Add(complain);
            await _context.SaveChangesAsync();

            return RedirectToAction("Post", new { id = f_id });
        }


        public async Task<IActionResult> CancelMeeting(int? id, int? f_id)
        {
            var ID = HttpContext.Session.GetInt32("F_ID");
            if (ID == null || ID != f_id)
                return RedirectToAction("SignIn");

            FARMER_ID = (int)ID;
            ViewData["farmer_id"] = FARMER_ID;

            var meeting = await _context.Meeting.Where(x => x.meeting_id == id).Include(x => x.Post).Include(x => x.Specialist).FirstOrDefaultAsync();
            var farmer = meeting.Post.Farmer;

            meeting.Post.post_status = "Cancelled";
            _context.Update(meeting);
            await _context.SaveChangesAsync();

            _context.Meeting.Remove(meeting);
            await _context.SaveChangesAsync();

            return RedirectToAction("Post", new { id = farmer.farmer_id });

        }

        public async Task<IActionResult> ViewPostPending(int? id, int? f_id)
        {
            //var posts = _context.Post.Where(p => p.post_id == id).FirstOrDefault();
            //posts.Post_Time = _context.Post_Time.Where(p => p.post_id == posts.post_id).ToList();
            //posts.Meeting = _context.Meeting.Include(m => m.Specialist).Where(p => p.post_id == posts.post_id).FirstOrDefault();
            var ID = HttpContext.Session.GetInt32("F_ID");
            if (ID == null || ID != f_id)
                return RedirectToAction("SignIn");

            FARMER_ID = (int)ID;
            ViewData["farmer_id"] = FARMER_ID;


            var posts = await _context.Post
                .Where(p => p.post_id == id)
                .Include( p => p.Farmer )
                .Include(p => p.Post_Time)
                .Include(p => p.Post_Images)
                .Include(p => p.Meeting)
                .ThenInclude(p => p.Specialist)
                .FirstOrDefaultAsync();

            //posts.Meeting.Specialist = _context.Specialist.Where(p => p.specialist_id == posts.Meeting.specialist_id).FirstOrDefault();
            //using (var context = new TeleFarmingContext())
            //{
            //    var posts = context.Post
            //       .Where(post => post.post_id == id)
            //       .Include(post_time => post_time.Post_Time)
            //       .Include(meeting => meeting.Meeting)
            //       .ThenInclude(specialist => specialist.Specialist)
            //       .ToList();
            //    return View(posts);
            //}
            if (posts.Meeting != null || posts.post_status.Equals("Finished"))
            {
                return RedirectToAction("ViewPost", new { id = posts.post_id, f_id = f_id });
            }

            return View(posts);
        }

        public async Task<IActionResult> ViewPost(int? id, int? f_id)
        {
            //var posts = _context.Post.Where(p => p.post_id == id).FirstOrDefault();
            //posts.Post_Time = _context.Post_Time.Where(p => p.post_id == posts.post_id).ToList();
            //posts.Meeting = _context.Meeting.Include(m => m.Specialist).Where(p => p.post_id == posts.post_id).FirstOrDefault();

            //posts.Meeting.Specialist = _context.Specialist.Where(p => p.specialist_id == posts.Meeting.specialist_id).FirstOrDefault();
            //using (var context = new TeleFarmingContext())
            //{
            //    var posts = context.Post
            //       .Where(post => post.post_id == id)
            //       .Include(post_time => post_time.Post_Time)
            //       .Include(meeting => meeting.Meeting)
            //       .ThenInclude(specialist => specialist.Specialist)
            //       .ToList();
            //    return View(posts);
            //}
            var ID = HttpContext.Session.GetInt32("F_ID");
            if (ID == null || ID != f_id)
                return RedirectToAction("SignIn");

            FARMER_ID = (int)ID;
            ViewData["farmer_id"] = FARMER_ID;


            var posts = await _context.Post
                 .Where(p => p.post_id == id)
                 .Include( p => p.Farmer )
                 .Include(p => p.Post_Time)
                 .Include(p => p.Post_Images)
                 .Include(p => p.Meeting)
                 .ThenInclude( p => p.MeetingFailure )
                 .Include(p => p.Meeting)
                 .ThenInclude(p => p.Specialist)
                 .FirstOrDefaultAsync();

            if ( posts.Meeting != null || posts.post_status.Equals("Finished") )
            {
                return View(posts);
            }

            return RedirectToAction("ViewPostPending", new { id = posts.post_id, f_id = f_id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post(Combined_Read_Write_Posts c)
        {
            var ID = HttpContext.Session.GetInt32("F_ID");
            if (ID == null || ID != c.Post.farmer_id)
                return RedirectToAction("SignIn");

            FARMER_ID = (int)ID;
            ViewData["farmer_id"] = FARMER_ID;

            //for (int i = 0; i < c.Post.Post_Time.Count; i++)
            //{
            //    if (c.Post.Post_Time[i].time.CompareTo(DateTime.Now) < 0)
            //    {
            //        c.Post.Post_Time.RemoveAt(i);
            //    }
            //}

            //for (int i = 0; i < c.Post.Post_Time.Count; i++)
            //{
            //    if (c.Post.Post_Time[i].time.CompareTo(DateTime.Now) < 0)
            //    {
            //        c.Post.Post_Time.RemoveAt(i);
            //    }
            //}

            //for (int i = 0; i < c.Post.Post_Time.Count; i++)
            //{
            //    if (c.Post.Post_Time[i].time.CompareTo(DateTime.Now) < 0)
            //    {
            //        c.Post.Post_Time.RemoveAt(i);
            //    }
            //}

            c.Post.post_time = DateTime.Now;
            _context.Add(c.Post);
            await _context.SaveChangesAsync();

            for ( int i = 0; i < c.Post.Post_Images.Count; i++)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(c.Post.Post_Images[i].ImageFile.FileName);
                string extension = Path.GetExtension(c.Post.Post_Images[i].ImageFile.FileName);
                fileName = fileName + "post" + c.Post.farmer_id + extension;
                string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await c.Post.Post_Images[i].ImageFile.CopyToAsync(fileStream);
                }
                c.Post.Post_Images[i].image_path = fileName;
                c.Post.Post_Images[i].post_id = c.Post.post_id;
            }

            //var posts = _context.Post.Include("Farmer").Where(p => p.farmer_id == c.Post.farmer_id).ToList();
            
            //List<Post_Time> post_time_list = new List<Post_Time>();
            //c.Post.Post_Time = post_time_list;

            //Post_Time post_time = new Post_Time();
            //c.Post.Post_Time.Add(post_time);

            foreach (var x in c.Post.Post_Time)
            {
                if (x.time.CompareTo(DateTime.Now) < 0)
                    continue;
                x.post_id = c.Post.post_id;
            }

            //foreach (var x in c.Post.Post_Images)
            //{
            //    x.post_id = c.Post.post_id;
            //}

            //c.Post.Post_Time[0].time = c.time;
            //foreach (var x in c.Post.Post_Time)
            //{
            //    _context.Add(x);
            //}

            //foreach (var x in c.Post.Post_Images)
            //{
            //    _context.Add(x);
            //}
            //_context.Update(c.Post);

            await _context.SaveChangesAsync();
            return RedirectToAction("Post", new { id = c.Post.farmer_id });
        }


        public async Task<IActionResult> Post(int? id, String type)
        {
            //var farmer = _context.Farmer.Include(m => m.Post).Single(m => m.farmer_id == id);
            //IList<Post> post_list = _context.Post.Include(c => c.Farmer).Where(c => c.farmer_id == id).ToList();

            //var posts = _context.Post.Include("Farmer").Where(p => p.farmer_id == id).ToList();
            //posts.Reverse();
            //var farmers = _context.Farmer.Include("Post").Where(p => p.farmer_id == id).FirstOrDefault();
            var ID = HttpContext.Session.GetInt32("F_ID");
            if (ID == null || ID != id)
                return RedirectToAction("SignIn");

            FARMER_ID = (int)ID;
            ViewData["farmer_id"] = FARMER_ID;
     

            if (type == null)
                ViewData["type"] = "Pending";
            else
                ViewData["type"] = type;
        

            var farmer =
                await _context.Farmer
                .Where(p => p.farmer_id == id)
                .Include(m => m.Post.Where( x => x.post_status.Equals(ViewData["type"]) ) )
                .ThenInclude(m => m.Post_Time)
                .Include(m => m.Post)
                .ThenInclude(m => m.Post_Images)
                .FirstOrDefaultAsync();

            farmer.Post.Reverse();

            Combined_Read_Write_Posts c = new Combined_Read_Write_Posts();
            c.Farmer = farmer;
            c.Post = new Post();

            List<Post_Time> lpt = new List<Post_Time>(3);
            for (int i = 0; i < 3; i++)
                lpt.Add(new Post_Time());
            c.Post.Post_Time = lpt;

            List<Post_Images> lpI = new List<Post_Images>(5);
            for (int i = 0; i < 5; i++)
                lpI.Add(new Post_Images());
            c.Post.Post_Images = lpI;

            return View( c );
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login( Combined_Read_Write_Posts objUser )
        {
            if (objUser.Farmer.password == null || objUser.Farmer.email == null)
                return RedirectToAction("SignIn");

            byte[] hashedPasswordWithSalt = GenerateSaltedHash(Encoding.UTF8.GetBytes(objUser.Farmer.password), Encoding.UTF8.GetBytes("farmer_salt"));
            String inputPasswordHashed = Convert.ToBase64String(hashedPasswordWithSalt);


            var farmer = await _context.Farmer
                .FirstOrDefaultAsync( m => m.email == objUser.Farmer.email && m.password == inputPasswordHashed );
            if( farmer == null)
            {
                return RedirectToAction("SignIn");
            }

            HttpContext.Session.SetInt32("F_ID", farmer.farmer_id);
            FARMER_ID = farmer.farmer_id;
            ViewData["farmer_id"] = FARMER_ID;
            return RedirectToAction("Post", "Farmers", new { id = farmer.farmer_id });
        }


        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        public IActionResult SignOutUser(int? id)
        {
            var ID = HttpContext.Session.GetInt32("F_ID");
            if (ID == null || ID != id)
                return RedirectToAction("SignIn");

            HttpContext.Session.Remove("F_ID");
            return RedirectToAction("SignIn");
        }

        public async Task<IActionResult> ProfilePage( int? id )
        {

            var ID = HttpContext.Session.GetInt32("F_ID");
            if (ID == null || ID != id)
                return RedirectToAction("SignIn");

            FARMER_ID = (int)ID;
            ViewData["farmer_id"] = FARMER_ID;

            if (id == null)
            {
                return NotFound();
            }

            var farmer = await _context.Farmer.FindAsync(id);
            if (farmer == null)
            {
                return NotFound();
            }
            return View(farmer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProfilePage(int id, [Bind("farmer_id,name,contact_number,bkash_number,email,password,ImageFile")] Farmer farmer)
        {
            var ID = HttpContext.Session.GetInt32("F_ID");
            if (ID == null || ID != id)
                return RedirectToAction("SignIn");

            FARMER_ID = (int)ID;
            ViewData["farmer_id"] = FARMER_ID;

            if (id != farmer.farmer_id)
            {
                return NotFound();
            }
           
                try
                {
                    var PrevData = await _context.Farmer.FindAsync(farmer.farmer_id);
                    string PreviousImageName = PrevData.profile_picture_path;

                    if (PreviousImageName != null)
                    {
                        farmer.profile_picture_path = PreviousImageName;
                    }

                    if (farmer.password == null)
                    {
                        farmer.password = PrevData.password;
                    }
                    else
                    {
                        byte[] hashedPasswordWithSalt = GenerateSaltedHash(Encoding.UTF8.GetBytes(farmer.password), Encoding.UTF8.GetBytes("farmer_salt"));
                        farmer.password = Convert.ToBase64String(hashedPasswordWithSalt);
                    }

                    _context.Farmer.Remove(PrevData);

                    if (farmer.ImageFile != null)
                    {

                        if (PreviousImageName != null)
                        {
                            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Image", PreviousImageName);
                            if (System.IO.File.Exists(imagePath))
                                System.IO.File.Delete(imagePath);
                        }
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(farmer.ImageFile.FileName);
                        string extension = Path.GetExtension(farmer.ImageFile.FileName);
                        farmer.profile_picture_path = fileName = fileName + "farmer" + farmer.farmer_id.ToString() + extension;
                        string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await farmer.ImageFile.CopyToAsync(fileStream);
                        }

                    }



                    _context.Update(farmer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FarmerExists(farmer.farmer_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction( "Post", "Farmers", new { id = farmer.farmer_id } );
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






















        // GET: Farmers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Farmer.ToListAsync());
        }

        // GET: Farmers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmer = await _context.Farmer
                .FirstOrDefaultAsync(m => m.farmer_id == id);
            if (farmer == null)
            {
                return NotFound();
            }

            return View(farmer);
        }

        // GET: Farmers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Farmers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("farmer_id,name,contact_number,bkash_number,email,password,profile_picture_path")] Farmer farmer)
        {
            //if (ModelState.IsValid)
            //{
                byte[] hashedPasswordWithSalt = GenerateSaltedHash( Encoding.UTF8.GetBytes(farmer.password), Encoding.UTF8.GetBytes("farmer_salt") );
                farmer.password = Convert.ToBase64String(hashedPasswordWithSalt);

                farmer.profile_picture_path = "default.png";

                _context.Add(farmer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SignIn));
            //}
            //return View(farmer);
        }

        // GET: Farmers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmer = await _context.Farmer.FindAsync(id);
            if (farmer == null)
            {
                return NotFound();
            }
            return View(farmer);
        }

        // POST: Farmers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("farmer_id,name,contact_number,bkash_number,email,password,profile_picture_path")] Farmer farmer)
        {
            if (id != farmer.farmer_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(farmer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FarmerExists(farmer.farmer_id))
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
            return View(farmer);
        }

        // GET: Farmers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmer = await _context.Farmer
                .FirstOrDefaultAsync(m => m.farmer_id == id);
            if (farmer == null)
            {
                return NotFound();
            }

            return View(farmer);
        }

        // POST: Farmers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var farmer = await _context.Farmer.FindAsync(id);
            _context.Farmer.Remove(farmer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FarmerExists(int id)
        {
            return _context.Farmer.Any(e => e.farmer_id == id);
        }
    }
}
