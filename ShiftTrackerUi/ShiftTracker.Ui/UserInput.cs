// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Net;
using RestSharp;

namespace ShiftTracker.Ui
{
    internal class UserInput
    {
        private ShiftsService shiftsService = new();

        internal void MainMenu()
        {
            bool closeApp = false;
            while (closeApp == false)
            {
                Console.WriteLine("\n\nMAIN MENU");
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("\nType 0 to Close Application.");
                Console.WriteLine("Type 1 to View Shifts");
                Console.WriteLine("Type 2 to Add Shift");
                Console.WriteLine("Type 3 to Delete Category");
                Console.WriteLine("Type 4 to Update Category");
                //Console.WriteLine("Type 5 to View Contacts");
                //Console.WriteLine("Type 6 to Add Contacts");
                //Console.WriteLine("Type 7 to Delete Contact");
                //Console.WriteLine("Type 8 to Update Contact");
                //Console.WriteLine("Type 9 to View Contacts of One Category");

                string commandInput = Console.ReadLine();
                while (string.IsNullOrEmpty(commandInput) || !int.TryParse(commandInput, out _))
                {
                    Console.WriteLine("\nInvalid Command. Please type a number from 0 to 2.\n");
                    commandInput = Console.ReadLine();
                }

                int command = Convert.ToInt32(commandInput);

                switch (command)
                {
                    case 0:
                        closeApp = true;
                        break;
                    case 1:
                        shiftsService.GetShifts();
                        break;
                    case 2:
                        ProcessAddShift();
                        break;
                    case 3:
                        ProcessDeleteShift();
                        break;
                    case 4:
                        ProcessUpdateShift();
                        break;
                    //case 5:
                    //    contactsController.ViewContacts();
                    //    break;
                    //case 6:
                    //    ProcessAddContact();
                    //    break;
                    //case 7:
                    //    ProcessDeleteContact();
                    //    break;
                    //case 8:
                    //    ProcessContactUpdate();
                    //    break;
                    //case 9:
                    //    ProcessContactsByCategory();
                    //    break;



                    default:
                        Console.WriteLine("\nInvalid Command. Please type a number from 0 to 8.\n");
                        break;
                }
            }
        }

        // https://stackoverflow.com/questions/36926867/deserializing-json-into-a-list-of-objects-cannot-create-and-populate-list-type

        private void ProcessAddShift()
        {
            shiftsService.GetShifts();

            Shift shift = new();
            shift.Start = GetDateTimeInput("Please add shift start", "Add");
            shift.End = GetDateTimeInput("Please add shift end", "Add");

            while (Validator.IsEndDateValid(shift.Start, shift.End))
                shift.End = GetDateTimeInput("End date has to be after start date. Try again.", "Add");

            shift.Location = GetStringInput("Please add shift location.", "Add");
            shift.Minutes = Helpers.CalculateDuration(shift.Start, shift.End);
            shift.Pay = GetMoneyInput("Please add your pay for this shift.", "Add");

            shiftsService.AddShift(shift);
        }

        private void ProcessDeleteShift()
        {
            shiftsService.GetShifts();

            int shiftId = GetIntegerInput("Please add id of the category you want to delete.");

            var shiftResponse = shiftsService.DeleteShift(shiftId);

            while (shiftResponse.StatusCode == HttpStatusCode.NotFound)
            {
                shiftId = GetIntegerInput($"A shift with the id {shiftId} doesn't exist. Try again.");
            }
        }

        private void ProcessUpdateShift()
        {
            shiftsService.GetShifts();

            var shiftToUpdate = ProcessGetShiftById();

            var startUpdate = GetDateTimeInput("Please enter new start date or type 0 to keep start date", "Update");
            if (startUpdate != DateTime.MinValue) shiftToUpdate.Start = startUpdate;

            var endUpdate = GetDateTimeInput("Please enter new end date or type 0 to keep end date", "Update");
            if (endUpdate != DateTime.MinValue) shiftToUpdate.End = endUpdate;

            var payUpdate = GetMoneyInput("Please enter new phone number or type 0 to keep pay value", "Update");
            if (payUpdate != Decimal.MinValue) shiftToUpdate.Pay = payUpdate;

            var location = GetStringInput("Please enter new phone number or type 0 to keep number", "Update");
            if (location != "0") shiftToUpdate.Location = location;

            shiftsService.UpdateShift(shiftToUpdate);
        }

