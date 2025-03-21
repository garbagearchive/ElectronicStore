using FinalAPIDoAn.Data;
using FinalAPIDoAn.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.ComponentModel.DataAnnotations;

    namespace FinalAPIDoAn.Controllers
    {
        [Route("api/repairs")]
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
                var repairs = _dbc.ProductRepairs.ToList();
                return Ok(new { data = repairs });
            }

            [HttpGet("Search/{id}")]
            public IActionResult GetRepairById(int id)
            {
                var repair = _dbc.ProductRepairs.SingleOrDefault(r => r.RepairId == id);
                if (repair == null)
                {
                    return NotFound(new { message = "Repair not found." });
                }
                return Ok(new { data = repair });
            }

            [HttpPost("Add")]
            public IActionResult AddRepair([FromBody] RepairDto repairDto)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var repair = new ProductRepair
                {
                    ProductId = repairDto.ProductID,
                    UserId = repairDto.UserID,
                    IssueDescription = repairDto.IssueDescription,
                    RepairStatus = repairDto.RepairStatus ?? "Pending",
                    RepairRequestDate = repairDto.RepairRequestDate,
                    RepairCompletionDate = repairDto.RepairCompletionDate
                };

                _dbc.ProductRepairs.Add(repair);
                _dbc.SaveChanges();

                return CreatedAtAction(nameof(GetRepairById), new { id = repair.RepairId }, repair);
            }

            [HttpPut("Update/{id}")]
            public IActionResult UpdateRepair(int id, [FromBody] RepairDto repairDto)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var repair = _dbc.ProductRepairs.FirstOrDefault(r => r.RepairId == id);
                if (repair == null)
                {
                    return NotFound(new { message = "Repair not found." });
                }

                repair.ProductId = repairDto.ProductID;
                repair.UserId = repairDto.UserID;
                repair.IssueDescription = repairDto.IssueDescription;
                repair.RepairStatus = repairDto.RepairStatus ?? "Success";
                repair.RepairCompletionDate = repairDto.RepairCompletionDate;

                _dbc.ProductRepairs.Update(repair);
                _dbc.SaveChanges();

                return Ok(new { data = repair });
            }

            [HttpDelete("Delete/{repairid}")]
            public IActionResult DeleteRepair(int repairid)
            {
                var repair = _dbc.ProductRepairs.FirstOrDefault(r => r.RepairId == repairid);
                if (repair == null)
                {
                    return NotFound(new { message = "Repair not found." });
                }

                _dbc.ProductRepairs.Remove(repair);
                _dbc.SaveChanges();
                return Ok(new { message = "Deleted successfully." });
            }
        }

        public class RepairDto
        {
            [Required]
            public int ProductID { get; set; }

            [Required]
            public int UserID { get; set; }

            [Required]
            public DateTime RepairRequestDate { get; set; }

            [Required]
            public string IssueDescription { get; set; }

            public string RepairStatus { get; set; } = "Pending";

            public DateTime? RepairCompletionDate { get; set; }
        }
    }
