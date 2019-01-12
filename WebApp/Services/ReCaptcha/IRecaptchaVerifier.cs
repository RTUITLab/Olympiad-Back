using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Services.ReCaptcha
{
    public interface IRecaptchaVerifier
    {
        Task<RecaptchaResult> Check(string token, string ip);
    }
}
