using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Server.IndigoServices
{
    public class CustomUserNamePasswordValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            AppLogger.Message(String.Format("CustomUserNamePassword.Validate {0} {1}", userName, password));
        }
    }
}
