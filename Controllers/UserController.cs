using Firebase.Auth;
using Full_projeject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Full_projeject.Controllers
{
    public class UserController : Controller
    {
        private static string ApiKey = "AIzaSyA9srRlIq2pA08k2Hy9_GwUC7m0acZBgoA";
        private static string Bucket = "gs://login-91a8d.appspot.com";

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SignUp()
        {

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> SignUp(SignupModel signupModel)
        {

            // Regex pattern for Canadian and American phone numbers (NPA-NXX-XXXX)
            string phoneNumberPattern = @"^(\([0-9]{3}\) |[0-9]{3}-)[0-9]{3}-[0-9]{4}$";

            // Checking the phone number format
            if (!Regex.IsMatch(signupModel.PhoneNumber, phoneNumberPattern))
            {
                ModelState.AddModelError("PhoneNumber", "Invalid phone number. The format should be: NPA-NXX-XXXX");
                return View();
            }

            try
            {
                // firebase authentification 
                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                // create user with email and password
                var a = await auth.CreateUserWithEmailAndPasswordAsync(signupModel.Email, signupModel.Password);
                ModelState.AddModelError(string.Empty, "Please Verify Your Email!");
                // send email verification
                await auth.SendEmailVerificationAsync(a.FirebaseToken);
                return RedirectToAction("Login");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View();
        }
            
        


        [AllowAnonymous]
        [HttpGet]
        public  ActionResult Login(String returnUrl)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                   
                }

            }
            catch(Exception e)
            {
                
            }

            return View();

        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(LoginModel loginModel, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                    var a = await auth.SignInWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);
                    if(!a.User.IsEmailVerified)
                    {
                        ModelState.AddModelError(string.Empty, "Please Verify Your Email!");
                        return View();
                    }

                    string token = a.FirebaseToken;
                    var user = a.User;
                    if(token.Length > 0)
                    {
                        SignInUser(user.Email, token, loginModel.RememberMe);
                        if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                        
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Email or Password");
                    }
                    

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Email or Password");
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString()); // This is for demonstration, replace it with actual logging.
                // You may wanna send the exception message to the view instead
                //if the reason is wrong password or email
                if(e is HttpException httpException)
                {
                    if (httpException.GetHttpCode() == 400)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Email or Password");
                    }
                }
                // Add a generic error message
                ModelState.AddModelError("", "An error occurred while trying to process your request. Please try again later.");
            }

            return View();

        }

        private void SignInUser(string email, string token, bool isPersistent)
        {
            var claims = new List<Claim>();

            try
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, email));
                claims.Add(new Claim(ClaimTypes.Email, email));
                //add token
                claims.Add(new Claim(ClaimTypes.Authentication, token));
                var claimIdentity = new ClaimsIdentity(claims, "ApplicationCookie");
                var ctx = HttpContext.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent}, claimIdentity);


            }
            catch (Exception e)
            {
                throw e;
            }

        }


        

        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogOff()
        {
            try
            {
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                authenticationManager.SignOut();
            }
            catch(Exception e)
            {
            }
            return RedirectToAction("Index", "Home");
        }


        // reset password 
        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ResetPassword(string email)
        {
            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                await auth.SendPasswordResetEmailAsync(email);
                if(ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Please Check Your Email!");
                }
                return RedirectToAction("Login");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View();
        }


    }


    
}