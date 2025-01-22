using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.Dtos.Admin;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Admin.Products.Components
{
    public class ProductShowcaseAdministrationBase : ComponentBase
    {
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public IProductService ProductService { get; set; }

        public AdminUserLoginDto Admin { get; set; }
        public IEnumerable<AdminProductDto> Products { get; set; }
        public ProductDto ProductShowcase { get; set; }
        public UpdateProductShowcaseDto UpdateDto { get; set; }
        public string ErrorMessage { get; set; }
        public string ConfirmationMessage { get; set; }


        protected override async Task OnInitializedAsync()
        {
            Admin = await AdminService.AuthenticateUser();

            Products = await ProductService.SearchProducts(new ProductFilterDto());

            ProductShowcase = await ProductService.GetShowcaseProduct();

            if (ProductShowcase == null)
            {
                UpdateDto = new UpdateProductShowcaseDto();
            }
            else
            {
                UpdateDto = new UpdateProductShowcaseDto()
                {
                    ProductId = ProductShowcase.Id
                };
            }
        }

        protected async Task UpdateProductShowcase_Click()
        {
            try
            {
                if (UpdateDto.ProductId == Guid.Empty)
                {
                    ApplyErrorMessage("Please select a product.");
                    return;
                }
                var product = await ProductService.UpdateProductShowcase(UpdateDto);

                if (product == null)
                {
                    ApplyErrorMessage("The product showcase was not updated.");
                    return;
                }
                ProductShowcase = product;
                ApplyConfirmationMessage("The product showcase was updated.");
            }
            catch (Exception e)
            {
                ApplyErrorMessage(e.Message);
            }
        }

        protected void ApplyErrorMessage(string message)
        {
            ConfirmationMessage = string.Empty;
            ErrorMessage = message;
        }

        protected void ApplyConfirmationMessage(string message)
        {
            ConfirmationMessage = message;
            ErrorMessage = string.Empty;
        }

    }
}
