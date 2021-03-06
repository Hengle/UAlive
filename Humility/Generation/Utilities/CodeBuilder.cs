﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;
using System.Reflection;
using Ludiq;
using System.Linq;

namespace Lasm.UAlive
{
    /// <summary>
    /// Gives you access to utilities methods to more easily build custom code.
    /// </summary>
    public static class CodeBuilder
    {
        /// <summary>
        /// Creates the opening of a new body as a string.
        /// </summary>
        public static string OpenBody(int indent)
        {
            var output = string.Empty;

            output += Indent(indent) + "{";

            return output;
        }

        /// <summary>
        /// Creates the opening of a new body as a string with custom indent spacing.
        /// </summary>
        public static string OpenBody(int indent, int spaces)
        {
            var output = string.Empty;

            output += Indent(indent) + "{";

            for (int i = 0; i < spaces; i++)
            {
                output += "\n";
            }

            return output;
        }

        /// <summary>
        /// Creates the closing of a body as a string.
        /// </summary>
        public static string CloseBody(int indent)
        {
            var output = string.Empty;

            output += Indent(indent) + "}";

            return output;
        }

        /// <summary>
        /// Creates the closing of a body as a string with custom indent spacing.
        /// </summary>
        public static string CloseBody(int indent, int spaces)
        {
            var output = string.Empty;

            output += Indent(indent) + "}";

            for (int i = 0; i < spaces; i++)
            {
                output += "\n";
            }

            return output;
        }

        /// <summary>
        /// Creates an indentation. The spacing is equal to 4 whitespaces.
        /// </summary>
        public static string Indent(int amount)
        {
            var output = string.Empty;

            for (int i = 0; i < amount; i++)
            {
                output += "    ";
            }

            return output;
        }

        /// <summary>
        /// Creates an indentation with a custom amount of whitespaces per indent.
        /// </summary>
        public static string Indent(int amount, int spacing)
        {
            var output = string.Empty;
            var space = string.Empty;

            for (int i = 0; i < spacing; i++)
            {
                space += " ";
            }

            for (int i = 0; i < amount; i++)
            {
                output += space;
            }

            return output;
        }

        /// <summary>
        /// Creates a series of using statements for namespace access.
        /// </summary>
        public static string Using(string[] namespaces)
        {
            var output = string.Empty;

            for (int i = 0; i < namespaces.Length; i++)
            {
                output += "using " + namespaces[i] + ";" + (i < namespaces.Length - 1 ? "\n" : string.Empty);
            }

            return output;
        }

        public static string Using(List<string> namespaces)
        {
            var output = string.Empty;
            var _namespaces = namespaces.ToArray();
            return Using(_namespaces);
        }

        public static string Using(this ClassGenerator generator)
        {
            var usings = new List<string>();
            for (int i = 0; i < generator.fields.Count; i++)
            {
                var @namespace = generator.fields[i].type.Namespace;
                if (!usings.Contains(@namespace)) usings.Add(@namespace);
            }

            for (int i = 0; i < generator.properties.Count; i++)
            {
                var @namespace = generator.properties[i].returnType.Namespace;
                if (!usings.Contains(@namespace)) usings.Add(@namespace);
            }

            for (int i = 0; i < generator.methods.Count; i++)
            {
                var @namespace = generator.methods[i].returnType.Namespace;
                if (!usings.Contains(@namespace)) usings.Add(@namespace);
            }

            var output = string.Empty;

            for (int i = 0; i < usings.Count; i++)
            {
                output += "using " + usings[i] + ";" + (i < usings.Count - 1 ? "\n" : string.Empty);
            }

            return output;
        }

        public static List<string> Usings(this ClassGenerator generator)
        {
            var usings = new List<string>();

            for (int i = 0; i < generator.attributes.Count; i++)
            {
                usings.MergeUnique(generator.attributes[i].Usings());
            }

            for (int i = 0; i < generator.fields.Count; i++)
            {
                usings.MergeUnique(generator.fields[i].Usings());
            }

            for (int i = 0; i < generator.properties.Count; i++)
            {
                usings.MergeUnique(generator.properties[i].Usings());
            }

            for (int i = 0; i < generator.methods.Count; i++)
            {
                usings.MergeUnique(generator.methods[i].Usings());
            }

            return usings;
        }

        public static AccessModifier GetScope(this MethodInfo method)
        {
            if (method.IsPublic) return AccessModifier.Public;
            if (method.IsPrivate) return AccessModifier.Private;
            if (method.IsPrivate && method.IsFamily) return AccessModifier.PrivateProtected;
            if (method.IsFamilyAndAssembly) return AccessModifier.ProtectedInternal;
            if (method.IsFamily) return AccessModifier.Protected;
            if (method.IsAssembly) return AccessModifier.Internal;
            return AccessModifier.Public;
        }

        public static MethodModifier GetModifier(this MethodInfo method)
        {
            if (method.IsStatic) return MethodModifier.Static;
            if (method.IsVirtual) return MethodModifier.Virtual;
            if (method.IsAbstract) return MethodModifier.Abstract;
            if (method.IsFinal) return MethodModifier.Sealed;
            return MethodModifier.None;
        }

        public static string Parameters(this List<ParameterGenerator> parameters)
        {
            var output = "(";
            for (int i = 0; i< parameters.Count; i++)
            {
                output += parameters[i].Generate(0);
                if (i < parameters.Count - 1) output += ", ";
            }
            output += ")";
            return output;
        }

        public static string InitializeVariable(string name, Type type)
        {
            return !type.Is().NullOrVoid() ? type.As().CSharpName() + " " + CodeBuilder.Assign(name, type.Default().As().Code(false)) + "\n" : string.Empty;
        }

        public static string Qoute()
        {
            return @"""";
        }

        public static string Comma()
        {
            return ", ";
        }

        public static bool Null(string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string NullOrNot(object obj, string @null, string not)
        {
            return obj == null ? @null : not;
        }

        public static string NullVoidOrNot(Type type, string nullOrVoid, string not)
        {
            return type.Is().NullOrVoid() ? nullOrVoid : not;
        }

        public static string NullAsEmptyOr(string str, string or)
        {
            return Null(str) ? string.Empty : or;
        }

        public static string End()
        {
            return ");";
        }

        public static string SingleLineLambda(string parameters, string body)
        {
            return "(" + parameters + ")=>{ " + body + " }";
        }

        public static string Assign(string member, string value, Type castedType)
        {
            return member + " = " + "(" + castedType.As().CSharpName() + ")" + value + ";";
        }

        public static string Assign(string member, string value)
        {
            return member + " = " + value + ";";
        }

        public static string Return(string value)
        {
            return "return " + value + ";";
        }
    }
}