        private Shift ProcessGetShiftById()
        {
            shiftsService.GetShifts();

            int shiftId = GetIntegerInput("Please add id of the shift");

            var shiftResponse = shiftsService.GetShiftById(shiftId);

            while (shiftResponse.StatusCode == HttpStatusCode.NotFound)
            {
                shiftId = GetIntegerInput($"A shift with the id {shiftId} doesn't exist. Try again.");
                shiftResponse = shiftsService.GetShiftById(shiftId);
            }

            return shiftResponse.Data;
        }

        private string GetStringInput(string message, string operation)
        {
            Console.WriteLine(message);
            string input = Console.ReadLine();

            if (operation.Equals("Update"))
            {
                while (input != "0" && !Validator.IsStringValid(input))
                {
                    Console.WriteLine("\nInvalid date");
                    input = Console.ReadLine();
                }
            }
            else
            {
                while (!Validator.IsStringValid(input))
                {
                    Console.WriteLine("\nInvalid input");
                    input = Console.ReadLine();
                }
            }

            return input;
        }

        private int GetIntegerInput(string message)
        {
            Console.WriteLine(message);
            string idInput = Console.ReadLine();

            while (!Validator.IsIdValid(idInput))
            {
                Console.WriteLine("\nInvalid input");
                idInput = Console.ReadLine();
            }

            return Int32.Parse(idInput);
        }

        private DateTime GetDateTimeInput(string message, string operation)
        {
            Console.WriteLine(message);
            string input = Console.ReadLine();

            if (input == "0")
                return DateTime.MinValue;

            if (operation.Equals("Update"))
            {
                while (input != "0" && !Validator.IsDateTimeValid(input))
                {
                    Console.WriteLine("\nInvalid date");
                    input = Console.ReadLine();
                }
            }
            else
            {
                while (!Validator.IsDateTimeValid(input))
                {
                    Console.WriteLine("\nInvalid date");
                    input = Console.ReadLine();
                }
            }
            
            return DateTime.Parse(input);
        }

        private decimal GetMoneyInput(string message, string operation)
        {
            Console.WriteLine(message);
            string input = Console.ReadLine();

            if (input == "0")
                return Decimal.MinValue;

            if (operation.Equals("Update"))
            {
                while (input != "0" && !Validator.IsMoneyValid(input))
                {
                    Console.WriteLine("\nInvalid date");
                    input = Console.ReadLine();
                }
            }
            else
            {
                while (!Validator.IsMoneyValid(input))
                {
                    Console.WriteLine("\nInvalid date");
                    input = Console.ReadLine();
                }
            }

            return decimal.Parse(input);
        }

        //private void ProcessCategoryUpdate()
        //{
        //    contactsController.ViewCategories();

        //    int id = GetIntegerInput("Please add id of the category you want to update.");
        //    var cat = contactsController.GetCategoryById(id);

        //    while (cat == null)
        //    {
        //        id = GetIntegerInput($"A category with the id {id} doesn't exist. Try again.");
        //    }

        //    string name = GetStringInput("Please enter new name for category.");
        //    contactsController.UpdateCategory(id, name);
        //}

        //private void ProcessAddContact()
        //{
        //    contactsController.ViewCategories();
        //    Contact contact = new();
        //    contact.CategoryId = GetIntegerInput("Please add categoryId for contact.");
        //    contact.FirstName = GetStringInput("Please type first name.");
        //    contact.LastName = GetStringInput("Please type last name.");
        //    contact.Number = GetPhoneInput("Please type phone number.");

        //    contactsController.AddContact(contact);
        //}
        }
}