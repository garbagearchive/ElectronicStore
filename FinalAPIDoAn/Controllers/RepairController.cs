using FinalAPIDoAn.MyModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepairController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;
        public RepairController(KetNoiCSDL dbc)
        {
            _dbc = dbc;
        }
        [HttpGet("List")]
        public IActionResult GetAllRepair()
        {
            var repair = _dbc.ProductRepairs.ToList();
            return Ok(new { data = repair });
        }
        [HttpGet("Search")]
        public IActionResult GetRepairById(int id)
        {
            var repair = _dbc.ProductRepairs.SingleOrDefault(o => o.ProductId == id);
            if (repair == null) return NotFound(new { message = "Repair not found." });
            return Ok(new { data = repair });
        }
        [HttpPost("Add")]
        public IActionResult AddRepair([FromBody] RepairDto repairDto)
        {
            if (repairDto.RepairID <= 0 || repairDto.ProductID <= 0 || repairDto.UserID <= 0 || string.IsNullOrWhiteSpace(repairDto.IssueDescription))
            {
                return BadRequest(new { message = "Invalid data." });
            }

            var repair = new ProductRepair
            {
                RepairId = repairDto.RepairID,
                ProductId = repairDto.ProductID,
                UserId = repairDto.UserID,
                IssueDescription = repairDto.IssueDescription,
                RepairStatus = repairDto.RepairStatus ?? "Pending"
            };


            _dbc.ProductRepairs.Add(repair);
            _dbc.SaveChanges();

            return CreatedAtAction(nameof(GetRepairById), new { id = repair.ProductId }, repair);
        }
        [HttpPut(" Update")]
        public IActionResult UpdateRepair([FromBody] RepairDto repairDto)
        {
            if (repairDto.RepairID <= 0 || repairDto.ProductID <= 0 || repairDto.UserID <= 0 || string.IsNullOrWhiteSpace(repairDto.IssueDescription))
            {
                return BadRequest(new { message = "Invalid data." });
            }

            var repair = new ProductRepair
            {
                RepairId = repairDto.RepairID,
                ProductId = repairDto.ProductID,
                UserId = repairDto.UserID,
                IssueDescription = repairDto.IssueDescription,
                RepairStatus = repairDto.RepairStatus ?? "Success",
                RepairCompletionDate = repairDto.RepairCompletionDate
            };


            _dbc.ProductRepairs.Update(repair);
            _dbc.SaveChanges();

            return CreatedAtAction(nameof(GetRepairById), new { id = repair.ProductId }, repair);
        }

        [HttpDelete("Delete")]
        public IActionResult DeleteUpdate(int repairid)
        {
            var repair= _dbc.ProductRepairs.SingleOrDefault(o => o.ProductId == repairid);
            if (repair == null) return NotFound(new { message = "Not found." });
            _dbc.ProductRepairs.Remove(repair);
            _dbc.SaveChanges();
            return Ok(new { message = "Deleted successfully." });
        }

    }
    public class RepairDto
    {
        public required int RepairID { get; set; }
        public required int ProductID { get; set; }
        public required int UserID { get; set; }
        public required DateTime RepairRequestDate { get; set; }
        public required string IssueDescription { get; set; }
        public required string RepairStatus{ get; set; }
        public required DateTime RepairCompletionDate { get; set; }
    }
}
