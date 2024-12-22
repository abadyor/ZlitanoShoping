namespace APPL.Extensions
{
    public static class Constant
    {
        public const string RegisterRoute = "api/account/identity/create";

        public const string LoginRoute = "api/account/identity/login";

        public const string RefreshTokenRoute = "api/account/identity/refresh-token";

        public const string GetRolesRoute = "api/account/identity/role/list";

        public const string CreateRoleRoute = "api/account/identity/role/create";

        public const string CreateAdminRoute = "setting";

        public const string BrowserStorageKey = "x-key";

        public const string HttpClientName = "ClientUI";

        public const string HttpClientHeaderScheme = "Bearer";

        public const string AuthenticationType = "JwtAuth";

        public const string GetUserWithRolesRoute = "api/account/identity/users-with-roles";
        public const string ChangeUserRoleRoute = "api/account/identity/change-role";

        public static class Role
        {
            public const string Admin = "Admin";
            public const string User = "User";
        }
        //-Category-subCategory
        public const string AddCategoryRoute = "api/category/add-category";

        public const string AddSubCategoryRoute = "api/subcategory/add-subcategory";

        public const string GetCategoryRoute = "api/category/get-category";

        public const string GetSubCategoryRoute = "api/subcategory/get-subcategory";

        public const string GetCategorysRoute = "api/category/get-categorys";

        public const string GetSubCategorysRoute = "api/subcategory/get-subcategorys";
        public const string UpdatecategoryRoute = "api/category/update-category";
        public const string UpdatesubcategoryRoute = "api/subcategory/update-subcategory";
        public const string DeleteSubCategoryRoute = "api/subcategory/delete-subcategory";
        public const string DeleteCategoryRoute = "api/category/delete-category";

        //------------store ----------
        public const string AddStoreRoute = "api/store/add-store";

        public const string DeleteStoreRoute = "api/store/delete-store";

        public const string GetStoresRoute = "api/store/get-stores";

        public const string GetStoreRoute = "api/store/get-store";

        public const string GetStoreWithTagsbyRoute = "api/store/get-store-with-tags";
        public const string GetStoreWithTagsRoute = "api/store/get-stores-with-tags";
        public const string UpdateStoreRoute = "api/store/update-store";





        public const string GetVehicleOwnersRoute = "api/vehicle/get-vehicle-owners";

        public const string UpdateVehicleRoute = "api/vehicle/update-vehicle";

        public const string UpdateVehicleBrandRoute = "api/vehicle/update-vehicle-brand";
        public const string UpdateVehicleOwnerRoute = "api/vehicle/update-vehicle-owner";

        public const string DeleteVehicleRoute = "api/vehicle/delete-vehicle";

        public const string DeleteVehicleBrandRoute = "api/vehicle/delete-vehicle-brand";

        public const string DeleteVehicleOwnerRoute = "api/vehicle/delete-vehicle-owner";
    }
}
