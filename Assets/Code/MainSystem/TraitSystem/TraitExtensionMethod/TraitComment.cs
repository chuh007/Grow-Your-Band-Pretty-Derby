using System.Linq;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.TraitExtensionMethod
{
    public static class TraitComment
    {
        public static MemberTraitComment GetCommentForMember(this ITraitHolder holder, string id, MemberType member)
        {
            return holder.ActiveTraits
                .FirstOrDefault(t => t.Data.TraitID == id)?
                .GetMemberTraitComment();
        }
    }
}