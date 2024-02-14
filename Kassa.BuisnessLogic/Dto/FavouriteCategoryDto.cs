using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic.Dto;

/// <summary>
/// It's a category that contains the favourite products.
/// </summary>
public sealed class FavouriteCategoryDto(int favourite) : ICategoryItemDto, ICategoryDto
{
    public int Id
    {

        get;
    } = favourite;

    public int? CategoryId
    {
        get;
    } = null;

    public int[] Favourites
    {
        get;
    } = [];

    public string Name
    {
        get;
    } = $"Favourites {favourite}";

    public int Favourite
    {
        get;
    } = favourite;

    public ICollection<ICategoryItem>? Items
    {
        get;
    } = null;
}
