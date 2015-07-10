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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Health.Models
{
    public class SearchViewModel
    {
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Identifier")]
        public string Identifier { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "BirthDate")]
        public string BirthDate { get; set; }

        [Required]
        [Display(Name = "Age")]
        public string Age { get; set; }

        [Display(Name = "Line")]
        public string Line { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Photo")]
        public string Photo { get; set; }

        [Display(Name = "Contact Information")]
        public List<string> ContactInfo { get; set; }

        [Display(Name = "Emergency Contact")]
        public List<string> EmergencyContact { get; set; }

        [Display(Name = "Practitioner Contact")]
        public List<string> PractitionerContact { get; set; }

        [Display(Name = "Medications")]
        public List<MedicationViewModel> Medications { get; set; }

        [Display(Name = "Special Needs")]
        public List<AlertViewModel> SpecialNeeds { get; set; }

        [Display(Name = "Conditions")]
        public List<ConditionViewModel> Conditions { get; set; }

        [Display(Name = "Allergies")]
        public List<AllergyViewModel> Allergies { get; set; }

        [Display(Name = "Devices")]
        public List<DeviceViewModel> Devices { get; set; }

        [Display(Name = "Medication Count")]
        public int MedicationCount { get; set; }

        [Display(Name = "Conditions Count")]
        public int ConditionsCount { get; set; }

        [Display(Name = "Allergies Count")]
        public int AllergiesCount { get; set; }

        [Display(Name = "Devices Count")]
        public int DevicesCount { get; set; }


    }
}
