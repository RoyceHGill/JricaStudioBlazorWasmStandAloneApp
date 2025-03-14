﻿using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.Dtos.Admin;
using Microsoft.AspNetCore.Components.Forms;



namespace JricaStudioApp.Services.Contracts
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetItems();

        Task<ProductDto> GetItem(Guid id);

        Task<IEnumerable<ProductDto>> GetRandomProducts(int targetLength);

        Task<ProductDto> GetShowcaseProduct();

        Task<IEnumerable<AdminProductDto>> SearchProducts(ProductFilterDto dto);

        Task<IEnumerable<AdminProductCategoryDto>> GetCategories();

        Task<AdminProductDto> DeleteProduct(Guid id);

        Task<AdminProductCategoryDto> PostNewProductCategory(AddProductCategoryDto dto);
        Task<AdminProductCategoryDto> DeleteProductCategory(Guid id);

        Task<UploadResultDto> UploadProductImage(IBrowserFile file);
        Task<AdminProductDto> PostNewProduct(AdminProductToAddDto dto);
        Task<ImageUpdateResultDto> PatchProductImage(Guid id, IBrowserFile file);
        Task<EditProductDto> PutProduct(Guid id, EditProductDto dto);
        Task<ProductDto> UpdateProductShowcase(UpdateProductShowcaseDto updateDto);
    }
}
