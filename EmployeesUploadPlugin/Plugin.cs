using Newtonsoft.Json;
using PhoneApp.Domain.Attributes;
using PhoneApp.Domain.DTO;
using PhoneApp.Domain.Interfaces;
using System.Collections.Generic;
using System.IO;
using System;
using System.Net;
using System.Text;
using System.Linq;

namespace EmployeesUploadPlugin
{
    [Author(Name = "Irina Zelenova")]
    public class Plugin : IPluggable
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();     

        public IEnumerable<DataTransferObject> Run(IEnumerable<DataTransferObject> args)
        {
            logger.Info(@"Use command ""upload"" for upload java dictonary or press Enter");
            var employeesList = args.Cast<EmployeesDTO>().ToList();
           
            Console.Write("> ");
            string command = Console.ReadLine();
            switch (command)
            {
                case "upload":
                    string result = GET("https://dummyjson.com/users");
                    JsonFileModel employees = JsonConvert.DeserializeObject<JsonFileModel>(result);
                    foreach (var employee in employees.users)
                    {
                        EmployeesDTO dTO = new EmployeesDTO();
                        dTO.Name = employee.firstName + " " + employee.lastName;
                        dTO.Phone = employee.phone;
                        employeesList.Add(dTO);
                    }
                    logger.Info($"Upload {employees.users.Count} employees");
                    break;
            }
            Console.WriteLine("");
            return employeesList.Cast<DataTransferObject>();
        }

        private string GET(string Url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);

            req.Method = "GET";
            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8"; ;
            // req.ContentType = "multipart/form-data";

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();


            if (resp != null)
            {
                Stream stream = resp.GetResponseStream();
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                StreamReader sr = new StreamReader(stream);
                if (resp.CharacterSet.ToUpper().IndexOf("UTF") < 0)
                {
                    sr = new StreamReader(stream, Encoding.GetEncoding(resp.CharacterSet));
                }

                string text = sr.ReadToEnd();


                sr.Close();
                resp.Close();

                return text;
            }
            else
            {

                return "";
            }
        }

        private class Users
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string phone { get; set; }
        }

        private class JsonFileModel
        {
            public List<Users> users { get; set; }

        }
    }
}
