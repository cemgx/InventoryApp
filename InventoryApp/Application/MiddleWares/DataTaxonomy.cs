using Microsoft.Extensions.Compliance.Classification;

namespace InventoryApp.Application.MiddleWares
{
    public static class DataTaxonomy
    {
        public static string TaxonomyName { get; } = typeof(DataTaxonomy).FullName!;
        public static DataClassification SensitiveData { get; } = new(TaxonomyName, nameof(SensitiveData));
        public static DataClassification PiiData { get; } = new(TaxonomyName, nameof(PiiData));
    }

    public class SensitiveDataAttirbute : DataClassificationAttribute
    {
        public SensitiveDataAttirbute() : base(DataTaxonomy.SensitiveData)
        {
        }
    }
    public class PiiDataAttirbute : DataClassificationAttribute
    {
        public PiiDataAttirbute() : base(DataTaxonomy.PiiData)
        {
        }
    }
}