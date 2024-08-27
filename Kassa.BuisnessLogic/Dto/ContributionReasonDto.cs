using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Dto;
public class ContributionReasonDto: IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    } = string.Empty;

    public bool IsRequiredComment
    {
        get; set;
    }
}
