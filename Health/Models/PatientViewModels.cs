using System.ComponentModel.DataAnnotations;

namespace Health.Models
{
    public class SearchViewModel
    {
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle name")]
        public string MiddleName { get; set; }

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

        [Display(Name = "Line")]
        public string Line { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "CareProvider")]
        public string CareProvider { get; set; }

        [Display(Name = "MaritalStatus")]
        public string MaritalStatus { get; set; }

        [Display(Name = "Telephone")]
        public string Telephone { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "ProcedureDate")]
        public string ProcedureDate { get; set; }

        [Display(Name = "Procedure")]
        public string Procedure { get; set; }

        [Display(Name = "Allergy")]
        public string Allergy { get; set; }

        [Display(Name = "AllergyReaction")]
        public string AllergyReaction { get; set; }

        [Display(Name = "Alert")]
        public string Alert { get; set; }

        [Display(Name = "FamilyHistory")]
        public string FamilyHistory { get; set; }

        [Display(Name = "Medications")]
        public string Medications { get; set; }

        [Display(Name = "MedicationsAdmin")]
        public string MedicationsAdmin { get; set; }

    }
}
