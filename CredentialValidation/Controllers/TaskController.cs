using CredentialValidation.Models;
using CredentialValidation.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CredentialValidation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : Controller
    {
        [HttpPost]
        public IActionResult Validate(UserModel user)
        {
            ResponseModel response =  ValidateService.Validate(user);

            ObjectResult result = new ObjectResult(response);

            return result;

        }
    }
}
