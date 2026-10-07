namespace VendorMngSystem.Models
{
    public class VendorsCategory
    {
        public int Id { get; set; }
        public string VendorsCategoryName { get; set; } 


        //All nevigation Properties
        public ICollection<Vendors> vendors { get; set; } = new List<Vendors>();
    }
}
