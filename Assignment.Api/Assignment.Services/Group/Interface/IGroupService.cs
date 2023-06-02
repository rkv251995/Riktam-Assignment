using Assignment.Dtos.Common;
using Assignment.Dtos.Group;
using Assignment.Models.Group;
using Assignment.Models.Message;

namespace Assignment.Services.Group.Interface
{
    public interface IGroupService
    {
        Task<CommonDto<GroupDto>> CreateGroupAsync(AddUpdateGroupModel model, string loggedInUserId);
        Task<CommonDto<GroupDto>> UpdateGroupAsync(Guid groupId, AddUpdateGroupModel model, string loggedInUserId);
        Task<CommonDto<GroupDto>> FindGroupByIdAsync(Guid groupId);
        Task<CommonDto<Guid>> DeleteGroupByIdAsync(Guid groupId, Guid loggedInUserId);
        Task<CommonDto<GroupDto>> JoinGroupAsync(Guid groupId, Guid userId);
        Task<CommonDto<GroupDto>> LeaveGroupAsync(Guid groupId, Guid userId);
        Task<CommonDto<Guid>> SentMessageAsync(Guid groupId, MessageModel model);
        Task<CommonDto<Guid>> DeleteMessageAsync(Guid groupId, Guid messageId, Guid loggedInUserId);
        Task<CommonDto<GroupMessageDto>> FindAllMessagesByGroupIdAsync(Guid groupId);
        Task<CommonDto<GroupMemberDto>> FindAllMembersByGroupIdAsync(Guid groupId);
    }
}
