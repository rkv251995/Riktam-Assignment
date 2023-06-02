using Assignment.Dtos.Common;
using Assignment.Dtos.Group;
using Assignment.Infrastructures.EntityFrameworkCore;
using Assignment.Infrastructures.EntityFrameworkCore.Entity;
using Assignment.Models.Common;
using Assignment.Models.Group;
using Assignment.Models.Message;
using Assignment.Services.Group.Interface;
using Assignment.Utilities.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Net;
using GroupData = Assignment.Infrastructures.EntityFrameworkCore.Entity.Group;

namespace Assignment.Services.Group
{
    public class GroupService : BaseService, IGroupService
    {
        public GroupService(DataContext dataContext) : base(dataContext)
        { }

        public async Task<CommonDto<GroupDto>> CreateGroupAsync(AddUpdateGroupModel model, string loggedInUserId)
        {
            GroupData group = new()
            {
                Name = model.Name,
                Description = model.Description,
                CreatedBy = new(loggedInUserId ?? string.Empty),
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            await _dataContext.Groups.AddAsync(group);

            await _dataContext.SaveChangesAsync();

            return await FindGroupByIdAsync(group.Id);
        }

        public async Task<CommonDto<GroupDto>> UpdateGroupAsync(Guid groupId, AddUpdateGroupModel model, string loggedInUserId)
        {
            GroupData? group = await _dataContext.Groups.FirstOrDefaultAsync(f => f.Id == groupId && f.IsActive);

            if (group is null)
                throw new RaiseError(new ExceptionDetail { Code = "427", Message = "Group doesn't exists." }.ToString());

            if (group.CreatedBy != new Guid(loggedInUserId ?? string.Empty))
                throw new RaiseError(new ExceptionDetail { Code = ((int)HttpStatusCode.Forbidden).ToString(), Message = HttpStatusCode.Forbidden.ToString() }.ToString());

            group.Name = model.Name;
            group.Description = model.Description;
            group.UpdatedBy = new(loggedInUserId ?? string.Empty);
            group.UpdatedDate = DateTime.Now;

            _dataContext.Groups.Update(group);
            await _dataContext.SaveChangesAsync();

            return await FindGroupByIdAsync(groupId);
        }

        public async Task<CommonDto<GroupDto>> FindGroupByIdAsync(Guid groupId)
        {
            GroupData? group = await _dataContext.Groups.FirstOrDefaultAsync(f => f.Id == groupId && f.IsActive);

            if (group is null)
                throw new RaiseError(new ExceptionDetail { Code = "427", Message = "Group doesn't exists." }.ToString());

            return new CommonDto<GroupDto>
            {
                IsSuccess = true,
                Message = "Group Data.",
                Response = new GroupDto
                {
                    Id = group.Id,
                    Name = group.Name,
                    Description = group.Description,
                    CreatedBy = group.CreatedBy,
                    CreatedDate = group.CreatedDate
                }
            };
        }

        public async Task<CommonDto<Guid>> DeleteGroupByIdAsync(Guid groupId, Guid loggedInUserId)
        {
            GroupData? group = await _dataContext.Groups.FirstOrDefaultAsync(f => f.Id == groupId && f.IsActive);

            if (group is null)
                throw new RaiseError(new ExceptionDetail { Code = "427", Message = "Group doesn't exists." }.ToString());

            if (group.CreatedBy != loggedInUserId)
                throw new RaiseError(new ExceptionDetail { Code = ((int)HttpStatusCode.Forbidden).ToString(), Message = HttpStatusCode.Forbidden.ToString() }.ToString());

            group.IsActive = false;
            group.UpdatedBy = loggedInUserId;
            group.UpdatedDate = DateTime.Now;

            _dataContext.Groups.Update(group);
            await _dataContext.SaveChangesAsync();

            return new CommonDto<Guid>
            {
                IsSuccess = true,
                Message = "Group is deleted successfully.",
                Response = group.Id
            };
        }

        public async Task<CommonDto<GroupDto>> JoinGroupAsync(Guid groupId, Guid userId)
        {
            if (!await _dataContext.Groups.AnyAsync(a => a.Id == groupId && a.IsActive))
                throw new RaiseError(new ExceptionDetail { Code = "427", Message = "Group doesn't exists." }.ToString());

            await _dataContext.GroupUserMappings.AddAsync(new()
            {
                GroupId = groupId,
                UserId = userId,
                CreatedBy = userId,
                CreatedDate = DateTime.Now,
                IsActive = true
            });

            await _dataContext.SaveChangesAsync();

            return await FindGroupByIdAsync(groupId);
        }

        public async Task<CommonDto<GroupDto>> LeaveGroupAsync(Guid groupId, Guid userId)
        {
            if (!await _dataContext.Groups.AnyAsync(a => a.Id == groupId && a.IsActive))
                throw new RaiseError(new ExceptionDetail { Code = "427", Message = "Group doesn't exists." }.ToString());

            GroupUserMapping? data = await _dataContext.GroupUserMappings.FirstOrDefaultAsync(f => f.GroupId == groupId && f.UserId == userId);

            if (data is null)
                throw new RaiseError(new ExceptionDetail { Code = "428", Message = "Group-User doesn't exists." }.ToString());

            data.IsActive = false;
            data.UpdatedBy = userId;
            data.UpdatedDate = DateTime.Now;

            _dataContext.GroupUserMappings.Update(data);
            await _dataContext.SaveChangesAsync();

            return await FindGroupByIdAsync(groupId);
        }

        public async Task<CommonDto<Guid>> SentMessageAsync(Guid groupId, MessageModel model)
        {
            if (!await _dataContext.Groups.AnyAsync(a => a.Id == groupId && a.IsActive))
                throw new RaiseError(new ExceptionDetail { Code = "427", Message = "Group doesn't exists." }.ToString());

            Message message = new()
            {
                Text = model.Message,
                CreatedBy = new(model.LoginUserId ?? string.Empty),
                CreatedDate = DateTime.Now,
                IsActive = true,
                MessageGroupMappings = new List<MessageGroupMapping>
                {
                    new MessageGroupMapping
                    {
                        GroupId = groupId,
                        CreatedBy = new(model.LoginUserId ?? string.Empty),
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    }
                }
            };

            await _dataContext.Messages.AddAsync(message);

            return new CommonDto<Guid>
            {
                IsSuccess = true,
                Message = "Message Sent.",
                Response = message.Id
            };
        }

        public async Task<CommonDto<Guid>> DeleteMessageAsync(Guid groupId, Guid messageId, Guid loggedInUserId)
        {
            Message? message = await _dataContext.Messages.FirstOrDefaultAsync(f => f.Id == messageId && f.IsActive);

            if (message is null)
                throw new RaiseError(new ExceptionDetail { Code = "428", Message = "Message doesn't exists." }.ToString());

            message.IsActive = false;
            message.UpdatedBy = loggedInUserId;
            message.UpdatedDate = DateTime.Now;

            foreach (MessageGroupMapping item in message.MessageGroupMappings)
            {
                item.IsActive = false;
                item.UpdatedBy = loggedInUserId;
                item.UpdatedDate = DateTime.Now;
            }

            _dataContext.Messages.Update(message);
            await _dataContext.SaveChangesAsync();

            return new CommonDto<Guid>
            {
                IsSuccess = true,
                Message = "Message is deleted successfully.",
                Response = message.Id
            };
        }

        public async Task<CommonDto<GroupMessageDto>> FindAllMessagesByGroupIdAsync(Guid groupId)
        {
            GroupData? group = await _dataContext.Groups.Include(i => i.MessageGroupMappings)
                                                        .ThenInclude(ti => ti.Message)
                                                        .FirstOrDefaultAsync(f => f.Id == groupId && f.IsActive);

            if (group is null)
                throw new RaiseError(new ExceptionDetail { Code = "427", Message = "Group doesn't exists." }.ToString());

            return new CommonDto<GroupMessageDto>
            {
                IsSuccess = true,
                Message = "Data Found.",
                Response = new GroupMessageDto
                {
                    Id = group.Id,
                    Name = group.Name,
                    Description = group.Name,
                    CreatedBy = group.CreatedBy,
                    CreatedDate = group.CreatedDate,
                    Messages = group.MessageGroupMappings.Select(s => new MessageDto
                    {
                        Id = s.Message.Id,
                        Message = s.Message.Text,
                        CreatedBy = s.Message.CreatedBy,
                        CreatedDate = s.Message.CreatedDate,
                    })
                    .ToList()
                }
            };
        }

        public async Task<CommonDto<GroupMemberDto>> FindAllMembersByGroupIdAsync(Guid groupId)
        {
            GroupData? group = await _dataContext.Groups.Include(i => i.GroupUserMappings)
                                                        .ThenInclude(ti => ti.User)
                                                        .FirstOrDefaultAsync(f => f.Id == groupId && f.IsActive);

            if (group is null)
                throw new RaiseError(new ExceptionDetail { Code = "427", Message = "Group doesn't exists." }.ToString());

            return new CommonDto<GroupMemberDto>
            {
                IsSuccess = true,
                Message = "Data Found.",
                Response = new GroupMemberDto
                {
                    Id = group.Id,
                    Name = group.Name,
                    Description = group.Description,
                    CreatedBy = group.CreatedBy,
                    CreatedDate = group.CreatedDate,
                    Members = group.GroupUserMappings.Select(s => new MemberDto
                    {
                        FirstName = s.User.FirstName,
                        LastName = s.User.LastName,
                        Username = s.User.Username,
                        Email = s.User.Email,
                        Mobile = s.User.Mobile,
                        DateOfBirth = s.User.DateOfBirth,
                        Address = s.User.Address,
                        City = s.User.City,
                        State = s.User.State,
                        Country = s.User.Country
                    })
                    .ToList()
                }
            };
        }
    }
}
