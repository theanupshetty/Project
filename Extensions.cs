using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplication.App_GlobalResources;
using WebApplication.Areas.Admin.Models;

namespace WebApplication.Extensions
{
    public static class Extensions
    {
        public static string MySubstring(
            this string str, int index, int length)
        {
            StringBuilder sb = new StringBuilder(str);

            return sb.ToString(index, length);
        }

        public static bool ContainsAll(this string source, params string[] values)
        {
            return values.All(x => source.Contains(x));
        }

        public static string ToStrings(this int[] source)
        {
            return source.Select(i => i.ToString(CultureInfo.InvariantCulture)).Aggregate((s1, s2) => s1 + ", " + s2);
        }


        public static List<T> Deserialize<T>(this string SerializedJSONString)
        {
            var stuff = JsonConvert.DeserializeObject<List<T>>(SerializedJSONString);
            return stuff;
        }

        public static MvcHtmlString Translate(this HtmlHelper htmlHelper, string key, bool isCaps)
        {
            var thread = Utility.Culture.GetCurrentCulture();
            var culture = new CultureInfo(thread);
            var resourceManager = new ResourceManager(typeof(Resource));
            string val = resourceManager.GetString(key, culture);
            if (isCaps)
            {
                return MvcHtmlString.Create(String.IsNullOrEmpty(val) ? key.ToUpper() : val.ToUpper());
            }
            // if value is not found return the key itself
            return MvcHtmlString.Create(String.IsNullOrEmpty(val) ? key : val);
        }

        public static string Translate(this string key, bool isCaps)
        {
            var thread = Utility.Culture.GetCurrentCulture();
            var culture = new CultureInfo(thread);
            var resourceManager = new ResourceManager(typeof(Resource));
            string val = resourceManager.GetString(key, culture);
            if (isCaps)
            {
                return String.IsNullOrEmpty(val) ? key.ToUpper() : val.ToUpper();
            }
            // if value is not found return the key itself
            return String.IsNullOrEmpty(val) ? key : val;
        }

        public static List<T> SortColumns<T>( this List<T> obj,JQDataTableParamModel param)
        {
            //obj = obj.OrderBy(param.columns[param.order[0].column].name + " " + param.order[0].dir);
            return obj;
        }
        public static List<T> SearchColumns<T>(this List<T> obj, string searchkey)
        {
            if (!string.IsNullOrEmpty(searchkey))
            {
                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);

                if (properties == null)
                    throw new ArgumentException("{typeof(T).Name}' does not implement a public get property named '{key}.");
                var filteredData = obj.Where(d => properties.Any(p => p.GetValue(d).ToString().Contains(searchkey)));
                return filteredData.ToList();
            }
            return obj;
        }
    }

    public static class ExcelPackageExtensions
    {
        public static DataTable ToDataTable(this ExcelPackage package)
        {
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            DataTable table = new DataTable();
            foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
            {
                table.Columns.Add(firstRowCell.Text);
            }
            for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
            {
                var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                var newRow = table.NewRow();
                foreach (var cell in row)
                {
                    newRow[cell.Start.Column - 1] = cell.Text;
                }
                table.Rows.Add(newRow);
            }
            return table;
        }
    }

    public static class UrlExtensions
    {
        public static string AbsoluteContent(this UrlHelper urlHelper, string contentPath)
        {
            // Build a URI for the requested path
            var url = new Uri(HttpContext.Current.Request.Url, urlHelper.Content(contentPath));
            // Return the absolute UrI
            return url.AbsoluteUri;
        }
    }
    public static class IEnumerableExtension
    {
        public static IEnumerable<T> AsNotNull<T>(this IEnumerable<T> original)
        {
            return original ?? Enumerable.Empty<T>();
        }
    }

    public class RequiredIfNotAttribute : ValidationAttribute, IClientValidatable
    {
        private String PropertyName { get; set; }
        private Object InvalidValue { get; set; }
        private readonly RequiredAttribute _innerAttribute;

        public RequiredIfNotAttribute(String propertyName, Object invalidValue)
        {
            PropertyName = propertyName;
            InvalidValue = invalidValue;
            _innerAttribute = new RequiredAttribute();
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var dependentValue = context.ObjectInstance.GetType().GetProperty(PropertyName).GetValue(context.ObjectInstance, null);
            if (dependentValue == null)
            {
                return new ValidationResult(FormatErrorMessage(context.DisplayName), new[] { context.MemberName });
            }
            if (dependentValue.ToString() != InvalidValue.ToString())
            {
                if (!_innerAttribute.IsValid(value))
                {
                    return new ValidationResult(FormatErrorMessage(context.DisplayName), new[] { context.MemberName });
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessageString,
                ValidationType = "requiredifnot",
            };
            rule.ValidationParameters["dependentproperty"] = (context as ViewContext).ViewData.TemplateInfo.GetFullHtmlFieldId(PropertyName);
            rule.ValidationParameters["invalidvalue"] = InvalidValue is bool ? InvalidValue.ToString().ToLower() : InvalidValue;

            yield return rule;
        }
    }
}