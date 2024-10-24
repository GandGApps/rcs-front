﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;
using Riok.Mapperly.Abstractions;

namespace Kassa.BuisnessLogic;

[Mapper]
public static partial class Mapper
{
    public static partial Receipt MapDtoToReceiptDto(ReceiptDto receipt);
    public static partial ReceiptDto MapReceiptDtoToReceipt(Receipt receipt);

    public static partial Ingredient MapIngredientDtoToIngredient(IngredientDto ingredient);
    public static partial IngredientDto MapIngredientToIngredientDto(Ingredient ingredient);

    public static partial Additive MapAdditiveDtoToAdditive(AdditiveDto additive);
    public static partial AdditiveDto MapAdditiveToAdditiveDto(Additive additive);

    public static partial Product MapProductDtoToProduct(ProductDto product);
    public static partial ProductDto MapProductToProductDto(Product product);

    public static partial Receipt MapDtoToReceipt(ReceiptDto receipt);
    public static partial ReceiptDto MapReceiptToDto(Receipt receipt);

    public static partial IngredientUsage MapDtoToIngredientUsage(IngredientUsageDto ingredientUsage);
    public static partial IngredientUsageDto MapIngredientUsageToDto(IngredientUsage ingredientUsage);

    public static partial Order MapDtoToOrder(OrderDto order);
    public static partial OrderDto MapOrderToDto(Order order);

    public static partial OrderedProduct MapDtoToOrderedProduct(OrderedProductDto product);
    public static partial OrderedProductDto MapOrderedProductToDto(OrderedProduct product);

    public static partial OrderedAdditive MapDtoToOrderedAdditive(OrderedAdditiveDto additive);
    public static partial OrderedAdditiveDto MapOrderedAdditiveToDto(OrderedAdditive additive);

    public static partial Client MapDtoToClient(ClientDto client);
    public static partial ClientDto MapClientToDto(Client client);

    public static partial Courier MapDtoToCourier(CourierDto courier);
    public static partial CourierDto MapCourierToDto(Courier courier);

    [MapProperty(nameof(ProductShoppingListItemDto.ItemId), nameof(OrderedProductDto.ProductId))]
    [MapProperty(nameof(ProductShoppingListItemDto.SubtotalSum), nameof(OrderedProductDto.SubTotalPrice))]
    [MapProperty(nameof(ProductShoppingListItemDto.TotalSum), nameof(OrderedProductDto.TotalPrice))]
    [MapProperty(nameof(ProductShoppingListItemDto.Comment), nameof(OrderedProductDto.Comment))]
    [MapperIgnoreSource(nameof(ProductShoppingListItemDto.CurrencySymbol))]
    [MapperIgnoreSource(nameof(ProductShoppingListItemDto.HasDiscount))]
    [MapperIgnoreSource(nameof(ProductShoppingListItemDto.IsSelected))]
    [MapperIgnoreSource(nameof(ProductShoppingListItemDto.Measure))]
    [MapperIgnoreSource(nameof(ProductShoppingListItemDto.Name))]
    [MapperIgnoreTarget(nameof(OrderedProductDto.Additives))]
    public static partial OrderedProductDto MapShoppingListItemToOrderedProductDto(ProductShoppingListItemDto item);

    [MapProperty(nameof(AdditiveShoppingListItemDto.ItemId), nameof(OrderedAdditiveDto.AdditiveId))]
    [MapProperty(nameof(AdditiveShoppingListItemDto.Price), nameof(OrderedAdditiveDto.Price))]
    [MapProperty(nameof(AdditiveShoppingListItemDto.TotalSum), nameof(OrderedAdditiveDto.TotalPrice))]
    [MapProperty(nameof(AdditiveShoppingListItemDto.SubtotalSum), nameof(OrderedAdditiveDto.SubtotalPrice))]
    [MapperIgnoreSource(nameof(AdditiveShoppingListItemDto.CurrencySymbol))]
    [MapperIgnoreSource(nameof(AdditiveShoppingListItemDto.IsSelected))]
    [MapperIgnoreSource(nameof(AdditiveShoppingListItemDto.Name))]
    [MapperIgnoreSource(nameof(AdditiveShoppingListItemDto.HasDiscount))]
    [MapperIgnoreSource(nameof(AdditiveShoppingListItemDto.ContainingProduct))]
    [MapperIgnoreSource(nameof(AdditiveShoppingListItemDto.Portion))]
    public static partial OrderedAdditiveDto MapAdditiveShoppingListItemToOrderedAdditiveDto(AdditiveShoppingListItemDto item);


    public static partial District MapDtoToDistrict(DistrictDto district);
    public static partial DistrictDto MapDistrictToDto(District district);

    public static partial Street MapDtoToStreet(StreetDto street);
    public static partial StreetDto MapStreetToDto(Street street);

    public static partial PaymentInfoDto MapPaymentInfoToDto(PaymentInfo paymentInfo);
    public static partial PaymentInfo MapDtoToPaymentInfo(PaymentInfoDto paymentInfo);

    public static partial Shift MapDtoToShift(ShiftDto shift);
    public static partial ShiftDto MapShiftToDto(Shift shift);

    public static partial User MapDtoToUser(UserDto user);
    public static partial UserDto MapUserToDto(User user);

    public static partial SeizureReason MapDtoToWithdrawalReason(SeizureReasonDto withdrawalReason);
    public static partial SeizureReasonDto MapWithdrawalReasonToDto(SeizureReason withdrawalReason);

    public static partial Member MapDtoToMember(MemberDto member);
    public static partial MemberDto MapMemberToDto(Member member);

    [return:NotNullIfNotNull("categoryDto")]
    public static partial Category? MapDtoToCategory(CategoryDto? categoryDto);

    [return:NotNullIfNotNull("category")]
    public static partial CategoryDto? MapCategoryToDto(Category? category);

    
    public static partial OrderDto MapOrderEditDtoToOrderDto(OrderEditDto orderEditDto);
    [MapperIgnoreSource(nameof(OrderEditDto.Products))]
    public static partial OrderEditDto MapOrderDtoToOrderEditDto(OrderDto orderDto);

    public static partial ContributionReasonDto MapContributionReasonToDto(ContributionReason ContributionReason);
    public static partial ContributionReason MapDtoToContributionReason(ContributionReasonDto ContributionReason);

    public static partial SeizureReasonDto MapSeizureReasonToDto(SeizureReason SeizureReason);
    public static partial SeizureReason MapDtoToSeizureReason(SeizureReasonDto SeizureReason);
}
