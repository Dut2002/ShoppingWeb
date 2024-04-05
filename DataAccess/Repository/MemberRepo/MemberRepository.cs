using BusinessObject;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.MemberRepo
{
    public class MemberRepository : IMemberRepository
    {
        public void AddMember(Member member) => MemberDAO.Instance.AddMember(member);

        public void DeleteMember(int memberId) => MemberDAO.Instance.Delete(memberId);

        public IEnumerable<Member> GetMembersList()
        {
            return MemberDAO.Instance.GetMembersList();
        }

        public IEnumerable<Member> SearchMember(string name)
        {
            return MemberDAO.Instance.SearchMember(name);
        }

        public Member Login(string email, string password)
        {
            return MemberDAO.Instance.Login(email, password);
        }

        public void UpdateMember(Member member) => MemberDAO.Instance.Update(member);

        public IEnumerable<Member> SearchMemberByCountry(string country, IEnumerable<Member> searchList)
        {
            return MemberDAO.Instance.FilterMemberByCountry(country, searchList);
        }

        public IEnumerable<Member> SearchMemberByCity(string country, string city, IEnumerable<Member> searchList)
        {
            return MemberDAO.Instance.FilterMemberByCity(country, city, searchList);
        }

        public Member GetMember(int memberId)
        {
            return MemberDAO.Instance.GetMember(memberId);
        }

        public int GetNextMemberId()
        {
            return MemberDAO.Instance.GetNextMemberId();
        }

        public Member GetMember(string memberEmail) => MemberDAO.Instance.GetMember(memberEmail);
    }
}
