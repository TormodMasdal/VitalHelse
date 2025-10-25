using VitalHelse.Data;
using VitalHelse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace VitalHelse.Controllers;

public class ProfileController : Controller
{
    public IActionResult privateCustomer()
    {
        return View();
    }
    
}