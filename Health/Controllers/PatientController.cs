/**
 * Copyright 2015-2015 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: oskamt
 * Date: 26-3-2015
 */

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
                SearchViewModel m = new SearchViewModel();

                var client = new FhirClient("http://fhirtest.uhn.ca/baseDstu1");

                //search patients based on patientID clicked
                Bundle resultsPatients = client.Search<Patient>(new string[] { "_id=" + ID});

                //gets patient based on ID
                foreach (var entry in resultsPatients.Entries)
                {

                        ResourceEntry<Patient> patient = (ResourceEntry<Patient>)entry;

                        m = getPatientInfo(patient);
                     
                }

            Bundle resultsAllergies = client.Search<AllergyIntolerance>(new string[] { "subject=" + ID});
            foreach (var entry in resultsAllergies.Entries)
            {
                m.AllergiesCount++;
            }
            Bundle resultsMedications = client.Search<MedicationPrescription>(new string[] { "patient._id=" + ID});
            foreach (var entry in resultsMedications.Entries)
            {
                m.MedicationCount++;
            }
            Bundle resultsConditions = client.Search<Condition>(new string[] { "subject=" + ID});
            foreach (var entry in resultsConditions.Entries)
            {
                m.ConditionsCount++;
            }
            Bundle resultsDevices = client.Search<Device>(new string[] { "patient._id=" + ID });
            foreach (var entry in resultsDevices.Entries)
            {
                m.DevicesCount++;
            }


                patients.Add(m);

                return View(patients);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            
 
        }

        public ActionResult Allergies(string ID)
        {
            SearchViewModel m = new SearchViewModel();
            List<AllergyViewModel> allergyList = new List<AllergyViewModel>();

            var client = new FhirClient("http://fhirtest.uhn.ca/baseDstu1");

            //get patient basic info
            Bundle resultsPatients = client.Search<Patient>(new string[] { "_id=" + ID });
            foreach (var entry in resultsPatients.Entries)
            {
                ResourceEntry<Patient> patient = (ResourceEntry<Patient>)entry;

                m = getPatientInfo(patient);
            }

            //get patient allergy information
            Bundle resultsAllergies = client.Search<AllergyIntolerance>(new string[] { "subject=" + ID });
            foreach (var entry in resultsAllergies.Entries)
            {
                AllergyViewModel allergies = new AllergyViewModel();
                ResourceEntry<AllergyIntolerance> allergy = (ResourceEntry<AllergyIntolerance>)entry;
                allergies.AllergyName = allergy.Resource.Substance.DisplayElement.Value;

                    if (allergy.Resource.Reaction != null)
                    {
                        allergies.Severity = allergy.Resource.Reaction.FirstOrDefault<ResourceReference>().Display;
                    }
                    else
                    {
                        allergies.Severity = "Unknown Sensitivity to";
                    }
                    m.AllergiesCount++;

                    allergyList.Add(allergies);

            }
            m.Allergies = allergyList;

            patients.Add(m);

            return View(patients);
        }

        public ActionResult Medications(string ID)
        {
            SearchViewModel m = new SearchViewModel();
            List<MedicationViewModel> medicationList = new List<MedicationViewModel>();

            var client = new FhirClient("http://fhirtest.uhn.ca/baseDstu1");

            //search patients based on patientID clicked
            Bundle resultsPatients = client.Search<Patient>(new string[] { "_id=" + ID});
            

            //gets patient based on ID
            foreach (var entry in resultsPatients.Entries)
            {
                    ResourceEntry<Patient> patient = (ResourceEntry<Patient>)entry;

                    m = getPatientInfo(patient);
            }

            Bundle resultsMedications = client.Search<MedicationPrescription>(new string[] { "patient._id=" + ID });
            foreach (var entry in resultsMedications.Entries)
            {
                m.MedicationCount++;
                MedicationViewModel meds = new MedicationViewModel();
                ResourceEntry<MedicationPrescription> medication = (ResourceEntry<MedicationPrescription>)entry;

                //get name of medication
                meds.MedicationName = medication.Resource.Medication.Display;

                //get date medication was prescribed
                if (medication.Resource.DateWrittenElement == null)
                {
                    meds.IsActive = "Unknown";
                }
                else
                {
                    meds.IsActive = medication.Resource.DateWrittenElement.Value;
                }
                medicationList.Add(meds);
            }             

            m.Medications = medicationList;

            patients.Add(m);

            return View(patients);
        }

        public ActionResult SpecialNeeds(string ID)
        {
            SearchViewModel m = new SearchViewModel();
            List<AlertViewModel> specialNeedList = new List<AlertViewModel>();

            var client = new FhirClient("http://fhirtest.uhn.ca/baseDstu1");

            //search patients based on patientID clicked
            Bundle resultsPatients = client.Search<Patient>(new string[] { "_id=" + ID});
            

            //gets patient based on ID
            foreach (var entry in resultsPatients.Entries)
            {

                    ResourceEntry<Patient> patient = (ResourceEntry<Patient>)entry;

                    m = getPatientInfo(patient);
            }

            Bundle resultsAlerts = client.Search<Alert>(new string[] {"subject=" + ID});
            foreach (var entry in resultsAlerts.Entries)
            {
                if (entry.ToString().Contains("Alert"))
                {
                    AlertViewModel alerts = new AlertViewModel();
                    ResourceEntry<Alert> alert = (ResourceEntry<Alert>)entry;

                    alerts.SpecialNeed = alert.Resource.Note;

                    specialNeedList.Add(alerts);
                }
                    
            }
            m.SpecialNeeds = specialNeedList;

            patients.Add(m);

            return View(patients);
        }

        public ActionResult ContactInfo(string ID)
        {
            SearchViewModel m = new SearchViewModel();
            List<string> EmergencyContactInformation = new List<string>();
            List<string> PractitionerContactInformation = new List<string>();

            var client = new FhirClient("http://fhirtest.uhn.ca/baseDstu1");

            //search patients based on patientID clicked
            Bundle resultsPatients = client.Search<Patient>(new string[] { "_id=" + ID });
            

            //gets patient based on ID
            foreach (var entry in resultsPatients.Entries)
            {

                    ResourceEntry<Patient> patient = (ResourceEntry<Patient>)entry;

                    m = getPatientInfo(patient);
            
                if (patient.Resource.Contact != null)
                {
                    //get patients contact info
                    foreach (var contact in patient.Resource.Contact)
                    {
                        EmergencyContactInformation.Add(contact.Relationship.FirstOrDefault<CodeableConcept>().Coding.FirstOrDefault<Coding>().Display);
                        EmergencyContactInformation.Add(contact.Name.TextElement.Value);
                        if (contact.Telecom != null)
                        {
                            foreach (var contactNumber in contact.Telecom)
                            {
                                EmergencyContactInformation.Add(contactNumber.Use + ": " + contactNumber.Value);
                            }

                        }
                        else
                        {
                            EmergencyContactInformation.Add("Unknown Contact Number");
                        }

                    }
                    m.EmergencyContact = EmergencyContactInformation;
                }
                    
                if (patient.Resource.CareProvider != null)
                {
                    //get patients contact info
                    foreach (var contact in patient.Resource.CareProvider)
                    {
                        PractitionerContactInformation.Add("Dr. " + contact.Display);
                    }
                    m.PractitionerContact = PractitionerContactInformation;
                }
                   
                }

            patients.Add(m);

            return View(patients);
        }

        public ActionResult Devices(string ID)
        {
            SearchViewModel m = new SearchViewModel();
            List<DeviceViewModel> deviceList = new List<DeviceViewModel>();

            var client = new FhirClient("http://fhirtest.uhn.ca/baseDstu1");

            //search patients based on patientID clicked
            Bundle resultsPatients = client.Search<Patient>(new string[] { "_id=" + ID});
            

            //gets patient based on ID
            foreach (var entry in resultsPatients.Entries)
            {

                    ResourceEntry<Patient> patient = (ResourceEntry<Patient>)entry;

                    m = getPatientInfo(patient);
            }

            Bundle resultsDevices = client.Search<Device>(new string[] {"patient._id=" + ID});
            foreach (var entry in resultsDevices.Entries)
            {
                    DeviceViewModel devices = new DeviceViewModel();
                    ResourceEntry<Device> device = (ResourceEntry<Device>)entry;
                    m.DevicesCount++;

                    devices.DeviceName = device.Resource.Type.TextElement.Value;
                    deviceList.Add(devices);

            }
            m.Devices = deviceList;

            patients.Add(m);

            return View(patients);
        }

        public ActionResult MedicalHistory(string ID)
        {
            SearchViewModel m = new SearchViewModel();
            List<ConditionViewModel> conditionList = new List<ConditionViewModel>();

            var client = new FhirClient("http://fhirtest.uhn.ca/baseDstu1");

            //search patients based on patientID clicked
            Bundle resultsPatients = client.Search<Patient>(new string[] { "_id=" + ID });
            

            //gets patient based on ID
            foreach (var entry in resultsPatients.Entries)
            {
                    ResourceEntry<Patient> patient = (ResourceEntry<Patient>)entry;

                    m = getPatientInfo(patient);
                }

            Bundle resultsConditions = client.Search<Condition>(new string[]{"subject=" + ID});

            foreach (var entry in resultsConditions.Entries)
            {
                 ConditionViewModel conditions = new ConditionViewModel();
                    ResourceEntry<Condition> condition = (ResourceEntry<Condition>)entry;
                    if (condition.Resource.Code != null)
                    {
                        conditions.ConditionName = condition.Resource.Code.Text;
                        
                    }
                    else
                    {
                        conditions.ConditionName = "Unknown";

                    }

                if (condition.Resource.Onset != null)
                    {
                        conditions.Date = (condition.Resource.Onset as Date).Value;
                    }
                    else
                    {
                        conditions.Date = "Unknown";
                    }
                m.ConditionsCount++;
                conditionList.Add(conditions);
            }
                   
            m.Conditions = conditionList;

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
                // get the user's submitted form values
                string id = collection.GetValue("Identifier").AttemptedValue;
                string lastName = collection.GetValue("LastName").AttemptedValue;
                string firstName = collection.GetValue("FirstName").AttemptedValue;

                List<SearchViewModel> patients = new List<SearchViewModel>();

                var client = new FhirClient("http://fhirtest.uhn.ca/baseDstu1");

                

                // search by values extracted above
                Bundle results;
                

                if (id.Equals(""))
                {
                        results = client.Search<Patient>(new string[] { 
                        "given=" + firstName,
                        "family=" + lastName
                        });
                }
                else
                {
                        results = client.Search<Patient>(new string[] { 
                    "_id=" + id
                    });
                }

                

                // prepare a list of patient search model object (it will be populated in the entries loop below)
                // entries loop = patients found
                

                foreach (var entry in results.Entries)
                {

                    SearchViewModel m = new SearchViewModel();

                    // since we are searching for a Patient resource, the entries represent patients (needs casting to the resource patient)
                    ResourceEntry<Patient> patient = (ResourceEntry<Patient>)entry;

                    m = getPatientInfo(patient);
                    

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
                return RedirectToAction("Index", "Home", new {error = "Member ID does not exist."});
            }
        }

        //gets basic patient information
        private SearchViewModel getPatientInfo(ResourceEntry<Patient> patient)
        {
            SearchViewModel patientInfo = new SearchViewModel();

                        patientInfo.LastName = patient.Resource.Name.FirstOrDefault<HumanName>().FamilyElement.FirstOrDefault<FhirString>().Value;
                        patientInfo.FirstName = patient.Resource.Name.First<HumanName>().GivenElement.First<FhirString>().Value;
                        patientInfo.Identifier = patient.Id.Segments.ElementAt(3);

                        //get patient photo if it exists
                        List<Attachment> photos = patient.Resource.Photo;

                        if (photos != null)
                        {
                            Attachment photo = photos.FirstOrDefault();
                            if (photo != null)
                            {
                                if (photo.Data != null)
                                {
                                    patientInfo.Photo = Convert.ToBase64String(photo.Data);
                                }
                            }
                        }

                        //get patient gender
                        if (patient.Resource.Gender == null)
                        {
                            patientInfo.Gender = "unknown";
                        }
                        else if (patient.Resource.Gender.Text != null)
                        {
                            patientInfo.Gender = patient.Resource.Gender.Text;
                        }
                        else
                        {
                            patientInfo.Gender = patient.Resource.Gender.Coding.FirstOrDefault<Coding>().Code;
                        }

                        //get birthdate and age
                        var dateOfBirth = patient.Resource.BirthDateElement.Value;
                        patientInfo.BirthDate = patient.Resource.BirthDateElement.Value;
                        DateTime dob = Convert.ToDateTime(dateOfBirth);
                        DateTime now = DateTime.Now;
                        var age = (int)(now.Year - dob.Year);
                        if (dob.Month > now.Month)
                        {
                            age--;
                        }
                        patientInfo.Age = age + " Years";

                        //get address information, check for address line
                        if (patient.Resource.Address == null)
                        {
                            patientInfo.Line = "N/A";
                            patientInfo.City = "N/A";
                            patientInfo.State = "N/A";
                        }
                        else
                        {
                            if (patient.Resource.Address.FirstOrDefault<Address>().LineElement == null)
                            {
                                patientInfo.Line = "N/A";
                            }
                            else
                            {
                                patientInfo.Line = patient.Resource.Address.FirstOrDefault<Address>().LineElement.First<FhirString>().Value;
                            }

                            if (patient.Resource.Address.FirstOrDefault<Address>().CityElement == null)
                            {
                                patientInfo.City = "N/A";
                            }
                            else
                            {
                                patientInfo.City = patient.Resource.Address.FirstOrDefault<Address>().CityElement.Value;
                            }

                            if (patient.Resource.Address.FirstOrDefault<Address>().CountryElement == null)
                            {
                                patientInfo.State = "N/A";
                            }
                            else
                            {
                                patientInfo.State = patient.Resource.Address.FirstOrDefault<Address>().CountryElement.Value;
                            }

                        }
                        List<string> ContactInformation = new List<string>();
            //get patients contact info
            foreach (var contact in patient.Resource.Telecom)
            {
                ContactInformation.Add(contact.Value);
            }
            patientInfo.ContactInfo = ContactInformation;
            return patientInfo;
        }

    }

}
