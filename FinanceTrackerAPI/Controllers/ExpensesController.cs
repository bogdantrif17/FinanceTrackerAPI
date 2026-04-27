using FinanceTrackerAPI.Data;
using FinanceTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ExpensesController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/expenses
        [HttpGet]
        public ActionResult<List<Expense>> GetAll()
        {
            return Ok(_db.Expenses.ToList());
        }

        // POST: api/expenses
        [HttpPost]
        public ActionResult<Expense> Create(Expense expense)
        {
            expense.Date = DateTime.Now;
            _db.Expenses.Add(expense);
            _db.SaveChanges();
            return Ok(expense);
        }

        // DELETE: api/expenses/1
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var expense = _db.Expenses.FirstOrDefault(e => e.Id == id);
            if (expense == null) return NotFound();
            _db.Expenses.Remove(expense);
            _db.SaveChanges();
            return Ok();
        }
        // PUT: api/expenses/1
        [HttpPut("{id}")]
        public ActionResult<Expense> Update(int id, Expense updated)
        {
            var expense = _db.Expenses.FirstOrDefault(e => e.Id == id);
            if (expense == null) return NotFound();

            expense.Description = updated.Description;
            expense.Amount = updated.Amount;
            expense.Category = updated.Category;

            _db.SaveChanges();
            return Ok(expense);
        }
        // GET: api/expenses/summary?month=4&year=2026
        [HttpGet("summary")]
        public ActionResult GetSummary(int month, int year)
        {
            var expenses = _db.Expenses
                .Where(e => e.Date.Month == month && e.Date.Year == year)
                .ToList();

            var result = new
            {
                Total = expenses.Sum(e => e.Amount),
                Count = expenses.Count,
                ByCategory = expenses
                    .GroupBy(e => e.Category)
                    .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount) })
            };

            return Ok(result);
        }
    }
}