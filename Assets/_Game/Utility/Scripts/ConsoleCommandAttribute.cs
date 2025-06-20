using UnityEngine;
using System;

namespace LOK1game.Utility
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ConsoleCommandAttribute : Attribute
    {
        public string CommandName { get; private set; }
        public string Description { get; private set; }

        public ConsoleCommandAttribute(string commandName, string description = "")
        {
            CommandName = commandName;
            Description = description;
        }
    }
} 