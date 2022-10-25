using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SocialBrothersCase
{
    public class Address
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Street { get; set; }
        [Required]
        public int HouseNumber { get; set; }
        [Required]
        public string? ZipCode { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? Country { get; set; }


        public IDictionary<string, string> validate()
        {
            IDictionary<string, string> errors = new Dictionary<string, string>();

            if (HouseNumber<1 || HouseNumber > 999)
            {
                errors.Add("HouseNumber", "field must be positive and not longer than 3 digits [1..999]");
            }

            // There is no correct zip code format that applies to all countries but
            //Every postal code system uses only A - Z and / or 0 - 9 and sometimes space / dash
            //Not every country uses postal codes(ex.Ireland outside of Dublin), but we'll ignore that here.
            //The shortest postal code format is Sierra Leone with NN
            //The longest is American Samoa with NNNNN-NNNNNN
            //You should allow one space or dash.
            //Should not begin or end with space or dash

            //Another solution is to load a specific regex expression for every country,but we would need to at least offer a country list through our API
            //or it will be ambiguous

            string ZipCodePattern = "(?i)^[a-z0-9][a-z0-9\\-]{0,10}[a-z0-9]$"; //maximum of 10 characters A-Z,0-9 and 1 whitespace or dash
            if (!Regex.IsMatch(ZipCode, ZipCodePattern))
            {
                errors.Add("ZipCode" ,"field can only contain a maximum of 11 alphanumeric characters and 1 seperator (whitespace or dash)");
            }

            string StreetPattern = "^[a-zA-Z 0-9\\.\\,\\-]*$"; // only alphanumeric, comma, dash and . (dot? fullstop?)
            if (!Regex.IsMatch(Street, StreetPattern))
            {
                errors.Add("Street" ,"field can only contain alphanumeric, comma, dash and .");
            }

            string CountryPattern = "^[a-zA-Z]*$"; // only alphanumeric, probably inaccurate and may have to add special characters
            if (!Regex.IsMatch(Country, CountryPattern))
            {
                errors.Add("Country" ,"field can only countain alphabetic characters");
            }
            if (!Regex.IsMatch(City, CountryPattern)) // need to double check this one too
            {
                errors.Add("City" ,"field can only countain alphabetic characters");
            }




            return errors;
        }
    }

}