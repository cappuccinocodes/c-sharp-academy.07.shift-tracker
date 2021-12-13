using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace ShiftTracker.Ui
{
    public class ShiftsService
    {
        private RestClient client = new("https://localhost:7116/api/");

        public void GetShifts()
        {

            var request = new RestRequest("shifts");
            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string rawResponse = response.Content;

                var serialize = JsonConvert.DeserializeObject<List<Shift>>(rawResponse);

                

                //foreach (var shift in returnedList)
                //{
                //    printList.Add(new Shift
                //    {
                //        ShiftId = shift.ShiftId,
                //        Start = shift.Start,
                //        End = shift.End,
                //        Pay = shift.Pay,
                //        Minutes = shift.Minutes,
                //        Location = shift.Location
                //    });
                //}

                TableVisualisationEngine.ShowTable(serialize, "Categories Menu");


            }
        }
    }
}
