using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;

namespace Kassa.BuisnessLogic.Dto;
public class CourierDto: IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public string FirstName
    {
        get; set;
    } = string.Empty;

    public string LastName
    {
        get; set;
    } = string.Empty;

    public string MiddleName
    {
        get; set;
    } = string.Empty;

    public string Phone
    {
        get; set;
    } = string.Empty;
}
