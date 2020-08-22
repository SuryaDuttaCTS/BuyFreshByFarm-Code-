using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyBook.Areas.Identity.Pages.Account.Manage
{
    public class SetPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;
        public SetPasswordModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public static string input_id {get; set;}

        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);

            input_id = id;
            if (user == null && objFromDb==null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var hasPassword = false;
            if (user != null)
            {
                hasPassword = await _userManager.HasPasswordAsync(user);
            }
           
            if (hasPassword)
            {
                return RedirectToPage("./ChangePassword");
            }

            return Page();
        }

        //public async Task<IActionResult> OnGetAsync(string? id)
        //{
        //    var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);


        //    if (objFromDb == null)
        //    {
        //        return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        //    }

        //    var hasspassword = await _userManager.HasPasswordAsync(objFromDb);

        //    if (hasspassword)
        //    {
        //        return RedirectToPage("./ChangePassword");
        //    }

        //    return Page();
        //}






        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || input_id == null)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null && input_id == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == input_id);

            input_id = null;

            if (user != null)
            { var addPasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword);
                if (!addPasswordResult.Succeeded)
                {
                    foreach (var error in addPasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your password has been set.";

                return RedirectToPage();
            }
            else
            { 

                await _userManager.RemovePasswordAsync(objFromDb);

                var addPasswordResult = await _userManager.AddPasswordAsync(objFromDb, Input.NewPassword);




                if (!addPasswordResult.Succeeded)
                {
                    foreach (var error in addPasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }

                await _signInManager.RefreshSignInAsync(objFromDb);
                StatusMessage = "Your password has been set.";

                return RedirectToPage();

            }



            }
    }
}
