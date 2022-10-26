using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        // GET: api/Addresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddress()
        {
            return await _context.Address.ToListAsync();
        }

        // GET: api/Addresses/Linq
        [HttpGet("/linq")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddressLinq(string? search, string? sortby)
        {
            
            if (sortby != null)
            {
                PropertyInfo propertyInfo = typeof(Address).GetProperty(sortby, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            }
            Stopwatch sw;
            sw = Stopwatch.StartNew();
            
            for (int i = 0; i < 10; i++) {
                var query = _context.Address;
                foreach(PropertyInfo p in typeof(Address).GetProperties())
                {
                    
                    query.Where(a => a.GetType().GetProperty(p.Name).GetValue(a).ToString().Contains(search) );
                }
                Console.WriteLine("Mode:Linq - iteration:"+i+" "+sw.ElapsedMilliseconds);
                query.ToList();
            }

            return await _context.Address.ToListAsync();
        }


        // GET: api/Addresses/rawsql
        [SwaggerOperation(Summary = "Retrieves all addresses containing the search value in any column")]
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
                if (p.PropertyType == typeof(string))
                {
                    raw_query += p.Name + " LIKE '%" + search + "%' OR "; // LIKE operator is not case sensitive, we can use COLLATE to change that if needed
                }
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
    }
}
