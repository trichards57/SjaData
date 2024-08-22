using Microsoft.Extensions.Compliance.Classification;

namespace SjaData.Model.DataTypes;

public static class DataClassifications
{
    public static DataClassification PatientData { get; } = new DataClassification("PatientDataTaxonomy", "PatientData");
}
