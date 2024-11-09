using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Diagnostics;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Splat;

namespace Kassa.DataAccess;

/// <summary>
/// Utility class to access internal mock data,
/// Unuse this class in production code
/// </summary>
public static class InternalMockData
{
    public static string GetMemberPincode(Member member)
    {
        return member.Pincode;
    }

    public static Member? GetMemberByPincode(IRepository<Member> repository, string pincode)
    {
        if (repository is IRepository<Member>.MockRepository mockRepository)
        {
            return mockRepository._items.Values.FirstOrDefault(x => x.Pincode == pincode);
        }

        return ThrowHelper.ThrowInvalidOperationException<Member>("This method can only be used with a mock repository");
    }
}
