using MoqUnitTest.Moq.Enum;
using System;
using System.Reflection;

namespace MoqUnitTest.Moq.Models.Extension
{

    public static class MoqGeneratorExtension
    {
        public static object Generate(this PropertyInfo prop, object obj)
        {
            var random = new Random();

            if (prop.PropertyType == typeof(string))
                return "TestData".GenerateId();
            if (prop.PropertyType == typeof(Int32))
                return (Int32)random.Next(0, 50);
            if (prop.PropertyType == typeof(Int64))
                return (Int64)random.Next(0, 50);
            if (prop.PropertyType == typeof(long))
                return (long)random.Next(0, 50);
            if (prop.PropertyType == typeof(float))
                return (float)random.Next(0, 500);
            if (prop.PropertyType == typeof(decimal))
                return (Decimal)random.Next(0, 500);
            if (prop.PropertyType == typeof(double))
                return (double)random.Next(0, 500);
            if (prop.PropertyType == typeof(bool))
                return random.Next(0, 2) == 1 ? true : false;
            if (prop.PropertyType == typeof(Guid))
                return Guid.NewGuid();
            if (prop.PropertyType == typeof(Int32?))
                return (Int32?)random.Next(0, 50);
            if (prop.PropertyType == typeof(Int64?))
                return (Int64?)random.Next(0, 50);
            if (prop.PropertyType == typeof(long?))
                return (long?)random.Next(0, 50);
            if (prop.PropertyType == typeof(float?))
                return (float?)random.Next(0, 500);
            if (prop.PropertyType == typeof(decimal?))
                return (decimal?)random.Next(0, 500);
            if (prop.PropertyType == typeof(double?))
                return (double?)random.Next(0, 500);
            if (prop.PropertyType == typeof(bool?))
                return random.Next(0, 2) == 1 ? true : false;
            //if (prop.PropertyType == typeof(Guid?))
            //    return Guid.NewGuid();

            return prop.GetValue(obj);
        }

        public static bool IsGenerable(this PropertyInfo prop)
        {
            if (prop.PropertyType == typeof(string))
                return true;
            if (prop.PropertyType == typeof(int))
                return true;
            if (prop.PropertyType == typeof(long))
                return true;
            if (prop.PropertyType == typeof(float))
                return true;
            if (prop.PropertyType == typeof(decimal))
                return true;
            if (prop.PropertyType == typeof(double))
                return true;
            if (prop.PropertyType == typeof(bool))
                return true;
            if (prop.PropertyType == typeof(Guid))
                return true;

            return false;
        }

        public static string GenerateId(this string obj)
        {
            var random = new Random();

            var startIndex = random.Next(0, 6);
            var lastIndex = startIndex + random.Next(0, 6);

            var id = Guid.NewGuid().ToString().Substring(startIndex, lastIndex);

            return obj + id;
        }

        public static string GeneratePhone()
        {
            var random = new Random();

            var phoneNumber = random.Next(1000, 9999).ToString() + random.Next(1000, 9999).ToString();

            return phoneNumber;
        }

        public static string GenerateUserLogin(this UserRole role, string login = "")
        {
            if (role == UserRole.User && string.IsNullOrWhiteSpace(login))
                return "user".GenerateId();
            else if (role == UserRole.Admin && string.IsNullOrWhiteSpace(login))
                return "admin".GenerateId();
            else if (string.IsNullOrWhiteSpace(login))
                return "user".GenerateId();
            else
                return login;
        }

        public static int GenerateCode()
        {
            var random = new Random();

            return random.Next(1000, 9999);
        }
    }
}
