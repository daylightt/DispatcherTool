using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using WebApplication1.Models;
using WebApplication1.ViewModel;
using Microsoft.AspNet.Identity;
using WebApplication1.Classes;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private static DispatcherDBEntities db = new DispatcherDBEntities();
        private DispatcherViewModel viewModel = new DispatcherViewModel();

        public ActionResult Index()
        {
            var browser = Request.Browser;
            ViewBag.browser = browser.Browser;

            viewModel.Engineers = db.Engineers.OrderBy(w => w.Name).ToList();
            viewModel.Tasks = db.Tasks.Where(s => s.Status1.Name == "Open").ToList(); 
            viewModel.EngineerSkills = db.EngineerSkills.ToList();
            viewModel.Categories = db.TaskCategories.ToList();

            //Get the time restrictions for the engineers
            timeRestrictions(viewModel);
            // List for the Scheduler & tooltip info
            gatherInfo(viewModel);
            //Load the xml data for the Tasks
            gatherXML();
            sendData();
            //Load the GIS data from the Tasks
            gatherMapPositions(viewModel);

            return View(viewModel);
        }

        public ActionResult TaskDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        [HttpPost]
        public ActionResult Filter(FormCollection collection)
        {
            string city = collection["city"];                    // Default value = "City"
            string postal = collection["postCode"];              // Default value = "Postal Code"
            string skill = collection["skillSet"];               // Default value = ""
            string category = collection["taskCategory"];        // Default value = ""
            string viewall = collection["viewAll"];              // Default value = null, when clicked = "View all"
            string status = collection["status"];                // Defualt Value = ""

            if (city != "City")
            {
                ViewBag.city = city;
            }
            if (postal != "Postal Code")
            {
                ViewBag.postal = postal;
            }
            ViewBag.skill = skill;
            ViewBag.category = category;
            ViewBag.status = status;

            // Default list for the Tasks nand Engineers
            viewModel.Engineers = db.Engineers.OrderBy(n => n.Name).ToList();
            viewModel.Tasks = db.Tasks.ToList();

            var keys = db.Engineers.Select(v => v.Id).ToList();
            var engSkills = db.EngineerSkills.Where(v => v.Skill.Name == skill).Select(k => k.W6Key).ToList();

            //Filters for the Tasks & Engineer
            if (viewall != "View all")
            {
                if (city != "City" && postal != "Postal Code" && category == "" && skill == "" && status == "")
                {
                    viewModel.Tasks =
                        db.Tasks.Where(w => city.Contains(w.City)).Where(v => v.PostCode.Contains(postal)).ToList();
                    viewModel.Engineers =
                        db.Engineers.Where(w => city.Contains(w.City)).Where(v => v.PostCode.Contains(postal)).ToList();
                }
                else if (city != "City" && postal != "Postal Code" && category == "" && skill == "" && status != "")
                {
                    viewModel.Tasks =
                        db.Tasks.Where(w => city.Contains(w.City)).Where(v => v.PostCode.Contains(postal)).Where(s => s.Status1.Name == status).ToList();
                    viewModel.Engineers =
                        db.Engineers.Where(w => city.Contains(w.City)).Where(v => v.PostCode.Contains(postal)).ToList();
                }
                else if (city != "City" && postal != "Postal Code" && category != "" && skill != "" && status == "")
                {
                    viewModel.Tasks =
                        db.Tasks.Where(w => city.Contains(w.City))
                            .Where(v => v.PostCode.Contains(postal))
                            .Where(c => c.TaskCategory1.Name == category)
                            .ToList();
                    viewModel.Engineers = db.Engineers.Where(c => city.Contains(c.City)).Where(p => p.PostCode.Contains(postal)).Where(k => engSkills.Contains(k.Id)).ToList();
                }
                else if (city != "City" && postal != "Postal Code" && category != "" && skill != "" && status != "")
                {
                    viewModel.Tasks =
                        db.Tasks.Where(w => city.Contains(w.City))
                            .Where(v => v.PostCode.Contains(postal))
                            .Where(c => c.TaskCategory1.Name == category)
                            .Where(s => s.Status1.Name == status)
                            .ToList();
                    viewModel.Engineers = db.Engineers.Where(c => city.Contains(c.City)).Where(p => p.PostCode.Contains(postal)).Where(k => engSkills.Contains(k.Id)).ToList();
                }
                else if (city == "City" && postal != "Postal Code" && category != "" && skill != "" && status == "")
                {
                    viewModel.Tasks =
                        db.Tasks.Where(v => v.PostCode.Contains(postal))
                            .Where(c => c.TaskCategory1.Name == category)
                            .ToList();
                    viewModel.Engineers =
                        db.Engineers.Where((p => p.PostCode.Contains(postal)))
                            .Where(k => engSkills.Contains(k.Id))
                            .ToList();
                }
                else if (city == "City" && postal != "Postal Code" && category != "" && skill != "" && status != "")
                {
                    viewModel.Tasks =
                        db.Tasks.Where(v => v.PostCode.Contains(postal))
                            .Where(c => c.TaskCategory1.Name == category)
                            .Where(s => s.Status1.Name == status)
                            .ToList();
                    viewModel.Engineers =
                        db.Engineers.Where((p => p.PostCode.Contains(postal)))
                            .Where(k => engSkills.Contains(k.Id))
                            .ToList();
                }
                else if (city != "City" && postal == "Postal Code" && category != "" && skill != "" && status == "")
                {
                    viewModel.Tasks =
                        db.Tasks.Where(w => city.Contains(w.City)).Where(c => c.TaskCategory1.Name == category).ToList();
                    viewModel.Engineers =
                        db.Engineers.Where(c => city.Contains(c.City)).Where(k => engSkills.Contains(k.Id)).ToList();
                }
                else if (city != "City" && postal == "Postal Code" && category != "" && skill != "" && status != "")
                {
                    viewModel.Tasks =
                        db.Tasks
                        .Where(w => city.Contains(w.City)).Where(c => c.TaskCategory1.Name == category)
                        .Where(s => s.Status1.Name == status)
                        .ToList();
                    viewModel.Engineers =
                        db.Engineers.Where(c => city.Contains(c.City)).Where(k => engSkills.Contains(k.Id)).ToList();
                }
                else if (city != "City" && postal == "Postal Code" && category == "" && skill == "" && status == "")
                {
                    viewModel.Tasks =
                        db.Tasks.Where(w => city.Contains(w.City)).ToList();
                    viewModel.Engineers =
                        db.Engineers.Where(w => city.Contains(w.City)).ToList();
                }
                else if (city != "City" && postal == "Postal Code" && category == "" && skill == "" && status != "")
                {
                    viewModel.Tasks =
                        db.Tasks.Where(w => city.Contains(w.City)).Where(s => s.Status1.Name == status).ToList();
                    viewModel.Engineers =
                        db.Engineers.Where(w => city.Contains(w.City)).ToList();
                }
                else if (city == "City" && postal != "Postal Code" && category == "" && skill == "" && status == "")
                {
                    viewModel.Tasks =
                        db.Tasks.Where(v => v.PostCode.Contains(postal)).ToList();
                    viewModel.Engineers =
                        db.Engineers.Where(v => v.PostCode.Contains(postal)).ToList();
                }
                else if (city == "City" && postal != "Postal Code" && category == "" && skill == "" && status != "")
                {
                    viewModel.Tasks =
                        db.Tasks.Where(v => v.PostCode.Contains(postal)).Where(s => s.Status1.Name == status).ToList();
                    viewModel.Engineers =
                        db.Engineers.Where(v => v.PostCode.Contains(postal)).ToList();
                }
                else if (city == "City" && postal == "Postal Code" && category != "" && skill != "" && status == "")
                {
                    viewModel.Tasks =
                        db.Tasks.Where(c => c.TaskCategory1.Name == category).ToList();
                    viewModel.Engineers = db.Engineers.Where(k => engSkills.Contains(k.Id)).ToList();
                }
                else if (city == "City" && postal == "Postal Code" && category != "" && skill != "" && status != "")
                {
                    viewModel.Tasks =
                        db.Tasks.Where(c => c.TaskCategory1.Name == category).Where(s => s.Status1.Name == status).ToList();
                    viewModel.Engineers = db.Engineers.Where(k => engSkills.Contains(k.Id)).ToList();
                }
                else if (city == "City" && postal == "Postal Code" && category == "" && skill != "" && status == "")
                    viewModel.Engineers = db.Engineers.Where(k => engSkills.Contains(k.Id)).ToList();
                else if (city == "City" && postal == "Postal Code" && category == "" && skill != "" && status != "")
                {
                    viewModel.Engineers = db.Engineers.Where(k => engSkills.Contains(k.Id)).ToList();
                    viewModel.Tasks = db.Tasks.Where(s => s.Status1.Name == status).ToList();
                }
                else if (city == "City" && postal == "Postal Code" && category != "" && skill == "" && status == "")
                    viewModel.Tasks = db.Tasks.Where(c => c.TaskCategory1.Name == category).ToList();
                else if (city == "City" && postal == "Postal Code" && category != "" && skill == "" && status != "")
                {
                    viewModel.Tasks = db.Tasks
                        .Where(c => c.TaskCategory1.Name == category)
                        .Where(s => s.Status1.Name == status)
                        .ToList();
                }
                else if (city == "City" && postal == "Postal Code" && category == "" && skill == "" && status != "")
                {
                    viewModel.Tasks = db.Tasks.Where(s => s.Status1.Name == status).ToList();
                }
            }

            viewModel.EngineerSkills = db.EngineerSkills.ToList();
            viewModel.Categories = db.TaskCategories.ToList();

            gatherInfo(viewModel);
            gatherXML();
            timeRestrictions(viewModel);
            gatherMapPositions(viewModel);

            return View("Index", viewModel);
        }

        //function used to aquire the tasks from the scheduler and save them into a Json file to the ~\Content
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Save(string jsonString)
        {
            //string path = HttpContext.Server.MapPath("~/Content/Tasks.xml");
            //XmlDocument doc = new XmlDocument();

            //try
            //{
            //    doc.LoadXml(xmlString);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}
            //doc.Save(path);

            string path = HttpContext.Server.MapPath("~/Content/Json.txt");
            //string json = JsonConvert.SerializeObject(xmlString, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(path, jsonString);

            //sendEmail();

            viewModel.Engineers = db.Engineers.ToList();
            viewModel.Tasks = db.Tasks.ToList();
            viewModel.EngineerSkills = db.EngineerSkills.ToList();
            viewModel.Categories = db.TaskCategories.ToList();

            gatherInfo(viewModel);
            gatherXML();
            timeRestrictions(viewModel);
            gatherMapPositions(viewModel);

            JsonToDatabase(jsonString);

            return View("Index", viewModel);
        }

        //Loads the scheduler events from Json.txt, and stores then into a ViewBag.
        public void sendData()
        {
            string json;

            using (FileStream fs = new FileStream(Server.MapPath("~/Content/json.txt"), FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    json = sr.ReadToEnd();
                }
            }

            ViewBag.json = json;
            sendTodaysData(json);
        }

        //Sends todays events to a textarea in the view, called by sendData()
        public void sendTodaysData(string json)
        {
            string[] id;
            string[] status;

            var date = System.DateTime.Today;
            string strDate = date.Year + "-" + date.Month + "-" + date.Day;

            JsonTextReader reader = new JsonTextReader(new StringReader(json));
            while (reader.Read())
            {
                if (reader.Value == null)
                    continue;

                if (reader.Value.ToString() == "start_date")
                {
                    reader.Read();
                    if (reader.Value.ToString().Contains(strDate))
                    {
                    }
                }
            }
        }

        //Populate Drop down list for Engineer's Skill Set
        public static List<SelectListItem> GetSkillSet()
        {
            var skillset = db.Skills.ToList();
            return skillset.Select(s => new SelectListItem { Text = s.Name, Value = s.Name }).ToList();
        }

        //Populate Drop down list for Task Categories
        public static List<SelectListItem> GetTaskCategory()
        {
            var categories = db.TaskCategories.ToList();
            return categories.Select(task => new SelectListItem { Text = task.Name, Value = task.Name }).ToList();
        }

        public static List<SelectListItem> GetTaskStatus()
        {
            var status = db.Status.Select(s => new SelectListItem { Text = s.Name, Value = s.Name }).ToList();
            var noDupes = status.Distinct().ToList();
            return noDupes;
        }

        //Function which creates Engineer List for the Scheduler Y_axis & creates tooltip info
        public void gatherInfo(DispatcherViewModel viewModel)
        {
            // List for the Scheduler & tooltip info
            var engList = new List<object>();
            var engString = new string[viewModel.Engineers.LongCount()];
            var i = 0;
            var postCode = "";

            foreach (var engineer in viewModel.Engineers)
            {
                engList.Add(new { key = engineer.Engineer_ID, label = engineer.Name });
                postCode = engineer.PostCode;
                if (postCode != null)
                    postCode = postCode.Substring(0, 3);
                engString[i] = "Postal Code: " + postCode + ", Telephone: " + engineer.MobilePhone + ", ID: " + engineer.Engineer_ID;
                i++;
            }

            //var status = db.Tasks.Select(s => new SelectListItem { Text = s.Status1.Name, Value = s.Status1.Name }).ToList();
            //var noDupes = status.Distinct().ToList();
            var statusList = new List<object>();
            //statusList.Add(new {key = "No Available Status", label = "No Available Status"});
            //foreach(var item in noDupes)
            //{
            //    statusList.Add(new { key = item.Text, label = item.Text });
            //}

            foreach (var entry in db.Status.ToList())
            {
                statusList.Add(new { key = entry.Name, label = entry.Name });
            }

            ViewBag.engineerId = viewModel.Engineers.Select(s => s.Engineer_ID).ToList();
            ViewBag.engineersInfo = engString;
            ViewBag.engineers = engList;
            ViewBag.statusList = statusList;
        }

        //Function to gather the longitude and latitude positions for the tasks
        public void gatherMapPositions(DispatcherViewModel viewModel)
        {
            var positions = new List<object>();

            foreach (var task in viewModel.Tasks)
            {
                var info = task.Customer.ContactName + ": " + task.Customer.PhoneNumber + "\n" + task.City + " " + task.Address + "\n";
                positions.Add(new { key = task.Id, lat = task.Latitude, lon = task.Longitude, text = info, status = task.Status1.Name });
            }

            ViewBag.positions = positions;
        }

        //Load the xml saved data for the Tasks
        public void gatherXML()
        {
            string path = HttpContext.Server.MapPath("~/Content/Tasks.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            doc.PreserveWhitespace = true;

            gatherTodaysXML(path);

            ViewBag.xml = doc.InnerXml;

            //var doc = XDocument.Load(path);

            //var res = (from i in doc.Root.Elements()
            //           where i.Element(XName.Get("start_date")).Value == "2014-10-21 09:45"
            //           select i).ToList();

            //ViewBag.xml = res[0];
        }

        //From the saved xml file, retrieve all of todays event to display them on the map.
        public void gatherTodaysXML(string path)
        {
            var doc = XDocument.Load(path);
            var date = System.DateTime.Today;
            string strDate = date.Year + "-" + date.Month + "-" + date.Day;

            var res = new XDocument(new XElement("data",
                                    (from i in doc.Root.Elements()
                                     where i.Element(XName.Get("start_date")).Value.Contains(strDate)
                                     select i)));

            ViewBag.xmlToday = res.ToString();
        }

        //load the schedule times into the view model.
        public void timeRestrictions(DispatcherViewModel viewModel)
        {
            //Retrieve the different number of schedules possible that the enigneers currently have
            var scheduleCount = viewModel.Engineers.Select(s => s.WorkingHours);
            scheduleCount = scheduleCount.Distinct().ToList();

            //Dictionary to hold the weekly schedule key, and corresponding engineers associated with said key
            Dictionary<String, List<String>> eng_hours = new Dictionary<String, List<String>>();
            //Dictionary to hold the weekly key and the correspond five day hours (regular)
            Dictionary<String, List<String>> weekly_hours = new Dictionary<String, List<String>>();
            //Dictionary to hold the weekly key and the correspond five day hours (regular)
            Dictionary<String, List<String>> optional_hours = new Dictionary<String, List<String>>();

            for (int i = 0; i < scheduleCount.Count(); i++)
            {
                var eng_list = viewModel.Engineers.Where(k => k.WorkingHours == scheduleCount.ElementAt(i));
                var weeklyKey = scheduleCount.ElementAt(i).ToString();
                var eng_ids = eng_list.Select(s => s.Engineer_ID.ToString()).ToList();

                var week = db.WeeklyHours.Where(k => k.Id.ToString() == weeklyKey).ToList();

                List<String> reg_shifts = new List<String>();
                List<String> opt_shifts = new List<String>();

                var list = populateShifts(week, reg_shifts, opt_shifts);
                reg_shifts = list.ElementAt(0);
                opt_shifts = list.ElementAt(1);

                eng_hours.Add(weeklyKey, eng_ids);
                weekly_hours.Add(weeklyKey, reg_shifts);
                optional_hours.Add(weeklyKey, opt_shifts);
            }

            viewModel.EngineerSchedule = eng_hours;
            viewModel.WeeklySchedule = weekly_hours;
            viewModel.OptionalSchedule = optional_hours;
        }

        //Function to filter the time before placing them into the view model.
        public List<List<String>> populateShifts(List<WeeklyHour> week, List<String> shifts, List<String> opt_shifts)
        {
            foreach (var day in week)
            {
                var mondayKey = day.Monday;
                var tuesdayKey = day.Tuesday;
                var wednesdayKey = day.Wednesday;
                var thursdayKey = day.Thursday;
                var fridayKey = day.Friday;
                var saturdayKey = day.Saturday;
                var sundayKey = day.Sunday;

                if (mondayKey != null)
                {
                    var monday = db.DailyHours.Where(k => k.Id == mondayKey).ToList();
                    shifts.Add(monday[0].RegularStart);
                    shifts.Add(monday[0].RegularFinish);

                    if (monday[0].hasOptional != false)
                    {
                        opt_shifts.Add(monday[0].OptionalStart);
                        opt_shifts.Add(monday[0].OptionalEnd);
                    }
                }
                else
                {
                    shifts.Add("N/A");
                    shifts.Add("N/A");
                    opt_shifts.Add("N/A");
                    opt_shifts.Add("N/A");
                }

                if (tuesdayKey != null)
                {
                    var tuesday = db.DailyHours.Where(k => k.Id == tuesdayKey).ToList();
                    shifts.Add(tuesday[0].RegularStart);
                    shifts.Add(tuesday[0].RegularFinish);

                    if (tuesday[0].hasOptional != false)
                    {
                        opt_shifts.Add(tuesday[0].OptionalStart);
                        opt_shifts.Add(tuesday[0].OptionalEnd);
                    }
                }
                else
                {
                    shifts.Add("N/A");
                    shifts.Add("N/A");
                    opt_shifts.Add("N/A");
                    opt_shifts.Add("N/A");
                }

                if (wednesdayKey != null)
                {
                    var wednesday = db.DailyHours.Where(k => k.Id == wednesdayKey).ToList();
                    shifts.Add(wednesday[0].RegularStart);
                    shifts.Add(wednesday[0].RegularFinish);

                    if (wednesday[0].hasOptional != false)
                    {
                        opt_shifts.Add(wednesday[0].OptionalStart);
                        opt_shifts.Add(wednesday[0].OptionalEnd);
                    }
                }
                else
                {
                    shifts.Add("N/A");
                    shifts.Add("N/A");
                    opt_shifts.Add("N/A");
                    opt_shifts.Add("N/A");
                }

                if (thursdayKey != null)
                {
                    var thursday = db.DailyHours.Where(k => k.Id == thursdayKey).ToList();
                    shifts.Add(thursday[0].RegularStart);
                    shifts.Add(thursday[0].RegularFinish);

                    if (thursday[0].hasOptional != false)
                    {
                        opt_shifts.Add(thursday[0].OptionalStart);
                        opt_shifts.Add(thursday[0].OptionalEnd);
                    }
                }
                else
                {
                    shifts.Add("N/A");
                    shifts.Add("N/A");
                    opt_shifts.Add("N/A");
                    opt_shifts.Add("N/A");
                }

                if (fridayKey != null)
                {
                    var friday = db.DailyHours.Where(k => k.Id == fridayKey).ToList();
                    shifts.Add(friday[0].RegularStart);
                    shifts.Add(friday[0].RegularFinish);

                    if (friday[0].hasOptional != false)
                    {
                        opt_shifts.Add(friday[0].OptionalStart);
                        opt_shifts.Add(friday[0].OptionalEnd);
                    }
                }
                else
                {
                    shifts.Add("N/A");
                    shifts.Add("N/A");
                    opt_shifts.Add("N/A");
                    opt_shifts.Add("N/A");
                }

                if (saturdayKey != null)
                {
                    var saturday = db.DailyHours.Where(k => k.Id == saturdayKey).ToList();
                    shifts.Add(saturday[0].RegularStart);
                    shifts.Add(saturday[0].RegularFinish);

                    if (saturday[0].hasOptional != false)
                    {
                        opt_shifts.Add(saturday[0].OptionalStart);
                        opt_shifts.Add(saturday[0].OptionalEnd);
                    }
                }
                else
                {
                    shifts.Add("N/A");
                    shifts.Add("N/A");
                    opt_shifts.Add("N/A");
                    opt_shifts.Add("N/A");
                }

                if (sundayKey != null)
                {
                    var sunday = db.DailyHours.Where(k => k.Id == sundayKey).ToList();
                    shifts.Add(sunday[0].RegularStart);
                    shifts.Add(sunday[0].RegularFinish);

                    if (sunday[0].hasOptional != false)
                    {
                        opt_shifts.Add(sunday[0].OptionalStart);
                        opt_shifts.Add(sunday[0].OptionalEnd);
                    }
                }
                else
                {
                    shifts.Add("N/A");
                    shifts.Add("N/A");
                    opt_shifts.Add("N/A");
                    opt_shifts.Add("N/A");
                }
            }

            List<List<String>> holdingList = new List<List<string>>();
            holdingList.Add(shifts);
            holdingList.Add(opt_shifts);

            return holdingList;
        }

        [Authorize]
        [HttpPost]
        public ActionResult sendEmail(string engID, string taskID, string start, string end)
        {
            var userEmail = User.Identity.Name;
            
            viewModel.Engineers = db.Engineers.ToList();

            var engEmail = viewModel.Engineers.Where(k => k.Id.ToString() == engID).Select(e => e.Email).ToList();

            const string fromPassword = "Stephen90";
            string subject = "Sent from Dispatcher";
            string body = "Hello Dave and Sayad!" + "/n This email was supposed to be sent to: " + engEmail[0];

            MailMessage objMail = new MailMessage(userEmail, engEmail[0], subject, body);
            //objMail.CC.Add("sbhimani@diabsolut.com");
            NetworkCredential objNC = new NetworkCredential(userEmail, fromPassword);
            SmtpClient objsmtp = new SmtpClient("smtp.gmail.com", 587); // for hotmail
            objsmtp.EnableSsl = true;
            objsmtp.Credentials = objNC;

            try
            {
                objsmtp.Send(objMail);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            viewModel.Tasks = db.Tasks.ToList();
            viewModel.EngineerSkills = db.EngineerSkills.ToList();
            viewModel.Categories = db.TaskCategories.ToList();

            gatherInfo(viewModel);
            gatherXML();
            timeRestrictions(viewModel);
            gatherMapPositions(viewModel);

            return View("Index", viewModel);
        }

        //Reads the JSON Data, then calls update to insert into DB.
        public void JsonToDatabase(string json)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(json));
            while (reader.Read())
            {
                if (reader.Value == null)
                    continue;

                if (reader.TokenType.ToString() == "PropertyName" && reader.Value.ToString() == "id")
                {
                    int id = (int)reader.ReadAsInt32();
                    string start = null;
                    string end = null;
                    int eng_id = 0;
                    string status = null;
                    string customer = null;
                    string number = null;
                    string category = null;

                    bool endOfObject = false;

                    while (reader.Read() && !endOfObject)
                    {
                        if (reader.Value == null)
                            continue;

                        if (reader.TokenType.ToString() == "PropertyName" && reader.Value.ToString() == "start_date")
                        {
                            start = (string)reader.ReadAsString();
                        }

                        else if (reader.TokenType.ToString() == "PropertyName" && reader.Value.ToString() == "end_date")
                        {
                            end = (string)reader.ReadAsString();
                        }

                        else if (reader.TokenType.ToString() == "PropertyName" && reader.Value.ToString() == "engineer_id")
                        {
                            eng_id = (int)reader.ReadAsInt32();
                        }

                        else if (reader.TokenType.ToString() == "PropertyName" && reader.Value.ToString() == "status")
                        {
                            status = (string)reader.ReadAsString();
                        }

                        else if (reader.TokenType.ToString() == "PropertyName" && reader.Value.ToString() == "customer")
                        {
                            customer = (string)reader.ReadAsString();
                        }

                        else if (reader.TokenType.ToString() == "PropertyName" && reader.Value.ToString() == "number")
                        {
                            number = (string)reader.ReadAsString();
                        }

                        else if (reader.TokenType.ToString() == "PropertyName" && reader.Value.ToString() == "category")
                        {
                            category = (string)reader.ReadAsString();
                            var info = new EventInfo(id, start, end, eng_id, status, customer, number, category);
                            update(info);
                            endOfObject = true;
                        }
                    }
                }
            } 
        }

        //Function to insert the JSON data into the database. Parameters are those from the JSON File, which is the data being saved fomr the scheduler
        private void update(EventInfo info)
        {
            var status_id = db.Status.Where(s => s.Name == info.status).ToList();
            var s_id = Convert.ToInt32(status_id[0].Id);

            var eng = db.Engineers.Where(e => e.Engineer_ID == info.eng_id).ToList();
            var e_id = Convert.ToInt32(eng[0].Id);

            Task t = null;

            foreach (var task in db.Tasks)
            {
                if (task.Id == info.id)
                {
                    if (task.TaskCategory1 != null)
                    {
                        task.TaskCategory1.Name = info.category;
                    }
                    task.Customer.ContactName = info.customer;
                    task.Customer.PhoneNumber = info.number;
                    task.AssignedTo = e_id;
                    task.Status = s_id;
                    task.StartTime = Convert.ToDateTime(info.start);
                    task.EndTime = Convert.ToDateTime(info.end);

                    t = task;
                    break;
                }
            }

            try 
            {
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges(); 
            }
            catch(Exception e) 
            { 
                Console.Write(e.Message); 
            }
        }

        public void deleteEvent(string taskId)
        {
            var info = new EventInfo();
            info.setID(Convert.ToInt32(taskId));
            Task t = null;

            foreach (var task in db.Tasks)
            {
                if (task.Id == info.id)
                {
                    if (task.TaskCategory1 != null)
                    {
                        task.TaskCategory1.Name = info.category;
                    }
                    task.Customer.ContactName = info.customer;
                    task.Customer.PhoneNumber = info.number;
                    task.AssignedTo = null;
                    task.Status = null;
                    task.StartTime = Convert.ToDateTime(info.start);
                    task.EndTime = Convert.ToDateTime(info.end);
                    t = task;
                    break;
                }
            }

            try
            {
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        public ActionResult timePeriod(string time)
        {
            //Start: month, day, year End: month, day, year --> Numeric values
            string[] times = time.Split(',');                             
            string start = times[0] + " " + times[1] + " " + times[2];
            string end = times[3] + " " + times[4] + " " + times[5];

            var sDate = DateTime.Parse(start);
            var eDate = DateTime.Parse(end);

            //viewModel.Tasks = db.Tasks.TakeWhile(sDate >= x <= eDate);
            viewModel.EngineerSkills = db.EngineerSkills.ToList();
            viewModel.Categories = db.TaskCategories.ToList();

            gatherInfo(viewModel);
            gatherXML();
            timeRestrictions(viewModel);
            gatherMapPositions(viewModel);

            return View("Index", viewModel);
        }
    }
}