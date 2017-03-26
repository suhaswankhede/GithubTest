using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

    public class CompanyIdentity : IIdentity
    {
        public CompanyIdentity(string location, int allowance)
        {
            Location = location;
            Allowance = allowance;
        }

        public int Allowance { get; private set; }

        public string AuthenticationType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Location { get; private set; }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
    public class claimsComponeIedntiry : ClaimsIdentity
    {
        public claimsComponeIedntiry(string location, int allowance)
        {
            AddClaim(new Claim(ClaimTypes.Locality, location));
            AddClaim(new Claim("Allowance", allowance.ToString()));
        }
    }
    public class CustomClaimsTransformer : ClaimsAuthenticationManager
    {
        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            string nameClaimValue = incomingPrincipal.Identity.Name;
            if (string.IsNullOrEmpty(nameClaimValue))
            {
                throw new SecurityException("A user with no name?");
            }
            return CreateClaimsPrincipal(nameClaimValue);
            //return base.Authenticate(resourceName, incomingPrincipal);
        }

        private ClaimsPrincipal CreateClaimsPrincipal(string userName)
        {
            bool likesJavaToo = false;
            if (userName.IndexOf("andras", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                likesJavaToo = true;
            }
            List<Claim> claimCollection = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim("likesJavaToo",likesJavaToo.ToString())

                };

            return new ClaimsPrincipal(new ClaimsIdentity(claimCollection, "Custom"));
        }
    }
    public class CustomAuthorisationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {

            string resource = context.Resource.First().Value;
            string action = context.Action.First().Value;
            if(action == "Show" && resource == "Code")
            {
                bool likeJava = context.Principal.HasClaim("likesJavaToo", "True");
                return !likeJava;
            }
            return false;
            //return base.CheckAccess(context);

        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            //ClaimsIntro();
            //WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
            SetCurrentPrincipal();
            //ShowMeTheCode();
            UseCurrentPrincipal();
            Console.Read();
        }
        [ClaimsPrincipalPermission(SecurityAction.Demand,Operation ="Show", Resource ="Code")]
        private static void ShowMeTheCode()
        {
            Console.WriteLine("Console.WriteLine");
        }

        private static void UseCurrentPrincipal()
        {
            ShowMeTheCode();
        }
        private static void SetCurrentPrincipal()
        {
            //Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            WindowsPrincipal incomingPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            Thread.CurrentPrincipal = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.ClaimsAuthenticationManager.Authenticate("none", incomingPrincipal);

        }

        private static void ClaimsIntro()
        {
            Setup();
            CheckCompatibility();
            CheckNewClaimsUsage();

        }

        private static void CheckNewClaimsUsage()
        {
            ClaimsPrincipal newClaimPrincial = Thread.CurrentPrincipal as ClaimsPrincipal;
            Claim newClaim = newClaimPrincial.FindFirst(ClaimTypes.Name);
            Console.WriteLine(newClaim.Value);

        }

        private static void CheckCompatibility()
        {
            IPrincipal currentPrincipal = Thread.CurrentPrincipal;
            Console.WriteLine(currentPrincipal.Identity.Name);
            Console.WriteLine(currentPrincipal.IsInRole("IT"));

        }

        private static void Setup()
        {
            IList<Claim> claimCollection = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Andras")
                , new Claim(ClaimTypes.Country, "Sweden")
                , new Claim(ClaimTypes.Gender, "M")
                , new Claim(ClaimTypes.Surname, "Nemes")
                , new Claim(ClaimTypes.Email, "hello@me.com")
                , new Claim(ClaimTypes.Role, "IT")
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claimCollection, "PSO",ClaimTypes.Email,ClaimTypes.Role);
            Console.WriteLine(claimsIdentity.IsAuthenticated);

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            Thread.CurrentPrincipal = claimsPrincipal;
        }
    }
}
