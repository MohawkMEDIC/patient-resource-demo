using Health.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Health.Controllers
{
    public class PatientController : Controller
    {
        private List<SearchViewModel> patients = new List<SearchViewModel>();
        //
        // GET: /Patient/
        public ActionResult Index(string ID)
        {
            try
            {
                var client = new FhirClient("http://cr.marc-hi.ca:8080/fhir");
                var client2 = new FhirClient("https://fhir.orionhealth.com/blaze/fhir");

                ViewData["ID"] = ID;
                SearchViewModel m = new SearchViewModel();

                //searches for alerts
                Bundle resultsAlerts = client2.Search<Alert>(new string[] { "subject=" });

                //gets alerts
                foreach (var entry in resultsAlerts.Entries)
                {
                    ResourceEntry<Alert> alerts = (ResourceEntry<Alert>)entry;
                    m.Alert = alerts.Resource.Note;
                    break;
                }

                //search patients based on ID clicked
                Bundle resultsPatients = client.Search<Patient>(new string[] { "identifier=" + ID });


                //gets patient based on ID
                foreach (var entry in resultsPatients.Entries)
                {
                    // since we are searching for a Patient resource, the entries represent patients (needs casting to the resource patient)
                    ResourceEntry<Patient> patient = (ResourceEntry<Patient>)entry;

                    // get the first values only for now, we will need to use a more sophisticated way to grab values
                    m.LastName = patient.Resource.Name.First<HumanName>().FamilyElement.First<FhirString>().Value;
                    m.MiddleName = patient.Resource.Name.First<HumanName>().GivenElement.ElementAt(1).Value;
                    m.FirstName = patient.Resource.Name.First<HumanName>().GivenElement.First<FhirString>().Value;
                    m.Identifier = patient.Resource.Identifier.First<Identifier>().Value;
                    m.Gender = patient.Resource.Gender.TextElement.Value;

                    var dateOfBirth = patient.Resource.BirthDateElement.Value;
                    DateTime dob = Convert.ToDateTime(dateOfBirth);
                    DateTime now = DateTime.Now;
                    var age = (int)(now.Year - dob.Year);
                    if (dob.Month > now.Month)
                    {
                        age--;
                    }

                    
                    m.BirthDate = age + " years old";


                    //m.Line = patient.Resource.Address.First<Address>().LineElement.First<FhirString>().Value;
                    //m.City = patient.Resource.Address.First<Address>().CityElement.Value;
                    //m.State = patient.Resource.Address.First<Address>().StateElement.Value;
                    //m.Country = patient.Resource.Address.First<Address>().CountryElement.Value;
                    //m.Telephone = patient.Resource.Telecom.First<Contact>().Value;
                    //m.Email = patient.Resource.Telecom.ElementAt(1).Value;

                    //try
                    //{
                    //    m.MaritalStatus = patient.Resource.MaritalStatus.TextElement.Value;
                    //}
                    //catch
                    //{
                    //    m.MaritalStatus = "N/A";
                    //}

                    //try
                    //{
                    //    m.CareProvider = patient.Resource.CareProvider.First<ResourceReference>().ReferenceElement.Value;
                    //}
                    //catch
                    //{
                    //    m.CareProvider = "N/A";
                    //}
                    

                    patients.Add(m);
                }

                return View(patients);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            
 
        }

        public ActionResult Allergies(string ID)
        {
            var client = new FhirClient("http://cr.marc-hi.ca:8080/fhir");
            var client2 = new FhirClient("https://fhir.orionhealth.com/blaze/fhir");

            SearchViewModel m = new SearchViewModel();

            Bundle resultsPatients = client.Search<Patient>(new string[] { "identifier=" + ID });


                //gets patient based on ID
            foreach (var entry in resultsPatients.Entries)
            {
                // since we are searching for a Patient resource, the entries represent patients (needs casting to the resource patient)
                ResourceEntry<Patient> patient = (ResourceEntry<Patient>)entry;

                // get the first values only for now, we will need to use a more sophisticated way to grab values
                m.LastName = patient.Resource.Name.First<HumanName>().FamilyElement.First<FhirString>().Value;
                m.FirstName = patient.Resource.Name.First<HumanName>().GivenElement.First<FhirString>().Value;
                m.Identifier = patient.Resource.Identifier.First<Identifier>().Value;
            }

            //searches for allergies
            Bundle resultsAllergy = client2.Search<AllergyIntolerance>(new string[] { "subject=" });

            //gets allergy and reaction to substance
            foreach (var entry in resultsAllergy.Entries)
            {
                ResourceEntry<AllergyIntolerance> allergy = (ResourceEntry<AllergyIntolerance>)entry;
                m.Allergy = allergy.Resource.Substance.DisplayElement.Value;
                m.AllergyReaction = allergy.Resource.Criticality_Element.Value.ToString();
                break;
            }
            patients.Add(m);
            return View(patients);
        }

        public ActionResult Medications(string ID)
        {
            var client = new FhirClient("http://cr.marc-hi.ca:8080/fhir");
            var client2 = new FhirClient("https://fhir.orionhealth.com/blaze/fhir");

            SearchViewModel m = new SearchViewModel();

            Bundle resultsPatients = client.Search<Patient>(new string[] { "identifier=" + ID });


            //gets patient based on ID
            foreach (var entry in resultsPatients.Entries)
            {
                // since we are searching for a Patient resource, the entries represent patients (needs casting to the resource patient)
                ResourceEntry<Patient> patient = (ResourceEntry<Patient>)entry;

                // get the first values only for now, we will need to use a more sophisticated way to grab values
                m.LastName = patient.Resource.Name.First<HumanName>().FamilyElement.First<FhirString>().Value;
                m.FirstName = patient.Resource.Name.First<HumanName>().GivenElement.First<FhirString>().Value;
                m.Identifier = patient.Resource.Identifier.First<Identifier>().Value;
            }


            //gets meds
            Bundle resultsMedications = client2.Search<Medication>(new string[] { "code=" });
            Bundle resultsMedicationsAdmin = client2.Search<MedicationAdministration>(new string[] { "prescription=" });
            foreach (var entry in resultsMedications.Entries)
            {
                ResourceEntry<Medication> meds = (ResourceEntry<Medication>)entry;
                m.Medications = meds.Resource.Name;
                break;
            }

            foreach (var entry in resultsMedicationsAdmin.Entries)
            {
                ResourceEntry<MedicationAdministration> medsA = (ResourceEntry<MedicationAdministration>)entry;
                m.MedicationsAdmin = medsA.Resource.Dosage.ElementAt(0).Quantity.Value + medsA.Resource.Dosage.ElementAt(0).Quantity.Code;
                break;
            }
            patients.Add(m);

            return View(patients);
        }

        public ActionResult MedicalHistory(string ID)
        {
            var client = new FhirClient("http://cr.marc-hi.ca:8080/fhir");
            var client2 = new FhirClient("https://fhir.orionhealth.com/blaze/fhir");

            SearchViewModel m = new SearchViewModel();

            Bundle resultsPatients = client.Search<Patient>(new string[] { "identifier=" + ID });


            //gets patient based on ID
            foreach (var entry in resultsPatients.Entries)
            {
                // since we are searching for a Patient resource, the entries represent patients (needs casting to the resource patient)
                ResourceEntry<Patient> patient = (ResourceEntry<Patient>)entry;

                // get the first values only for now, we will need to use a more sophisticated way to grab values
                m.LastName = patient.Resource.Name.First<HumanName>().FamilyElement.First<FhirString>().Value;
                m.FirstName = patient.Resource.Name.First<HumanName>().GivenElement.First<FhirString>().Value;
                m.Identifier = patient.Resource.Identifier.First<Identifier>().Value;

            }


            //gets family history
            Bundle resultsFamilyHistory = client2.Search<FamilyHistory>(new string[] { "subject=" });

            //gets family history
            foreach (var entry in resultsFamilyHistory.Entries)
            {
                ResourceEntry<FamilyHistory> familyHistoryNotes = (ResourceEntry<FamilyHistory>)entry;
                m.FamilyHistory = familyHistoryNotes.Resource.Note;
                break;
            }

            //searches for procedures in 1991
            Bundle resultsProcedure = client2.Search<Procedure>(new string[] { "date=1991" });

            //gets first procedure and procedure date
            foreach (var entry in resultsProcedure.Entries)
            {
                ResourceEntry<Procedure> procedure = (ResourceEntry<Procedure>)entry;
                m.ProcedureDate = procedure.Resource.Date.Start;
                m.Procedure = procedure.Resource.Type.Coding.ElementAt(0).DisplayElement.Value;
                break;
            }
            patients.Add(m);
            return View(patients);
        }

        //
        // POST: /Patient/Search
        [HttpPost]
        public ActionResult Search(FormCollection collection)
        {
            try
            {
                var client = new FhirClient("http://cr.marc-hi.ca:8080/fhir");

                // get the user's submitted form values
                string id = collection.GetValue("Identifier").AttemptedValue;
                string lastName = collection.GetValue("LastName").AttemptedValue;
                string firstName = collection.GetValue("FirstName").AttemptedValue;

                // search by values extracted above
                Bundle results = client.Search<Patient>(new string[] { 
                    "family=" + lastName, 
                    "given=" + firstName,
                    "identifier=" + id
                });

                // prepare a list of patient search model object (it will be populated in the entries loop below)
                // entries loop = patients found
                List<SearchViewModel> patients = new List<SearchViewModel>();
                foreach (var entry in results.Entries)
                {
                    SearchViewModel m = new SearchViewModel();

                    // since we are searching for a Patient resource, the entries represent patients (needs casting to the resource patient)
                    ResourceEntry<Patient> patient = (ResourceEntry<Patient>)entry;
                    // get the first values only for now, we will need to use a more sophisticated way to grab values
                    try
                    {
                        m.LastName = patient.Resource.Name.First<HumanName>().FamilyElement.First<FhirString>().Value;
                        m.MiddleName = patient.Resource.Name.First<HumanName>().GivenElement.ElementAt(1).Value;
                        m.FirstName = patient.Resource.Name.First<HumanName>().GivenElement.First<FhirString>().Value;
                        m.Identifier = patient.Resource.Identifier.First<Identifier>().Value;
                        m.Gender = patient.Resource.Gender.TextElement.Value;
                        m.BirthDate = patient.Resource.BirthDateElement.Value;
                        m.Line = patient.Resource.Address.First<Address>().LineElement.First<FhirString>().Value;
                        m.City = patient.Resource.Address.First<Address>().CityElement.Value;
                        m.State = patient.Resource.Address.First<Address>().StateElement.Value;
                    }
                    catch
                    {
                        m.LastName = "N/A";
                        m.FirstName = "N/A";
                        m.Identifier = "N/A";
                    }

                    patients.Add(m);
                }

                // if no patient is found, redirect back to the Home page where the user can attempt another search
                // TODO: send back an appropriate error message to be displayed to the user
                if (patients.Count == 0)
                {
                    return RedirectToAction("Index", "Home");
                }

                return View(patients);
            }
            catch
            {
                // any exception? go back to the search form in the Home page
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
