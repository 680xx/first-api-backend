using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using first_api_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using first_api_backend.Context;

namespace first_api_backend.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CompanyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _context.Companies.ToListAsync();
            return Ok(companies);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompany(long id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return Ok(company);
        }

        // POST: api/company
        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] Company company)
        {
            if (company == null)
            {
                return BadRequest("Product cannot be null.");
            }
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
        }

        // PUT: api/companies/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(long id, [FromBody] Company updatedCompany)
        {
            if (id != updatedCompany.Id)
            {
                return BadRequest("Product ID mismatch.");
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            company.Name = updatedCompany.Name;
            company.OrganizationNumber = updatedCompany.OrganizationNumber;
            company.ContactPerson = updatedCompany.ContactPerson;
            company.PhoneNumber = updatedCompany.PhoneNumber;

            _context.Companies.Update(company);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

        // DELETE: api/companies/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(long id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }
    }
}