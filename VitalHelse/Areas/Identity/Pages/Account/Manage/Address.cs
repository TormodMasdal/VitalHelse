using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VitalHelse.Models;

namespace VitalHelse.Areas.Identity.Pages.Account.Manage;

public class AddressModel : PageModel
{
    private readonly UserManager<AspNetUsers> _userManager;
    private readonly SignInManager<AspNetUsers> _signInManager;

    public AddressModel(
        UserManager<AspNetUsers> userManager,
        SignInManager<AspNetUsers> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    [TempData]
    public string? StatusMessage { get; set; }

    [BindProperty] 
    public InputModel Input { get; set; } = default!;

    public class InputModel
    {
        [Display(Name = "Adresse")] public string? Address { get; set; }

        [MaxLength(4), MinLength(4)]
        [Display(Name = "Postnummer")]
        public string? PostalCode { get; set; }

        [Display(Name = "Poststed")] public string? PostalPlace { get; set; }
    }
    
    private async Task LoadAsync(AspNetUsers user)
    {
        Input = new InputModel
        {
            Address = user.Address,
            PostalCode = user.PostalCode,
            PostalPlace = user.PostalPlace,
        };
    }
    

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }
        await LoadAsync(user);
        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }
        
        user.Address = Input.Address;
        user.PostalCode = Input.PostalCode;
        user.PostalPlace = Input.PostalPlace;
        
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            StatusMessage = "En uventet feil oppsto ved lagring av brukerdata";
            return RedirectToPage();
        }
        
        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Profilen har blitt oppdatert";
        return RedirectToPage();
    }

}