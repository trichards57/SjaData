﻿// <auto-generated />
using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SjaData.Model;

#pragma warning disable 219, 612, 618
#nullable disable

namespace SjaData.Server.Data.Compiled
{
    internal partial class HoursEntryEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "SjaData.Server.Data.HoursEntry",
                typeof(HoursEntry),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(int),
                propertyInfo: typeof(HoursEntry).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(HoursEntry).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: 0);
            id.TypeMapping = IntTypeMapping.Default.Clone(
                comparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                keyComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                providerValueComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v));
            id.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            var date = runtimeEntityType.AddProperty(
                "Date",
                typeof(DateOnly),
                propertyInfo: typeof(HoursEntry).GetProperty("Date", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(HoursEntry).GetField("<Date>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateOnly(1, 1, 1));
            date.TypeMapping = SqlServerDateOnlyTypeMapping.Default.Clone(
                comparer: new ValueComparer<DateOnly>(
                    (DateOnly v1, DateOnly v2) => v1.Equals(v2),
                    (DateOnly v) => v.GetHashCode(),
                    (DateOnly v) => v),
                keyComparer: new ValueComparer<DateOnly>(
                    (DateOnly v1, DateOnly v2) => v1.Equals(v2),
                    (DateOnly v) => v.GetHashCode(),
                    (DateOnly v) => v),
                providerValueComparer: new ValueComparer<DateOnly>(
                    (DateOnly v1, DateOnly v2) => v1.Equals(v2),
                    (DateOnly v) => v.GetHashCode(),
                    (DateOnly v) => v));
            date.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var deletedAt = runtimeEntityType.AddProperty(
                "DeletedAt",
                typeof(DateTimeOffset?),
                propertyInfo: typeof(HoursEntry).GetProperty("DeletedAt", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(HoursEntry).GetField("<DeletedAt>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            deletedAt.TypeMapping = SqlServerDateTimeOffsetTypeMapping.Default.Clone(
                comparer: new ValueComparer<DateTimeOffset?>(
                    (Nullable<DateTimeOffset> v1, Nullable<DateTimeOffset> v2) => v1.HasValue && v2.HasValue && ((DateTimeOffset)v1).EqualsExact((DateTimeOffset)v2) || !v1.HasValue && !v2.HasValue,
                    (Nullable<DateTimeOffset> v) => v.HasValue ? ((DateTimeOffset)v).GetHashCode() : 0,
                    (Nullable<DateTimeOffset> v) => v.HasValue ? (Nullable<DateTimeOffset>)(DateTimeOffset)v : default(Nullable<DateTimeOffset>)),
                keyComparer: new ValueComparer<DateTimeOffset?>(
                    (Nullable<DateTimeOffset> v1, Nullable<DateTimeOffset> v2) => v1.HasValue && v2.HasValue && ((DateTimeOffset)v1).EqualsExact((DateTimeOffset)v2) || !v1.HasValue && !v2.HasValue,
                    (Nullable<DateTimeOffset> v) => v.HasValue ? ((DateTimeOffset)v).GetHashCode() : 0,
                    (Nullable<DateTimeOffset> v) => v.HasValue ? (Nullable<DateTimeOffset>)(DateTimeOffset)v : default(Nullable<DateTimeOffset>)),
                providerValueComparer: new ValueComparer<DateTimeOffset?>(
                    (Nullable<DateTimeOffset> v1, Nullable<DateTimeOffset> v2) => v1.HasValue && v2.HasValue && ((DateTimeOffset)v1).EqualsExact((DateTimeOffset)v2) || !v1.HasValue && !v2.HasValue,
                    (Nullable<DateTimeOffset> v) => v.HasValue ? ((DateTimeOffset)v).GetHashCode() : 0,
                    (Nullable<DateTimeOffset> v) => v.HasValue ? (Nullable<DateTimeOffset>)(DateTimeOffset)v : default(Nullable<DateTimeOffset>)));
            deletedAt.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var hours = runtimeEntityType.AddProperty(
                "Hours",
                typeof(TimeSpan),
                propertyInfo: typeof(HoursEntry).GetProperty("Hours", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(HoursEntry).GetField("<Hours>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new TimeSpan(0, 0, 0, 0, 0));
            hours.TypeMapping = SqlServerTimeSpanTypeMapping.Default.Clone(
                comparer: new ValueComparer<TimeSpan>(
                    (TimeSpan v1, TimeSpan v2) => v1.Equals(v2),
                    (TimeSpan v) => v.GetHashCode(),
                    (TimeSpan v) => v),
                keyComparer: new ValueComparer<TimeSpan>(
                    (TimeSpan v1, TimeSpan v2) => v1.Equals(v2),
                    (TimeSpan v) => v.GetHashCode(),
                    (TimeSpan v) => v),
                providerValueComparer: new ValueComparer<TimeSpan>(
                    (TimeSpan v1, TimeSpan v2) => v1.Equals(v2),
                    (TimeSpan v) => v.GetHashCode(),
                    (TimeSpan v) => v));
            hours.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var name = runtimeEntityType.AddProperty(
                "Name",
                typeof(string),
                propertyInfo: typeof(HoursEntry).GetProperty("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(HoursEntry).GetField("<Name>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 100);
            name.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                keyComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "nvarchar(100)",
                    size: 100,
                    unicode: true,
                    dbType: System.Data.DbType.String));
            name.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var personId = runtimeEntityType.AddProperty(
                "PersonId",
                typeof(int),
                propertyInfo: typeof(HoursEntry).GetProperty("PersonId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(HoursEntry).GetField("<PersonId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            personId.TypeMapping = IntTypeMapping.Default.Clone(
                comparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                keyComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                providerValueComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v));
            personId.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var region = runtimeEntityType.AddProperty(
                "Region",
                typeof(Region),
                propertyInfo: typeof(HoursEntry).GetProperty("Region", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(HoursEntry).GetField("<Region>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            region.TypeMapping = IntTypeMapping.Default.Clone(
                comparer: new ValueComparer<Region>(
                    (Region v1, Region v2) => object.Equals((object)v1, (object)v2),
                    (Region v) => v.GetHashCode(),
                    (Region v) => v),
                keyComparer: new ValueComparer<Region>(
                    (Region v1, Region v2) => object.Equals((object)v1, (object)v2),
                    (Region v) => v.GetHashCode(),
                    (Region v) => v),
                providerValueComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                converter: new ValueConverter<Region, int>(
                    (Region value) => (int)value,
                    (int value) => (Region)value),
                jsonValueReaderWriter: new JsonConvertedValueReaderWriter<Region, int>(
                    JsonInt32ReaderWriter.Instance,
                    new ValueConverter<Region, int>(
                        (Region value) => (int)value,
                        (int value) => (Region)value)));
            region.SetSentinelFromProviderValue(0);
            region.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var trust = runtimeEntityType.AddProperty(
                "Trust",
                typeof(Trust),
                propertyInfo: typeof(HoursEntry).GetProperty("Trust", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(HoursEntry).GetField("<Trust>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            trust.TypeMapping = IntTypeMapping.Default.Clone(
                comparer: new ValueComparer<Trust>(
                    (Trust v1, Trust v2) => object.Equals((object)v1, (object)v2),
                    (Trust v) => v.GetHashCode(),
                    (Trust v) => v),
                keyComparer: new ValueComparer<Trust>(
                    (Trust v1, Trust v2) => object.Equals((object)v1, (object)v2),
                    (Trust v) => v.GetHashCode(),
                    (Trust v) => v),
                providerValueComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                converter: new ValueConverter<Trust, int>(
                    (Trust value) => (int)value,
                    (int value) => (Trust)value),
                jsonValueReaderWriter: new JsonConvertedValueReaderWriter<Trust, int>(
                    JsonInt32ReaderWriter.Instance,
                    new ValueConverter<Trust, int>(
                        (Trust value) => (int)value,
                        (int value) => (Trust)value)));
            trust.SetSentinelFromProviderValue(0);
            trust.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updatedAt = runtimeEntityType.AddProperty(
                "UpdatedAt",
                typeof(DateTimeOffset),
                propertyInfo: typeof(HoursEntry).GetProperty("UpdatedAt", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(HoursEntry).GetField("<UpdatedAt>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
            updatedAt.TypeMapping = SqlServerDateTimeOffsetTypeMapping.Default.Clone(
                comparer: new ValueComparer<DateTimeOffset>(
                    (DateTimeOffset v1, DateTimeOffset v2) => v1.EqualsExact(v2),
                    (DateTimeOffset v) => v.GetHashCode(),
                    (DateTimeOffset v) => v),
                keyComparer: new ValueComparer<DateTimeOffset>(
                    (DateTimeOffset v1, DateTimeOffset v2) => v1.EqualsExact(v2),
                    (DateTimeOffset v) => v.GetHashCode(),
                    (DateTimeOffset v) => v),
                providerValueComparer: new ValueComparer<DateTimeOffset>(
                    (DateTimeOffset v1, DateTimeOffset v2) => v1.EqualsExact(v2),
                    (DateTimeOffset v) => v.GetHashCode(),
                    (DateTimeOffset v) => v));
            updatedAt.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            return runtimeEntityType;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "Hours");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
