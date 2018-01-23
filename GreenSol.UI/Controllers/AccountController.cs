using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using GreenSol.UI.Models;
using GreenSol.UI.Infrastructure;
using GreenSol.Domain.Entities;
using GreenSol.Domain.Abstract;
using System.Web.Routing;
using System.Collections.Generic;

namespace GreenSol.UI.Controllers
{
    [Authorize]
    [Authorize(Roles = "Administrators")]
    public class AccountController : Controller
    {
        private IAlbumRepository repository;

        public AccountController(IAlbumRepository repo)
        {
            this.repository = repo;
        }

        private AppUserManager _userManager;

        public AppUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
            private set { _userManager = value; }
        }

        //returns an implementation of the IAuthenticationManager interface that is responsible for performing common authentication options
        private IAuthenticationManager AuthManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            //If the user is an Administrator, he can manage all the accounts
            bool isAdmin = HttpContext.User.IsInRole("Administrators");
            if (isAdmin)
                return View(UserManager.Users);
            //else, the user can only mange his own
            return View(UserManager.Users.AsEnumerable().Where(u => u.UserName == HttpContext.User.Identity.GetUserName())); 
        }

        #region CREATE
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        [AllowAnonymous]       
        [HttpPost]
        public async Task<ActionResult> Create(CreateModel model)
        {
            bool isAdmin = HttpContext.User.IsInRole("Administrators");
            if (ModelState.IsValid)
            {
                AppUser user = await UserManager.FindByEmailAsync(model.Email);

                if (user != null)
                    ModelState.AddModelError("Email", "This email has been taken");
                else
                {
                    user = new AppUser { UserName = model.Name, Email = model.Email };

                    IdentityResult result = await UserManager.CreateAsync(user, model.Password);    //

                    if (result.Succeeded)
                    {
                        if (!isAdmin)
                            return RedirectToAction("Login");
                        else
                            return RedirectToAction("Index");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
            }
            return View(model);
        }
        #endregion

        #region DELETE
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            AppUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] { "User Not Found" });
            }
        }
        #endregion

        #region EDIT
        [AllowAnonymous]
        public async Task<ActionResult> Edit(string id)
        {
            AppUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Edit(string id, string email, string password)
        {
            bool isAdmin = HttpContext.User.IsInRole("Administrators");

            //validate the user data to ensure that I don’t violate the custom policies I defined before
            //(They are password and user validators) 
            AppUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                //i have to change the value of the Email property before i perform the validation 
                //because the ValidateAsync method only accepts instances of the user class
                user.Email = email;
                IdentityResult validEmail = await UserManager.UserValidator.ValidateAsync(user);
                if (!validEmail.Succeeded)
                {
                    AddErrorsFromResult(validEmail);
                }

                IdentityResult validPass = null;
                if (password != string.Empty)
                {
                    validPass = await UserManager.PasswordValidator.ValidateAsync(password);
                    if (validPass.Succeeded)
                    {
                        //ASP.NET Identity stores hashes of passwords, rather than the passwords themselves
                        user.PasswordHash = UserManager.PasswordHasher.HashPassword(password);
                    }
                    else
                    {
                        AddErrorsFromResult(validPass);
                    }
                }

                if ((validEmail.Succeeded && validPass == null)
                    || (validEmail.Succeeded && password != string.Empty && validPass.Succeeded))
                {
                    //Changes to the user class are not stored in the database until the UpdateAsync method is called
                    IdentityResult result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        if (isAdmin)
                            return RedirectToAction("Index");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(user);
        }
        #endregion

        #region LOGIN
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return View("Error!", new string[] { "You're already logged in!" });
            }
            ViewBag.ReturnUrl = returnUrl == null ? Url.Action("Index", "Home") : returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel details, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await UserManager.FindAsync(details.Name, details.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid name or password.");
                }
                //
                else
                {
                    //If the FindAsync method does return an AppUser object, 
                    //then I need to create the cookie that the browser will send in subsequent requests to show they are authenticated. H
                    ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

                    ident.AddClaims(LocationClaimsProvider.GetClaims(ident));

                    ident.AddClaims(ClaimsRoles.CreateRolesFromClaims(ident));

                    //I will use Email-type claim to identify users (I did this in Global.asax/Application_Start()),
                    //external authority will give me this claim
                    //but LOCAL AUTHORIRY will not, so I manually create one 
                    ident.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", user.Email));

                    //invalidate any existing authentication cookies and create the new one
                    AuthManager.SignOut();
                    AuthManager.SignIn(
                        new AuthenticationProperties
                        {
                            //Keep the cookie persistent, 
                            //the user doesn't have to authenticate again when starting a new session
                            IsPersistent = false
                        },
                        ident);
                    // Migrate the user's shopping cart
                    Session["CartId"] = user.UserName;
                    MigrateShoppingCart(user.UserName);
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(details);
        }

        #endregion

        #region LOGIN GOOGLE
        [HttpPost]
        [AllowAnonymous]
        public ActionResult GoogleLogin(string returnUrl)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleLoginCallback", new { returnUrl = returnUrl })
            };
            //causes ASP.NET Identity to respond to an unauthorized error by redirecting the user to the Google authentication page, 
            //rather than the one defined by the application
            HttpContext.GetOwinContext().Authentication.Challenge(properties, "Google");

            return new HttpUnauthorizedResult();
        }

        [AllowAnonymous]
        public async Task<ActionResult> GoogleLoginCallback(string returnUrl)
        {
            ExternalLoginInfo loginInfo = await AuthManager.GetExternalLoginInfoAsync();
            //Locate the user based on the value of the ExternalLoginInfo.Login property,
            //returns an AppUser object if the user has been authenticated with the application before
            AppUser user = await UserManager.FindAsync(loginInfo.Login);
            
            //If this's the first time login:
            //Create a new AppUser object, populate it with values, and save it to the database. 
            //I also save details of how the user logged in so that I can find them next time
            if (user == null)
            {
                user = new AppUser
                {
                    Email = loginInfo.Email,
                    UserName = loginInfo.DefaultUserName,
                    //City = Cities.LONDON,
                    //Country = Countries.UK
                };

                IdentityResult result = await UserManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return View("Error", result.Errors);
                }
                else
                {
                    result = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
            }
            //generate an identity the user, 
            ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            
            //Copy the claims provided by Google, this may cause error "Sequence contains more than one matching element" 
            //due to duplicated claims provided by both LOCAL AUTHORITY AND EXTERNAL AUTHORITY (Google)
            //To avoid, make sure that every types of claim has ONLY ONE ISSUER 
            //or a specify a claim (that you know which is unique in the claim collection) to be the idenifier
            //by add this line in Global.asax/Application_Start()
            //AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Email; //presume that Email is a unique.
            ident.AddClaims(loginInfo.ExternalIdentity.Claims);     /*this will also add Email type claim*/
            
            //And create an authentication cookie so that the application knows the user has been authenticated
            //AuthManager.SignOut();
            AuthManager.SignIn(
                new AuthenticationProperties
                {
                    IsPersistent = false
                },
                ident);

            // Migrate the user's shopping cart
            MigrateShoppingCart(user.UserName);

            return Redirect(returnUrl ?? "/");
        }
        #endregion

        #region LOGOUT
        [HttpPost]
        //[Authorize]
        [AllowAnonymous]
        public ActionResult Logout()
        {
            AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            Session.Clear();

            return RedirectToAction("Index", "Home");
        }
        #endregion

        private void MigrateShoppingCart(string UserName)
        {           
            // Associate shopping cart items with logged-in user
            var cart = ShoppingCart.GetCart(repository, this);
            cart.MigrateCart(UserName);

            Session[ShoppingCart.CartSessionKey] = UserName;
        }
    }
}
