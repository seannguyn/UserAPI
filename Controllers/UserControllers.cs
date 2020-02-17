using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersAPI.Encryption;
using UsersAPI.Models;
using UsersAPI.Repositories;
using UsersAPI.UtilClasses;

namespace UsersAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserControllers : ControllerBase
    {
        private UserRepo _userRepo;
        private GroupRepo _groupRepo;
        private UserGroupRepo _userGroupRepo;

        public UserControllers(UserRepo userRepo, UserGroupRepo userGroupRepo, GroupRepo groupRepo)
        {
            _userRepo = userRepo;
            _userGroupRepo = userGroupRepo;
            _groupRepo = groupRepo;
        }

        // GET api/user
        [HttpGet]
        public ActionResult<CustomResponse<User>> Get()
        {
            CustomResponse<User> response = new CustomResponse<User>(_userRepo.Users.ToList());
            return Ok(response);
        }

        // GET api/user
        [HttpGet]
        [Route("search")]
        public ActionResult<CustomResponse<User>> SearchUser()
        {
            CustomResponse<User> response = new CustomResponse<User>();

            IEnumerable<User> filteredUserList = _userRepo.Users;
            NameValueCollection queryMap = HttpUtility.ParseQueryString(Request.QueryString.Value);

            foreach (string key in queryMap.AllKeys)
            {
                filteredUserList = filteredUserList.WhereDynamic<User>(key,queryMap[key]);
            }
            response.addResult(filteredUserList.ToList());
            return Ok(response);
        }

        // GET: api/user/{id}
        [HttpGet]
        [Route("{id}")]
        public ActionResult<CustomResponse<User>> GetById(int id)
        {
            if (id <= 0)
            {
                return NotFound("User id must be higher than zero");
            }
            User user = _userRepo.Users.FirstOrDefault(s => s.UserId == id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            CustomResponse<User> response = new CustomResponse<User>(user);
            return Ok(response);
        }

        // POST: api/user
        [HttpPost]
        public async Task<ActionResult<CustomResponse<User>>> Post([FromBody]User user)
        {
            CustomResponse<User> response = new CustomResponse<User>();
            if (user == null)
            {
                return NotFound("User data is not supplied");
            }

            User existingUser = _userRepo.Users.FirstOrDefault(u => u.Username == user.Username);

            if (existingUser != null)
            {
                response = new CustomResponse<User>();
                response.addMessage("Username already exist");
                response.incError();
                return response;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // test Password
            bool passwordTest = checkAndHashPassword(user.Password, response, user);
            if (!passwordTest) return response;

            await _userRepo.Users.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            response = new CustomResponse<User>(user);
            return Created($"api/user/${user.UserId}", response);
        }

        // PUT: api/user/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomResponse<User>>> Update(int id, [FromBody]User user)
        {
            CustomResponse<User> response = new CustomResponse<User>();

            if (user == null)
            {
                return NotFound("User data is not supplied");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User existingUser = _userRepo.Users.FirstOrDefault(u => u.UserId == id);

            if (existingUser == null)
            {
                return NotFound("User does not exist in the database");
            }
            if (existingUser.Username != user.Username)
            {
                response.addMessage("Username cannot be changed");
            }

            checkAndHashPassword(user.Password, response, existingUser);

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;

            existingUser.DateOfBirth = user.DateOfBirth;
            existingUser.Mobile = user.Mobile;
            existingUser.Phone = user.Phone;

            await _userRepo.SaveChangesAsync();

            response.addResult(existingUser);
            return Ok(response);
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<CustomResponse<User>>> Delete(int? id)
        {
            CustomResponse<User> response = new CustomResponse<User>();
            if (id == null)
            {
                return NotFound("Id is not supplied");
            }
            User user = _userRepo.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                return NotFound("No User found with particular id supplied");
            }
            _userRepo.Users.Remove(user);
            await _userRepo.SaveChangesAsync();

            response.addMessage("Users delete successully");
            return Ok(response);
        }

        // GET: api/user/{id}/group
        [HttpGet]
        [Route("{id}/group")]
        public ActionResult<CustomResponse<Group>> GetGroupForUser(int id)
        {
            List<Group> groupList = _userGroupRepo.UserGroups.Where(x => x.UserId == id).Select(x => x.Group).ToList();
            CustomResponse<Group> response = new CustomResponse<Group>(groupList);
            return Ok(response);
        }

        // POST: api/user/{id}/group/add
        [HttpPost]
        [Route("{id}/group/add")]
        public async Task<ActionResult<CustomResponse<Group>>> SetGroupForUser(int id, [FromBody]int[] groupIdList)
        {
            CustomResponse<Group> response = new CustomResponse<Group>();

            foreach (int groupId in groupIdList)
            {
                UserGroup ug = _userGroupRepo.UserGroups.FirstOrDefault(x => x.GroupID == groupId && x.UserId == id);
                if (ug != null)
                {
                    Group addedGroup = _groupRepo.Groups.FirstOrDefault(x => x.GroupID == groupId);
                    response.addMessage("Group "+addedGroup.GroupName +" is already added");
                    continue;
                }

                UserGroup newUg = await UserGroup.buildUserGroup(id, _userGroupRepo.Users, groupId, _userGroupRepo.Groups);

                await _userGroupRepo.UserGroups.AddAsync(newUg);
                await _userGroupRepo.SaveChangesAsync();
            }

            List<Group> groupList = _userGroupRepo.UserGroups.Where(x => x.UserId == id).Select(x => x.Group).ToList();
            response.addResult(groupList);
            return Created($"{id}/group", response);
        }

        // DELETE: api/user/{id}/group/remove
        [HttpDelete]
        [Route("{id}/group/remove")]
        public async Task<ActionResult<CustomResponse<Group>>> RemoveGroupForUser(int id, [FromBody]int[] groupIdList)
        {
            CustomResponse<Group> response = new CustomResponse<Group>();

            foreach (int groupId in groupIdList)
            {
                UserGroup ug = _userGroupRepo.UserGroups.FirstOrDefault(x => x.GroupID == groupId && x.UserId == id);
                if (ug == null) continue;

                _userGroupRepo.UserGroups.Remove(ug);
                await _userGroupRepo.SaveChangesAsync();
            }

            List<Group> groupList = _userGroupRepo.UserGroups.Where(x => x.UserId == id).Select(x => x.Group).ToList();
            response.addResult(groupList);
            return Ok(response);
        }

        // DELETE: api/user/{id}/group/clear
        [HttpDelete]
        [Route("{id}/group/clear")]
        public async Task<ActionResult<CustomResponse<Group>>> RemoveAllGroupForUser(int id)
        {
            CustomResponse<Group> response = new CustomResponse<Group>();

            int[] groupIdArray = _userGroupRepo.UserGroups.Where(x => x.UserId == id).Select(x => x.GroupID).ToArray();

            foreach (int groupId in groupIdArray)
            {
                UserGroup ug = _userGroupRepo.UserGroups.FirstOrDefault(x => x.GroupID == groupId && x.UserId == id);
                if (ug == null) continue;

                _userGroupRepo.UserGroups.Remove(ug);
                await _userGroupRepo.SaveChangesAsync();
            }

            List<Group> groupList = _userGroupRepo.UserGroups.Where(x => x.UserId == id).Select(x => x.Group).ToList();
            response.addResult(groupList);
            return Ok(response);
        }

        public static bool checkAndHashPassword(string newUnecryptedPassword, CustomResponse<User> response, User user)
        {
            int passwordScore = newUnecryptedPassword.checkPasswordStrength();
            if (passwordScore < 4)
            {
                // validate password strength
                response.addMessage(PasswordScore.PASSWORD_MESSAGE);
                return false;
            }
            else
            {
                // hash password, using strategy pattern
                Models.User.setEncryptionMethod(new BCrypt_Encrypt());
                string newEncryptedPassword = Models.User.encryptPassword(newUnecryptedPassword);
                user.Password = newEncryptedPassword;
            }
            return true;
        }

        ~UserControllers()
        {
            _userRepo.Dispose();
            _userGroupRepo.Dispose();
        }
    }
}
