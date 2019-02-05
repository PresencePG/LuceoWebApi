using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Web.Http;
using Winston.Data;
using Winston.Data.BOSImport;
using System.Text;
using System.Xml;

namespace LuceoWebApi.Controllers
{
    public class LuceoController : ApiController
    {
        // GET: Luceo
        public string Get(string url)
        {
            string user = System.Configuration.ConfigurationManager.AppSettings["user"];
            string pass = System.Configuration.ConfigurationManager.AppSettings["pass"];
            string luceouri = System.Configuration.ConfigurationManager.AppSettings["luceouri"];
            if(url.Contains("attachment"))
            {
                luceouri = luceouri.Replace("https://ws-winstonretail.luceosolutions.com/rest/candidate/?params=", "https://ws-winstonretail.luceosolutions.com/rest/candidate/");
            }
            else
            {
                int id;
                url = url.Replace("/", "");
                if(url.Length>5)
                url = url.Remove(5);
                bool isNumeric = int.TryParse(url, out id);
                if (isNumeric)
                    luceouri = luceouri.Replace("https://ws-winstonretail.luceosolutions.com/rest/candidate/?params=", "https://ws-winstonretail.luceosolutions.com/rest/candidate/");
            }
            
            Uri uri = new Uri(luceouri+url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.9)"; // Pretend to be FireFox so the host will treat us with respect
            request.Accept = "application/xml";

            // Authentication			
            var cache = new CredentialCache();
            cache.Add(uri, "Basic", new NetworkCredential(user, pass));
            request.Credentials = cache;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                   return  reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            throw new WebException(reader.ReadToEnd(), ex);
                        }
                    }
                }
                throw ex;


                //https://ws-winstonretail.luceosolutions.com/rest/candidate/51269/
            }
        }
    }
}
