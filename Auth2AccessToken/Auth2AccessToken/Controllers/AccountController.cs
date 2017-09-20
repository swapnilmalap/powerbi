using Auth2AccessToken.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Auth2AccessToken.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                List<KeyValuePair<string, string>> vals = new List<KeyValuePair<string, string>>();
                vals.Add(new KeyValuePair<string, string>("grant_type", "password"));
                vals.Add(new KeyValuePair<string, string>("scope", "openid"));
                vals.Add(new KeyValuePair<string, string>("resource", "https://analysis.windows.net/powerbi/api"));
                //vals.Add(new KeyValuePair<string, string>("client_id", "70dc6d78-9a0c-4747-91c9-add1da7e36f4"));
                //vals.Add(new KeyValuePair<string, string>("client_secret", "7nTqTLO/Lv3m3MreABSUVNxEbQR+vxydpNgial7ybU0="));//M5PJ3hQTEwQHXmK3JvKxqaeqyBBNPbEpbo4siFD8z/g=

                vals.Add(new KeyValuePair<string, string>("client_id", "6334f62b-b4c5-4784-8719-8d8c62905bf1"));
                vals.Add(new KeyValuePair<string, string>("client_secret", "M5PJ3hQTEwQHXmK3JvKxqaeqyBBNPbEpbo4siFD8z/g="));
                vals.Add(new KeyValuePair<string, string>("username", model.Email));
                vals.Add(new KeyValuePair<string, string>("password", model.Password));
                string TenantId = "86ba6754-ade1-48ce-ad3a-7388495377b6";
                string url = string.Format("https://login.microsoftonline.com/86ba6754-ade1-48ce-ad3a-7388495377b6/oauth2/token", TenantId);
                HttpClient hc = new HttpClient();
                HttpContent content = new FormUrlEncodedContent(vals);
                HttpResponseMessage hrm = hc.PostAsync(url, content).Result;
                string responseData = "";
                if (hrm.IsSuccessStatusCode)
                {
                    Stream data = await hrm.Content.ReadAsStreamAsync();
                    using (StreamReader reader = new StreamReader(data, Encoding.UTF8))
                    {
                        responseData = reader.ReadToEnd();
                    }
                }
                var Token = JsonConvert.DeserializeObject<AccessToken>(responseData);

                if (Token != null)
                {
                    Session["Token"] = Token;
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid username and password");
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }
    }
}