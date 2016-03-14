using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace AppForTestSite
{
    public class HomeController : Controller
    {
        private StringBuilder sourcePath = null;
        WebClient webClient = null;
        byte[] tempBuffer = null;
        Stopwatch timer = null;
        List<string> resultList = null;
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Post(string path)
        {
            resultList = new List<string>();
            var driver = new PhantomJSDriver();
            webClient = new WebClient();
            timer = new Stopwatch();
            try
            {
                Uri uri = new Uri(path);
                driver.Url = path;
                timer.Restart();
                tempBuffer = webClient.DownloadData(path);
                resultList.Add(timer.ElapsedMilliseconds.ToString());
                timer.Stop();
                resultList.Add(path);
            }
            catch (Exception)
            {
                return null;
            }
            var linkCollection = driver.FindElementsByTagName("link");
            var iframeCollection = driver.FindElementsByTagName("iframe");
            var imageCollection = driver.FindElementsByTagName("img");
            var areaCollection = driver.FindElementsByTagName("area");
            var scriptCollection = driver.FindElementsByTagName("script");
            sourcePath = new StringBuilder();
            foreach (var t in linkCollection)
            {
                sourcePath.Clear();
                sourcePath.Append(t.GetAttribute("href"));
                try
                {
                    timer.Restart();
                    tempBuffer = webClient.DownloadData(sourcePath.ToString());
                    resultList.Add(timer.ElapsedMilliseconds.ToString());
                    timer.Stop();
                    resultList.Add(sourcePath.ToString());
                }
                catch (Exception)
                {
                    continue;
                }
                tempBuffer = null;
            }
            foreach (var t in iframeCollection)
            {
                sourcePath.Clear();
                sourcePath.Append(t.GetAttribute("src"));
                try
                {
                    timer.Restart();
                    tempBuffer = webClient.DownloadData(sourcePath.ToString());
                    resultList.Add(timer.ElapsedMilliseconds.ToString());
                    timer.Stop();
                    resultList.Add(sourcePath.ToString());
                }
                catch (Exception)
                {
                    continue;
                }
                tempBuffer = null;
            }
            foreach (var t in imageCollection)
            {
                sourcePath.Clear();
                sourcePath.Append(t.GetAttribute("src"));
                try
                {
                    timer.Restart();
                    tempBuffer = webClient.DownloadData(sourcePath.ToString());
                    resultList.Add(timer.ElapsedMilliseconds.ToString());
                    timer.Stop();
                    resultList.Add(sourcePath.ToString());
                }
                catch (Exception)
                {
                    continue;
                }
                tempBuffer = null;
            }
            foreach (var t in areaCollection)
            {
                sourcePath.Clear();
                sourcePath.Append(t.GetAttribute("href"));               
                try
                {
                    timer.Restart();
                    tempBuffer = webClient.DownloadData(sourcePath.ToString());
                    resultList.Add(timer.ElapsedMilliseconds.ToString());
                    timer.Stop();
                    resultList.Add(sourcePath.ToString());
                }
                catch (Exception)
                {
                    continue;
                }
                tempBuffer = null;
            }
            foreach (var t in scriptCollection)
            {
                sourcePath.Clear();
                sourcePath.Append(t.GetAttribute("src"));
                try
                {
                    timer.Restart();
                    tempBuffer = webClient.DownloadData(sourcePath.ToString());
                    resultList.Add(timer.ElapsedMilliseconds.ToString());
                    timer.Stop();
                    resultList.Add(sourcePath.ToString());
                }
                catch (Exception)
                {
                    continue;
                }
                tempBuffer = null;
            }
            using (TestContext testDB = new TestContext())
            {
                int sumTime = 0;
                for (int i = 0; i <= resultList.Count -1; i += 2)
                {
                    sumTime += Convert.ToInt32(resultList[i]);
                }
                bool existe = false;
                int id = 0;
                foreach (Website d in testDB.Websites.ToList())
                {
                    if (path == d.NamePath)
                    {
                        existe = true;
                        id = d.Id;
                    }
                }
                if (existe)
                {
                    Website ws = testDB.Websites.Find(id);
                    ws.LoadTime = sumTime;
                    try
                    {
                        testDB.Entry(ws).State = EntityState.Modified;
                        testDB.SaveChanges();
                    }
                    catch (Exception)
                    {
                        // return "The data could not be saved, please try again later.";
                    }
                }
                else
                {
                    Website webSite = new Website();
                    webSite.NamePath = path;
                    webSite.LoadTime = sumTime;
                    try
                    {
                        testDB.Entry(webSite).State = EntityState.Added;
                        testDB.SaveChanges();
                    }
                    catch (Exception)
                    {
                        //return "The data could not be saved, please try again later.";
                    }
                }
                return Json(resultList);
            }
        }
        List<Website> listTableSite;
        List<Website> topListTableSite;
        public ActionResult TableSitesView()
        {
            using (TestContext testDB = new TestContext())
            {
                listTableSite = new List<Website>();
                topListTableSite = new List<Website>();
                listTableSite = testDB.Websites.ToList();
                var sortListTableSite = listTableSite.OrderByDescending(w => w.LoadTime);
                int i = 0;
                foreach (Website w in sortListTableSite)
                {
                    if (i < 10)
                    {
                        topListTableSite.Add(w);
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                ViewBag.topList = topListTableSite;
                return PartialView("~/Views/Home/TableSitesView.cshtml");
            }
        }
        public string ClearDb()
        {
            using (TestContext testDB = new TestContext())
            {
                try
                {
                    foreach (Website d in testDB.Websites.ToList())
                    {
                        Website ws = testDB.Websites.Find(d.Id);
                        testDB.Websites.Remove(ws);
                        testDB.SaveChanges();
                    }
                    return "The data deleted successfully.";
                }
                catch (Exception)
                {
                    return "The data could not be deleted, please try again later.";
                }
            }
        }
    }
}