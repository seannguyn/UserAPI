using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using UsersAPI.Models;
using UsersAPI.Repositories;
using System.Linq;
using System.Threading.Tasks;
using UsersAPI.UtilClasses;

namespace UsersAPI.Controllers
{
    [Route("api/group")]
    [ApiController]
    public class GroupControllers : ControllerBase
    {
        private GroupRepo _groupRepo;

        public GroupControllers(GroupRepo groupRepo)
        {
            _groupRepo = groupRepo;
        }

        // GET api/group
        [HttpGet]
        public ActionResult<CustomResponse<Group>> Get()
        {
            CustomResponse<Group> response = new CustomResponse<Group>(_groupRepo.Groups.ToList());
            return Ok(response);
        }

        // GET: api/group/{id}
        [HttpGet]
        [Route("{id}")]
        public ActionResult<CustomResponse<Group>> GetById(int id)
        {
            CustomResponse<Group> response = new CustomResponse<Group>();

            if (id <= 0)
            {
                return NotFound("Group id must be higher than zero");
            }
            Group group = _groupRepo.Groups.FirstOrDefault(s => s.GroupID == id);
            if (group == null)
            {
                return NotFound("Group not found");
            }
            response.addResult(group);
            return Ok(response);
        }

        // POST: api/group
        [HttpPost]
        public async Task<ActionResult<CustomResponse<Group>>> Post([FromBody]Group group)
        {
            CustomResponse<Group> response = new CustomResponse<Group>();

            if (group == null)
            {
                return NotFound("Group data is not supplied");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _groupRepo.Groups.AddAsync(group);
            await _groupRepo.SaveChangesAsync();

            response.addResult(group);
            return Ok(response);
        }

        // PUT: api/group/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomResponse<Group>>> Update(int id, [FromBody]Group group)
        {
            CustomResponse<Group> response = new CustomResponse<Group>();

            if (group == null)
            {
                return NotFound("Group data is not supplied");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Group existingGroup = _groupRepo.Groups.FirstOrDefault(u => u.GroupID == id);

            if (existingGroup == null)
            {
                return NotFound("Group does not exist in the database");
            }

            existingGroup.GroupName = group.GroupName;
            existingGroup.Description = group.Description;

            await _groupRepo.SaveChangesAsync();

            response.addResult(existingGroup);
            return Ok(response);
        }

        // DELETE: api/group/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<CustomResponse<Group>>> Delete(int? id)
        {
            CustomResponse<Group> response = new CustomResponse<Group>();

            if (id == null)
            {
                return NotFound("Id is not supplied");
            }
            Group group = _groupRepo.Groups.FirstOrDefault(u => u.GroupID == id);
            if (group == null)
            {
                return NotFound("No Group found with particular id supplied");
            }
            _groupRepo.Groups.Remove(group);
            await _groupRepo.SaveChangesAsync();

            response.addMessage("Group deleted successfully");
            return Ok(response);
        }

        ~GroupControllers()
        {
            _groupRepo.Dispose();
        }
    }
}
