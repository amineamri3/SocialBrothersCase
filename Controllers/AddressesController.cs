using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using SocialBrothersCase;
using SocialBrothersCase.Data;
using Swashbuckle.AspNetCore.Annotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SocialBrothersCase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly SocialBrothersCaseContext _context;

        public AddressesController(SocialBrothersCaseContext context)
        {
            _context = context;
        }

  

        // GET: api/Addresses/Code
        [HttpGet("/code")]
        [SwaggerOperation(Summary = "Retrieves all addresses containing the search value in any column", Description = "We grab the whole table and filter the data programatically")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddressCode(
            [FromQuery, SwaggerParameter(Description = "the value we are searching for", Required = false)] string? search,
            [FromQuery, SwaggerParameter(Description = "the field we want to sort the result by, this is optional and case-insensetive", Required = false)] string? sortby,
            [FromQuery, SwaggerParameter(Description = "the direction of the sort, can be either ASC (Ascending) or DESC (Descending). this is optional and case-insensetive", Required = false)] string? sort)
        {

            IDictionary<string, string> errors = new Dictionary<string, string>();
            PropertyInfo propertyInfo = null;
            string sort_direction = "";
            if (sortby != null)
            {
                propertyInfo = typeof(Address).GetProperty(sortby, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance); //returns null if not found  
            }
            if (sort != null && !sort.ToUpper().Equals("ASC") && !sort.ToUpper().Equals("DESC"))
            {
                errors.Add("sort", "invalid sort field, expected value ASC or DESC");
            }
            else if (sort != null)
            {
                sort_direction = sort; // sort_direction can have its initial value "" because its an optional field in queries
            }
            if (sortby != null && propertyInfo == null)
            {
                errors.Add("sortby", "invalid sortby field"); // this code can be removed if you want the request to ignore invalid sortby parameter
            }
            if (errors.Count > 0)
            {
                return BadRequest(errors); // if optional fields are filled but contain invalid data
            }
            var stringProperties = typeof(Address).GetProperties();

            var all = _context.Address.ToList();
            all = all.Where(a =>stringProperties.Any(prop =>prop.GetValue(a, null).ToString().Contains(search))).ToList();
            // sort not implemented, solution is already way too slow
            return all;

            

        }

        // GET: api/Addresses/Linq
        [HttpGet("/linq")]
        [HttpGet]
        [SwaggerOperation(Summary = "Retrieves all addresses containing the search value in any column", Description = "This route takes the route of constructing a Linq expression dynamically")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddressLinq(
            [FromQuery, SwaggerParameter(Description = "the value we are searching for", Required = false)] string? search,
            [FromQuery, SwaggerParameter(Description = "the field we want to sort the result by, this is optional and case-insensetive", Required = false)] string? sortby,
            [FromQuery, SwaggerParameter(Description = "the direction of the sort, can be either ASC (Ascending) or DESC (Descending). this is optional and case-insensetive", Required = false)] string? sort)
        {
            if (search == null && sortby == null) return await _context.Address.ToListAsync();

            IDictionary<string, string> errors = new Dictionary<string, string>();
            PropertyInfo propertyInfo = null;
            string sort_direction = "";
            if (sortby != null)
            {
                propertyInfo = typeof(Address).GetProperty(sortby, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance); //returns null if not found  
            }
            if (sort != null && !sort.ToUpper().Equals("ASC") && !sort.ToUpper().Equals("DESC"))
            {
                errors.Add("sort", "invalid sort field, expected value ASC or DESC");
            }
            else if (sort != null)
            {
                sort_direction = sort; // sort_direction can have its initial value "" because its an optional field in queries
            }
            if (sortby != null && propertyInfo == null)
            {
                errors.Add("sortby", "invalid sortby field"); // this code can be removed if you want the request to ignore invalid sortby parameter
            }
            if (errors.Count > 0)
            {
                return BadRequest(errors); // if optional fields are filled but contain invalid data
            }

            var stringProperties = typeof(Address).GetProperties().Where(p => p.PropertyType == typeof(string)).ToList();
            
            var query = _context.Address.AsQueryable();
            var where_clause = "";
            if (search != null) {
            foreach (var property in stringProperties)
            {
                where_clause += property.Name + ".Contains(@0) OR ";
            }
            where_clause += "false";

            query = query.Where(where_clause, search);
            }
            if (sort_direction != "") sortby += " " + sort_direction;
            query = query.OrderBy(sortby);



            return await query.ToListAsync();
        }


        // GET: api/Addresses/rawsql
        [SwaggerOperation(Summary = "Retrieves all addresses containing the search value in any column", Description ="This route takes the route of constructing a raw SQL query by grabbing the entity attributes dynamically")]
        [SwaggerResponse(200, "Addresses retrieved")]
        [SwaggerResponse(404, "No addresses found")]
        [SwaggerResponse(400, "Invalid parameters passed")]
        [HttpGet("/rawsql")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresssql(
            [FromQuery, SwaggerParameter(Description = "the value we are searching for", Required =false)] string? search,
            [FromQuery, SwaggerParameter(Description = "the field we want to sort the result by, this is optional and case-insensetive", Required = false)] string? sortby,
            [FromQuery, SwaggerParameter(Description = "the direction of the sort, can be either ASC (Ascending) or DESC (Descending). this is optional and case-insensetive", Required = false)] string? sort)
        {
            IDictionary<string, string> errors = new Dictionary<string, string>();
            PropertyInfo propertyInfo = null;
            string sort_direction = "";
            if (sortby != null)
            {
                propertyInfo = typeof(Address).GetProperty(sortby, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance); //returns null if not found  
            }
            if(sort!=null && !sort.ToUpper().Equals("ASC") && !sort.ToUpper().Equals("DESC"))
            {
                errors.Add("sort", "invalid sort field, expected value ASC or DESC");
            }else if (sort != null)
            {
                sort_direction = sort; // sort_direction can have its initial value "" because its an optional field in queries
            }
            if (sortby != null && propertyInfo == null)
            {
                errors.Add("sortby", "invalid sortby field"); // this code can be removed if you want the request to ignore invalid sortby parameter
            }
            if (errors.Count > 0)
            {
                return BadRequest(errors); // if optional fields are filled but contain invalid data
            }


            string raw_query = "SELECT * From Address WHERE ";
            foreach (PropertyInfo p in typeof(Address).GetProperties())
            {
                //TODO: check the p.type, some types like blob might cause issues with the LIKE operator (to further check)
                raw_query += p.Name + " LIKE '%" + search + "%' OR "; // LIKE operator is not case sensitive, we can use COLLATE to change that if needed
                //TODO: Sanetize inputs or use some form of parametered query builder to secure against injections
            }
            raw_query += "0"; // to fix the last OR without complicating the loop


            if (propertyInfo != null) // if a correct attribute was passed in the sortby param
            {
                raw_query += " ORDER BY " + propertyInfo.Name +" "+ sort_direction;
            }
    
            List<Address> result = await _context.Address.FromSqlRaw(raw_query).ToListAsync();

            if(result.Count == 0) return NotFound();

            return result;   
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            var address = await _context.Address.FindAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            return address;
        }

        // PUT: api/Addresses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(int id, Address address)
        {
            if (id != address.Id)
            {
                return BadRequest();
            }
            IDictionary<string, string> err = address.validate();

            if (err.Count > 0)
            {
                return BadRequest(err);
            }

            _context.Entry(address).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Addresses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Address>> PostAddress(Address address)
        {
            IDictionary<string, string> err = address.validate();

            if (err.Count > 0){
                return BadRequest(err);        
            }
            
            _context.Address.Add(address);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetAddress", new { id = address.Id }, address);
            
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.Address.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            _context.Address.Remove(address);
            await _context.SaveChangesAsync();

            return NoContent();
        }



    
        [SwaggerOperation(Summary = "Calculate the distance between 2 addresses", Description = "This route takes 2 id's , fetches their data from the database and forward geocodes the addresses using a public API")]
        [SwaggerResponse(200, "Distance calculated")]
        [SwaggerResponse(404, "IDs not found")]
        [SwaggerResponse(400, "Invalid data recieved")]
        [HttpGet("/distance")]
        public async Task<ActionResult<string>> GetDistance(
            [FromQuery, SwaggerParameter(Description = "The id of the first address", Required = true)] int id1,
            [FromQuery, SwaggerParameter(Description = "The id of the second address", Required = true)] int id2)
        {
            string apiKey = "5c0aeaece57797c00c644cc377cdd648";

            var address1 = await _context.Address.FindAsync(id1);
            var address2 = await _context.Address.FindAsync(id2);
            if (address1 == null || address2 == null) return NotFound();
            string str_adr1 = address1.HouseNumber + " " + address1.Street + ", " + address1.City + ", " + address1.Country;
            string str_adr2 = address2.HouseNumber + " " + address2.Street + ", " + address2.City + ", " + address2.Country;
            

            //string apikey = Environment.GetEnvironmentVariable("apiKey"); for the sake of simplicity we will keep the key hardcoded

            
            string url1 = $"http://api.positionstack.com/v1/forward?access_key={apiKey}&query={str_adr1}&limit=1";
            string url2 = $"http://api.positionstack.com/v1/forward?access_key={apiKey}&query={str_adr2}&limit=1";

            Console.WriteLine(url1);
            Console.WriteLine(url2);

            HttpClientHandler handler = new HttpClientHandler(){AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate};
            HttpClient client = new HttpClient(handler);
            HttpResponseMessage response1 = client.GetAsync(url1).Result;
            HttpResponseMessage response2 = client.GetAsync(url2).Result;
            response1.EnsureSuccessStatusCode(); // TODO: handle exception
            response2.EnsureSuccessStatusCode();
            string result1 = response1.Content.ReadAsStringAsync().Result;
            string result2 = response2.Content.ReadAsStringAsync().Result;

            JObject res1json = JObject.Parse(result1);
            JObject res2json = JObject.Parse(result2);

            var x1 = res1json["data"][0]["latitude"].Value<double>();
            var y1 = res1json["data"][0]["longitude"].Value<double>();
            var x2 = res2json["data"][0]["latitude"].Value<double>();
            var y2 = res2json["data"][0]["longitude"].Value<double>();



            return DistanceTo(x1,y1,x2,y2).ToString("0.### Km"); ;



        }





    private bool AddressExists(int id)
        {
            return _context.Address.Any(e => e.Id == id);
        }


        private bool isAttribute (Type obj,string att)
        {
            foreach(PropertyInfo p in obj.GetProperties())
            {
                if (p.Name.Equals(att)) return true;
            }
            return false;
        }

        public static double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }
    }
}
