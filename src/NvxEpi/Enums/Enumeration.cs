using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Reflection;
using PepperDash.Core;

namespace NvxEpi.Enums
{
    public abstract class Enumeration<TEnum> : IComparable<Enumeration<TEnum>> where TEnum : Enumeration<TEnum>
    {
        private static readonly CCriticalSection _lock = new CCriticalSection();
        private static IEnumerable<TEnum> _all;

        protected Enumeration(int value, string name)
        {
            Value = value;
            Name = name;
        }

        public string Name { get; private set; }

        public int Value { get; private set; }

        public int CompareTo(Enumeration<TEnum> other)
        {
            return other == null ? 1 : Value.CompareTo(other.Value);
        }

        public override string ToString()
        {
            return Name;
        }

        public static IEnumerable<TEnum> GetAll()
        {
            CheckAll();
            return _all;
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration<TEnum>;

            if (otherValue == null)
                return false;

            bool typeMatches = GetType().GetCType() == obj.GetType().GetCType();
            bool valueMatches = Value.Equals(otherValue.Value);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object other)
        {
            if (other == null) return 1;

            var otherEnum = other as Enumeration<TEnum>;
            if (otherEnum != null)
                return Value.CompareTo(otherEnum.Value);

            throw new ArgumentException("Object is not an Enum Class");
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private static IEnumerable<TEnum> GetAllOptions()
        {
            try
            {
                var baseType = typeof (TEnum).GetCType();
                var a = baseType.Assembly;

                Debug.Console(2, "Base type: {0}", baseType.Name);
                IEnumerable<CType> enumTypes = a.GetTypes().Where(baseType.IsAssignableFrom);

                var options = new List<TEnum>();
                foreach (CType enumType in enumTypes)
                {
                    Debug.Console(2, "Found enum type: {0}", enumType.Name);
                    var fields =
                        enumType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
                            .Select(x => x.GetValue(null))
                            .Cast<TEnum>();

                    foreach (var field in fields.Where(field => field != null))
                    {
                        Debug.Console(2, "Adding field to this enum:{0} - {1}", field.Name, enumType.Name);
                        if (options.Contains(field))
                            throw new Exception("This enum already exists");

                        options.Add(field);
                    }
                }

                return options;
            }
            catch (Exception ex)
            {
                var error = "Error getting all options -" +
                               string.Format("{0}\r{1}\r{2}", ex.Message, ex.InnerException, ex.StackTrace);
                Debug.Console(0, error);
                throw;
            }
        }

        private static void CheckAll()
        {
            _lock.Enter();
            try
            {
                if (_all == null)
                    _all = GetAllOptions();
            }
            finally
            {
                _lock.Leave();
            }
        }

        public static TEnum FromName(string name, bool ignoreCase)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(name);

            CheckAll();
            if (ignoreCase)
            {
                var result = _all.FirstOrDefault(x => x.Name.Equals(name));
                if (result == null)
                    throw new ArgumentNullException(name);

                return result;
            }
            else
            {
                var result = _all.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (result == null)
                    throw new ArgumentNullException(name);

                return result;
            }
        }

        public static bool TryFromValue(int value, out TEnum result)
        {
            CheckAll();
            result = _all.FirstOrDefault(x => x.Value == value);
            return result != null;
        }

        public static bool TryFromName(string name, out TEnum result, bool ignoreCase)
        {
            CheckAll();
            if (ignoreCase)
            {
                result = _all.FirstOrDefault(x => x.Name.Equals(name));
                return result != null;
            }

            result = _all.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return result != null;
        }

        public static bool TryFromNameNoSpaces(string name, out TEnum result)
        {
            CheckAll();

            result = null;

            var nameToCheck = name.Replace(" ", "");
            foreach (var item in _all)
            {
                string itemNameToCheck = item.Name.Replace(" ", "");
                if (!nameToCheck.Equals(itemNameToCheck, StringComparison.OrdinalIgnoreCase))
                    continue;

                result = item;
                break;
            }

            return result != null;
        }

        public static TEnum FromValue(int value)
        {
            CheckAll();
            var result = _all.FirstOrDefault(x => x.Value == value);
            if (result == null)
                throw new ArgumentNullException(value.ToString(CultureInfo.InvariantCulture));

            return result;
        }
    }
}