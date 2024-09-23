using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;

namespace Kassa.BuisnessLogic.Dto;
public class ClientDto: IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public string FirstName
    {
        get; set;
    } = null!;

    public string LastName
    {
        get; set;
    } = null!;

    public string MiddleName
    {
        get; set;
    } = null!;

    public string House
    {
        get; set;
    } = null!;

    public string Building
    {
        get; set;
    } = null!;

    public string Entrance
    {
        get; set;
    } = null!;

    public string Floor
    {
        get; set;
    } = null!;

    public string Apartment
    {
        get; set;
    } = null!;

    public string Intercom
    {
        get; set;
    } = null!;

    public string Address => $"{House} {Building} {Entrance} {Floor} {Apartment} {Intercom}";

    public string AddressNote
    {
        get; set;
    } = null!;

    public string Miscellaneous
    {
        get; set;
    } = null!;

    public string Phone
    {
        get; set;
    } = null!;

    public string Card
    {
        get; set;
    } = null!;

    public Guid StreetId
    {
        get; set;
    }
}
