using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FromApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Deserializers;

namespace FromApi.Controllers
{
    /*public class ModelView
    {
        public List<SelectListItem> possibleFilterList;
        public SelectList possibleFilters { get; set; }
        public string SelectedFilter {get;set;}
    }*/
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public void ListsForDrop(out List<SelectListItem> possibleFilterList, out List<SelectListItem> Schemelist)
        {
            var client = new RestClient("https://mfi-dev-neu-kfg-02.azurewebsites.net/api/api/filterhelp");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            client.AddDefaultHeader("Authorization", "bc4b0be2fdd660c46df39aa418423b6d");
            //

            client.UserAgent = "youragent.com";
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);
            //Console.WriteLine(response.Content);

            dynamic data = JObject.Parse(response.Content);
            var PossibleFilters = data["data"]["possible filters"];
            var schemes = data["data"]["scheme"];
            ViewBag.PossibleFilter = PossibleFilters;
            possibleFilterList = new List<SelectListItem>();
            Schemelist = new List<SelectListItem>();
            //for (int i = 0; i < PossibleFilters.Count; i++)
            //{
            //    possibleFilterList.Add(PossibleFilters[i]["filterType"].ToString());
            //}
            foreach (var Filtername in PossibleFilters)
            {
                possibleFilterList.Add(new SelectListItem()
                {
                    Text = Filtername.filterType,
                    Value = Filtername.filterType
                });
            }
            foreach (var scheme in schemes.data.schemes.data)
            {
                Schemelist.Add(new SelectListItem()
                {
                    Text = scheme.Value,
                    Value = scheme.Name
                });
            }
        }
        public IActionResult Viewreportnames()
        {
            var client = new RestClient("https://dev-kfgfly.marketforce.com/api/api/reportnames");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            client.UserAgent = "youragent.com";
            client.AddDefaultHeader("Authorization", "bc4b0be2fdd660c46df39aa418423b6d");
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);
            //Console.WriteLine(response.Content);
            var model = response.Content;
            //dynamic data = JsonConvert.DeserializeObject(model);
            dynamic data = JObject.Parse(model);

            ViewBag.Json = data;
            return View();
        }
        [HttpPost]
        public IActionResult FilteredData(IFormCollection form)
        {
            List<SelectListItem> possibleFilterList = new List<SelectListItem>();
            List<SelectListItem> Schemelist = new List<SelectListItem>();
            ListsForDrop(out possibleFilterList, out Schemelist);
            ViewBag.SchemeList = Schemelist;
            ViewBag.PossibleFilterList = possibleFilterList;
            string programValue = form["program"].ToString();
            string schemeValue = form["scheme"].ToString();
            var client = new RestClient("https://mfi-dev-neu-kfg-02.azurewebsites.net/api/api/filterhelp?filter=" + programValue + "&program=" + schemeValue);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            
            client.AddDefaultHeader("Authorization", "bc4b0be2fdd660c46df39aa418423b6d");
            //

            client.UserAgent = "youragent.com";
            request.AlwaysMultipartFormData = true;
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            IRestResponse response = client.Execute(request);
            //Console.WriteLine(response.Content);
            try
            {
                
                
                //string json = JsonConvert.SerializeObject(PrettyJson);
                dynamic Parsedata = JObject.Parse(response.Content);
                var maindata = Parsedata["data"];
                if (maindata.HasValues == true)
                { 
                    var PrettyJson = response.Content;
                    ViewBag.PrettyJson = PrettyJson;
                }
                else { ViewBag.Data = "Data Is Null or There is an Error"; }

            }
            catch (Exception ex)
            {
                ViewBag.Data = ex.Message;
            }

            return View();
        }


        public IActionResult FilteredData()
        {
            List<SelectListItem> possibleFilterList = new List<SelectListItem>();
            List<SelectListItem> Schemelist = new List<SelectListItem>();
            ListsForDrop(out possibleFilterList, out Schemelist);
            ViewBag.SchemeList = Schemelist;
            ViewBag.PossibleFilterList = possibleFilterList;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}