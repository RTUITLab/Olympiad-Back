using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.ViewModels.Validations;

namespace WebApp.ViewModels
{
    //[Validator(typeof(CredentialsViewModelValidator))] TODO validation
    public class CredentialsViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
