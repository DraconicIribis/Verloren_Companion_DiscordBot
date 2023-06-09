using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Verloren_Companion_Bot
{
    class DreamEvents
    {
        public async Task<string[][]> getDreamEvents()
        {
            string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
            string ApplicationName = "Google Sheets API .NET Quickstart";
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            String spreadsheetId = "12ZDd5NJ27LHfamTqYGwosDm8hqvcoNoXVInNrWCRfI0";
            String dreamRange = "Dream Events!B:D";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, dreamRange);

            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;

            string[][] jaggedDeep = new string[3][];

            List<string> severity0 = new List<string>();
            List<string> severity1 = new List<string>();
            List<string> severity2 = new List<string>();

            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    severity0.Add(row[0].ToString());
                    severity1.Add(row[1].ToString());
                    severity2.Add(row[2].ToString());
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }

            string[] severity0A = severity0.ToArray();
            string[] severity1A = severity1.ToArray();
            string[] severity2A = severity2.ToArray();

            jaggedDeep[0] = severity0A;
            jaggedDeep[1] = severity1A;
            jaggedDeep[2] = severity2A;

            return jaggedDeep;
        }
    }
}