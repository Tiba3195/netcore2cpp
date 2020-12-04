using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
//using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

namespace DotNetLib
{


    [AttributeUsage(AttributeTargets.Field)]
    public sealed partial class RequiredAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public sealed partial class NativeExpose : Attribute
    {
    }

    // Used on an optionsObject field to rename the corresponding commandline option.
    [AttributeUsage(AttributeTargets.Field)]
    public sealed partial class NameAttribute : Attribute
    {
        public NameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public partial class RuntimeReflector
    {
        private object optionsObject;
        private Queue<FieldInfo> requiredOptions = new Queue<FieldInfo>();
        private Dictionary<string, FieldInfo> optionalOptions = new Dictionary<string, FieldInfo>();
        private Dictionary<string, MethodInfo> Methods = new Dictionary<string, MethodInfo>();
        private Queue<PropertyInfo> requiredOptions2 = new Queue<PropertyInfo>();
        private Dictionary<string, PropertyInfo> optionalOptions2 = new Dictionary<string, PropertyInfo>();
        private List<string> requiredUsageHelp = new List<string>();
        private List<string> optionalUsageHelp = new List<string>();

        public object GetValue(string fieldname)
        {
            FieldInfo fi;
            if (optionalOptions.TryGetValue(fieldname.ToLowerInvariant(), out fi))
            {
                if (GetAttribute<DotNetLib.NativeExpose>(fi) is object)
                {
                    return fi.GetValue(optionsObject);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void InvokeMethod(string _name)
        {
            var method = Methods[_name.ToLowerInvariant()];
            method.Invoke(optionsObject, null);
        }

        public void InvokeMethod(string _name, string[] args)
        {
            var method = Methods[_name.ToLowerInvariant()];
            var praminfo = method.GetParameters();
            var newargs = new object[(args.Count())];

            for (int i = 0, loopTo = args.Count() - 1; i <= loopTo; i++)
            {
                newargs[i] = ChangeType(args[i], praminfo[i].ParameterType);
            }
               
            method.Invoke(optionsObject, newargs);
        }

        public void InvokeMethod(string _name, string args)
        {
            var method = Methods[_name.ToLowerInvariant()];
            var praminfo = method.GetParameters();
            method.Invoke(optionsObject, new object[] { ChangeType(args, praminfo[0].ParameterType) });
        }

        // Constructor.
        public RuntimeReflector(object optionsObject)
        {
            this.optionsObject = optionsObject;
            foreach (MethodInfo method in optionsObject.GetType().GetMethods())
            {
                if (GetAttribute<DotNetLib.NativeExpose>(method) is object)
                {
                    string methodname = GetMethodName(method);
                    if (!Methods.Keys.Contains(methodname))
                    {
                        Methods.Add(methodname.ToLowerInvariant(), method);
                    }
                }
            }



            // Reflect to find what commandline options are available.
            foreach (FieldInfo field in optionsObject.GetType().GetFields())
            {
                string fieldName = GetOptionName(field);
                if (GetAttribute<DotNetLib.NativeExpose>(field) is object)
                {
                    if (GetAttribute<DotNetLib.RequiredAttribute>(field) is object)
                    {
                        // Record a required option.
                        requiredOptions.Enqueue(field);
                        requiredUsageHelp.Add(string.Format("<{0}>", fieldName));
                    }
                    else
                    {

                        // Record an optional option.
                        optionalOptions.Add(fieldName.ToLowerInvariant(), field);
                        if (field.FieldType == typeof(bool))
                        {
                            optionalUsageHelp.Add(string.Format("/{0}", fieldName));
                        }
                        else
                        {
                            optionalUsageHelp.Add(string.Format("/{0}:value", fieldName));
                        }
                    }
                }
            }

            foreach (PropertyInfo field in optionsObject.GetType().GetProperties())
            {
                string fieldName = GetOptionProperty(field);
                if (GetAttribute<DotNetLib.NativeExpose>(field) is object)
                {
                    if (GetAttribute<DotNetLib.RequiredAttribute>(field) is object)
                    {
                        // Record a required option.
                        requiredOptions2.Enqueue(field);
                        requiredUsageHelp.Add(string.Format("<{0}>", fieldName));
                    }
                    else
                    {
                        // Record an optional option.
                        optionalOptions2.Add(fieldName.ToLowerInvariant(), field);
                        if (field.PropertyType == typeof(bool))
                        {
                            optionalUsageHelp.Add(string.Format("/{0}", fieldName));
                        }
                        else
                        {
                            optionalUsageHelp.Add(string.Format("/{0}:value", fieldName));
                        }
                    }
                }
            }
        }

        public bool ParseCommandLine(string args)
        {
            // Parse each argument in turn.

            if (!ParseArgument(args.Trim()))
            {
                return false;
            }


            // Make sure we got all the required options.
            var missingRequiredOption = requiredOptions.FirstOrDefault(field => !IsList(field) || GetList(field).Count == 0);
            var missingRequiredOption2 = requiredOptions2.FirstOrDefault(field => !IsList(field) || GetList(field).Count == 0);
            if (missingRequiredOption is object)
            {
                ShowError("Missing argument '{0}'", GetOptionName(missingRequiredOption));
                return false;
            }

            if (missingRequiredOption2 is object)
            {
                ShowError("Missing argument '{0}'", GetOptionProperty(missingRequiredOption2));
                return false;
            }

            return true;
        }

        public bool ParseCommandLine(string[] args)
        {
            // Parse each argument in turn.
            foreach (string arg in args)
            {
                if (!ParseArgument(arg.Trim()))
                {
                    return false;
                }
            }

            // Make sure we got all the required options.
            var missingRequiredOption = requiredOptions.FirstOrDefault(field => !IsList(field) || GetList(field).Count == 0);
            var missingRequiredOption2 = requiredOptions2.FirstOrDefault(field => !IsList(field) || GetList(field).Count == 0);
            if (missingRequiredOption is object)
            {
                ShowError("Missing argument '{0}'", GetOptionName(missingRequiredOption));
                return false;
            }

            if (missingRequiredOption2 is object)
            {
                ShowError("Missing argument '{0}'", GetOptionProperty(missingRequiredOption2));
                return false;
            }

            return true;
        }

        public bool ParseArgument(string arg)
        {
            if (arg.StartsWith("/"))
            {
                // Parse an optional argument.
                var separators = new[] { ':' };
                var split = arg.Substring(1).Split(separators, 2, StringSplitOptions.None);
                string name = split[0];
                string value = split.Length > 1 ? split[1] : "true";
                FieldInfo field;
                var field2 = default(PropertyInfo);
                if (!optionalOptions.TryGetValue(name.ToLowerInvariant(), out field) && !optionalOptions2.TryGetValue(name.ToLowerInvariant(), out field2))
                {
                    ShowError("Unknown option '{0}'", name);
                    return false;
                }

                if (field is object)
                {
                    return SetOption(field, value);
                }

                if (field2 is object)
                {
                    return SetOption(field2, value);
                }
            }
            else
            {
                // Parse a required argument.
                if (requiredOptions.Count == 0 && requiredOptions2.Count == 0)
                {
                    ShowError("Too many arguments");
                    return false;
                }

                var field = requiredOptions.Peek();
                if (!IsList(field))
                {
                    requiredOptions.Dequeue();
                }
                else
                {
                    return SetOption(field, arg);
                }

                var field2 = requiredOptions2.Peek();
                if (!IsList(field))
                {
                    requiredOptions2.Dequeue();
                }
                else
                {
                    return SetOption(field2, arg);
                }
            }

            return default;
        }

        public bool SetOption(FieldInfo field, string value)
        {
            try
            {
                if (IsList(field))
                {
                    // Append this value to a list of options.
                    GetList(field).Add(ChangeType(value, ListElementType(field)));
                }
                else
                {
                    // Set the value of a single option.
                    field.SetValue(optionsObject, ChangeType(value, field.FieldType));
                }

                return true;
            }
            catch
            {
                ShowError("Invalid value '{0}' for option '{1}'", value, GetOptionName(field));
                return false;
            }
        }

        public bool SetOption(PropertyInfo field, string value)
        {
            try
            {
                if (IsList(field))
                {
                    // Append this value to a list of options.
                    GetList(field).Add(ChangeType(value, ListElementType(field)));
                }
                else
                {
                    // Set the value of a single option.
                    field.SetValue(optionsObject, ChangeType(value, field.PropertyType));
                }

                return true;
            }
            catch
            {
                ShowError("Invalid value '{0}' for option '{1}'", value, GetOptionName(field));
                return false;
            }
        }

        public object ChangeType(string value, Type type)
        {
            var converter = TypeDescriptor.GetConverter(type);
            return converter.ConvertFromInvariantString(value);
        }

        public bool IsList(FieldInfo field)
        {
            return typeof(IList).IsAssignableFrom(field.FieldType);
        }

        public IList GetList(FieldInfo field)
        {
            return (IList)field.GetValue(optionsObject);
        }

        public bool IsList(PropertyInfo field)
        {
            return typeof(IList).IsAssignableFrom(field.PropertyType);
        }

        public IList GetList(PropertyInfo field)
        {
            return (IList)field.GetValue(optionsObject);
        }

        public Type ListElementType(FieldInfo field)
        {
            IEnumerable<object> interfaces = from i in field.FieldType.GetInterfaces()
                                where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                                select i;            

            return ((Type)interfaces.First()).GetGenericArguments()[0];
        }

        public Type ListElementType(PropertyInfo field)
        {
            IEnumerable< object> interfaces = from i in field.PropertyType.GetInterfaces()
                                where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                                select i;           

            return ((Type)interfaces.First()).GetGenericArguments()[0];
        }

        public string GetOptionName(FieldInfo field)
        {
            NameAttribute nameAttribute = GetAttribute<DotNetLib.NameAttribute>(field);
            if (nameAttribute is NameAttribute)
            {
                return nameAttribute.Name;
            }
            else
            {
                return field.Name;
            }
        }

        public string GetMethodName(MethodInfo method)
        {
            NameAttribute nameAttribute = GetAttribute<DotNetLib.NameAttribute>(method);
            if (nameAttribute is NameAttribute)
            {
                return nameAttribute.Name;
            }
            else
            {
                return method.Name;
            }
        }

        public string GetOptionName(PropertyInfo field)
        {
            NameAttribute nameAttribute = GetAttribute<DotNetLib.NameAttribute>(field);
            if (nameAttribute is NameAttribute)
            {
                return nameAttribute.Name;
            }
            else
            {
                return field.Name;
            }
        }

        public string GetOptionProperty(PropertyInfo field)
        {
            NameAttribute nameAttribute = GetAttribute<NameAttribute>(field);
            if (nameAttribute is NameAttribute)
            {
                return nameAttribute.Name;
            }
            else
            {
                return field.Name;
            }
        }

        public void ShowError(string message, params object[] args)
        {
            string name = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName);
            Console.Error.WriteLine(message, args);
            Console.Error.WriteLine();
            Console.Error.WriteLine("Usage: {0} {1}", name, string.Join(" ", requiredUsageHelp));
            if (optionalUsageHelp.Count > 0)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("Options:");
                foreach (string optional in optionalUsageHelp)
                    Console.Error.WriteLine("    {0}", optional);
            }
        }
        static public T GetAttribute<T>(ICustomAttributeProvider provider) where T : Attribute
        {
            return provider.GetCustomAttributes(typeof(T), false).OfType<T>().FirstOrDefault();
        }

    }
}
